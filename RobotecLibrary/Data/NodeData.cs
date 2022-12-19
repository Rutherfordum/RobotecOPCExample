using System.Collections.Generic;

namespace Robotec.RW_Nodes
{
    /// <summary>
    /// Ноды которые пишет АРМ в робота
    /// </summary>
    public static class RWNodeData
    {
        /// <summary>
        /// Управление захватом
        /// Type BOOL
        /// </summary>
        public static readonly string GRIP_CONTROL_OPEN =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_GRIP_CONTROL_OPEN";

        /// <summary>
        /// Запуск выбранной программы робота,
        /// Type BOOL
        /// </summary>
        public static readonly string R_START = 
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START";

        /// <summary>
        /// Остановка выполняемой программы робота
        /// Type BOOL
        /// </summary>
        public static readonly string R_STOP =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_STOP";

        /// <summary>
        /// Пауза выполняемой программы робота
        /// Type BOOL
        /// </summary>
        public static readonly string R_HOLD =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_HOLD";

        /// <summary>
        /// Команда на перемещение роботу в домашнюю позицию
        /// Type BOOL
        /// </summary>
        public static readonly string MOVE_HOME =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_HOME";

        /// <summary>
        /// Аварийное завершение измерения. Образец вернуть в позицию, откуда брал. Выйти в позицию HOME.
        /// Type BOOL
        /// </summary>
        public static readonly string R_ALM =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_ALM";

        /// <summary>
        /// ID робота
        /// Type INT
        /// Range 1-4 
        /// </summary>
        public static readonly string R_PARAM_ROB_ID =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.ROB_ID";

        /// <summary>
        /// Порт для связи с роботом
        /// Type INT
        /// </summary>
        public static readonly string R_PARAM_CONN_PORT =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.CONN_PORT";

        /// <summary>
        /// Высота над детектором
        /// Type DOUBLE
        /// Range 2.5, 5, 10, 20
        /// </summary>
        public static readonly string R_PARAM_SAMPLE_HEIGHT =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_HEIGHT";

        /// <summary>
        /// Позиция откуда забирать образец
        /// Type INT
        /// Range 1-36
        /// </summary>
        public static readonly string R_PARAM_SAMPLE_TAKE_POS_ID =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_TAKE_POS_ID";

        /// <summary>
        /// Номер ячейки задания для робота
        /// Type INT
        /// Range 1-36
        /// </summary>
        public static readonly string R_PARAM_SAMPLE_POS_ID =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_R_PARAM.SAMPLE_POS_ID";

        /// <summary>
        /// Детектор завершил измерение. Сигнал роботу выйти от детек-тора и установить объект
        /// Type BOOL
        /// </summary>
        public static readonly string D_FINISH =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_D_FINISH";

        /// <summary>
        /// Команда роботу переместиться в заданную позицию
        /// Type BOOL
        /// </summary>
        public static readonly string R_MOVE_XYZ =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MOV_XYZ";

        /// <summary>
        /// Позиции для перемещения в указанную точку, добавьте в конце ноды or X,Y,Z,A,B,C.
        /// Type DOUBLE
        /// </summary>
        public static readonly string MOVE_XYZ =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.I_MOV_XYZ.";

        /// <summary>
        /// Масссив наличия образцов в магазине, добавьте в конце ноды 1-36
        /// Type INT
        /// Range 1-36
        /// </summary>
        public static readonly string SAMPLE_MATRIX = 
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.Sample Matrix elem"; // высота

        /// <summary>
        /// Параметры подтверждены,  СТАРТ выполнения задания.
        /// Type BOOL
        /// </summary>
        public static readonly string ACKPARAM = 
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_ACKPARAM";

        /// <summary>
        /// Флаг ручного режима
        /// Type BOOL
        /// </summary>
        public static readonly string MANUAL_JOG =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_MANUAL_JOG";

        /// <summary>
        /// Задание роботу (основное, домой, ручной режим)
        /// Type STRING
        /// RANGE use TASK node
        /// </summary>
        public static readonly string PG_TASK =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.PG_TASK";

        /// <summary>
        /// Cброс ошибки\квитирование с АРМ
        /// Type BOOL
        /// </summary>
        public static readonly string RESET_ERR =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_RESET_ERR";

        public static readonly Dictionary<Enum_PG_TASK, string> TASK = new Dictionary<Enum_PG_TASK, string>()
        {
            {Enum_PG_TASK.MANUAL_JOG_DELTA, "#MANUAL_JOG_DELTA"},
            {Enum_PG_TASK.MANUAL_JOG_ABS, "#MANUAL_JOG_ABS"},
            {Enum_PG_TASK.RETURN_HOME, "#RETURN_HOME"},
            {Enum_PG_TASK.NOTASK, "#NOTASK"},
            {Enum_PG_TASK.MAIN_MATRIX, "#MAIN_MATRIX"},
            {Enum_PG_TASK.MAIN_POSITIONS, "#MAIN_POSITIONS"}
        };

    }
}

namespace Robotec.R_Nodes
{
    /// <summary>
    /// Ноды которые читает АРМ с робота
    /// </summary>
    public static class RNodeData
    {
        /// <summary>
        /// Сообщения об ошибок
        /// Return string
        /// </summary>
        public const string OA_MESSAGE = 
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.OA_MESSAGE";


        /// <summary>
        /// Робот в домашней позиции
        /// Return BOOL
        /// </summary>
        public const string IN_HOME =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$IN_HOME";

        /// <summary>
        /// Текущий статус программы
        /// Return STRING
        /// </summary>
        public const string PRO_STATE =
            "ns=5;s=MotionDeviceSystem.ProcessData.System.$PRO_STATE";

        /// <summary>
        /// Движение робота в указанную позицию завершено
        /// Return BOOL
        /// </summary>
        public const string JOG_FINISH =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.JOG_Finish";

        /// <summary>
        /// Текущая позиция робота : Составная переменная доступная для чтения,
        /// добавьте в конце ноды or X,Y,Z,A,B,C.
        /// Return DOUBLE
        /// </summary>
        public const string POS_ACT =
            "ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_ACT.";

        /// <summary>
        /// Cледующая целевая позиция робота : Составная переменная доступная для чтения
        /// добавьте в конце ноды or X,Y,Z,A,B,C.
        /// Return DOUBLE
        /// </summary>
        public const string POS_FOR =
            "ns=5;s=MotionDeviceSystem.ProcessData.System.R1.$POS_FOR.";

        /// <summary>
        /// Робот в режиме Т1
        /// Return BOOL
        /// </summary>
        public const string T1 =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$T1";

        /// <summary>
        /// Робот в режиме Внешней автоматики EXT
        /// Return BOOL
        /// </summary>
        public const string EXT =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$EXT";

        /// <summary>
        /// Контур аварийного выключения замкнут
        /// Return BOOL
        /// </summary>
        public const string ALARM_STOP =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$ALARM_STOP";

        /// <summary>
        /// Переключатель защиты оператора закрыт
        /// Return BOOL
        /// </summary>
        public const string USER_SAF =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$USER_SAF";

        /// <summary>
        /// Общая ошибка
        /// Return BOOL
        /// </summary>
        public const string STOPMESS =
            "ns=5;s=MotionDeviceSystem.ProcessData.STEU.Mada.$machine.$STOPMESS";

        /// <summary>
        /// Режим работы робота
        /// Return STRING
        /// </summary>
        public const string ModeOP =
            "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.ModeOP";

    }
}