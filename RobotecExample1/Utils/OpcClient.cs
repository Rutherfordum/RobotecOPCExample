using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

/// <summary>
/// Класс реализует основные методы для обмена сообщениями между OPC клиентом и OPC сервером
/// </summary>
public class OpcClient
{
    /// <summary>
    /// Событие клиент установил связь с сервером
    /// </summary>
    public event Action OnConnected;

    /// <summary>
    /// Событие клиент отключился от сервера
    /// </summary>
    public event Action OnDisconnected;

    /// <summary>
    /// События клиент потерял или разорвал связь с сервером
    /// </summary>
    public event Action OnFaulted;

    public readonly UaTcpSessionChannel Session;

    public OpcClient()
    { }

    /// <summary>
    /// Создаем конфигурацию OPC клиента
    /// </summary>
    /// <param name="serverAddress"> укажите IP адрес сервера и Port. Например 172.31.1.45:4540 </param>
    /// <param name="userName"> укажите имя пользователся для авторизации </param>
    /// <param name="password"> укажите пароль для авторизации </param>
    public OpcClient(string serverAddress, string userName, string password)
    {
        var clientDescription = new ApplicationDescription
        {
            ApplicationName = "Workstation.UaClient.FeatureTests",
            ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
            ApplicationType = ApplicationType.Client
        };

        Session = new UaTcpSessionChannel(
            clientDescription,
            null, // no x509 certificates
            new UserNameIdentity(userName, password), // user identity
            $"opc.tcp://{serverAddress}", // the public endpoint of a server at opcua.rocks.
            SecurityPolicyUris.None); // no encryption

        Session.Opened += SessionOpened;
        Session.Faulted += SessionFaulted;
        Session.Closed += SessionClosed;
    }

    /// <summary>
    /// Устанавливает соединение с OPC сервером
    /// </summary>
    public void Connect()
    {
        Session.OpenAsync();
    }

    /// <summary>
    /// Отключает соединение с OPC сервером
    /// </summary>
    public void Disconnect()
    {
        Session.CloseAsync();
    }

    /// <summary>
    /// Чтение ноды с сервера OPC
    /// </summary>
    /// <param name="id"> укажите id ноды для чтения</param>
    /// <returns> вернет результат в виде обьекта </returns>
    public object ReadNode(string id)
    {
        if (Session == null || Session.State == CommunicationState.Faulted ||
            Session.State != CommunicationState.Opened)
            return null;

        var readValue = new ReadRequest
        {
            NodesToRead = new[]
            {
                new ReadValueId {
                    NodeId = NodeId.Parse(id),
                    AttributeId = AttributeIds.Value
                }
            }
        };

        var readResult = Session.ReadAsync(readValue).Result;
        
        if (readResult == null)
            return 0;
        else
            return readResult.Results[0].Value;
    }

    /// <summary>
    /// Запись значения на сервер OPC
    /// </summary>
    /// <param name="id"> укажите id ноды для записи </param>
    /// <param name="value"> укажите значение для записи: double, int, string </param>
    public void WriteNode(string id, object value)
    {
        if (Session == null || Session.State == CommunicationState.Faulted || Session.State != CommunicationState.Opened)
            throw new Exception("Client is null or fauler");

        var writeValue = new WriteRequest
        {
            NodesToWrite = new[]
            {
                new WriteValue
                {
                    AttributeId = AttributeIds.Value,
                    NodeId = NodeId.Parse(id),
                    Value = new DataValue(value)
                }
            }
        };

        Session.WriteAsync(writeValue);
    }

    public async Task<Transform> ReadTransformNode(string id)
    {
        Transform transform = new Transform();
        await Task.Run(() =>
        {
            Task.Delay(2000);
            var x = ReadNode($"{id}X");
            var y = ReadNode($"{id}Y");
            var z = ReadNode($"{id}Z");
            var a = ReadNode($"{id}A");
            var b = ReadNode($"{id}B");
            var c = ReadNode($"{id}C");
            if (x != null &&
                y != null &&
                z != null &&
                a != null &&
                b != null &&
                c != null)
            {
                transform.X = (float)(double)x;
                transform.Y = (float)(double)y;
                transform.Z = (float)(double)z;
                transform.A = (float)(double)a;
                transform.B = (float)(double)b;
                transform.C = (float)(double)c;
            }

            return transform;
        });

        return new Transform();
    }

    public async Task<Transform> ReadTransformNode(string id_x, string id_y, string id_z, string id_a, string id_b, string id_c)
    {
        Transform transform = new Transform();
        await Task.Run(() =>
        {
            Task.Delay(2000);
            transform.X = (float)(double)ReadNode(id_x);
            transform.Y = (float)(double)ReadNode(id_y);
            transform.Z = (float)(double)ReadNode(id_z);
            transform.A = (float)(double)ReadNode(id_a);
            transform.B = (float)(double)ReadNode(id_b);
            transform.C = (float)(double)ReadNode(id_c);
            return transform;
        });

        return new Transform();
    }

    private void SessionFaulted(object sender, EventArgs e)
    {
        OnFaulted?.Invoke();
    }

    private void SessionClosed(object sender, EventArgs e)
    {
        OnDisconnected?.Invoke();
    }

    private void SessionOpened(object sender, EventArgs e)
    {
        OnConnected?.Invoke();
    }
}
