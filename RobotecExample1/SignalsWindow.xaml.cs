using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RobotecExample
{
    /// <summary>
    /// Логика взаимодействия для SignalsWindow.xaml
    /// </summary>
    public partial class SignalsWindow : Window
    {
        public SignalsWindow()
        {
            InitializeComponent();
        }
      
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Отменяем закрытие формы
            e.Cancel = true;
        }
    }
}
