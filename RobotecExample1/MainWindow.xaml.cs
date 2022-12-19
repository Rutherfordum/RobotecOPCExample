using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Robotec;

namespace RobotecExample
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
            DataContext = controller;
            robot1.IsChecked = true;
            height0.IsChecked = true;
            ResizeMode = ResizeMode.NoResize;
            positionToggle.Checked += PositionToggle_Checked;
            matrixToggle.Checked += MatrixToggle_Checked;
            matrixToggle.IsChecked = true;
            fromCell.MaxLength = 2;
            toCell.MaxLength = 2;
        }

      //  public SignalsWindow SignalsFromRobotWindow = new SignalsWindow();

        public event Action StartTaskButtonEvent;
        public event Action StopTaskButtonEvent;
        public event Action ResetDataButtonEvent;
        public event Action PositionHomeButtonEvent;
        public event Action PauseContinueButtonEvent;
        public event Action ManualControlButtonEvent;
        public event Action StartMovingManualButtonEvent;
        public event Action ConnectButtonEvent;
        public event Action DisconnectButtonEvent;
        public event Action EndMeasurementButtonEvent;
        public event Action ResetErrorButtonEvent;
        public event Action ContinueTaskButtonEvent;
        public event Action EmergencyStopButtonEvent;
        public event Action OpenGripperButtonEvent;
        public event Action CloseGripperButtonEvent;


        private CheckBox[] _checkBoxes;

        public int FromCell => int.Parse(fromCell.Text);
        public int ToCell => int.Parse(toCell.Text);

        public Transform GetManualTransformMoving()
        {
            var x = float.Parse(moveX.Text);
            var y = float.Parse(moveY.Text);
            var z = float.Parse(moveZ.Text);
            var a = float.Parse(moveA.Text);
            var b = float.Parse(moveB.Text);
            var c = float.Parse(moveC.Text);

            return new Transform(x, y, z, a, b, c);
        }

        public void SetActualTransform(Transform transform)
        {
            currentX.Text = transform.X.ToString();
            currentY.Text = transform.Y.ToString();
            currentZ.Text = transform.Z.ToString();
            currentA.Text = transform.A.ToString();
            currentB.Text = transform.B.ToString();
            currentC.Text = transform.C.ToString();
        }

        public void SetConnectStatus(bool value)
        {
            connectStatus.Text = value ? "Connected" : "Disconnected";
        }

        public void SetTargetTransform(Transform transform)
        {
            targetX.Text = transform.X.ToString();
            targetY.Text = transform.Y.ToString();
            targetZ.Text = transform.Z.ToString();
            targetA.Text = transform.A.ToString();
            targetB.Text = transform.B.ToString();
            targetC.Text = transform.C.ToString();
        }

        public int GetSelectedRobot()
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

        public float GetSelectedHeight()
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

        public bool[] GetSelectedCells()
        {
            var boxes = GetCheckBoxCells();

            bool[] cells = new bool[boxes.Length + 1];

            for (var i = 0; i < boxes.Length; i++)
                cells[i] = (bool)boxes[i].IsChecked;

            return cells;
        }

        public void ResetCells()
        {
            var boxes = GetCheckBoxCells();

            foreach (var t in boxes)
                t.IsChecked = false;
        }

        public void ResetHeight()
        {
            height0.IsChecked = true;
        }

        public void ResetManualTransform()
        {
            moveX.Text = 0.ToString();
            moveY.Text = 0.ToString();
            moveZ.Text = 0.ToString();
            moveA.Text = 0.ToString();
            moveB.Text = 0.ToString();
            moveC.Text = 0.ToString();
        }

        public void ResetActualTransform()
        {
            currentX.Text = 0.ToString();
            currentY.Text = 0.ToString();
            currentZ.Text = 0.ToString();
            currentA.Text = 0.ToString();
            currentB.Text = 0.ToString();
            currentC.Text = 0.ToString();
        }

        public void ResetTargetTransform()
        {
            targetX.Text = 0.ToString();
            targetY.Text = 0.ToString();
            targetZ.Text = 0.ToString();
            targetA.Text = 0.ToString();
            targetB.Text = 0.ToString();
            targetC.Text = 0.ToString();
        }

        #region private methods

        private CheckBox[] GetCheckBoxCells()
        {
            _checkBoxes = new CheckBox[]
            {
                checkbox0,checkbox1,checkbox2,checkbox3,checkbox4,checkbox5,
                checkbox6, checkbox7, checkbox8,checkbox9,checkbox10,checkbox11,
                checkbox12, checkbox13,checkbox14,checkbox15,checkbox16,checkbox17,
                checkbox18, checkbox19, checkbox20,checkbox21,checkbox22,checkbox23,
                checkbox24, checkbox25, checkbox26,checkbox27,checkbox28,checkbox29,
                checkbox30, checkbox31, checkbox32,checkbox33,checkbox34,checkbox35,
            };
            return _checkBoxes;
        }

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
            StopTaskButtonEvent?.Invoke();
        }

        private void StartButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            StartTaskButtonEvent?.Invoke();
        }

        private void ClearButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            ResetDataButtonEvent?.Invoke();
        }

        private void PauseStartButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            PauseContinueButtonEvent?.Invoke();
        }

        private void HomeButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            PositionHomeButtonEvent?.Invoke();
        }

        private void ManualControlButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            ManualControlButtonEvent?.Invoke();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            EndMeasurementButtonEvent?.Invoke();
        }

        private void resetErrorButton_Click(object sender, RoutedEventArgs e)
        {
            ResetErrorButtonEvent?.Invoke();
        }

        private void MatrixToggle_Checked(object sender, RoutedEventArgs e)
        {
            moveMatrixPanel.Visibility = Visibility.Visible;
            movePositionPanel.Visibility = Visibility.Hidden;
        }

        private void PositionToggle_Checked(object sender, RoutedEventArgs e)
        {
            moveMatrixPanel.Visibility = Visibility.Hidden;
            movePositionPanel.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartMovingManualButtonEvent?.Invoke();
        }

        private void openWindowSignals_Click(object sender, RoutedEventArgs e)
        {
          /*  if (SignalsFromRobotWindow == null)
                SignalsFromRobotWindow = new SignalsWindow();

            SignalsFromRobotWindow.Show();*/
        }

        private void fromCell_KeyDown(object sender, KeyEventArgs e)
        {
            KeyConverter converter = new KeyConverter();
            string key = converter.ConvertToString(e.Key);

            if (!Char.IsDigit(key.ToCharArray()[0]) && key.ToCharArray()[0] != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        private void toCell_KeyDown(object sender, KeyEventArgs e)
        {
            KeyConverter converter = new KeyConverter();
            string key = converter.ConvertToString(e.Key);

            if (!Char.IsDigit(key.ToCharArray()[0]) && key.ToCharArray()[0] != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        private void Continue_programm_Click(object sender, RoutedEventArgs e)
        {
            ContinueTaskButtonEvent?.Invoke();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EmergencyStopButtonEvent?.Invoke();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenGripperButtonEvent?.Invoke();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CloseGripperButtonEvent?.Invoke();
        }
        #endregion
    }
}
