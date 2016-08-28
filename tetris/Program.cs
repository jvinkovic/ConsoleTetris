using System;

namespace tetris
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool play = true;
            while (play)
            {
                Tetris tetris = new Tetris();
                tetris.Play();

                Console.WriteLine("Type \"y\" to play again.");
                string again = Console.ReadLine();
                if (again != "y")
                {
                    play = false;
                }
            }
        }
    }
}