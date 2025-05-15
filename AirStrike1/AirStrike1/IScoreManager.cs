using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStrike1
{
    internal interface IScoreManager
    {
        void IncreaseScore(int amount);
        int GetScore();
    }
}
