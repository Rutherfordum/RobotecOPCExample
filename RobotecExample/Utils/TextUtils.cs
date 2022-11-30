using System.Windows.Forms;

namespace RobotecExample.Utils
{
    public static class TextUtils
    {
        public static void TextBoxOnlyDoubleType(KeyPressEventArgs e, TextBox textBox)
        {
            char ch = e.KeyChar;

            if (ch == 46 && textBox.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }

            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

    }
}