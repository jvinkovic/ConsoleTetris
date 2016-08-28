namespace tetris
{
    public class LineShape : Shape
    {
        public LineShape()
        {
            shape = new char[,] { { '*', '*', '*', '*' } };
        }

        // no need for flipping
        public override void Rotate(RotationsEnum rotation)
        {
            Transpose();
        }
    }
}