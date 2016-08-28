namespace tetris
{
    public class ZShape : Shape
    {
        public ZShape()
        {
            shape = new char[,] {  { ' ', '*' },

                                       { '*', '*' },
                                       { '*', ' ' } };
        }
    }
}