namespace tetris
{
    public class LShape : Shape
    {
        public LShape()
        {
            shape = new char[,] {  { '*', ' ' },
                                   { '*', ' ' },
                                   { '*', '*' } };
        }
    }
}