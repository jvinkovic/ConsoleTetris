namespace tetris
{
    public class JShape : Shape
    {
        public JShape()
        {
            shape = new char[,] {  { ' ', '*' },
                                   { ' ', '*' },
                                   { '*', '*' } };
        }
    }
}