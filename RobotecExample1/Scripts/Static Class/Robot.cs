using System;
using System.Threading.Tasks;
using System.Windows;
using Robotec.OPC;
using Robotec.R_Nodes;
using Robotec.RW_Nodes;
using Workstation.ServiceModel.Ua;

namespace Robotec
{
    public class Robot
    {
        /// <summary>
        /// Аварийное завершение программы, робот вернет образец на место, и вернется в положение домой
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task EmergencyStop(OpcClient client)
        {
            //await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.RETURN_HOME]);
            //await Task.Delay(500);
            //await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            await client.WriteNodeAsync(RWNodeData.R_ALM, true);
            await Task.Delay(500);
            await client.WriteNodeAsync(RWNodeData.R_ALM, false);

            Utils.MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Аварийное завершение программы?");
        }

        /// <summary>
        /// Остановить выполненние программы ихмерение и выйти из него
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Stop(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);
            Utils.MessageBoxCustomAsync(client, RWNodeData.R_STOP, "", "Вы действительно хотите остановить измерения?");
        }

        /// <summary>
        /// Поставить программу робота неа паузу
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Pause(OpcClient client)
        {
            await client.WriteNodeAsync(RWNodeData.R_HOLD, true);
        }

        /// <summary>
        /// Вывести программу из паузы и продолжить программу
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Continue(OpcClient client)
        {
            await client.WriteNodeAsync(RWNodeData.R_HOLD, false);
        }

        /// <summary>
        /// Вернуть робота в домашнее положение
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task GoToHome(OpcClient client)
        {
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.RETURN_HOME]);
            await Task.Delay(500);
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            await client.WriteNodeAsync(RWNodeData.MOVE_HOME, true);
            await Task.Delay(500);
            await client.WriteNodeAsync(RWNodeData.MOVE_HOME, false);

            Utils.MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Вернуть робота в домашнее положение?");
        }

        /// <summary>
        /// Отмена без вызова таска, квитирование, сброс ошибок ???????
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task ResetError(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.RESET_ERR, true);
            await Task.Delay(1000);
            await client.WriteNodeAsync(RWNodeData.RESET_ERR, false);

            MessageBox.Show("Ошибки были сброшены", "", MessageBoxButton.OK);
        }

        /// <summary>
        /// Сброс параметров программы робота
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task ResetParameters(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.R_STOP, false);
            await client.WriteNodeAsync(RWNodeData.R_START, false);
            await client.WriteNodeAsync(RWNodeData.R_HOLD, false);
            await client.WriteNodeAsync(RWNodeData.MOVE_HOME, false);
            await client.WriteNodeAsync(RWNodeData.D_FINISH, false);
            await client.WriteNodeAsync(RWNodeData.R_ALM, false);
            await client.WriteNodeAsync(RWNodeData.R_MOVE_XYZ, false);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_CONN_PORT, 0000);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_ROB_ID, 0);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_HEIGHT, (double)2.5);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_POS_ID, 0);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_TAKE_POS_ID, 0);
            await SendCellMatrix(client, new bool[36]);
            MessageBox.Show("Данные очищены", "", MessageBoxButton.OK);
        }

        /// <summary>
        /// Отправить параметры на выполнение программы робота
        /// </summary>
        /// <param name="client"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task SendParameters(OpcClient client, Parameters parameters)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.R_PARAM_CONN_PORT, Int32.Parse(parameters.Port));
            await client.WriteNodeAsync(RWNodeData.R_PARAM_ROB_ID, (Int32) parameters.Robot_id);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_HEIGHT, (Int32) (parameters.Height * 10f));
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_TAKE_POS_ID, (Int32)parameters.TakeFromCell);
            await client.WriteNodeAsync(RWNodeData.R_PARAM_SAMPLE_POS_ID, (Int32)parameters.PutToCell);
            await SendCellMatrix(client, parameters.CellMatrix);
        }

        /// <summary>
        /// Получить актальные аозиции робота
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Transform GetActualTransformRobot(OpcClient client)
        {
            try
            {
                if (client.Session.State != CommunicationState.Opened)
                    return new Transform();

                return ReadTransform(client, RNodeData.POS_ACT).Result;
            }
            catch (Exception e)
            {
                return new Transform();
            }
        }

        /// <summary>
        /// Получить позиции робота целевые
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Transform GetTargetTransformRobot(OpcClient client)
        {
            try
            {
                if (client.Session.State != CommunicationState.Opened)
                    return new Transform();

                return ReadTransform(client, RNodeData.POS_FOR).Result;
            }
            catch (Exception e)
            {
                return new Transform();
            }
        }

        private static async Task<Transform> ReadTransform(OpcClient client, string id)
        {
            if (client.Session.State != CommunicationState.Opened)
                return new Transform();

            var xx = client.ReadNode($"{id}X");
            var yy = client.ReadNode($"{id}Y");
            var zz = client.ReadNode($"{id}Z");
            var aa = client.ReadNode($"{id}A");
            var bb = client.ReadNode($"{id}B");
            var cc = client.ReadNode($"{id}C");

            double.TryParse(xx.ToString(), out var x);
            double.TryParse(yy.ToString(), out var y);
            double.TryParse(zz.ToString(), out var z);
            double.TryParse(aa.ToString(), out var a);
            double.TryParse(bb.ToString(), out var b);
            double.TryParse(cc.ToString(), out var c);

            return new Transform((float)x, (float)y, (float)z, (float)a, (float)b, (float)c);
        }

        private static async Task SendCellMatrix(OpcClient client, bool[] matrix)
        {
            int j = 0;
            bool[] matrixCells = matrix;

            for (int i = 0; i < matrixCells.Length; i++)
            {
                j = i + 1;
                bool value = matrixCells[i];
                await client.WriteNodeAsync(
                    $"{RWNodeData.SAMPLE_MATRIX}{j}", value);
            }
        }
    }
}