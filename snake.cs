using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    class Snake : GameObject
    {
        private List<Point> body;
        private int direction;      // direction used in the last move
        private int nextDirection;  // direction requested by the player
        private bool growPending;

        // 0 = up, 1 = right, 2 = down, 3 = left
        public Snake(int length, Point start, int cellSize)
            : base(start, cellSize)
        {
            body = new List<Point>();
            direction = 1;
            nextDirection = 1;
            growPending = false;

            for (int i = 0; i < length; i++)
            {
                body.Add(new Point(start.X - i * cellSize, start.Y));
            }
        }

        public Point Head
        {
            get { return body[0]; }
        }

        // The cell the head will enter on the next move (does not move the snake)
        public Point NextHead()
        {
            Point p = body[0];

            if (nextDirection == 0) p.Y -= CellSize;
            else if (nextDirection == 1) p.X += CellSize;
            else if (nextDirection == 2) p.Y += CellSize;
            else if (nextDirection == 3) p.X -= CellSize;

            return p;
        }

        // True if the given cell is on the snake's body
        public bool WouldHitBody(Point p)
        {
            // The tail cell becomes free on the next move, unless the snake is growing
            int count = growPending ? body.Count : body.Count - 1;

            for (int i = 0; i < count; i++)
            {
                if (body[i] == p)
                    return true;
            }
            return false;
        }

        public void Move()
        {
            direction = nextDirection;
            Point newHead = body[0];

            if (direction == 0) newHead.Y -= CellSize;
            else if (direction == 1) newHead.X += CellSize;
            else if (direction == 2) newHead.Y += CellSize;
            else if (direction == 3) newHead.X -= CellSize;

            body.Insert(0, newHead);

            if (growPending)
                growPending = false;                 // keep the tail = snake grows
            else
                body.RemoveAt(body.Count - 1);       // remove tail = snake moves

            Position = newHead;
        }

        public void ChangeDirection(int newDirection)
        {
            // The opposite of direction d is (d + 2) % 4
            if (newDirection == (direction + 2) % 4)
                return;

            nextDirection = newDirection;
        }

        public void Grow()
        {
            growPending = true;
        }

        public bool Contains(Point p)
        {
            return body.Contains(p);
        }

        public bool HitsItself()
        {
            for (int i = 1; i < body.Count; i++)
            {
                if (body[i] == Head)
                    return true;
            }
            return false;
        }

        public override void Draw(Graphics g)
        {
            for (int i = 0; i < body.Count; i++)
            {
                Brush brush = (i == 0) ? Brushes.DarkGreen : Brushes.LimeGreen;
                g.FillEllipse(brush, body[i].X, body[i].Y, CellSize, CellSize);
            }
        }
    }
}