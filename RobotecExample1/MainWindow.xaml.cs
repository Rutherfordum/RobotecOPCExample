using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Robotec;
using RobotecExample.Controller;

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
            robot1.IsChecked = true;
            height0.IsChecked = true;
            ResizeMode = ResizeMode.NoResize;
            positionToggle.Checked += PositionToggle_Checked;
            matrixToggle.Checked += MatrixToggle_Checked;
            matrixToggle.IsChecked = true;
            fromCell.MaxLength = 2;
            toCell.MaxLength = 2;
        }

        public event Action StartButtonEvent;
        public event Action StopButtonEvent;
        public event Action ClearButtonEvent;
        public event Action HomeButtonEvent;
        public event Action PauseStartButtonEvent;
        public event Action ManualControlButtonEvent;
        public event Action ManualControlStartButtonEvent;
        public event Action ConnectButtonEvent;
        public event Action DisconnectButtonEvent;
        public event Action FinishButtonEvent;
        public event Func<Task> ResetErrorButtonEvent;
        public event Action ContinueButtonEvent;


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

            return new Transform(x,y,z,a,b,c);
        }

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

        /// <summary>
        /// Возвращает высоту от пользователся
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Возвращает массив заполненых ячеек
        /// </summary>
        /// <returns></returns>
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

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            FinishButtonEvent?.Invoke();
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
            ManualControlStartButtonEvent?.Invoke();
        }

        private void openWindowSignals_Click(object sender, RoutedEventArgs e)
        {
            new SignalsFromRobot().ShowDialog();
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

        #endregion

        private void Continue_programm_Click(object sender, RoutedEventArgs e)
        {
            ContinueButtonEvent?.Invoke();
        }
    }
}
