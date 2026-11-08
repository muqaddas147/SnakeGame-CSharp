using System.Drawing;

namespace SnakeGame
{
    class Obstacle : GameObject
    {
        public Obstacle(Point position, int cellSize)
            : base(position, cellSize)
        {
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Gray, Position.X, Position.Y, CellSize, CellSize);
            g.DrawRectangle(Pens.White, Position.X, Position.Y, CellSize - 1, CellSize - 1);
        }
    }
}
