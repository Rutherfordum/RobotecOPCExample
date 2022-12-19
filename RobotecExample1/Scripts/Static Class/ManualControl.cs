using System.Threading.Tasks;
using Robotec.OPC;
using Robotec.RW_Nodes;
using Workstation.ServiceModel.Ua;

namespace Robotec.Control
{
    public static class ManualControl
    {
        /// <summary>
        /// Активировать режим ручного управления
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Enable(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.MANUAL_JOG_DELTA]);
            await Task.Delay(1000);
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);
            Utils.MessageBoxCustomAsync(client, RWNodeData.MANUAL_JOG, RWNodeData.R_STOP,
                "Перейти в режим ручного управления?");
        }

        /// <summary>
        /// Деактивировать режим ручного управления
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Disable(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;
            await client.WriteNodeAsync(RWNodeData.PG_TASK, RWNodeData.TASK[Enum_PG_TASK.NOTASK]);
            Utils.MessageBoxCustomAsync(client, RWNodeData.R_STOP, "",
                "Вы действительно хотите выйти из режима ручного управления?");
        }

        /// <summary>
        /// Отправить введеные в ручном режиме позиции
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task SendTransform(OpcClient client, Transform transform)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}X", (double)transform.X);
            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}Y", (double)transform.Y);
            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}Z", (double)transform.Z);
            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}A", (double)transform.A);
            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}B", (double)transform.B);
            await client.WriteNodeAsync($"{RWNodeData.MOVE_XYZ}C", (double)transform.C);
        }

        /// <summary>
        /// Начать движение, робот будет двигаться относительно текущего положения робота
        /// </summary>
        /// <param name="client"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static async Task StartRobotMovement(OpcClient client)
        {
            if (client.Session.State != CommunicationState.Opened)
                return;

            Utils.MessageBoxCustomAsync(client, RWNodeData.R_START, "", "Начать движение?");
        }
    }
}