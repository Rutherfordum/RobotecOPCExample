using System.Threading.Tasks;
using System.Windows;
using Robotec.OPC;
using Robotec.RW_Nodes;
using Workstation.ServiceModel.Ua;

namespace Robotec
{
    public static class Measurement
    {
        /// <summary>
        /// Начать выполнение программы измерения
        /// </summary>
        /// <param name="client"></param>
        /// <param name="task"> есть измерения 2-х типов, по матрице - MAIN_MATRIX и по позиции - MAIN_POSITIONS,  </param>
        /// <returns></returns>
        public static async Task Start(OpcClient client, Enum_PG_TASK task)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[task]);
            await Task.Delay(1000);
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            Utils.MessageBoxCustomAsync(client, RWNodeData.R_START, RWNodeData.R_STOP, "Начать измерения?");
        }

        /// <summary>
        /// Продолжить измерения 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="task"> есть измерения 2-х типов, по матрице - MAIN_MATRIX и по позиции - MAIN_POSITIONS,  </param>
        /// <returns></returns>
        public static async Task Continue(OpcClient client, Enum_PG_TASK task)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[task]);
            await Task.Delay(1000);
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);

            Utils.MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Продолжить измерения?");
        }

        /// <summary>
        /// Закончить измерение текущего образтца
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Finish(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.D_FINISH, true);
            await Task.Delay(1000);
            await client.WriteNodeAsync(RWNodeData.D_FINISH, false);
            MessageBox.Show("Измерение завершено", "", MessageBoxButton.OK);
        }
    }
}