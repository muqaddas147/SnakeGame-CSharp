using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    class GameManager
    {
        private GameBoard board;
        private Snake snake;
        private Food food;
        private List<Obstacle> obstacles;
        private Random random;
        private Difficulty difficulty;
        private int score;
        private bool isGameOver;
        private bool isPaused;
        private bool ateFood;

        public GameManager(GameBoard board, Difficulty difficulty)
        {
            this.board = board;
            this.difficulty = difficulty;
            random = new Random();
            score = 0;
            isGameOver = false;
            isPaused = false;
            ateFood = false;

            Point start = new Point(200, 200);
            snake = new Snake(5, start, board.CellSize);

            obstacles = new List<Obstacle>();
            CreateObstacles(start.Y);
            SpawnFood();
        }

        public int Score { get { return score; } }
        public bool IsGameOver { get { return isGameOver; } }
        public bool IsPaused { get { return isPaused; } }
        public bool AteFood { get { return ateFood; } }

        // Difficulty decides the timer interval (smaller = faster)
        public int TimerInterval
        {
            get
            {
                switch (difficulty)
                {
                    case Difficulty.Easy: return 200;
                    case Difficulty.Medium: return 120;
                    default: return 70;
                }
            }
        }

        // Harder levels also get more obstacles
        private int ObstacleCount
        {
            get
            {
                switch (difficulty)
                {
                    case Difficulty.Easy: return 0;
                    case Difficulty.Medium: return 4;
                    default: return 8;
                }
            }
        }

        // Called by the timer on every tick
        // Called by the timer on every tick
        public void Update()
        {
            ateFood = false;
            if (isGameOver || isPaused) return;

            // Look at the cell the head is about to enter BEFORE moving
            Point next = snake.NextHead();

            // Collision detection: wall, obstacle or own body
            if (!board.IsInside(next) || IsObstacleAt(next) || snake.WouldHitBody(next))
            {
                isGameOver = true;   // the snake does not move into the blocked cell
                return;
            }

            snake.Move();

            // Eating food
            if (snake.Head == food.Position)
            {
                score += food.Points;
                snake.Grow();
                ateFood = true;
                SpawnFood();
            }
        }

        public void ChangeDirection(int newDirection)
        {
            if (!isPaused && !isGameOver)
                snake.ChangeDirection(newDirection);
        }

        public void TogglePause()
        {
            if (!isGameOver)
                isPaused = !isPaused;
        }

        // Polymorphism: every item is handled through the GameObject type
        public void Draw(Graphics g)
        {
            List<GameObject> items = new List<GameObject>();
            items.Add(food);
            foreach (Obstacle o in obstacles)
                items.Add(o);
            items.Add(snake);

            foreach (GameObject item in items)
                item.Draw(g);
        }

        private void SpawnFood()
        {
            Point p;
            do
            {
                p = board.RandomCell(random);
            } while (!IsFree(p));

            if (random.Next(100) < 20)
                food = new GoldenFood(p, board.CellSize);   // 20% chance
            else
                food = new Food(p, board.CellSize);
        }

        private void CreateObstacles(int safeRow)
        {
            while (obstacles.Count < ObstacleCount)
            {
                Point p = board.RandomCell(random);

                // safeRow = the row the snake starts in, keep it clear
                if (p.Y != safeRow && IsFree(p))
                    obstacles.Add(new Obstacle(p, board.CellSize));
            }
        }

        // A cell is free if neither the snake nor an obstacle is on it
        private bool IsFree(Point p)
        {
            return !snake.Contains(p) && !IsObstacleAt(p);
        }

        private bool IsObstacleAt(Point p)
        {
            foreach (Obstacle o in obstacles)
            {
                if (o.Position == p)
                    return true;
            }
            return false;
        }
    }
}
