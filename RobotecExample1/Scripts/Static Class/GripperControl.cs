using System.Threading.Tasks;
using Robotec.OPC;
using Robotec.RW_Nodes;

namespace RobotecExample.Scripts.Static_Class
{
    public static class GripperControl
    {
        /// <summary>
        /// Открыть гриппер
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Open(OpcClient client)
        {
            await client.WriteNodeAsync(RWNodeData.GRIP_CONTROL_OPEN, true);
        }

        /// <summary>
        /// Закрыть гриппер
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task Close(OpcClient client)
        {
            await client.WriteNodeAsync(RWNodeData.GRIP_CONTROL_OPEN, false);
        }
    }
}