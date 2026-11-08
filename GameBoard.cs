using System;
using System.Drawing;

namespace SnakeGame
{
    class GameBoard
    {
        private int columns;
        private int rows;
        private int cellSize;

        public GameBoard(int pixelWidth, int pixelHeight, int cellSize)
        {
            this.cellSize = cellSize;
            columns = pixelWidth / cellSize;
            rows = pixelHeight / cellSize;
        }

        public int CellSize
        {
            get { return cellSize; }
        }

        // True if the cell is inside the board (boundary collision check)
        public bool IsInside(Point p)
        {
            return p.X >= 0 && p.Y >= 0 &&
                   p.X < columns * cellSize &&
                   p.Y < rows * cellSize;
        }

        // Random cell that always lies on the grid
        public Point RandomCell(Random random)
        {
            return new Point(random.Next(columns) * cellSize,
                             random.Next(rows) * cellSize);
        }
    }
}
