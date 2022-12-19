using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robotec;
using Robotec.OPC;
using Robotec.R_Nodes;
using Robotec.RW_Nodes;
using RobotecExample.Annotations;
using RobotecExample.Scripts;
using RobotecExample.Scripts.Static_Class;
using Workstation.ServiceModel.Ua;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace RobotecExample
{
    public class RobotecController : INotifyPropertyChanged
    {
        private readonly MainWindow _view;

        private OpcClient _client;
        private Parameters _parameters;
        private Transform _manualTransform;
        private Transform _currentTransform;
        private Transform _targetTransform;

        private bool IN_HOME;
        private string PRO_STATE;
        private bool JOG_FINISH;

        private bool T1;
        private bool EXT;
        private bool ALARM_STOP;
        private bool USER_SAF;
        private bool STOPMESS;
        private string MODEOP;
        private string MessageStatus;

        private string _statusMessage;

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public RobotecController(MainWindow view)
        {
            _view = view;

            _view.ConnectButtonEvent += Connect;
            _view.DisconnectButtonEvent += Disconnect;

            _view.StartTaskButtonEvent += StartTask;
            _view.StopTaskButtonEvent += StopTask;
            _view.ContinueTaskButtonEvent += ContinueTask;
            _view.EndMeasurementButtonEvent += EndMeasurement;

            _view.PositionHomeButtonEvent += PositionHome;
            _view.ResetDataButtonEvent += ResetData;
            _view.ResetErrorButtonEvent += ResetError;
            _view.PauseContinueButtonEvent += PauseContinue;
            _view.EmergencyStopButtonEvent += EmergencyStop;

            _view.ManualControlButtonEvent += ManualControl;
            _view.StartMovingManualButtonEvent += StartMovingManual;
            _view.OpenGripperButtonEvent += OpenGripper;
            _view.CloseGripperButtonEvent += CloseGripper;

        }

        #region Подключение и отключения от сервера

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

        private void Disconnect()
        {
            _client?.Disconnect();
            _view.SetConnectStatus(false);
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

            // чтение актуальной позиции робота после подключения 
            Update();
        }

        private void Faulted()
        {
            _view.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate () { _view.SetConnectStatus(false); });

            Robot.Stop(_client);
        }

        #endregion

        #region Управление заданием

        private async void StartTask()
        {
            if (_client == null) return;
            GetDataFromView();
            await Robot.SendParameters(_client, _parameters);

            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Measurement.Start(_client, task);
        }

        private async void StopTask()
        {
            if (_client == null) return;

            await Robot.Stop(_client);
        }

        private async void ContinueTask()
        {
            if (_client == null) return;

            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Measurement.Continue(_client, task);
        }

        private async void EndMeasurement()
        {
            if (_client == null) return;
            await Measurement.Finish(_client);
        }

        #endregion

        #region Управление роботом

        private async void PositionHome()
        {
            if (_client == null) return;

            await Robot.GoToHome(_client);
        }
        private async void ResetData()
        {
            if (_client == null) return;

            await Robotec.Control.ManualControl.SendTransform(_client, new Transform());
            await Robot.ResetParameters(_client);
            ResetViewParameters();
        }
        private async void ResetError()
        {
            if (_client == null) return;

            await Robot.ResetError(_client);
        }
        private async void PauseContinue()
        {
            if (_client == null) return;

            bool isPause = (bool)await _client.ReadNodeAsync(RWNodeData.R_HOLD);

            if (!isPause)
                await Robot.Pause(_client);
            else
                await Robot.Continue(_client);
        }
        private async void EmergencyStop()
        {
            if (_client == null) return;

            await Robot.EmergencyStop(_client);
        }

        #endregion

        #region Управление захватом

        public async void OpenGripper()
        {
            if (_client == null) return;

            await GripperControl.Open(_client);
        }

        public async void CloseGripper()
        {
            if (_client == null) return;

            await GripperControl.Close(_client);
        }

        #endregion

        #region Ручное управление роботом

        private async void ManualControl()
        {
            if (_client == null) return;

            GetDataFromView();
            await Robotec.Control.ManualControl.SendTransform(_client, _manualTransform);
            await Robotec.Control.ManualControl.Enable(_client);
        }

        private async void StartMovingManual()
        {
            if (_client == null) return;

            GetDataFromView();
            await Robotec.Control.ManualControl.SendTransform(_client, _manualTransform);
            await Robotec.Control.ManualControl.StartRobotMovement(_client);
        }

        #endregion

        #region Чтение позиций робота

        private void Update()
        {
            if (_client == null) return;

            OutMessageAlarm();
            while (_client.Session.State == CommunicationState.Opened)

            //(_client.Session.State != CommunicationState.Faulted ||
            //_client.Session.State != CommunicationState.Closed)
            //    &&
            {
                _currentTransform = Robot.GetActualTransformRobot(_client);
                _targetTransform = Robot.GetTargetTransformRobot(_client);
                //     ReadAllSignalsFromRobot();
                Task.Delay(500);
                TransformDataToView();
                //      ReadedSignalsFromRobotToView();
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


        #endregion

        //public Dictionary<MessageEnum, string> MessageAlm = new Dictionary<MessageEnum, string>()
        //{
        //    {MessageEnum.NO_MESSAGE, "#NO_MESSAGE"},
        //    {MessageEnum.CANT_START_SAMPLE_INGRIP, "#CANT_START_SAMPLE_INGRIP"},
        //    {MessageEnum.NO_SAMPLE_IN_SLOT, "#NO_SAMPLE_IN_SLOT"},
        //    {MessageEnum.SAMPLE_LOST, "#SAMPLE_LOST"},
        //    {MessageEnum.GRIPPER_COLLISION, "#GRIPPER_COLLISION"},
        //    {MessageEnum.NOT_IN_HOME, "#NOT_IN_HOME"}
        //};

        private async void OutMessageAlarm()
        {
            try
            {
                while (_client.Session.State == CommunicationState.Opened)
                {
                    string mes =
                        (string) await _client.ReadNodeAsync(
                            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.OA_MESSAGE") ?? "Disconnected";
                    switch (mes)
                    {
                        case "Disconnected":
                            StatusMessage = "Связь с роботом потеряна!";
                            break;
                        case "#NO_MESSAGE":
                            StatusMessage = "";
                            break;

                        case "#CANT_START_SAMPLE_INGRIP":
                            StatusMessage = "Внимание образец в захвате, продолжение\\старт задания невозможен!";
                            break;

                        case "#NO_SAMPLE_IN_SLOT":
                            StatusMessage = "Образец в ячейке не обнаружен!";
                            break;

                        case "#SAMPLE_LOST":
                            StatusMessage = "Образец утерян во время выполнения задания, проверьте закрытие захвата!";
                            break;

                        case "#GRIPPER_COLLISION":
                            StatusMessage = "Обнаружена коллизия захвата, комплекс остановлен!";
                            break;

                        case "#NOT_IN_HOME":
                            StatusMessage = "Робот не достиг домашней позиции!";
                            break;

                        case "#GRIP_ERROR_WHEN_OPENED":
                            StatusMessage = "Ошибка при открытие захвата! Проверьте захват!";
                            break;

                        case "#NO_SAFEBACK_POSITION":
                            StatusMessage = "Нет безопасного пути возврат в домашнее положение!";
                            break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void GetDataFromView()
        {
            _parameters.CellMatrix = _view.GetSelectedCells();
            _parameters.Port = "4840";
            _parameters.Robot_id = _view.GetSelectedRobot();
            _parameters.Height = _view.GetSelectedHeight();
            _parameters.TakeFromCell = _view.FromCell;
            _parameters.PutToCell = _view.ToCell;
            _manualTransform = _view.GetManualTransformMoving();
        }

        private void ResetViewParameters()
        {
            _view.ResetCells();
            _view.ResetActualTransform();
            _view.ResetHeight();
            _view.ResetManualTransform();
            _view.ResetTargetTransform();
        }

        private async void ReadAllSignalsFromRobot()
        {
            T1 = (bool)await _client.ReadNodeAsync(RNodeData.T1);
            JOG_FINISH = (bool)await _client.ReadNodeAsync(RNodeData.JOG_FINISH);
            EXT = (bool)await _client.ReadNodeAsync(RNodeData.EXT);
            ALARM_STOP = (bool)await _client.ReadNodeAsync(RNodeData.ALARM_STOP);
            USER_SAF = (bool)await _client.ReadNodeAsync(RNodeData.USER_SAF);
            STOPMESS = (bool)await _client.ReadNodeAsync(RNodeData.STOPMESS);
            MODEOP = (string)await _client.ReadNodeAsync(RNodeData.ModeOP);
            IN_HOME = (bool)await _client.ReadNodeAsync(RNodeData.IN_HOME);
            PRO_STATE = (string)await _client.ReadNodeAsync(RNodeData.PRO_STATE);
        }

        private void ReadedSignalsFromRobotToView()
        {
            /*  _view.SignalsFromRobotWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
              {
                  _view.SignalsFromRobotWindow.text4.Text = "T1: " + T1.ToString();
                  _view.SignalsFromRobotWindow.text3.Text = "JOG_FINISH: " + JOG_FINISH.ToString();
                  _view.SignalsFromRobotWindow.text1.Text = "IN_HOME: " + IN_HOME.ToString();
                  _view.SignalsFromRobotWindow.text2.Text = "PRO_STATE: " + PRO_STATE;
                  _view.SignalsFromRobotWindow.text5.Text = "EXT: " + EXT.ToString();
                  _view.SignalsFromRobotWindow.text6.Text = "ALARM_STOP: " + ALARM_STOP.ToString();
                  _view.SignalsFromRobotWindow.text7.Text = "USER_SAF: " + USER_SAF.ToString();
                  _view.SignalsFromRobotWindow.text8.Text = "STOPMESS: " + STOPMESS.ToString();
                  _view.SignalsFromRobotWindow.text9.Text = "MODEOP: " + MODEOP;
              });*/
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}