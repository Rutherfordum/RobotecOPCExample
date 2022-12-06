namespace RobotecExample1.Utils
{
    public static class RobotecData
    {
        public const string IP1 = "172.31.1.147";
        public const string PORT1 = "4840";

        public const string User = "OpcUaOperator";
        public const string Password = "kuka";

        public const string IP2 = "10.91.75.144";
        public const string PORT2 = "4840";

        public const string IP3 = "170.30.4.147";
        public const string PORT3 = "4840";

        public const string IP4 = "172.31.1.147";
        public const string PORT4 = "4840";

        //Ноды которые пишет АРМ в робота
        public const string R_STOP = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP"; // останов программы
        public const string R_START = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START"; // запуск программы
        public const string R_HOLD = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD"; // пауза программы
        public const string R_ALM = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_ALM"; //Аварийное завершение измерения. Образец вернуть в позицию, откуда брал. Выйти в позицию HOME.
        public const string MOV_HOME = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME"; // отправить в домашнию позицию0
        public const string R_PARAM_PORT= "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.CONN_PORT"; // Порт роботы
        public const string R_PARAM_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.ROB_ID"; // Номер выборанного робота
        public const string R_PARAM_HEIGHT = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_HEIGHT"; // высота
        public const string R_PARAM_POS_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_POS_ID"; 
        public const string D_FINISH= "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_D_FINISH"; // Детектор завершил измерение. Сигнал роботу выйти от детек-тора и установить объект
        public const string R_MOV_XYZ = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_XYZ"; // Перемещение робота в указанную оператором позицию.
        public const string Sample_Matrix_elem = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.Sample Matrix elem"; // высота
        public const string IA_ACKPARAM = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_ACKPARAM"; //Параметры подтверждены, СТАРТ выполнения задания.
        
        // Ноды которые читает АРМ с робота
        public const string POS_ACT = "ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT";
        public const string POS_FOR = "ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR";
    }
}