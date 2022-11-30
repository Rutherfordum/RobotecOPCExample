using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotecExample.Utils;

namespace RobotecExample
{
    public partial class RobotecForm : Form
    {
        public RobotecForm()
        {
            InitializeComponent();
        }

        public event Action StartButtonEvent;
        public event Action StopButtonEvent;
        public event Action ClearButtonEvent;
        public event Action HomeButtonEvent;
        public event Action PauseNextButtonEvent;
        public event Action ManualControlButtonEvent;
        public event Action ConnectButtonEvent;
        public event Action DisconnectButtonEvent;

        public void SetActualTransform(Transform transform)
        {
            current_X.Text = transform.X.ToString();
            current_Y.Text = transform.Y.ToString();
            current_Z.Text = transform.Z.ToString();
            current_A.Text = transform.A.ToString();
            current_B.Text = transform.B.ToString();
            current_C.Text = transform.C.ToString();
        }

        public void SetTargetTransform(Transform transform)
        {
            target_X.Text = transform.X.ToString();
            target_Y.Text = transform.Y.ToString();
            target_Z.Text = transform.Z.ToString();
            target_A.Text = transform.A.ToString();
            target_B.Text = transform.B.ToString();
            target_C.Text = transform.C.ToString();
        }

        public int GetCurrentSelectedRobot()
        {
            if (radioButton1.Checked)
                return 1;


            else if (radioButton2.Checked)
                return 2;


            else if (radioButton3.Checked)
                return 3;


            else if (radioButton4.Checked)
                return 4;

            return 0;
        }

        public float GetCurrentSelectedHeight()
        {
            if (radioButton5.Checked)
                return 2.5f;

            if (radioButton6.Checked)
                return 5f;

            if (radioButton7.Checked)
                return 10f;

            if (radioButton8.Checked)
                return 20f;

            return 2.5f;
        }

        public bool[] GetCurrentSelectedCells()
        {
            return new[]
            {
                checkBox0.Checked, checkBox1.Checked, checkBox2.Checked,
                checkBox3.Checked, checkBox4.Checked, checkBox5.Checked,
                checkBox6.Checked, checkBox7.Checked, checkBox8.Checked,
                checkBox9.Checked, checkBox10.Checked, checkBox11.Checked,
                checkBox12.Checked, checkBox13.Checked, checkBox14.Checked,
                checkBox15.Checked, checkBox16.Checked, checkBox17.Checked,
                checkBox18.Checked, checkBox19.Checked, checkBox20.Checked,
                checkBox21.Checked, checkBox22.Checked, checkBox23.Checked,
                checkBox24.Checked, checkBox25.Checked, checkBox26.Checked,
                checkBox27.Checked, checkBox28.Checked, checkBox29.Checked,
                checkBox30.Checked, checkBox31.Checked, checkBox32.Checked,
                checkBox33.Checked, checkBox34.Checked, checkBox35.Checked,

            };
        }


        #region private methods

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartButtonEvent?.Invoke();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopButtonEvent?.Invoke();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearButtonEvent?.Invoke();
        }

        private void buttonPauseNext_Click(object sender, EventArgs e)
        {
            PauseNextButtonEvent?.Invoke();
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            HomeButtonEvent?.Invoke();
        }

        private void buttonManualControl_Click(object sender, EventArgs e)
        {
            ManualControlButtonEvent?.Invoke();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            ConnectButtonEvent?.Invoke();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectButtonEvent?.Invoke();
        }

        #endregion

    }
}
