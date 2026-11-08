using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;


namespace SnakeGame
{
    public partial class Form1 : Form
    {
        
        private GameBoard board;
        private GameManager game;
        private HighScoreManager highScores;
        private int highScore;

        public Form1()
{
    InitializeComponent();
    // Black area = exactly 800 x 600: add the border padding (left + right, top + bottom)
    this.ClientSize = new Size(800 + pnlBorder.Padding.Horizontal + pnlSide.Width,
                               600 + pnlBorder.Padding.Vertical);
    this.PerformLayout();

    // Reduces flickering when the panel is redrawn
    typeof(Panel).InvokeMember("DoubleBuffered",
        BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
        null, pnlGame, new object[] { true });

    // Fixed size, so food and obstacles can never spawn outside the visible area
    board = new GameBoard(800, 600, 20);

    highScores = new HighScoreManager(
        Path.Combine(Application.StartupPath, "highscore.txt"));
    highScore = highScores.Load();
    lblHighScore.Text = "High Score: " + highScore;

    cmbDifficulty.SelectedIndex = 1;   // Medium by default
    timer1.Stop();                     // game starts only after Start button
}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            game.Update();

            if (game.AteFood)
                SystemSounds.Asterisk.Play();

            UpdateScore();
            pnlGame.Refresh();

            if (game.IsGameOver)
                EndGame();
        }
         private void EndGame()
        {
            timer1.Stop();
            SystemSounds.Hand.Play();

            string message = "Game Over!\nFinal Score: " + game.Score;

            if (game.Score > highScore)
            {
                highScore = game.Score;
                highScores.Save(highScore);
                lblHighScore.Text = "High Score: " + highScore;
                message += "\nNew High Score!";
            }

            message += "\n\nPlay again?  (Yes = Restart, No = Exit)";

            DialogResult result = MessageBox.Show(message, "Game Over",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                StartNewGame();
            else
                Close();
        }
        //--------------Drawing--------------

        private void pnlGame_Paint(object sender, PaintEventArgs e)
        {
            if (game == null)
            {
                DrawCenteredText(e.Graphics, "Select Difficulty level \n& \nPress Start Game");
                return;
            }

            game.Draw(e.Graphics);

            if (game.IsPaused)
                DrawCenteredText(e.Graphics, "PAUSED");
            else if (game.IsGameOver)
                DrawCenteredText(e.Graphics, "GAME OVER");
            // Wall drawn last, so it stays on top of everything
          
        }

        private void DrawCenteredText(Graphics g, string text)
        {
            using (Font font = new Font("Arial", 32, FontStyle.Bold))
            {
                SizeF size = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.White,
                    (pnlGame.Width - size.Width) / 2,
                    (pnlGame.Height - size.Height) / 2);
            }
        }

        private void UpdateScore()
        {
            lblScore.Text = "Score: " + game.Score;
        }


        //-----------BUTTONS-----------

        private void btnStart_Click(object sender, EventArgs e)
        {
    // A game exists, it has not ended: do not restart it
    if (game != null && !game.IsGameOver)
    {

        // Stop the snake while the message is on screen
        bool wasRunning = timer1.Enabled;
        timer1.Stop();

        MessageBox.Show("The game is already started.\nUse the Restart Game button to begin a new game.",
            "Game Running", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Continue only if the game was running (not paused)
        if (wasRunning)
            timer1.Start();

        return;
}

    StartNewGame(); 
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
    // No game is running yet: restart makes no sense
    if (game == null || game.IsGameOver)
    {
        MessageBox.Show("The game has not started yet.\nClick Start Game first.",
            "Restart", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    StartNewGame();

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
    // No game has been started yet: nothing to pause
    if (game == null || game.IsGameOver)
    {
        MessageBox.Show("The game has not started yet.\nClick Start Game first.",
            "Pause", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    TogglePause();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
    // Stop the snake while the question is on screen
    bool wasRunning = timer1.Enabled;
    timer1.Stop();

    DialogResult result = MessageBox.Show("Do you really want to exit the game?",
        "Exit Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    if (result == DialogResult.Yes)
    {
        Close();                 // exit the game
    }
    else if (wasRunning)
    {
        timer1.Start();          // No: resume the game
    }

        }
        // ---------- Game flow ----------
        private void StartNewGame()
        {
   // Board = exactly the visible black area, rounded down to whole 20 px cells
    board = new GameBoard(pnlGame.ClientSize.Width, pnlGame.ClientSize.Height, 20);

    Difficulty level = (Difficulty)cmbDifficulty.SelectedIndex;
    game = new GameManager(board, level);

    timer1.Interval = game.TimerInterval;
    timer1.Start();

    btnPause.Text = "Pause";
    UpdateScore();
    pnlGame.Invalidate();
        }
        private void TogglePause()
        {
            if (game == null || game.IsGameOver) return;

            game.TogglePause();
            timer1.Enabled = !game.IsPaused;
            btnPause.Text = game.IsPaused ? "Resume" : "Pause";
            pnlGame.Invalidate();
        }

        private void cmbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only restart if a game is already running
            if (game != null && !game.IsGameOver)
                StartNewGame();
        }

         // ---------- Keyboard ----------
        // ProcessCmdKey sees the key BEFORE any button or combo box can steal it,
        // so the arrow keys always control the snake.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (game != null && !game.IsGameOver)
            {
                switch (keyData)
                {
                    case Keys.Up:
                                            case Keys.W:
                        game.ChangeDirection(0);
                        return true;
                    case Keys.Right:
                    case Keys.D:
                        game.ChangeDirection(1);
                        return true;
                    case Keys.Down:
                    case Keys.S:
                        game.ChangeDirection(2);
                        return true;
                    case Keys.Left:
                    case Keys.A:
                        game.ChangeDirection(3);
                        return true;
                    case Keys.P:
                        TogglePause();
 return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
    }

