using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class GameForm : Form
    {
        private List<Trap> traps = new List<Trap>();
        private Timer gameTimer;
        //private bool isPaused = false;
        private PlayerMover playerMover;
        private Random rnd = new Random();
        private int elapsedTime = 0;
        private GameState CurrentGameState;
        private bool levelTransitioning = false;
        private Timer mysteryBoxTimer = new Timer();
        public string PlayerName { get; set; }
        private Keys lastKeyPressed = Keys.None;
        private TrapManager trapManager = new TrapManager();

        public GameForm()
        {
            InitializeComponent();
            InitializeGame();
            lblSeviye.Text = CurrentGameState.Level.ToString();
        }
        private void InitializeGame()
        {
            this.KeyDown += new KeyEventHandler(GameForm_KeyDown);
            this.KeyPreview = true;

            CurrentGameState = new GameState(UpdateLives, PauseGame, ResumeGame);
            playerMover = new PlayerMover(CurrentGameState);
            CurrentGameState.CurrentForm = "Form1";
            trapManager.InitializeTraps();

            mysteryBoxTimer = new Timer();
            mysteryBoxTimer.Interval = 10000; // 10 saniye
            mysteryBoxTimer.Tick += MysteryTimer_Tick;
            mysteryBoxTimer.Start();
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += new EventHandler(timer1_Tick);
            gameTimer.Start();

            AddTrapsToForm();
            UpdateUI();
        }
        private void AddTrapsToForm()
        {
            foreach (var trap in trapManager.Traps)
            {
                this.Controls.Add(trap.ObjectPictureBox);
                trap.ObjectPictureBox.BringToFront();
            }
        }
        private void UpdateUI()
        {
            lblCan.Text = CurrentGameState.Lives.ToString();
            lblSeviye.Text = CurrentGameState.Level.ToString();
            lblTimer.Text = CurrentGameState.TimeElapsed.ToString(@"mm\:ss");
        }

        private void UpdateLives(int newLives)
        {
            lblCan.Text = newLives.ToString();
            if (newLives <= 0)
            {
                timer1.Stop();
                mysteryBoxTimer.Stop();
                TogglePause();
                MessageBox.Show("Game Over!");
                this.Close();
                this.Dispose();
            }
        }   
        private void GoToNextLevel()
        {
            // Timer'ı durdur ve mevcut oyun durumunu kaydet
            if (levelTransitioning) return;
            levelTransitioning = true;
            mysteryBoxTimer.Stop();
            mysteryBoxTimer.Dispose();
            gameTimer.Stop();
            
            CurrentGameState = new GameState(UpdateLives,ResumeGame,PauseGame)
            {
                Lives = CurrentGameState.Lives + 1, // Can sayısını artır
                TimeElapsed = TimeSpan.FromSeconds(elapsedTime), // Mevcut süreyi sakla
                Level = 2, // Seviyeyi güncelle
            };
            traps.Clear();
            int remainingLives = CurrentGameState.Lives;
            int elapsedTimeInSeconds = (int)CurrentGameState.TimeElapsed.TotalSeconds; // TimeSpan'i int'e dönüştür
            int score = remainingLives * 500 + (1000 - elapsedTimeInSeconds);
            // Yeni seviye formunu oluştur ve göster

            var nextLevelForm = new frmLevel2(CurrentGameState);
            nextLevelForm.Score = score;
            nextLevelForm.PlayerName = lblOyuncuAd.Text;
            nextLevelForm.Show();
            this.Close(); // Mevcut formu kapat
            gameTimer.Dispose();
            mysteryBoxTimer.Dispose();
            CurrentGameState.CurrentForm = "Form1";
            this.Dispose();
        }
        private void CheckTraps()
        {
            trapManager.CheckTraps(picturePlayer, GameOver);
        }
        private void GameOver()
        {
            CurrentGameState.Lives--;
            lblCan.Text = CurrentGameState.Lives.ToString();
            if (CurrentGameState.Lives <= 0)
            {
                // Oyunu bitir
                gameTimer.Stop();
                TogglePause();
                MessageBox.Show("Game Over!");
                this.Close();
                this.Dispose();
            }
        }
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                TogglePause();
            }
            if (CurrentGameState.isPaused || e.KeyCode == lastKeyPressed) return;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A || e.KeyCode == Keys.S || e.KeyCode == Keys.D)
            {
                Point currentLocation = new Point(picturePlayer.Left, picturePlayer.Top);
                Point nextLocation = playerMover.GetNextPosition(currentLocation, e.KeyCode);
                picturePlayer.Left = nextLocation.X;
                picturePlayer.Top = nextLocation.Y;

                PlayerMoved();
                lastKeyPressed = e.KeyCode;
            }
            picturePlayer.BringToFront();
            if (picturePlayer.Bounds.IntersectsWith(pictureFinish.Bounds))
            {
                GoToNextLevel();
            }
        }
        private void TogglePause()
        {
            if (CurrentGameState.isPaused)
            {
                CurrentGameState.ResumeGame();
            }
            else
            {
                CurrentGameState.PauseGame();
            }
        }
        private void PauseGame()
        {
            // Zamanlayıcıları durdur
            gameTimer.Stop();
            mysteryBoxTimer.Stop();
            CurrentGameState.isPaused = true;
            lblGameStatus.Text = "Game Stopped";
        }

        private void ResumeGame()
        {
            // Zamanlayıcıları başlat
            gameTimer.Start();
            mysteryBoxTimer.Start();
            CurrentGameState.isPaused = false;
             lblGameStatus.Text = "Game Resumed";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            lblTimer.Text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            lblOyuncuAd.Text = PlayerName;
            this.KeyDown += new KeyEventHandler(GameForm_KeyDown);
            this.KeyUp += new KeyEventHandler(this.GameForm_KeyUp);

        }
        private void PlayerMoved()
        {
            CheckTraps(); // Tuzakları kontrol et
        }

        private void MysteryTimer_Tick(object sender, EventArgs e)
        {
            ApplyMysteryBoxEffect();
        }
        private void ApplyMysteryBoxEffect()
        {
            int chance = rnd.Next(100);
            if (CurrentGameState.isPaused) return;
            if (chance < 80) // %80 ihtimal
            {
                CurrentGameState.Lives++; // Can ekle
            }
            else // %20 ihtimal
            {
                CurrentGameState.Lives--; // Can azalt
                //if (CurrentGameState.Lives <= 0)
                //{

                //    mysteryBoxTimer.Stop();
                //    MessageBox.Show("Game Over!");
                //    // Burada oyun bitirme işlemleri yapılabilir
                
            }
            UpdateLives(CurrentGameState.Lives);
        }
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            lastKeyPressed = Keys.None;
        }
    }
}


