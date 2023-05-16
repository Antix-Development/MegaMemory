
// HighScores 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MegaMemory
{
    class HighScores
    {
        public List<Score> Scores;

        /// <summary>
        /// Load highscores
        /// </summary>
        public HighScores()
        {
            Scores = new List<Score>();
            foreach (string line in File.ReadAllLines(Directory.GetCurrentDirectory() + @"\assets\scores.txt"))
            {
                string[] parts = line.Split(',');
                Scores.Add(new Score(parts[0], Convert.ToInt32(parts[1])));
            }
        }

        /// <summary>
        /// Add a new Score
        /// </summary>
        /// <param name="score"></param>
        public void addScore(Score score)
        {
            Scores.Add(score);
        }

        /// <summary>
        /// Sort scores in high to low order
        /// </summary>
        public void sortScores()
        {
            Scores.Sort((a, b) => b.Points.CompareTo(a.Points)); // https://stackoverflow.com/questions/9716273/sort-list-of-object-by-properties-c-sharp
        }

        /// <summary>
        /// Save HighScore file
        /// </summary>
        public void saveScores()
        {
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + @"\assets\scores.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);

            for (int i = 0; i < 10; i++) // save only the top ten and ignore any extras
            {
                Score score = Scores[i];
                string s = score.Name + "," + score.Points;
                writer.WriteLine(s);
            }
            writer.Close();
        }

        /// <summary>
        /// Get formatted text representing HighScores
        /// </summary>
        /// <returns></returns>
        public String[] getStrings()
        {
            // NOTE: this is mildly messy but works so here it is.. in all its glory :)

            String[] strings = new string[6]; // columns of text that will be returned

            StringBuilder n1 = new StringBuilder();
            StringBuilder n2 = new StringBuilder();
            StringBuilder s1 = new StringBuilder();
            StringBuilder s2 = new StringBuilder();

            strings[0] = "1st\r\n3rd\r\n5th\r\n7th\r\n9th";

            n1.Append(Scores[0].Name + "\r\n");
            n1.Append(Scores[2].Name + "\r\n");
            n1.Append(Scores[4].Name + "\r\n");
            n1.Append(Scores[6].Name + "\r\n");
            n1.Append(Scores[8].Name + "\r\n");
            strings[1] = n1.ToString();

            s1.Append(Scores[0].Points + "\r\n");
            s1.Append(Scores[2].Points + "\r\n");
            s1.Append(Scores[4].Points + "\r\n");
            s1.Append(Scores[6].Points + "\r\n");
            s1.Append(Scores[8].Points + "\r\n");
            strings[2] = s1.ToString();

            strings[3] = "2nd\r\n4th\r\n6th\r\n8th\r\n10th";

            n2.Append(Scores[1].Name + "\r\n");
            n2.Append(Scores[3].Name + "\r\n");
            n2.Append(Scores[5].Name + "\r\n");
            n2.Append(Scores[7].Name + "\r\n");
            n2.Append(Scores[9].Name + "\r\n");
            strings[4] = n2.ToString();

            s2.Append(Scores[1].Points + "\r\n");
            s2.Append(Scores[3].Points + "\r\n");
            s2.Append(Scores[5].Points + "\r\n");
            s2.Append(Scores[7].Points + "\r\n");
            s2.Append(Scores[9].Points + "\r\n");
            strings[5] = s2.ToString();

            return strings;
        }
    }
}
