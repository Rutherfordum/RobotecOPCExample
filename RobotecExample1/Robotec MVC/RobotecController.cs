using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using RobotecExample.Utils;
using Workstation.ServiceModel.Ua;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

// проверка с null при чтение позиций 
namespace RobotecExample.Controller
{
    public class RobotecController
    {
        private readonly MainWindow _view;
        private readonly RobotecModel _model;
        private OpcClient _client;

        private Transform _currentTransform;
        private Transform _targetTransform;

        public RobotecController(MainWindow view, RobotecModel model)
        {
            _view = view;
            _model = model;

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

        /// <summary>
        /// Сброс ошибок
        /// </summary>
        private void ResetError()
        {
            _client.WriteNode(_model.RESET_ERR, true);
            Thread.Sleep(1000);
            _client.WriteNode(_model.RESET_ERR, false);
        }

        /// <summary>
        /// Завершить измерение
        /// </summary>
        private void Finish()
        {
            _client.WriteNode(_model.D_FINISH, true);
            Thread.Sleep(1000);
            _client.WriteNode(_model.D_FINISH, false);

            MessageBox.Show("Измерение завершено", "", MessageBoxButton.OK);
        }

        /// <summary>
        /// Ручное управление
        /// </summary>
        private void ManualControl()
        {
            _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.MANUAL_JOG_DELTA]);
            Thread.Sleep(1000);
            _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.NOTASK]);
            SendDataToRobot();

            MessageBoxCustom(_model.R_START, _model.R_STOP, "Перейти в ручное управление?");
        }

        /// <summary>
        /// Сброс данных на OPC сервере
        /// </summary>
        private void ResetData()
        {
            _view.ResetCells();
            _view.ResetHeight();

            _client.WriteNode(_model.R_STOP, false);
            _client.WriteNode(_model.R_START, false);
            _client.WriteNode(_model.R_HOLD, false);
            _client.WriteNode(_model.MOV_HOME, false);
            _client.WriteNode(_model.D_FINISH, false);
            _client.WriteNode(_model.R_ALM, false);
            _client.WriteNode(_model.R_MOV_XYZ, false);
            _client.WriteNode(_model.R_PARAM_PORT, 0000);
            _client.WriteNode(_model.R_PARAM_ID, 0);
            _client.WriteNode(_model.R_PARAM_HEIGHT, (double)2.5);
            _client.WriteNode(_model.R_PARAM_POS_ID, 0);
            SendCellsMatrix(_view.GetSelectedCells());
        }

        /// <summary>
        /// Сброс данных на OPC сервере
        /// </summary>
        private void Clear()
        {
            ResetData();
        }

        private void Stop()
        {
            _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.NOTASK]);
            MessageBoxCustom(_model.R_STOP, "", "Остановить?");
        }

        private void Start()
        {
            if ((bool)_view.matrixToggle.IsChecked)
            {
                _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.MAIN_MATRIX]);
                Thread.Sleep(1000);
                _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.NOTASK]);
            }

            if ((bool)_view.positionToggle.IsChecked)
            {
                _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.MAIN_POSITIONS]);
                Thread.Sleep(1000);
                _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.NOTASK]);
            }

            SendDataToRobot();
            MessageBoxCustom(_model.R_START, _model.R_STOP, "Начать измерения?");
        }

        private void Pause()
        {
            bool isPause = (bool)_client.ReadNode(_model.R_HOLD);
            Thread.Sleep(1000);
            _client.WriteNode(_model.R_HOLD, !isPause);
        }

        private void MoveRobotToHome()
        {
            _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.RETURN_HOME]);
            Thread.Sleep(1000);
            _client.WriteNode(_model.PG_TASK, _model.TASK[Enum_PG_TASK.NOTASK]);

            MessageBoxCustom(_model.MOV_HOME, "", "Вернуть робота в положение домой?");
        }

        /// <summary>
        /// отправялем данные с АРМ в робота
        /// </summary>
        private void SendDataToRobot()
        {
            _client.WriteNode(_model.R_PARAM_PORT, _model.PORT1);
            _client.WriteNode(_model.R_PARAM_ID, _view.GetSelectedRobot());
            _client.WriteNode(_model.R_PARAM_HEIGHT, (Int32)_view.GetSelectedHeight());
            SendCellsMatrix(_view.GetSelectedCells());
            _client.WriteNode(_model.R_PARAM_POS_TAKE_ID, _view.FromCell);
            _client.WriteNode(_model.R_PARAM_POS_ID, _view.ToCell);
            SendManualTransformToRobot(_view.GetManualTransformMoving());
        }

        private void SendCellsMatrix(bool[] matrix)
        {
            int j = 0;
            bool[] matrixCells = matrix;

            for (int i = 0; i < matrixCells.Length; i++)
            {
                j = i + 1;
                bool value = matrixCells[i];
                _client.WriteNode(
                    $"{_model.Sample_Matrix_elem}{j}", value);
            }
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
                        _client = new OpcClient($"{_model.IP1}:{_model.PORT1}", _model.User,
                            _model.Password);
                        break;

                    case 2:
                        _client = new OpcClient($"{_model.IP2}:{_model.PORT2}", _model.User,
                            _model.Password);
                        break;

                    case 3:
                        _client = new OpcClient($"{_model.IP3}:{_model.PORT3}", _model.User,
                            _model.Password);
                        break;

                    case 4:
                        _client = new OpcClient($"{_model.IP4}:{_model.PORT4}", _model.User,
                            _model.Password);
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
               ResetData();

           });
            TransformDataToView();
        }

        private void Faulted()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate () { _view.SetConnectStatus(false); });
        }

        #endregion

        private void TransformDataToView()
        {
            Thread.Sleep(5000);

            if (_client == null) return;

            while ((_client.Session.State != CommunicationState.Faulted ||
                    _client.Session.State != CommunicationState.Closed)
                   && (_client.Session.State == CommunicationState.Opened))

            {
                //_currentTransform = _client.ReadTransformNode(_model.POS_ACT).GetAwaiter().GetResult();
                //_targetTransform = _client.ReadTransformNode(_model.POS_FOR).GetAwaiter().GetResult();
                Read_POS_ACT().GetAwaiter().GetResult();
                Read_POS_TARGET().GetAwaiter().GetResult();


                _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    _view.SetActualTransform(_currentTransform);
                    _view.SetTargetTransform(_targetTransform);
                });
            }
        }

        private async Task<Transform> Read_POS_ACT()
        {
            if (_client == null || _client.Session.State == CommunicationState.Faulted || _client.Session.State == CommunicationState.Closing || _client.Session.State == CommunicationState.Closed)
                return new Transform();

            if (_currentTransform == null)
                _currentTransform = new Transform();

            await Task.Run(() =>
            {
                Task.Delay(1000);
                var x = _client.ReadNode($"{_model.POS_ACT}X");
                var y = _client.ReadNode($"{_model.POS_ACT}Y");
                var z = _client.ReadNode($"{_model.POS_ACT}Z");
                var a = _client.ReadNode($"{_model.POS_ACT}A");
                var b = _client.ReadNode($"{_model.POS_ACT}B");
                var c = _client.ReadNode($"{_model.POS_ACT}C");

                if (x != null &&
                    y != null &&
                    z != null &&
                    a != null &&
                    b != null &&
                    c != null)
                {
                    _currentTransform.X = (float)(double)x;
                    _currentTransform.Y = (float)(double)y;
                    _currentTransform.Z = (float)(double)z;
                    _currentTransform.A = (float)(double)a;
                    _currentTransform.B = (float)(double)b;
                    _currentTransform.C = (float)(double)c;
                }

                return _currentTransform;
            });

            return _currentTransform;
        }

        private async Task<Transform> Read_POS_TARGET()
        {
            if (_client == null || _client.Session.State == CommunicationState.Faulted || _client.Session.State == CommunicationState.Closing || _client.Session.State == CommunicationState.Closed)
                return new Transform();

            if (_targetTransform == null)
                _targetTransform = new Transform();

            await Task.Run(() =>
            {
                Task.Delay(1000);

                var x = _client.ReadNode($"{_model.POS_FOR}X");
                var y = _client.ReadNode($"{_model.POS_FOR}Y");
                var z = _client.ReadNode($"{_model.POS_FOR}Z");
                var a = _client.ReadNode($"{_model.POS_FOR}A");
                var b = _client.ReadNode($"{_model.POS_FOR}B");
                var c = _client.ReadNode($"{_model.POS_FOR}C");

                if (x != null &&
                    y != null &&
                    z != null &&
                    a != null &&
                    b != null &&
                    c != null)
                {
                    _targetTransform.X = (float)(double)x;
                    _targetTransform.Y = (float)(double)y;
                    _targetTransform.Z = (float)(double)z;
                    _targetTransform.A = (float)(double)a;
                    _targetTransform.B = (float)(double)b;
                    _targetTransform.C = (float)(double)c;
                }

                return _targetTransform;
            });

            return _targetTransform;
        }

        private void SendManualTransformToRobot(Transform transform)
        {
            _client.WriteNode(_model.I_MOV_X, (double)transform.X);
            _client.WriteNode(_model.I_MOV_Y, (double)transform.Y);
            _client.WriteNode(_model.I_MOV_Z, (double)transform.Z);
            _client.WriteNode(_model.I_MOV_A, (double)transform.A);
            _client.WriteNode(_model.I_MOV_B, (double)transform.B);
            _client.WriteNode(_model.I_MOV_C, (double)transform.C);
        }

        /// <summary>
        /// Диалоговое окно
        /// </summary>
        /// <param name="nodeYes"> при подтверждении отправляем ноду </param>
        /// <param name="nodeNo"> при отмене отправляем ноду  </param>
        /// <param name="label"> текст диалогово окна </param>
        private void MessageBoxCustom(string nodeYes, string nodeNo, string label)
        {
            MessageBoxResult result = MessageBox.Show(label, "", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        _client.WriteNode(nodeYes, true);
                        Thread.Sleep(1000);
                        _client.WriteNode(nodeYes, false);
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        _client.WriteNode(nodeNo, true);
                        Thread.Sleep(1000);
                        _client.WriteNode(nodeNo, false);
                        break;
                    }
            }
        }
    }
}