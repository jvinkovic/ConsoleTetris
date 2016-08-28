using System;

namespace tetris
{
    public abstract class Shape
    {
        /// <summary>
        /// representation of the shape
        /// </summary>
        public char[,] shape { get; protected set; }

        /// <summary>
        /// rotetes shape regarding supplied rotation enum
        /// </summary>
        /// <param name="rotation">how to rotate</param>
        public virtual void Rotate(RotationsEnum rotation)
        {
            switch (rotation)
            {
                case RotationsEnum.Clockwise:
                    // clockwise rotation is vertical flip and then transpose of matrix
                    VerticalFlip();
                    Transpose();

                    break;

                case RotationsEnum.CounterClockwise:
                    // counterclockwise rotation is transpose and then vertical flip of matrix
                    Transpose();
                    VerticalFlip();
                    break;
            }
        }

        protected void VerticalFlip()
        {
            int rowNum = shape.GetLength(0);
            int colNum = shape.GetLength(1);

            int halfHeight = rowNum / 2;

            char[,] flipped = new char[rowNum, colNum];
            for (int i = 0; i < halfHeight; i++)
            {
                // upper rows to lower ones
                Array.Copy(shape, i * colNum, flipped, (rowNum - i - 1) * colNum, colNum);

                // lower rows to upper ones
                Array.Copy(shape, (rowNum - i - 1) * colNum, flipped, i * colNum, colNum);
            }

            // if odd number of rows - copy middle row
            if (rowNum % 2 == 1)
                Array.Copy(shape, halfHeight * colNum, flipped, halfHeight * colNum, colNum);

            shape = flipped;
        }

        protected void Transpose()
        {
            int rowNum = shape.GetLength(0);
            int colNum = shape.GetLength(1);

            char[,] transposed = new char[colNum, rowNum];
            for (int r = 0; r < rowNum; r++)
            {
                for (int c = 0; c < colNum; c++)
                {
                    transposed[c, r] = shape[r, c];
                }
            }

            shape = transposed;
        }
    }
}