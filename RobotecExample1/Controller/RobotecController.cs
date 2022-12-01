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
            _view.StartButtonEvent += _view_StartButtonEvent; ;


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
        }

        private void _view_ConnectButtonEvent()
        {
            _client?.Disconnect();

            if (_view.robot1.IsChecked == true)
            {
                _client = new OpcClient("", "", "");
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
        }

        public void Update()
        {
            if (_client == null) return;
            while ((_client.Session.State != CommunicationState.Faulted || _client.Session.State != CommunicationState.Closed)
                   && _client.Session.State == CommunicationState.Opened)
            {
                var transform = new Transform();
                transform.X = (float)_client.ReadNode("id");//тут считаешь ноду которая отвечает за X координату и т.д.


                _view.SetActualTransform(transform);
            }
        }
    }
}