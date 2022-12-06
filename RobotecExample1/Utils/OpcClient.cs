using System;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

public class OpcClient
{

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action OnFaulted;


    public UaTcpSessionChannel Session;

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

    public void Connect()
    {
        Session.OpenAsync();
    }

    public void Disconnect()
    {
        Session.CloseAsync();
    }

    public object ReadNode(string id)
    {
        if (Session != null && Session.State != CommunicationState.Faulted)
        {
            var readValue = new ReadRequest
            {
                NodesToRead = new[] {
                        new ReadValueId {
                            NodeId = NodeId.Parse(id),
                            //AttributeId = AttributeIds.Value
                        }
                    }
            };

            var readResult = Session.ReadAsync(readValue).Result;
            return readResult.Results[0].Value ?? 0;
        }
        else
        {
            throw new Exception("Client is null or fauler");
        }
    }

    public void WriteNode(string id, object value)
    {
        if (Session != null && Session.State != CommunicationState.Faulted)
        {
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
        else
        {
            throw new Exception("Client is null or fauler");
        }
    }
}
