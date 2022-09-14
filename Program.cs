using System;
using System.Windows.Forms;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace RobotecOPCExample
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationName = "UA Reference Client Robotec";
            application.ApplicationType = ApplicationType.Client;
            application.ConfigSectionName = "Quickstarts.ReferenceClientRobotec";

            try
            {

                // load the application configuration.
                application.LoadApplicationConfiguration(false).Wait();

                // check the application certificate.
                var certOK = application.CheckApplicationInstanceCertificate(false, 0).Result;
                if (!certOK)
                {
                    throw new Exception("Application instance certificate invalid!");
                }

                // run the application interactively.
                //Application.Run(new MainForm(application.ApplicationConfiguration));
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{application.ApplicationName} \n {e}");
            }

        }
    }
}
