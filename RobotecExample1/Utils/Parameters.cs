namespace Robotec
{
    public class Parameters
    {
        public int Robot_id;
        public string Port;
        public float Height;
        public int TakeFromCell;
        public int PutToCell;
        public bool[] CellMatrix;
        public Parameters() { }

        public Parameters(int robot_id, string port, float height, bool[] cellMatrix, int take_from_cell, int put_to_cell)
        {
            Robot_id = robot_id;
            Port = port;
            Height = height;
            TakeFromCell = take_from_cell;
            PutToCell = put_to_cell;
            CellMatrix = cellMatrix;
        }
    }
}