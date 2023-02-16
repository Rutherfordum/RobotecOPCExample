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

        private List<OpcClient> _clients = new List<OpcClient>()
        {
            new OpcClient("172.31.1.147:4840","OpcUaOperator", "kuka"), // 1 робот
            new OpcClient("172.31.1.148:4840","OpcUaOperator", "kuka"), // 2 робот
            new OpcClient("172.31.1.149:4840","OpcUaOperator", "kuka"), // 3 робот
            new OpcClient("172.31.1.150:4840","OpcUaOperator", "kuka")  // 4 робот
        };

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
                _clients?.ForEach(client => client.Disconnect());

                await Task.Run(() =>
                {
                    _clients.OnConnected += Connected;
                    _clients.OnDisconnected += Disconnected;
                    _clients.OnFaulted += Faulted;

                    if (_clients != null)
                        _clients?.Connect();
                });
            }
            catch (Exception e)
            {
                _view.SetConnectStatus(false);
            }
        }

        private void Disconnect()
        {
            _clients?.Disconnect();
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

            Robot.Stop(_clients);
        }

        #endregion

        #region Управление заданием

        private async void StartTask()
        {
            if (_clients == null) return;
            GetDataFromView();
            await Robot.SendParameters(_clients, _parameters);

            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Measurement.Start(_clients, task);
        }

        private async void StopTask()
        {
            if (_clients == null) return;

            await Robot.Stop(_clients);
        }

        private async void ContinueTask()
        {
            if (_clients == null) return;

            Enum_PG_TASK task = Enum_PG_TASK.NOTASK;

            if ((bool)_view.matrixToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_MATRIX;

            if ((bool)_view.positionToggle.IsChecked)
                task = Enum_PG_TASK.MAIN_POSITIONS;

            await Measurement.Continue(_clients, task);
        }

        private async void EndMeasurement()
        {
            if (_clients == null) return;
            await Measurement.Finish(_clients);
        }

        #endregion

        #region Управление роботом

        private async void PositionHome()
        {
            if (_clients == null) return;

            await Robot.GoToHome(_clients);
        }
        private async void ResetData()
        {
            if (_clients == null) return;

            await Robotec.Control.ManualControl.SendTransform(_clients, new Transform());
            await Robot.ResetParameters(_clients);
            ResetViewParameters();
        }
        private async void ResetError()
        {
            if (_clients == null) return;

            await _clients.WriteNodeAsync(RWNodeData.R_START, true);
            await Task.Delay(500);
            await _clients.WriteNodeAsync(RWNodeData.R_START, false);

        }
        private async void PauseContinue()
        {
            if (_clients == null) return;

            bool isPause = (bool)await _clients.ReadNodeAsync(RWNodeData.R_HOLD);

            if (!isPause)
                await Robot.Pause(_clients);
            else
                await Robot.Continue(_clients);
        }
        private async void EmergencyStop()
        {
            if (_clients == null) return;

            await Robot.EmergencyStop(_clients);
        }

        #endregion

        #region Управление захватом

        public async void OpenGripper()
        {
            if (_clients == null) return;

            await GripperControl.Open(_clients);
        }

        public async void CloseGripper()
        {
            if (_clients == null) return;

            await GripperControl.Close(_clients);
        }

        #endregion

        #region Ручное управление роботом

        private async void ManualControl()
        {
            if (_clients == null) return;

            GetDataFromView();
            await Robotec.Control.ManualControl.SendTransform(_clients, _manualTransform);
            await Robotec.Control.ManualControl.Enable(_clients);
        }

        private async void StartMovingManual()
        {
            if (_clients == null) return;

            GetDataFromView();
            await Robotec.Control.ManualControl.SendTransform(_clients, _manualTransform);
            await Robotec.Control.ManualControl.StartRobotMovement(_clients);
        }

        #endregion

        #region Чтение позиций робота

        private void Update()
        {
            if (_clients == null) return;

            OutMessageAlarm();
            while (_clients.Session.State == CommunicationState.Opened)
            {
                _currentTransform = Robot.GetActualTransformRobot(_clients);
                _targetTransform = Robot.GetTargetTransformRobot(_clients);
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
                while (_clients.Session.State == CommunicationState.Opened)
                {
                    string mes = (string)await _clients.ReadNodeAsync(RNodeData.OA_MESSAGE) ?? "Disconnected";

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