using System.Drawing;

namespace SnakeGame
{
    class GoldenFood : Food
    {
        public GoldenFood(Point position, int cellSize)
            : base(position, cellSize)
        {
        }

        public override int Points
        {
            get { return 30; }
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Gold, Position.X, Position.Y, CellSize, CellSize);
            g.DrawEllipse(Pens.White, Position.X, Position.Y, CellSize - 1, CellSize - 1);
        }
    }
}
