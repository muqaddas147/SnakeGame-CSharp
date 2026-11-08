using System;
using System.IO;

namespace SnakeGame
{
    class HighScoreManager
    {
        private string filePath;

        public HighScoreManager(string filePath)
        {
            this.filePath = filePath;
        }

        public int Load()
        {
            try
            {
                if (File.Exists(filePath))
                    return int.Parse(File.ReadAllText(filePath).Trim());
            }
            catch (Exception)
            {
                // corrupted file: start from 0
            }
            return 0;
        }

        public void Save(int score)
        {
            File.WriteAllText(filePath, score.ToString());
        }
    }
}
