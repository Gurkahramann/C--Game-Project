using DevExpress.Utils.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmLevel2 : Form
    {
        private GameState gameState;
         private List<Trap> traps = new List<Trap>();
        private Timer gameTimer;
        private Keys lastKeyPressed = Keys.None;
        private  PlayerMover playerMover;
        private int elapsedTime = 0;
        public GameState CurrentGameState;
        public string PlayerName { get; set; }
        private bool levelTransitioning = false;
        public int Score { get; set; }
        private bool isPaused = false;
        private bool isRunning = false;
        private TrapManager trapManager = new TrapManager();
        private bool isSecond = false;
        private Random rnd = new Random();

        public frmLevel2(GameState gameState)
        {
            InitializeComponent();
            this.gameState = gameState;
            elapsedTime = (int)gameState.TimeElapsed.TotalSeconds;
            UpdateUI(); 
            InitializeLevel2();
            CurrentGameState = new GameState(UpdateLives, PauseGame, ResumeGame);
            playerMover = new PlayerMover(CurrentGameState);
            if (!mysteryTimer.Enabled)
            {
                mysteryTimer.Interval = 10000; // 10 saniye
                mysteryTimer.Tick += new EventHandler(mysteryTimer_Tick);
                mysteryTimer.Start();
            }

            if (!BombTimer.Enabled)
            {
                BombTimer.Interval = 3000; // 3 saniye
                BombTimer.Tick += BombTimer_Tick; // Burada BombTimer_Tick olayını ekleyin
                BombTimer.Start();
            }

        }
        private void UpdateLives(int newLives)
        {
            lblCan.Text = newLives.ToString();
            if (newLives <= 0)
            {
                timer1.Stop();
                mysteryTimer.Stop();
                MessageBox.Show("Game Over!");
                this.Close();
                this.Dispose();
            }
        }

        private void GoToNextLevel()
        {
            // Timer'ı durdur ve mevcut oyun durumunu kaydet
            BombTimer.Stop();
            BombTimer.Dispose();
            mysteryTimer.Stop();
            mysteryTimer.Dispose();
            gameTimer.Stop();
            gameTimer.Dispose();
            ClearBombs();
            if (levelTransitioning) return;
            levelTransitioning = true;
            CurrentGameState = new GameState(UpdateLives, PauseGame, ResumeGame)
            {
                Lives = gameState.Lives + 1, // Can sayısını artır
                TimeElapsed = TimeSpan.FromSeconds(elapsedTime), // Mevcut süreyi sakla
                Level = 3, // Seviyeyi güncelle
                CurrentForm = "Form2"
            };

            // Yeni seviye formunu oluştur ve göster
            var nextLevelForm = new frmLevel3(CurrentGameState);
            int remainingLives = gameState.Lives;
            int elapsedTimeInSeconds = (int)gameState.TimeElapsed.TotalSeconds; // TimeSpan'i int'e dönüştür
            int score = remainingLives * 500 + (1000 - elapsedTimeInSeconds);
            nextLevelForm.Score = score;
            nextLevelForm.PlayerName = lblOyuncuAd.Text;
            nextLevelForm.Show();
            this.Dispose();
            this.Close(); // Mevcut formu kapat
        }
        private void UpdateUI()
        {
            // Oyun durumu bileşenlerini güncelle
            lblCan.Text = gameState.Lives.ToString();
            lblTimer.Text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
            // Ve diğer UI bileşenlerini güncelle
        }
        private void InitializeLevel2()
        {
            // Oyunun yeni seviye için başlangıç durumlarını ayarlayın
            // Örneğin:
            lblCan.Text = gameState.Lives.ToString();
            lblSeviye.Text = gameState.Level.ToString();
            // Timer'ı alınan süreyle başlat
            if(isSecond)
            {
                isSecond = false;
                return;
            }
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += new EventHandler(timer1_Tick);
            gameTimer.Start();
            elapsedTime = (int)gameState.TimeElapsed.TotalSeconds;
            isSecond = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            lblTimer.Text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
        }

        private void FrmLevel2_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.P)
            {
                TogglePause();
            }
            if (isPaused || e.KeyCode == lastKeyPressed) return;

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A || e.KeyCode == Keys.S || e.KeyCode == Keys.D)
            {
                Point currentLocation = new Point(picturePlayer.Left, picturePlayer.Top);
                Point nextLocation = playerMover.GetNextPosition(currentLocation, e.KeyCode);
                picturePlayer.Left = nextLocation.X;
                picturePlayer.Top = nextLocation.Y;
                
                CheckBombs();
                lastKeyPressed = e.KeyCode;
            }
            picturePlayer.BringToFront();
            if (picturePlayer.Bounds.IntersectsWith(pictureFinish.Bounds))
            {
                GoToNextLevel();
            }
        }

        private void frmLevel2_Load(object sender, EventArgs e)
        {
            lblOyuncuAd.Text = PlayerName;
            lblScore.Text = Score.ToString();
        }
        private void CheckBombs()
        {
            foreach (var trap in traps)
            {
                if (trap.IsActive && picturePlayer.Bounds.IntersectsWith(trap.ObjectPictureBox.Bounds))
                {
                    // Player hit a bomb
                    gameState.Lives--;
                    lblCan.Text = gameState.Lives.ToString();
                    trap.ObjectPictureBox.Visible = false; // Make the bomb invisible
                    trap.IsActive = false; // Deactivate the bomb

                    if (gameState.Lives <= 0)
                    {
                        gameTimer.Stop();
                        BombTimer.Stop();
                        timer1.Stop();
                        MessageBox.Show("Game Over!");
                        this.Dispose();
                        this.Close(); // Close the form if no lives are left
                        trap.IsActive = false;
                        return;
                    }
                    break;
                }
            }
        }
        private void DropBombs()
        {
            // Assuming bombImage is the image you've added to your resources for the bomb
            Image bombImage = Properties.Resources.bombapng;
            Random rnd = new Random();

            // Clear the existing bombs
            ClearBombs();

            // Add new bombs
            for (int i = 0; i < 10; i++) // Assuming you want to drop 10 bombs
            {
                PictureBox bombPictureBox = new PictureBox
                {
                    Size = new Size(91, 68), // Assuming this is the size of your grid
                    Image = bombImage,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                // Use the same GetRandomLocation function to place bombs
                bombPictureBox.Location = GetRandomLocation(rnd);

                // Add the bomb PictureBox to your form and traps list
                traps.Add(new Trap { ObjectPictureBox = bombPictureBox, IsActive = true });
                this.Controls.Add(bombPictureBox);
                bombPictureBox.BringToFront();
            }
        }
        private Point GetRandomLocation(Random rnd)
        {
            List<Point> gridLocations = new List<Point>
            {
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
            };
            int index = rnd.Next(gridLocations.Count);
            return gridLocations[index];
        }
        private void ClearBombs()
        {
            foreach (var bomb in traps)
            {
                this.Controls.Remove(bomb.ObjectPictureBox);
                bomb.ObjectPictureBox.Dispose(); // This is important to free resources
            }
            traps.Clear(); // Clear the list after removing bombs from the form
        }
        private void TogglePause()
        {
            if (isPaused)
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
            BombTimer.Stop();
            mysteryTimer.Stop();
            timer1.Stop();
            isPaused = true;
            lblGameStatus.Text = "Game Stopped";
        }

        private void ResumeGame()
        {
            // Zamanlayıcıları başlat
            gameTimer.Start();
            BombTimer.Start(); // Bomba timer'ını başlat
            mysteryTimer.Start();
            isPaused = false;
            lblGameStatus.Text = "Game Resumed";
        }
       
        private void BombTimer_Tick(object sender, EventArgs e)
        {
            if (!isPaused) // Oyun duraklatılmışsa bomba düşürme ve kontrol etme işlemlerini yapma
            {
                DropBombs(); // Yeni bombaları yerleştir
                CheckBombs(); // Bombaları kontrol et
            }
        }
        private void ApplyMysteryBoxEffect()
        {
            int chance = rnd.Next(100); // 0 ile 99 arasında rastgele bir sayı üretir
            if (isRunning)
            {
                isRunning = false;
                return;
            }

            if (chance < 80) // %80 ihtimal
            {
                gameState.Lives++; // Can ekle
                lblCan.Text = gameState.Lives.ToString();
            }
            else // %20 ihtimal
            {
                gameState.Lives--; // Can azalt
                lblCan.Text = gameState.Lives.ToString();
                if (gameState.Lives <= 0)
                {
                    timer1.Stop();
                    mysteryTimer.Stop();
                    MessageBox.Show("Game Over!");
                    this.Close();
                    this.Dispose();
                }
            }
            lblCan.Text = gameState.Lives.ToString(); // Güncellenmiş can sayısını etikete yaz
            isRunning = true;

        }
        private void mysteryTimer_Tick(object sender, EventArgs e)
        {
            ApplyMysteryBoxEffect();
        }

        private void FrmLevel2_KeyUp(object sender, KeyEventArgs e)
        {
            lastKeyPressed = Keys.None;
        }

        private void lblSeviye_Click(object sender, EventArgs e)
        {

        }
    }
}
