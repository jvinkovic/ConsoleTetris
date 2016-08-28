namespace tetris
{
    public class BoxShape : Shape
    {
        public BoxShape()
        {
            shape = new char[,] {  { '*', '*' },
                                   { '*', '*' }};
        }

        public override void Rotate(RotationsEnum rotation)
        {
            // do nothing
        }
    }
}