using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tetris
{
    public class Tetris
    {
        /// <summary>
        /// indicator if enter is needed to finish move
        /// </summary>
        private bool _enterNeeded = true;

        long _score = 0;

        // "board" sizes
        private static int _sizeX = 20;

        private static int _sizeY = 20;

        // initial positions of falling shape
        private int _posX = _sizeX / 2 + 1;

        private int _posY = 0;

        private char[,] _board = InitilizeBoard();

        /// <summary>
        /// "a" - move left
        /// "d" - move right
        /// "w" - rotate countercockwise
        /// "s" - rotate clockwise
        /// "" - just go down
        /// </summary>
        /// I added "" (enter) so that game makes more sense
        private List<string> _validMoves = new List<string> { "a", "d", "w", "s", "" };

        private static char[,] InitilizeBoard()
        {
            char[,] board = new char[_sizeY + 1, _sizeX + 2];
            for (int r = 0; r < _sizeY; r++)
            {
                board[r, 0] = '*';
                for (int c = 1; c <= _sizeX; c++)
                {
                    board[r, c] = ' ';
                }
                board[r, _sizeX + 1] = '*';
            }

            for (int c = 0; c <= _sizeX + 1; c++)
            {
                board[_sizeY, c] = '*';
            }

            return board;
        }

        /// <summary>
        /// start playing TETRIS
        /// </summary>
        public void Play()
        {
            // for easier adding of some other shapes
            List<Type> allShapesDefined = GetListOfInheritedTypes<Shape>();

            Console.Clear();

            // make sure that console buffer is big enough
            if (Console.BufferHeight < _sizeY * 1.5 || Console.BufferHeight < _sizeX * 1.2)
            {
                Console.SetBufferSize((int)(_sizeX * 1.2), (int)(_sizeY * 1.5));
            }

            // make sure windows is right size also
            Console.SetWindowSize(_sizeX * 4, _sizeY * 2);

            while (true)
            {
                var piece = MakeRandomInstance<Shape>(allShapesDefined);

                var status = UpdateBoard(piece.shape, _posX, _posY);

                if (status != BoardStatus.Success)
                {
                    break;
                }

                RedrawBoard();

                bool nextPiece = false;
                while (!nextPiece)
                {
                    string input;
                    bool invalidInput = true;
                    do
                    {
                        Console.WriteLine("Your move?");

                        if (_enterNeeded)
                        {
                            input = Console.ReadLine();
                        }
                        else
                        {
                            input = Console.ReadKey().Key.ToString().ToLower();
                            if (input == "enter") input = "";
                        }

                        if (_validMoves.Contains(input))
                        {
                            invalidInput = false;
                        }
                        else
                        {
                            Console.WriteLine(" - INVALID INPUT!");
                        }
                    } while (invalidInput);

                    // "a" - move left
                    // "d" - move right
                    // "w" - rotate countercockwise
                    // "s" - rotate clockwise
                    // "" - nothing
                    char[,] oldShape = new char[piece.shape.GetLength(0), piece.shape.GetLength(1)];
                    Array.Copy(piece.shape, oldShape, piece.shape.Length);
                    switch (input)
                    {
                        case "a":
                            status = UpdateBoard(piece.shape, _posX - 1, _posY, oldShape);
                            break;

                        case "d":
                            status = UpdateBoard(piece.shape, _posX + 1, _posY, oldShape);
                            break;

                        case "w":
                            piece.Rotate(RotationsEnum.CounterClockwise);
                            status = UpdateBoard(piece.shape, _posX, _posY, oldShape);
                            if (status == BoardStatus.Fail)
                            {
                                piece.Rotate(RotationsEnum.Clockwise);
                            }
                            break;

                        case "s":
                            piece.Rotate(RotationsEnum.Clockwise);
                            status = UpdateBoard(piece.shape, _posX, _posY, oldShape);
                            if (status == BoardStatus.Fail)
                            {
                                piece.Rotate(RotationsEnum.CounterClockwise);
                            }
                            break;

                        default:
                            status = UpdateBoard(piece.shape, _posX, _posY, oldShape);
                            break;
                    }

                    RedrawBoard();

                    switch (status)
                    {
                        case (BoardStatus.Fail):
                            {
                                Console.WriteLine("Invalid move!");
                                break;
                            }

                        case (BoardStatus.HitBottom):
                            {
                                // restart positions
                                _posX = _sizeX / 2 + 1;
                                _posY = 0;
                                FindFullLine();
                                RedrawBoard();
                                nextPiece = true;
                                break;
                            }

                        case (BoardStatus.Success):
                            {
                                RedrawBoard();
                                break;
                            }
                    }
                }
            }

            GameOver();
        }

        /// <summary>
        /// find if there are any full lines
        /// remove them and update score
        /// </summary>
        private void FindFullLine()
        {
            List<int> toRemove = new List<int>();
            char[] line = new char[_sizeX + 2];
            for (int i = 0; i < _sizeY; i++)
            {
                for (int j = 0; j < _sizeX + 2; j++)
                {
                    line[j] = _board[i, j];
                }

                if (!line.Any(c => c != '*'))
                {
                    toRemove.Add(i);
                }
            }

            if (toRemove.Count > 0)
            {
                char[,] new_board = new char[_board.GetLength(0), _board.GetLength(1)];

                Array.Copy(_board, (toRemove.Max() + 1) * (_sizeX + 2), new_board, (toRemove.Max() + 1) * (_sizeX + 2), (_sizeY - toRemove.Max()) * (_sizeX + 2));

                // set bounds left and right
                for (int r = 0; r <= toRemove.Max(); r++)
                {
                    new_board[r, 0] = '*';
                    for (int c = 1; c <= _sizeX; c++)
                    {
                        new_board[r, c] = ' ';
                    }
                    new_board[r, _sizeX + 1] = '*';
                }
                int sizeOfDrop = toRemove.Count;
                for (int i = 1; i < toRemove.Max(); i++)
                {
                    if (!toRemove.Contains(i))
                    {
                        Array.Copy(_board, i * (_sizeX + 2), new_board, (i + sizeOfDrop) * (_sizeX + 2), (_sizeX + 2));
                    }
                    else
                    {
                        sizeOfDrop--;
                    }
                }

                _score += toRemove.Count;
                _board = new_board;
            }
        }

        private void GameOver()
        {
            string gameOver = "GAME OVER!";
            int start = _sizeX / 2 + 2 - gameOver.Length / 2;
            for (int i = 0; i < gameOver.Length; i++)
            {
                _board[_sizeY / 2, start + i] = gameOver[i];
                var s = gameOver[i];
            }
            RedrawBoard();
        }

        private void RedrawBoard()
        {
            Console.Clear();

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    Console.Write(_board[i, j]);
                }

                if (i == _board.GetLength(0) / 2)
                {
                    Console.Write("     a - move left; d - move righ");
                }

                if (i == _board.GetLength(0) / 3)
                {
                    Console.Write("     Score: " + _score);
                }

                if (i == _board.GetLength(0) / 2 + 1)
                {
                    Console.Write("     w - rotate counterclockwise; s - rotate clockwise");
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// update board and returns status
        /// </summary>
        /// <param name="shape">what shape to show</param>
        /// <param name="positionX">position x of the shape</param>
        /// <param name="positionY">position y of the shape</param>
        /// <returns>status of the board</returns>
        private BoardStatus UpdateBoard(char[,] shape, int positionX, int positionY, char[,] oldShape = null)
        {
            char[,] new_board = new char[_board.GetLength(0), _board.GetLength(1)];

            Array.Copy(_board, new_board, _board.Length);

            int shapeRows = shape.GetLength(0);
            int shapeCols = shape.GetLength(1);

            var status = BoardStatus.Success;

            if (positionY > 20 || positionX < 0 || positionX > 20 || positionY < 0)
            {
                return BoardStatus.Fail;
            }

            try
            {
                if (oldShape != null)
                {
                    positionY++;

                    // clean shape from board before update
                    for (int r = _posY; r < _posY + oldShape.GetLength(0); r++)
                    {
                        for (int c = _posX; c < _posX + oldShape.GetLength(1); c++)
                        {
                            if ('*' == oldShape[r - _posY, c - _posX])
                            {
                                new_board[r, c] = ' ';
                            }
                        }
                    }
                }

                // if it is out of board it will throw exception

                for (int i = 0; i < shapeRows; i++)
                {
                    bool colision = ('*' == new_board[i + positionY, positionX] && '*' == shape[i, 0]) || ('*' == new_board[i + positionY, positionX + shapeCols - 1] && '*' == shape[i, shapeCols - 1]);

                    if (colision)
                    {
                        return BoardStatus.Fail;
                    }
                }

                for (int r = positionY; r < positionY + shapeRows; r++)
                {
                    for (int c = positionX; c < positionX + shapeCols; c++)
                    {
                        if (c - positionX < shapeCols && '*' == shape[r - positionY, c - positionX])
                        {
                            new_board[r, c] = shape[r - positionY, c - positionX];
                        }
                    }
                }

                for (int j = 0; j < shapeCols; j++)
                {
                    bool bottom = '*' == new_board[positionY + shapeRows, positionX + j] && '*' == shape[shapeRows - 1, j];

                    bool hanging = '*' == new_board[positionY + shapeRows - 1, positionX + j] && ' ' == shape[shapeRows - 1, j];

                    if (shapeRows > 1)
                    {
                        hanging = hanging && '*' == shape[shapeRows - 2, j];
                    }

                    if (bottom || hanging)
                    {
                        status = BoardStatus.HitBottom;
                    }
                }
            }
            catch (Exception e)
            {
                return BoardStatus.Fail;
            }

            if (status != BoardStatus.Fail)
            {
                _posX = positionX;
                _posY = positionY;

                Array.Copy(new_board, _board, new_board.Length);
            }

            return status;
        }

        // might be costly operation but it is used just once...
        /// <summary>
        /// gets list of types inherited from T
        /// </summary>
        /// <typeparam name="T">parent class to get child clases from</typeparam>
        /// <returns>list of child types</returns>
        private List<Type> GetListOfInheritedTypes<T>() where T : class
        {
            List<T> shapes = new List<T>();

            var inherited = Assembly.GetAssembly(typeof(T)).GetTypes().Where(shape => shape.IsClass && !shape.IsAbstract && shape.IsSubclassOf(typeof(T)));

            return new List<Type>(inherited);
        }

        /// <summary>
        /// get me random instance of shape
        /// </summary>
        /// <typeparam name="T">parent class</typeparam>
        /// <param name="shapes">list of types of subclasses of parent class T</param>
        /// <returns></returns>
        private T MakeRandomInstance<T>(List<Type> shapes) where T : class
        {
            Random rnd = new Random();
            int i = rnd.Next(0, shapes.Count);

            return (T)Activator.CreateInstance(shapes[i]);
        }
    }
}