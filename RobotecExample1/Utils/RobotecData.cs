namespace RobotecExample1.Utils
{
    public static class RobotecData
    {

        //при инициализации очистить ячейки 
        public const string IP1 = "172.31.1.147";// Адрес 1 робота
        public const string PORT1 = "4840";// Порт 1 робота

        public const string IP2 = "172.31.1.147";// Адрес 2 робота
        public const string PORT2 = "4840";// Порт 2 робота

        public const string IP3 = "172.31.1.147";// Адрес 3 робота
        public const string PORT3 = "4840";// Порт 3 робота

        public const string IP4 = "172.31.1.147";// Адрес 4 робота
        public const string PORT4 = "4840";// Порт 4 робота

        public const string R_STOP = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP"; // останов программы
        public const string R_START = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START"; // запуск программы
        public const string R_HOLD = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD"; // пауза программы
        public const string MOV_HOME = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME"; // отправить в домашнию позицию0
        public const string R_PARAM_PORT= "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.CONN_PORT"; // Порт роботы
        public const string R_PARAM_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.ROB_ID"; // Номер выборанного робота
        public const string R_PARAM_HEIGHT = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_HEIGHT"; // высота
        public const string Sample_Matrix_elem = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.Sample Matrix elem"; // высота
    }
}