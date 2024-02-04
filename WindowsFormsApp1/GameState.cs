using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WindowsFormsApp1
{
    public class GameState
    {
        public int Lives { get; set; }
        private Random rnd = new Random();
        public int Score { get; set; }
        public int Level { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public  string PlayerName { get; set; }
        public bool isPaused = false;
        private Timer mysteryBoxTimer;
        private Action<int> updateLivesAction; 
        private Action pauseGameAction;
        public string CurrentForm { get; set; }
        private Action resumeGameAction;
        public GameState(Action<int> updateLives, Action pauseGame, Action resumeGame)
        {
            Lives = 3;
            Score = 0;
            Level = 1;
            TimeElapsed = TimeSpan.Zero;
            updateLivesAction = updateLives;
            pauseGameAction = pauseGame;
            resumeGameAction = resumeGame;
        }

        public void PauseGame()
        {
            pauseGameAction?.Invoke();
        }

        public void ResumeGame()
        {
            resumeGameAction?.Invoke();
        }

        public void NextLevel()
        {
            Level++;
            Lives++; // Her yeni seviyede can 1 artacak
                     // Not: Süre ve puan gibi diğer değerlerin nasıl güncelleneceği oyununuzun kurallarına bağlıdır.
        }
        public GameState CurrentGameState { get; private set; }
        public List<Point> GetGridLocations()
        {
            return new List<Point>
            {
                new Point(145, 166),
                new Point(145, 240),
                new Point(145, 314),
                new Point(242, 166),
                new Point(242, 240),
                new Point(242, 314),
                new Point(339, 166),
                new Point(339, 240),
                new Point(339, 314),
                new Point(436, 166),
                new Point(436, 240),
                new Point(436, 314),
                new Point(533, 166),
                new Point(533, 240),
                new Point(533, 314),
                new Point(921, 314),
                new Point(921, 240),
                new Point(921, 166),
                new Point(824, 314),
                new Point(824, 240),
                new Point(824, 166),
                new Point(727, 314),
                new Point(727, 240),
                new Point(727, 166),
                new Point(630, 314),
                new Point(630, 240),
                new Point(630, 166),
                new Point(1309, 314),
                new Point(1309, 240),
                new Point(1309, 166),
                new Point(1212, 314),
                new Point(1212, 240),
                new Point(1212, 166),
                new Point(1115, 314),
                new Point(1115, 240),
                new Point(1115, 166),
                new Point(1018, 314),
                new Point(1018, 240),
                new Point(1018, 166),
                new Point(1406, 240),
            };
        }

    }
}
