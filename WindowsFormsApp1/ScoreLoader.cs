using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ScoreLoader
    {
        public List<ScoreBoard> LoadTopScores(string filePath, int topCount = 5)
        {
            var scores = new List<ScoreBoard>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(new[] { "Oyuncu Adı :", " Skor :" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    scores.Add(new ScoreBoard { PlayerName = parts[0].Trim(), PlayerScore = int.Parse(parts[1].Trim()) });
                }
            }
            return scores.OrderByDescending(s => s.PlayerScore).Take(topCount).ToList();
        }
    }
}
