using System;
using Workstation.ServiceModel.Ua;

namespace RobotecExample1.Controller
{
    public class RobotecController
    {
        private MainWindow _view;
        private OpcClient _client;

        public RobotecController(MainWindow view)
        {
            _view = view;
            _view.ConnectButtonEvent += _view_ConnectButtonEvent;
            _view.DisconnectButtonEvent += _view_DisconnectButtonEvent;
            _view.StartButtonEvent += _view_StartButtonEvent;
        }

        private void _view_StartButtonEvent()
        {
            if ((_client.Session.State != CommunicationState.Faulted ||
                 _client.Session.State != CommunicationState.Closed)
                && _client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("id", "комманду"); // тут комманду отправляешь на Старт 
            }
        }

        private void _view_DisconnectButtonEvent()
        {
            _client?.Disconnect();
            _view.SetConnectStatus(false);
        }

        private void _view_ConnectButtonEvent()
        {
            try
            {
                _client?.Disconnect();

                if (_view.robot1.IsChecked == true)
                {
                    _client = new OpcClient("26.102.84.104", "OpcUaOperator", "kuka");
                }
                if (_view.robot2.IsChecked == true)
                {
                    _client = new OpcClient("", "", "");
                }
                if (_view.robot3.IsChecked == true)
                {
                    _client = new OpcClient("", "", "");
                }
                if (_view.robot4.IsChecked == true)
                {
                    _client = new OpcClient("", "", "");
                }

                _client?.Connect();
                _view.SetConnectStatus(true);
            }
            catch (Exception e)
            {
                _view.SetConnectStatus(false);
            }
            
        }

        public void Update()
        {
            if (_client == null) return;
            while ((_client.Session.State != CommunicationState.Faulted || _client.Session.State != CommunicationState.Closed)
                   && _client.Session.State == CommunicationState.Opened)
            {
                var transform = new Transform();
                transform.X = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.X");//тут считаешь ноду которая отвечает за X координату и т.д.
                transform.Y = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.Y");//тут считаешь ноду которая отвечает за X координату и т.д.
                transform.Z = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.Z");//тут считаешь ноду которая отвечает за X координату и т.д.
                transform.A = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.A");//тут считаешь ноду которая отвечает за X координату и т.д.
                transform.B = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.B");//тут считаешь ноду которая отвечает за X координату и т.д.
                transform.C = (float)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.C");//тут считаешь ноду которая отвечает за X координату и т.д.
                _view.SetActualTransform(transform);
            }
        }
    }
}