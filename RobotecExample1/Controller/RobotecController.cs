using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using RobotecExample1.Utils;
using Workstation.ServiceModel.Ua;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

// проверка с null при чтение позиций 
namespace RobotecExample1.Controller
{
    public class RobotecController
    {
        private MainWindow _view;
        private OpcClient _client;

        private Transform _currentTransform;
        private Transform _targetTransform;

        public RobotecController(MainWindow view)
        {
            _view = view;
            _view.StartButtonEvent += Start;
            _view.StopButtonEvent += Stop;
            _view.ClearButtonEvent += Clear;
            _view.HomeButtonEvent += MoveRobotToHome;
            _view.PauseStartButtonEvent += Pause;
            _view.ConnectButtonEvent += Connect;
            _view.DisconnectButtonEvent += Disconnect;
            _view.FinishButtonEvent += Finish;
            _view.ManualControlButtonEvent += ManualControl;
            _view.ResetErrorButtonEvent += ResetError;
        }

        private void ResetError()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            _client.WriteNode(RobotecData.RESET_ERR, true);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.RESET_ERR, false);
        }

        private void Finish()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            _client.WriteNode(RobotecData.D_FINISH, true);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.D_FINISH, false);

            MessageBox.Show("Измерение завершено", "", MessageBoxButton.OK);
        }

        private void ManualControl()
        {
            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.MANUAL_JOG_DELTA]);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.NOTASK]);
            MessageBoxCustom(RobotecData.MANUAL_CONTROL, RobotecData.R_STOP, "Перейти в ручное управление?");
        }

        public void ResetData()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            _view.ResetCells();
            _view.ResetHeight();

            _client.WriteNode(RobotecData.R_STOP, false);
            _client.WriteNode(RobotecData.R_START, false);
            _client.WriteNode(RobotecData.R_HOLD, false);
            _client.WriteNode(RobotecData.MOV_HOME, false);
            _client.WriteNode(RobotecData.D_FINISH, false);
            _client.WriteNode(RobotecData.R_ALM, false);
            _client.WriteNode(RobotecData.R_MOV_XYZ, false);
            _client.WriteNode(RobotecData.R_PARAM_PORT, 0000);
            _client.WriteNode(RobotecData.R_PARAM_ID, 0);
            _client.WriteNode(RobotecData.R_PARAM_HEIGHT, (double)2.5);
            _client.WriteNode(RobotecData.R_PARAM_POS_ID, 0);
            SendCellsMatrix(new bool[_view.GetCurrentSelectedCells().Length + 1]);
        }

        private void Clear()
        {
            ResetData();
        }

        private void Stop()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;
            MessageBoxCustom(RobotecData.R_STOP, "", "Остановить?");
        }

        private void Start()
        {
            SendDataToRobot();
            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.MAIN]);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.NOTASK]);
            MessageBoxCustom(RobotecData.R_START, RobotecData.R_STOP, "Начать измерения?");
        }

        private void Pause()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;
            bool isPause = (bool)_client.ReadNode(RobotecData.R_HOLD);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.R_HOLD, !isPause);
        }

        private void MoveRobotToHome()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.RETURN_HOME]);
            Thread.Sleep(1000);
            _client.WriteNode(RobotecData.PG_TASK, RobotecData.TASK[Enum_PG_TASK.NOTASK]);

            MessageBoxCustom(RobotecData.MOV_HOME, "", "Вернуть робота в положение домой?");
        }

        private void SendDataToRobot()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            _client.WriteNode(RobotecData.R_PARAM_PORT, 4840);
            _client.WriteNode(RobotecData.R_PARAM_ID, _view.GetCurrentSelectedRobot());
            _client.WriteNode(RobotecData.R_PARAM_HEIGHT, (Int32)_view.GetCurrentSelectedHeight());
            SendCellsMatrix(_view.GetCurrentSelectedCells());
        }

        private void SendCellsMatrix(bool[] matrix)
        {
            if (_client.Session.State != CommunicationState.Opened)
                return;

            int j = 0;
            bool[] matrixCells = matrix;

            for (int i = 0; i < matrixCells.Length; i++)
            {
                j = i + 1;
                bool value = matrixCells[i];
                _client.WriteNode(
                    $"{RobotecData.Sample_Matrix_elem}{j}", value);
            }
        }

        private void Disconnect()
        {
            _client?.Disconnect();
            _view.SetConnectStatus(false);
        }

        private async void Connect()
        {
            try
            {
                if (_client != null)
                    _client?.Disconnect();

                _view.connectStatus.Text = "Connection...";

                switch (_view.GetCurrentSelectedRobot())
                {
                    case 1:
                        _client = new OpcClient($"{RobotecData.IP1}:{RobotecData.PORT1}", RobotecData.User,
                            RobotecData.Password);
                        break;

                    case 2:
                        _client = new OpcClient($"{RobotecData.IP2}:{RobotecData.PORT2}", RobotecData.User,
                            RobotecData.Password);
                        break;

                    case 3:
                        _client = new OpcClient($"{RobotecData.IP3}:{RobotecData.PORT3}", RobotecData.User,
                            RobotecData.Password);
                        break;

                    case 4:
                        _client = new OpcClient($"{RobotecData.IP4}:{RobotecData.PORT4}", RobotecData.User,
                            RobotecData.Password);
                        break;
                }

                await Task.Run(() =>
                {
                    _client.OnConnected += Connected;
                    _client.OnDisconnected += Disconnected;
                    _client.OnFaulted += Faulted;

                    if (_client != null)
                        _client?.Connect();
                });
            }
            catch (Exception e)
            {
                _view.SetConnectStatus(false);
            }
        }

        private void Disconnected()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                _view.SetConnectStatus(false);
            });
        }

        private void Connected()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                _view.SetConnectStatus(true);
                ResetData();
            });
        }

        private void Faulted()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                _view.SetConnectStatus(false);
            });
        }

        private void TransformToView()
        {
            Thread.Sleep(5000);

            if (_client == null) return;

            while ((_client.Session.State != CommunicationState.Faulted ||
                    _client.Session.State != CommunicationState.Closed)
                   && (_client.Session.State == CommunicationState.Opened ||
                       _client.Session.State == CommunicationState.Opening))

            {
                Thread.Sleep(5000);

                _view.SetActualTransform(Read_POS_ACT());
                _view.SetTargetTransform(Read_POS_TARGET());
            }
        }

        private Transform Read_POS_ACT()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return new Transform(0, 0, 0, 0, 0, 0);

            if (_currentTransform == null)
                _currentTransform = new Transform();

            _currentTransform.X =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.X");
            _currentTransform.Y =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.Y");
            _currentTransform.Z =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.Z");
            _currentTransform.A =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.A");
            _currentTransform.B =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.B");
            _currentTransform.C =
                (float)(double)_client.ReadNode($"{RobotecData.POS_ACT}.C");

            return _currentTransform;
        }

        private Transform Read_POS_TARGET()
        {
            if (_client.Session.State != CommunicationState.Opened)
                return new Transform(0, 0, 0, 0, 0, 0);

            if (_targetTransform == null)
                _targetTransform = new Transform();

            _targetTransform.X =
                (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.X");
            _targetTransform.Y =
                (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.Y");
            _targetTransform.Z =
                (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.Z");
            _targetTransform.A =
                (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.A");
            _targetTransform.B =
          (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.B");
            _targetTransform.C =
                (float)(double)_client.ReadNode($"{RobotecData.POS_FOR}.C");

            return _targetTransform;
        }

        private void MessageBoxCustom(string nodeYes, string nodeNo, string label)
        {
            MessageBoxResult result = MessageBox.Show(label, "", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        if (_client.Session.State != CommunicationState.Opened)
                            return;
                        _client.WriteNode(nodeYes, true);
                        Thread.Sleep(1000);
                        _client.WriteNode(nodeYes, false);
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        if (_client.Session.State != CommunicationState.Opened)
                            return;
                        _client.WriteNode(nodeNo, true);
                        Thread.Sleep(1000);
                        _client.WriteNode(nodeNo, false);
                        break;
                    }
            }
        }
    }
}