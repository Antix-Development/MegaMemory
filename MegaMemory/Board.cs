
// Board 1.0, by Cliff Earl, Antix Development, April 2019

namespace MegaMemory
{
    class Board
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public string Name;

        /// <summary>
        /// Create new Board
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Board(int x, int y, int width, int height, string name)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
        }
    }
}
