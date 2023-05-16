
// Score 1.0, by Cliff Earl, Antix Development, April 2019

namespace MegaMemory
{
    class Score
    {
        public string Name;
        public int Points;

        /// <summary>
        /// Create a new HighScore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        public Score(string name, int score)
        {
            Name = name;
            Points = score;
        }
    }
}
