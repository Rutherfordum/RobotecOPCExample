using System.Collections.Generic;

// Реализовать пермещение робота в заданные координаты с АРМ
// Перемещение образца из одной ячейки в другую

// Задачи 
// Перемещение робота в домашнюю позицию 

namespace RobotecExample.Utils
{
    public class RobotecModel
    {
        public readonly string IP1 = "172.31.1.147";
        public readonly string PORT1 = "4840";

        public readonly string User = "OpcUaOperator";
        public readonly string Password = "kuka";

        public readonly string IP2 = "10.91.75.144";
        public readonly string PORT2 = "4840";

        public readonly string IP3 = "170.30.4.147";
        public readonly string PORT3 = "4840";

        public readonly string IP4 = "172.31.1.147";
        public readonly string PORT4 = "4840";

        //Ноды которые пишет АРМ в робота
        public readonly string R_STOP = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP"; // останов программы
        public readonly string R_START = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START"; // запуск программы
        public readonly string R_HOLD = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD"; // пауза программы
        public readonly string R_ALM = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_ALM"; //Аварийное завершение измерения. Образец вернуть в позицию, откуда брал. Выйти в позицию HOME.
        public readonly string MOV_HOME = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME"; // отправить в домашнию позицию0
        public readonly string R_PARAM_PORT = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.CONN_PORT"; // Порт роботы
        public readonly string R_PARAM_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.ROB_ID"; // Номер выборанного робота
        public readonly string R_PARAM_HEIGHT = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_HEIGHT"; // высота
        public readonly string R_PARAM_POS_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_POS_ID"; // указываем в какую ячейку мы положим образец
        public readonly string D_FINISH = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_D_FINISH"; // Детектор завершил измерение. Сигнал роботу выйти от детек-тора и установить объект
        public readonly string R_MOV_XYZ = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_XYZ"; // Перемещение робота в указанную оператором позицию.
        public readonly string Sample_Matrix_elem = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.Sample Matrix elem"; // высота
        public readonly string MANUAL_CONTROL = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MANUAL_JOG";
        public readonly string PG_TASK = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.PG_TASK";
        public readonly string RESET_ERR = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_RESET_ERR";
        public readonly string R_PARAM_POS_TAKE_ID = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_TAKE_POS_ID"; //нода на переменную откуда забирать образец
        public readonly string I_MOV_X = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.X";
        public readonly string I_MOV_Y = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.Y";
        public readonly string I_MOV_Z = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.Z";
        public readonly string I_MOV_A = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.A";
        public readonly string I_MOV_B = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.B";
        public readonly string I_MOV_C = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.C";

        public readonly Dictionary<Enum_PG_TASK, string> TASK = new Dictionary<Enum_PG_TASK, string>()
        {
            {Enum_PG_TASK.MAIN, "#MAIN"},
            {Enum_PG_TASK.MANUAL_JOG_DELTA, "#MANUAL_JOG_DELTA"},
            {Enum_PG_TASK.MANUAL_JOG_ABS, "#MANUAL_JOG_ABS"},
            {Enum_PG_TASK.TEST, "#TEST"},
            {Enum_PG_TASK.RETURN_HOME, "#RETURN_HOME"},
            {Enum_PG_TASK.NOTASK, "#NOTASK"},
            {Enum_PG_TASK.MAIN_MATRIX, "#MAIN_MATRIX"},
            {Enum_PG_TASK.MAIN_POSITIONS, "#MAIN_POSITIONS"}
        };

        // Ноды которые читает АРМ с робота
        public readonly string POS_ACT = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.OA_POS_ACT_";
        public readonly string POS_FOR = "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.OA_POS_FOR_";


    }

    public enum Enum_PG_TASK
    {
        MAIN,
        MANUAL_JOG_DELTA,
        MANUAL_JOG_ABS,
        TEST,
        RETURN_HOME,
        NOTASK,
        MAIN_MATRIX,
        MAIN_POSITIONS
    }
}