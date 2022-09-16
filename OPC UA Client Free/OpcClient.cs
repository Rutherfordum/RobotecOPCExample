using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OPC_UA_Client_Free
{
    public class OpcClient
    {

        public event Action connected;
        public event Action disconnected;

        public UaTcpSessionChannel session;

        public OpcClient(string serverAddress, string userName, string password)
        {
            var clientDescription = new ApplicationDescription
            {
                ApplicationName = "Workstation.UaClient.FeatureTests",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
                ApplicationType = ApplicationType.Client
            };

            session = new UaTcpSessionChannel(
                clientDescription,
                null, // no x509 certificates
                new UserNameIdentity(userName, password), // no user identity
                $"opc.tcp://{serverAddress}", // the public endpoint of a server at opcua.rocks.
                SecurityPolicyUris.None); // no encryption

            session.Opened += SessionOpened;
            session.Closed += SessionClosed; ;
        }

        private void SessionClosed(object sender, EventArgs e)
        {
            disconnected?.Invoke();
        }

        private void SessionOpened(object sender, EventArgs e)
        {
            connected?.Invoke();
        }

        public void Connect()
        {
            session.OpenAsync();
        }

        public void Disconect()
        {
            session.CloseAsync();
        }

        public object ReadNode(string id)
        {
            if (session != null && session.State != CommunicationState.Faulted)
            {
                var readValue = new ReadRequest
                {
                    NodesToRead = new[] {
                        new ReadValueId {
                            NodeId = NodeId.Parse(id),
                            AttributeId = AttributeIds.Value
                        }
                    }
                };
                var readResult = session.ReadAsync(readValue).Result;
                return readResult.Results[0].Value;
            }
            else
            {
                throw new Exception("Client is null or fauler");
                return null;
            }
        }

        public void WriteNode(string id, object value)
        {
            if (session != null && session.State != CommunicationState.Faulted)
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
                session.WriteAsync(writeValue);
            }
            else
            {
                throw new Exception("Client is null or fauler");
            }
        }
    }
}
