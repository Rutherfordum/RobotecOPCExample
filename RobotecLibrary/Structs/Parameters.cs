namespace Robotec
{
    public struct Parameters
    {
        /// <summary>
        /// Уникальный id робота
        /// </summary>
        public int Robot_id;

        /// <summary>
        /// Порт робота
        /// </summary>
        public string Port;

        /// <summary>
        /// Высота образтца над детектором
        /// </summary>
        public float Height;

        /// <summary>
        /// Номер ячейки откуда взять образце для измерения.
        /// Внимание данный параметр рекомендуется использовать при измерении по позиции - MAIN_POSITIONS
        /// </summary>
        public int TakeFromCell;

        /// <summary>
        /// Номер ячейки куда положить взятый образец после измерения
        /// Внимание данный параметр рекомендуется использовать при измерении по позиции - MAIN_POSITIONS
        /// </summary>
        public int PutToCell;

        /// <summary>
        /// Матрица заполенных ячеек, можно указать ячейки в которых есть образцы, возвращать образцы будет в те же ячейки 
        /// Внимание данный параметр рекомендуется использовать при измерении по позиции - MAIN_MATRIX
        /// </summary>
        public bool[] CellMatrix;

        public Parameters(int robot_id, string port, float height, bool[] cellMatrix, int take_from_cell, int put_to_cell)
        {
            Robot_id = robot_id;
            Port = port;
            Height = height;
            TakeFromCell = (take_from_cell > 0 && take_from_cell <= 36) ? take_from_cell : 0;
            PutToCell = (put_to_cell > 0 && put_to_cell <= 36) ? put_to_cell : 0;
            CellMatrix = cellMatrix;
        }
    }
}