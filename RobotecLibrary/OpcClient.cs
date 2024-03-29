﻿using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace Robotec.OPC
{
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

        /// <summary>
        /// Канал подключения к OPC серверу, дает расширенные функции работы с OPC сервером.
        /// </summary>
        public readonly UaTcpSessionChannel Session;

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
        public async Task Connect()
        {
            await Session.OpenAsync();
        }

        /// <summary>
        /// Отключает соединение с OPC сервером
        /// </summary>
        public async Task Disconnect()
        {
            await Session.CloseAsync();
        }

        /// <summary>
        /// Чтение ноды с сервера OPC
        /// </summary>
        /// <param name="id"> укажите id ноды для чтения</param>
        /// <returns> вернет результат в виде обьекта </returns>
        public async Task<object> ReadNodeAsync(string id)
        {
            if (Session == null || Session.State == CommunicationState.Faulted ||
                Session.State != CommunicationState.Opened)
                return "error read id, value is null";

            var readValue = new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(id),
                        AttributeId = AttributeIds.Value
                    }
                }
            };
            //var readResult = Session?.ReadAsync(readValue).Result;

            var readResult = await Session?.ReadAsync(readValue);
            // Get Type
            return readResult.Results[0].Value ?? "error read id, value is null";
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
                return "error read id, value is null";

            var readValue = new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(id),
                        AttributeId = AttributeIds.Value
                    }
                }
            };
            //var readResult = Session?.ReadAsync(readValue).Result;

            var readResult = Session?.ReadAsync(readValue).Result;
            // Get Type
            return readResult.Results[0].Value ?? "error read id, value is null";
        }

        /// <summary>
        /// Запись значения на сервер OPC
        /// </summary>
        /// <param name="id"> укажите id ноды для записи </param>
        /// <param name="value"> укажите значение для записи: double, int, string </param>
        public async Task WriteNodeAsync(string id, object value)
        {
            if (Session == null || Session.State == CommunicationState.Faulted ||
                Session.State != CommunicationState.Opened)
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

            await Session?.WriteAsync(writeValue);
        }

        /// <summary>
        /// Запись значения на сервер OPC
        /// </summary>
        /// <param name="id"> укажите id ноды для записи </param>
        /// <param name="value"> укажите значение для записи: double, int, string </param>
        public void WriteNode(string id, object value)
        {
            if (Session == null || Session.State == CommunicationState.Faulted ||
                Session.State != CommunicationState.Opened)
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

            Session?.WriteAsync(writeValue);
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
}