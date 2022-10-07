using System;
using System.Threading;
using Workstation.ServiceModel.Ua;

namespace OPC_UA_Client_Free
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // создаем клиента и указваем параметры
                var client = new OpcClient(
                    "172.31.1.147:4840",
                    "OpcUaOperator",
                    "kuka");

                // подписываемся на события вкл/выкл
                client.OnConnected += ClientOnConnected;
                client.OnDisconnected += ClientOnDisconnected;

                Console.WriteLine("Start Connect");

                // Подключаемся
                client.Connect();
                Thread.Sleep(2000);

                var random = new Random();
                while (
                    (client.Session.State != CommunicationState.Faulted || client.Session.State != CommunicationState.Closed) 
                       && client.Session.State == CommunicationState.Opened)
                {
                    Thread.Sleep(1000);

                    var pos_x = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.pos_x");
                    var time = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.time");
                    var status = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status");

                    Console.WriteLine($"pos_x: {pos_x}");
                    Console.WriteLine($"Current time: {time}");
                    Console.WriteLine($"status: {status}");

                    client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.time", random.NextDouble());

                    status = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.status");
                    time = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.time");

                    Console.WriteLine($"Writed time: {time}");
                    Console.WriteLine($"status: {status}");
                }

                client.Disconnect();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void ClientOnDisconnected()
        {
            Console.WriteLine("Disconected");
        }

        private static void ClientOnConnected()
        {
            Console.WriteLine("Connected");
        }
    }
}