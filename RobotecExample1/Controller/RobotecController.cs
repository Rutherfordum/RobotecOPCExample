using System;
using System.Threading;
using Workstation.ServiceModel.Ua;

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
            _view.ConnectButtonEvent += _view_ConnectButtonEvent;
            _view.DisconnectButtonEvent += _view_DisconnectButtonEvent;
            _view.SendDataButtonEvent += _view_SendDataButtonEvent;
            _view.HomeButtonEvent += _view_HomeButtonEvent;
            _view.ClearButtonEvent += _view_ClearButtonEvent;
            _view.PauseStartButtonEvent += _view_PauseStartButtonEvent;
            _view.StartButtonEvent += _view_StartButtonEvent;
            _view.StopButtonEvent += _view_StopButtonEvent;
        }

        private void _view_StopButtonEvent()
        {
            if (_client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP", true);
                Thread.Sleep(1000);
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP", false);
            }
        }

        private void _view_StartButtonEvent()
        {
            if (_client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START", true);
                Thread.Sleep(1000);
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START", false);
            }
        }

        private void _view_PauseStartButtonEvent()
        {
            if (_client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD", true);
                Thread.Sleep(1000);
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD", false);
            }
        }

        private void _view_ClearButtonEvent()
        {

        }

        private void _view_HomeButtonEvent()
        {
            if (_client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME", true);
                Thread.Sleep(1000);
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME", false);
            }
        }

        private void _view_SendDataButtonEvent()
        {
            if (_client.Session.State == CommunicationState.Opened)
            {
                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.CONN_PORT",
                    4840);

                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.ROB_ID",
                    _view.GetCurrentSelectedRobot());

                _client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_HEIGHT",
                    (Int32)_view.GetCurrentSelectedHeight());



                int j = 0;
                bool[] matrixCells = _view.GetCurrentSelectedCells();

                for (int i = 0; i < matrixCells.Length; i++)
                {
                    j = i + 1;
                    bool value = matrixCells[i];
                    _client.WriteNode(
                        $"ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.Sample Matrix elem{j}",
                        value);
                }
            }

        }

        private void _view_DisconnectButtonEvent()
        {
            _client?.Disconnect();
            _view.SetConnectStatus(false);
        }

        //Подключение к роботу
        private void _view_ConnectButtonEvent()
        {
            try
            {
                _client?.Disconnect();

                switch (_view.GetCurrentSelectedRobot())
                {
                    case 1:
                        _client = new OpcClient("172.31.1.147:4840", "OpcUaOperator", "kuka");
                        break;


                    case 2:
                        _client = new OpcClient("10.91.75.144:4840", "OpcUaOperator", "kuka");
                        break;


                    case 3:
                        _client = new OpcClient("10.91.75.144:4840", "OpcUaOperator", "kuka");
                        break;


                    case 4:
                        _client = new OpcClient("10.91.75.144:4840", "OpcUaOperator", "kuka");
                        break;
                }

                _client?.Connect();
                _view.SetConnectStatus(true);
                Update();
            }
            catch (Exception e)
            {
                _view.SetConnectStatus(false);
            }
        }

        public void Update()
        {
            /* if (_client == null) return;

             while ((_client.Session.State != CommunicationState.Faulted ||
                         _client.Session.State != CommunicationState.Closed)
                        && (_client.Session.State == CommunicationState.Opened ||
                            _client.Session.State == CommunicationState.Opening))

             {
                 Thread.Sleep(5000);
                 {
                     #region CurrentTransform

                     if (_currentTransform == null)
                         _currentTransform = new Transform();

                     _currentTransform.X = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.X");
                     _currentTransform.Y = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.Y");
                     _currentTransform.Z = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.Z");
                     _currentTransform.A = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.A");
                     _currentTransform.B = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.B");
                     _currentTransform.C = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.C");

                     _view.SetActualTransform(_currentTransform);

                     #endregion

                     #region TragetTransform

                     if (_targetTransform == null)
                         _targetTransform = new Transform();

                     _targetTransform.X = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.X");
                     _targetTransform.Y = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.Y");
                     _targetTransform.Z = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.Z");
                     _targetTransform.A = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.A");
                     _targetTransform.B = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.B");
                     _targetTransform.C = (float)(double)_client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.C");

                     _view.SetTargetTransform(_targetTransform);

                     #endregion
                 }
             */

        }
    }
}