using System;
using System.Windows;
using RobotecExample1.Controller;

namespace RobotecExample1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RobotecController controller = new RobotecController(this);
            robot1.IsChecked = true;
            height0.IsChecked = true;
        }

        public event Action StartButtonEvent;
        public event Action StopButtonEvent;
        public event Action ClearButtonEvent;
        public event Action HomeButtonEvent;
        public event Action PauseStartButtonEvent;
        public event Action ManualControlButtonEvent;
        public event Action ConnectButtonEvent;
        public event Action DisconnectButtonEvent;
        public event Action SendDataButtonEvent;


        /// <summary>
        /// Устанавлливает значения позиций робота в интерфейс
        /// </summary>
        /// <param name="transform"></param>
        public void SetActualTransform(Transform transform)
        {
            currentX.Text = transform.X.ToString();
            currentY.Text = transform.Y.ToString();
            currentZ.Text = transform.Z.ToString();
            currentA.Text = transform.A.ToString();
            currentB.Text = transform.B.ToString();
            currentC.Text = transform.C.ToString();
        }

        /// <summary>
        /// Устанавливаем статус подклюения
        /// </summary>
        /// <param name="value"></param>
        public void SetConnectStatus(bool value)
        {
            connectStatus.Text = value ? "Connected" : "Disconnected";
        }

        /// <summary>
        /// Устанавлливает значения позиций робота в интерфейс
        /// </summary>
        /// <param name="transform"></param>
        public void SetTargetTransform(Transform transform)
        {
            targetX.Text = transform.X.ToString();
            targetY.Text = transform.Y.ToString();
            targetZ.Text = transform.Z.ToString();
            targetA.Text = transform.A.ToString();
            targetB.Text = transform.B.ToString();
            targetC.Text = transform.C.ToString();
        }

        /// <summary>
        /// Возвращает выбранного робота пользователь на GUI
        /// </summary>
        /// <returns></returns>
        public int GetCurrentSelectedRobot()
        {
            if (robot1.IsChecked != null && (bool)robot1.IsChecked)
                return 1;


            if (robot2.IsChecked != null && (bool)robot2.IsChecked)
                return 2;


            if (robot3.IsChecked != null && (bool)robot3.IsChecked)
                return 3;


            if (robot4.IsChecked != null && (bool)robot4.IsChecked)
                return 4;

            return 0;
        }

        /// <summary>
        /// Возвращает высоту от пользователся
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSelectedHeight()
        {
            if (height0.IsChecked != null && (bool)height0.IsChecked)
                return 2.5f;

            if (height1.IsChecked != null && (bool)height1.IsChecked)
                return 5f;

            if (height2.IsChecked != null && (bool)height2.IsChecked)
                return 10f;

            if (height3.IsChecked != null && (bool)height3.IsChecked)
                return 20f;

            return 2.5f;
        }

        /// <summary>
        /// Возвращает массив заполненых ячеек
        /// </summary>
        /// <returns></returns>
        public bool[] GetCurrentSelectedCells()
        {
            return new bool[]
            {
                (bool) checkbox0.IsChecked, (bool) checkbox1.IsChecked, (bool) checkbox2.IsChecked,(bool) checkbox3.IsChecked,(bool) checkbox4.IsChecked,(bool) checkbox5.IsChecked,
                (bool) checkbox6.IsChecked, (bool) checkbox7.IsChecked, (bool) checkbox8.IsChecked,(bool) checkbox9.IsChecked,(bool) checkbox10.IsChecked,(bool) checkbox11.IsChecked,
                (bool) checkbox12.IsChecked, (bool) checkbox13.IsChecked,(bool) checkbox14.IsChecked,(bool) checkbox15.IsChecked,(bool) checkbox16.IsChecked,(bool) checkbox17.IsChecked,
                (bool) checkbox18.IsChecked, (bool) checkbox19.IsChecked, (bool) checkbox20.IsChecked,(bool) checkbox21.IsChecked,(bool) checkbox22.IsChecked,(bool) checkbox23.IsChecked,
                (bool) checkbox24.IsChecked, (bool) checkbox25.IsChecked, (bool) checkbox26.IsChecked,(bool) checkbox27.IsChecked,(bool) checkbox28.IsChecked,(bool) checkbox29.IsChecked,
                (bool) checkbox30.IsChecked, (bool) checkbox31.IsChecked, (bool) checkbox32.IsChecked,(bool) checkbox33.IsChecked,(bool) checkbox34.IsChecked,(bool) checkbox35.IsChecked,

            };
        }

        #region private methods

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectButtonEvent?.Invoke();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            DisconnectButtonEvent?.Invoke();
        }

        private void StopButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            StopButtonEvent?.Invoke();
        }

        private void StartButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            StartButtonEvent?.Invoke();
        }

        private void ClearButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            ClearButtonEvent?.Invoke();
        }

        private void PauseStartButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            PauseStartButtonEvent?.Invoke();
        }

        private void HomeButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            HomeButtonEvent?.Invoke();
        }

        private void ManualControlButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            ManualControlButtonEvent?.Invoke();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendDataButtonEvent?.Invoke();
        }

        #endregion
    }
}
