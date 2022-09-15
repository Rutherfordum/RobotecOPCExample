using System;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Security;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OPC_UA_Client_Free
{
    class Program
    {
        private static UaTcpSessionChannel client;


        static void Main(string[] args)
        {
            try
            {

                var client = new OpcClient(
                    "172.31.1.147:4840",
                    "OpcUaOperator",
                    "kuka");

                client.connected += Client_connected;
                client.disconnected += Client_disconnected;
                 
                Console.WriteLine("Start Connect");
                client.Connect();
               
                Thread.Sleep(2000);
                Console.WriteLine(
                    client.ReadNode(
                        "ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status"));

                client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status", false));
                Thread.Sleep(2000);
                Console.WriteLine(
                    client.ReadNode(
                        "ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status"));


                client.Disconect();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void Client_disconnected()
        {
            Console.WriteLine("Disconected");
        }

        private static void Client_connected()
        {
            Console.WriteLine("Connected");
        }

        static async Task MainTest(string[] args)
        {

            var clientDescription = new ApplicationDescription
            {
                ApplicationName = "Workstation.UaClient.FeatureTests",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
                ApplicationType = ApplicationType.Client
            };

            var channel = new UaTcpSessionChannel(
                clientDescription,
                null, // no x509 certificates
                new UserNameIdentity("OpcUaOperator", "kuka"), // no user identity
                "opc.tcp://172.31.1.147:4840", // the public endpoint of a server at opcua.rocks.
                SecurityPolicyUris.None); // no encryption
            try
            {
                channel.Opened += Channel_Opened;
                channel.Closed += Channel_Closed;

                // try opening a session and reading a few nodes.
                await channel.OpenAsync();

                var readInt = new ReadRequest
                {
                    NodesToRead = new[] {
                    new ReadValueId {
                        NodeId = NodeId.Parse("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.pos_x"),
                        AttributeId = AttributeIds.Value
                    }
                }
                };

                var readBool = new ReadRequest
                {
                    NodesToRead = new[] {
                        new ReadValueId {
                            NodeId = NodeId.Parse("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status"),
                            AttributeId = AttributeIds.Value
                        }
                    }
                };

                var readResultBool = await channel.ReadAsync(readBool);
                var status = readResultBool.Results[0].Value;
                Console.WriteLine(status);
                bool temp = (bool)status;

                var readResult = await channel.ReadAsync(readInt);

                var serverStatus = readResult.Results[0].Value;
                Console.WriteLine(serverStatus);

                var random = new Random();

                var writePos_X = new WriteRequest
                {
                    NodesToWrite = new[]
                    {
                        new WriteValue
                        {
                            AttributeId = AttributeIds.Value,
                            NodeId = NodeId.Parse("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.time"),
                            Value = new DataValue((double) random.NextDouble())
                        }
                    }
                };

                Console.WriteLine("Writed");
                while (true)
                {
                    Thread.Sleep(500);
                    await channel.WriteAsync(writePos_X);

                    readResult = await channel.ReadAsync(readInt);
                    serverStatus = readResult.Results[0].Value;
                    Console.WriteLine(serverStatus);

                    readResultBool = await channel.ReadAsync(readBool);
                    status = readResultBool.Results[0].Value;

                    if ((bool)status == temp)
                        DebugLog(temp);
                    Console.WriteLine(status);
                    temp = (bool)status;

                }
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                await channel.AbortAsync();
                Console.WriteLine(ex.Message);
            }
        }


        public void WriteNode()
        {

        }

        public void ReadNode(string id)
        {

        }

        public static void DebugLog(bool value)
        {
            Console.WriteLine($"Status is {value}");
        }

        private static void Channel_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }

        private static void Channel_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        private static void Client_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        public void testClient()
        {
            var clientDescription = new ApplicationDescription
            {
                ApplicationName = "Workstation.UaClient.FeatureTests",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
                ApplicationType = ApplicationType.Client
            };

            client = new UaTcpSessionChannel(
                clientDescription,
                null,
                new UserNameIdentity("OpcUaOperator", "kuka"),
                "opc.tcp://172.31.1.147:4840",
                SecurityPolicyUris.None);

            try
            {
                Console.WriteLine(client.State);
                client.OpenAsync();
                client.Opened += Client_Opened;
                Console.WriteLine(client.State);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.ReadLine();
            // build a ReadRequest. See 'OPC UA Spec Part 4' paragraph 5.10.2
            var readRequest = new ReadRequest
            {
                // set the NodesToRead to an array of ReadValueIds.
                NodesToRead = new[]
                {
                    // construct a ReadValueId from a NodeId and AttributeId.
                    new ReadValueId
                    {
                        // you can parse the nodeId from a string.
                        // e.g. NodeId.Parse("ns=2;s=Demo.Static.Scalar.Double")
                        NodeId = NodeId.Parse(VariableIds.Server_ServerStatus),
                        // variable class nodes have a Value attribute.
                        AttributeId = AttributeIds.Value
                    }
                }
            };

            // send the ReadRequest to the server.
            var readResult = client.ReadAsync(readRequest);
            Console.WriteLine(readResult);

        }
    }
}
