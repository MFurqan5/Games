using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStrike1.BL
{
    internal class ScoreManager : IScoreManager
    {
        private int score = 0;

        public void IncreaseScore(int amount)
        {
            score += amount;
        }

        public int GetScore()
        {
            return score;
        }
    }
}
