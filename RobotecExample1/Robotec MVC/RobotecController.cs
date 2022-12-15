using System;
using System.Threading;
using System.Threading.Tasks;
using Robotec;
using Robotec.OPC;
using Robotec.R_Nodes;
using Workstation.ServiceModel.Ua;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace RobotecExample.Controller
{
    // Вывести сигналы на АРМ все 
    // Учесть все возможные варианты программы и аварий
    // КАкие развития события могут быть, аварийно возвращаем образце на место, продолжаем программу далее
    // Выход в дом, продолжить выполнение программы
    // При останове не выходить из программы, а продолжить
    // робот на 100% запустить, все движения отработать, движения подтюнены под что ?!
    // Структурировать программы на роботе
    // Контролировать какие образцы проверены
    // Баг, у робота есть образец в гриппере, если начнем программу с 0 то, он может ударить
    // им в ячейку и сломаться, надо отселижавть есть ли в гриппере образце
    // Отслеживать положение грииппера, Поломка захвата, всегда закрывать иначе сгорит !!!

    public class RobotecController
    {
        private readonly MainWindow _view;

        private OpcClient _client;

        private Transform _currentTransform = new Transform();
        private Transform _targetTransform = new Transform();
        private Parameters _parameters = new Parameters();
        private Transform _manualTransform = new Transform();

        public RobotecController(MainWindow view)
        {
            _view = view;
            _view.StartButtonEvent += Start;
            _view.StopButtonEvent += Stop;
            _view.ClearButtonEvent += Clear;
            _view.HomeButtonEvent += ToHome;
            _view.PauseStartButtonEvent += Pause;
            _view.ConnectButtonEvent += Connect;
            _view.DisconnectButtonEvent += Disconnect;
            _view.FinishButtonEvent += FinishMeasurements;
            _view.ManualControlButtonEvent += ManualControl;
            _view.ManualControlStartButtonEvent += ManualControlStart;
            _view.ResetErrorButtonEvent += ResetError;
            _view.ContinueButtonEvent += _view_ContinueButtonEvent; ;

        }

        private async void _view_ContinueButtonEvent()
        {
            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Robotec.Utils.StartMeasurementsAsync(_client, task);
        }

        private async Task ResetError()
        {
            await Robotec.Utils.ResetErrorAsync(_client);
        }

        private async void FinishMeasurements()
        {
            await Robotec.Utils.FinishMeasurementsAsync(_client);
        }

        private async void ManualControl()
        {
            GetDataFromView();

            await Robotec.Utils.SendDataToRobot(_client, _parameters, _manualTransform);
            await Robotec.Utils.ManualControlModeAsync(_client);
        }

        private async void ManualControlStart()
        {
            GetDataFromView();
            await Robotec.Utils.SendDataToRobot(_client, _parameters, _manualTransform);
            await Robotec.Utils.ManualControlStartAsync(_client);
        }

        private void Clear()
        {
            GetDataFromView();
            Reset();
        }

        private async void Stop()
        {
            await Robotec.Utils.StopMeasurementsAsync(_client);
        }

        private async void Start()
        {
            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;
            GetDataFromView();

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Robotec.Utils.SendDataToRobot(_client, _parameters, _manualTransform);
            await Robotec.Utils.StartMeasurementsAsync(_client, task);
        }


        private async void Pause()
        {
            await Robotec.Utils.PauseMeasurementsAsync(_client);
        }

        private async void ToHome()
        {
            await Robotec.Utils.RobotToHomeAsync(_client);
        }

        #region Подключение и отключения от сервера

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

                switch (_view.GetSelectedRobot())
                {
                    case 1:
                        _client = new OpcClient("172.31.1.147:4840", "OpcUaOperator",
                            "kuka");
                        break;

                    case 2:
                        break;

                    case 3:
                        break;

                    case 4:
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
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate () { _view.SetConnectStatus(false); });
        }

        private void Connected()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                _view.SetConnectStatus(true);
                GetDataFromView();
            });

           // Reset();
            Update();
        }

        private async void Reset()
        {
            await Robotec.Utils.ResetData(_client, 36);
        }


        private void Faulted()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate () { _view.SetConnectStatus(false); });
        }

        #endregion

        

        private void GetDataFromView()
        {
            _parameters.CellMatrix = _view.GetSelectedCells();
            _parameters.Port = "4840";
            _parameters.Robot_id = _view.GetSelectedRobot();
            _parameters.Height = _view.GetSelectedHeight()*10;
            _parameters.TakeFromCell = _view.FromCell;
            _parameters.PutToCell = _view.ToCell;
            _manualTransform = _view.GetManualTransformMoving();
        }

        private async void Update()
        {
            if (_client == null) return;

            while ((_client.Session.State != CommunicationState.Faulted ||
                    _client.Session.State != CommunicationState.Closed)
                   && (_client.Session.State == CommunicationState.Opened))

            {
                TransformDataToView();
                await Task.Delay(500);
                _currentTransform = await _client.ReadTransformNodeAsync(RNodeData.POS_ACT);
                _targetTransform = await _client.ReadTransformNodeAsync(RNodeData.POS_FOR);

            }
        }

        private void TransformDataToView()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                _view.SetActualTransform(_currentTransform);
                _view.SetTargetTransform(_targetTransform);
            });
        }
    }
}