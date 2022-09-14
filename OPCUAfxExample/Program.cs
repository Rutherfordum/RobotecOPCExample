using System;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace OPCUAfxExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            using (var client =
                new OpcClient("opc.tcp://172.31.1.147:4840",new OpcSecurityPolicy(OpcSecurityMode.None)))
            {
                try
                {
                    client.Security.UserIdentity = new OpcClientIdentity("OpcUaOperator", "kuka");
                    
                    Console.WriteLine("Started connect");
                    client.Connect();
                    client.Disconnected += Client_Disconnected;
                    Console.WriteLine("Connected");

                    while (true)
                    {
                        if (client != null)
                        {
                            client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.pos_x");
                            client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.pos_x",
                                random.NextDouble());
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("I Can't Connect");
                }
                finally
                {
                   client.Disconnect();
                   Console.WriteLine("Disconnected");

                }
            }
        }

        private static void Client_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
    }
}
