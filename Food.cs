using System.Drawing;

namespace SnakeGame
{
    class Food : GameObject
    {
        public Food(Point position, int cellSize)
            : base(position, cellSize)
        {
        }

        public virtual int Points
        {
            get { return 10; }
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Red, Position.X, Position.Y, CellSize, CellSize);
        }
    }
}
