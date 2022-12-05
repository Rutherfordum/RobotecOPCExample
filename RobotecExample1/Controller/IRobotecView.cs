using System;

namespace RobotecExample1.Controller
{
    public interface IRobotecView
    {
        public event Action StartButtonEvent;
        public event Action StopButtonEvent;
        public event Action ClearButtonEvent;
        public event Action HomeButtonEvent;
        public event Action PauseStartButtonEvent;
        public event Action ManualControlButtonEvent;
        public event Action ConnectButtonEvent;
        public event Action DisconnectButtonEvent;
        public event Action SendDataButtonEvent;

        public void SetActualTransform(Transform transform);
        public void SetConnectStatus(bool value);
        public void SetTargetTransform(Transform transform);
        public int GetCurrentSelectedRobot();
        public float GetCurrentSelectedHeight();
        public bool[] GetCurrentSelectedCells();
    }
}