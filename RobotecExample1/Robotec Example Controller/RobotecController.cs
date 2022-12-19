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

            await _client.WriteNodeAsync(RWNodeData.R_START, true);
            await Task.Delay(500);
            await _client.WriteNodeAsync(RWNodeData.R_START, false);

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
            {
                _currentTransform = Robot.GetActualTransformRobot(_client);
                _targetTransform = Robot.GetTargetTransformRobot(_client);
                Task.Delay(500);
                TransformDataToView();
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

        private async void OutMessageAlarm()
        {
            try
            {
                while (_client.Session.State == CommunicationState.Opened)
                {
                    string mes = (string) await _client.ReadNodeAsync(RNodeData.OA_MESSAGE) ?? "Disconnected";
                    
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

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}