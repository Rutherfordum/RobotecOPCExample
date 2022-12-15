using System;
using System.Threading.Tasks;
using System.Windows;
using Robotec.OPC;
using Robotec.RW_Nodes;
using Workstation.ServiceModel.Ua;

namespace Robotec
{
    public static class Utils
    {
        public static async Task ResetErrorAsync(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNode(RWNodeData.RESET_ERR, true);
            await Task.Delay(1000);
            await client.WriteNode(RWNodeData.RESET_ERR, false);

            MessageBox.Show("Ошибки были сброшены", "", MessageBoxButton.OK);
        }

        public static async Task FinishMeasurementsAsync(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNode(RWNodeData.D_FINISH, true);
            await Task.Delay(1000);
            await client.WriteNode(RWNodeData.D_FINISH, false);
            MessageBox.Show("Измерение завершено", "", MessageBoxButton.OK);
        }

        public static async Task ManualControlModeAsync(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.MANUAL_JOG_DELTA]);
            await Task.Delay(1000);
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            MessageBoxCustomAsync(client, RWNodeData.MANUAL_JOG, "", "Перейти в ручное управление?");
        }

        public static async Task ManualControlStartAsync(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Начать движение?");
        }

        public static async Task StartMeasurementsAsync(OpcClient client, Enum_PG_TASK task)
        {
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[task]);
            await Task.Delay(1000);
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            MessageBoxCustomAsync(client, RWNodeData.R_START, RWNodeData.R_STOP, "Начать измерения ?");
        }

        public static async Task StopMeasurementsAsync(OpcClient client)
        {
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);
            MessageBoxCustomAsync(client, RWNodeData.R_STOP, "", "Вы действительно хотите остановить робота ?");
        }

        public static async Task PauseMeasurementsAsync(OpcClient client)
        {
            bool isPause = (bool)await client.ReadNodeAsync(RWNodeData.R_HOLD);
            await Task.Delay(1000);
            await client.WriteNode(RWNodeData.R_HOLD, !isPause);
        }

        public static async Task RobotToHomeAsync(OpcClient client)
        {
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.RETURN_HOME]);
            await Task.Delay(500);
            await client.WriteNode(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            await client.WriteNode(RWNodeData.MOVE_HOME, true);
            await Task.Delay(500);
            await client.WriteNode(RWNodeData.MOVE_HOME, false);

            MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Вернуть робота в положение домой?");
        }

        public static async Task ResetData(OpcClient client, int cellCount)
        {
            await client.WriteNode(RWNodeData.R_STOP, false);
            await client.WriteNode(RWNodeData.R_START, false);
            await client.WriteNode(RWNodeData.R_HOLD, false);
            await client.WriteNode(RWNodeData.MOVE_HOME, false);
            await client.WriteNode(RWNodeData.D_FINISH, false);
            await client.WriteNode(RWNodeData.R_ALM, false);
            await client.WriteNode(RWNodeData.R_MOVE_XYZ, false);
            await client.WriteNode(RWNodeData.R_PARAM_CONN_PORT, 0000);
            await client.WriteNode(RWNodeData.R_PARAM_ROB_ID, 0);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_HEIGHT, (double)2.5);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_POS_ID, 0);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_TAKE_POS_ID, 0);
            await SendCellMatrix(client, new bool[cellCount]);
            MessageBox.Show("Данные очищены", "", MessageBoxButton.OK);
        }

        public static async Task SendDataToRobot(OpcClient client, Parameters parameters, Transform manualTransform)
        {
            await client.WriteNode(RWNodeData.R_PARAM_CONN_PORT, parameters.Port);
            await client.WriteNode(RWNodeData.R_PARAM_ROB_ID, parameters.Robot_id);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_HEIGHT, parameters.Height);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_TAKE_POS_ID, parameters.TakeFromCell);
            await client.WriteNode(RWNodeData.R_PARAM_SAMPLE_POS_ID, parameters.PutToCell);
            await SendCellMatrix(client, parameters.CellMatrix);
            await SendManualTransform(client, manualTransform);
        }

        private static async Task SendManualTransform(OpcClient client, Transform transform)
        {
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}X", (double)transform.X);
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}Y", (double)transform.Y);
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}Z", (double)transform.Z);
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}A", (double)transform.A);
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}B", (double)transform.B);
            await client.WriteNode($"{RWNodeData.MOVE_XYZ}C", (double)transform.C);
        }

        private static async Task SendCellMatrix(OpcClient client, bool[] matrix)
        {
            int j = 0;
            bool[] matrixCells = matrix;

            for (int i = 0; i < matrixCells.Length; i++)
            {
                j = i + 1;
                bool value = matrixCells[i];
                await client.WriteNode(
                    $"{RWNodeData.SAMPLE_MATRIX}{j}", value);
            }
        }

        private static async void MessageBoxCustomAsync(OpcClient client, string nodeYes, string nodeNo, string label)
        {
            MessageBoxResult result = MessageBox.Show(label, "", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        await client.WriteNode(nodeYes, true);
                        await Task.Delay(1000);
                        await client.WriteNode(nodeYes, false);
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        await client.WriteNode(nodeNo, true);
                        await Task.Delay(1000);
                        await client.WriteNode(nodeNo, false);
                        break;
                    }
            }
        }

    }
}