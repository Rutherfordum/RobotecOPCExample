using System.Threading.Tasks;
using System.Windows;
using Robotec.OPC;

namespace Robotec
{
    public static class Utils
    {
        public static async void MessageBoxCustomAsync(OpcClient client, string nodeYes, string nodeNo, string label)
        {
            MessageBoxResult result = MessageBox.Show(label, "", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        await client.WriteNodeAsync(nodeYes, true);
                        await Task.Delay(1000);
                        await client.WriteNodeAsync(nodeYes, false);
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        if (string.IsNullOrEmpty(nodeNo)) return;
                        await client.WriteNodeAsync(nodeNo, true);
                        await Task.Delay(1000);
                        await client.WriteNodeAsync(nodeNo, false);
                        break;
                    }
            }
        }

    }
}