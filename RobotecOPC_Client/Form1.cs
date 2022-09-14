using System;
using System.Windows.Forms;
using Opc.UaFx;
using Opc.UaFx.Client;


// Документация https://docs.traeger.de/en/software/sdk/opc-ua/net/client.development.guide
// Примеры кода на C# https://github.com/Traeger-GmbH/opcuanet-samples/tree/master/cs

namespace RobotecOPC_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            IPtextBox.Text = "172.31.1.147";
            PorttextBox.Text = "4840";
            UsernametextBox.Text = "OpcUaOperator";
            PasswordtextBox.Text = "kuka";
        }

        private OpcClient client;
        private string nodeID = "ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.";
 
        /// <summary>
        /// Подключаемся к серверу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton(object sender, EventArgs e)
        {
            try
            {
                // создаю клиента нового
                client = new OpcClient($"opc.tcp://{IPtextBox.Text}:{PorttextBox.Text}",
                    new OpcSecurityPolicy(OpcSecurityMode.None));

                // указываем пароль и логин
                client.Security.UserIdentity = new OpcClientIdentity(UsernametextBox.Text, PasswordtextBox.Text);

                client.Connecting += Client_Connecting;
                client.Connected += Client_Connected;
                client.Disconnecting += Client_Disconnecting;
                client.Disconnected += Client_Disconnected;

                // подклюаемся
                client.Connect(); // connection
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        /// <summary>
        /// отклюаемся от сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton(object sender, EventArgs e)
        {
            // проверяю существует ли клиент
            if (client != null)
            {
                // отключаю коннект к серверу
                client.Disconnect();
            }
        }

        private void ReadNodeButton_Click(object sender, EventArgs e)
        {
            // проверяю существует ли клиент
            if (client != null)
            {
                // считываю значения с сервера
                var value = client.ReadNode(nodeID + NodeNameReadTextBox.Text);
                //вывод значения на UI
                NodeReadValueTextBox.Text = value.ToString();
            }
        }

        private void WriteNodeButton_Click(object sender, EventArgs e)
        {
            // проверяю существует ли клиент
            if (client != null)
            {
                // беру значения с UI
                var id = nodeID + NodeNameWriteTextBox.Text;
                // запись значений в сервер
                // учитывай что тип данных надо указать конкретный
                client.WriteNode(id, double.Parse(NodeWriteValueTextBox.Text));
            }
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            StatusConnect.Text = $"Connect status: Disconnected";
        }

        private void Client_Disconnecting(object sender, EventArgs e)
        {
            StatusConnect.Text = $"Connect status: Disconnecting";
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            StatusConnect.Text = $"Connect status: Connected";
        }

        private void Client_Connecting(object sender, EventArgs e)
        {
            StatusConnect.Text = $"Connect status: Connecting ...";
        }

       
    }
}
