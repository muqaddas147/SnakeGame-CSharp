using System.Drawing;

namespace SnakeGame
{
    abstract class GameObject
    {
        private Point position;
        private int cellSize;

        protected GameObject(Point position, int cellSize)
        {
            this.position = position;
            this.cellSize = cellSize;
        }

        public Point Position
        {
            get { return position; }
            protected set { position = value; }
        }

        public int CellSize
        {
            get { return cellSize; }
        }

        // Every game item draws itself in its own way
        public abstract void Draw(Graphics g);
    }
}
