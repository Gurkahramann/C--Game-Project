using DevExpress.UIAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmLevel3 : Form
    {
        private GameState gameState;
        private List<Trap> traps = new List<Trap>();
        private Timer gameTimer;
        private List<Soldier> enemySoldiers = new List<Soldier>();
        private int elapsedTime = 0;
        private Keys lastKeyPressed = Keys.None;
        //private bool isPaused = false;
        private PlayerMover playerMover;
        private Random rnd = new Random();
        private GameState CurrentGameState;
        private bool isRunning = false;
        public string PlayerName { get; set; }
        private bool levelTransitioning = false;

        private TrapManager trapManager = new TrapManager();
        public int Score { get; set; }
        public frmLevel3(GameState gameState)
        {
            InitializeComponent();
            this.gameState = gameState;
            elapsedTime = (int)gameState.TimeElapsed.TotalSeconds;

            gameTimer = new Timer
            {
                Interval = 1000 // 1 saniye
            };
            //gameTimer.Tick += timer1_Tick; // EventHandler ekle
            gameTimer.Start(); // Timer'ı başlat
            if (!mysteryTimer.Enabled)
            {
                mysteryTimer.Interval = 10000; // 10 saniye
                mysteryTimer.Tick += new EventHandler(mysteryTimer_Tick);
                mysteryTimer.Start();
            }
            UpdateUI();
            InitializeLevel3();
            if(!SoldierTimer.Enabled)
            {
                Timer soldierTimer = new Timer();
                soldierTimer.Interval = 2000; // 2 saniye
                soldierTimer.Tick += SoldierTimer_Tick;
                soldierTimer.Start();
            }
            if(!MoveTimer.Enabled)
            {
                Timer moveTimer = new Timer();
                moveTimer.Interval = 1000; // 1 saniye
                moveTimer.Tick += MoveTimer_Tick;
                moveTimer.Start();
            }

        }
        private void InitializeLevel3()
        {
            // Oyunun yeni seviye için başlangıç durumlarını ayarlayın
            // Örneğin:
            lblCan.Text = gameState.Lives.ToString();
            lblSeviye.Text = gameState.Level.ToString();
            playerMover = new PlayerMover(gameState);
            // Timer'ı alınan süreyle başlat
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += new EventHandler(timer1_Tick);
            gameTimer.Start();
            elapsedTime = (int)gameState.TimeElapsed.TotalSeconds;
        }
        private void UpdateUI()
        {
            // Oyun durumu bileşenlerini güncelle
            lblCan.Text = gameState.Lives.ToString();
            lblTimer.Text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
            // Ve diğer UI bileşenlerini güncelle
        }
        private void frmLevel3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                TogglePause();
            }
            if (gameState.isPaused || e.KeyCode == lastKeyPressed) return;

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A || e.KeyCode == Keys.S || e.KeyCode == Keys.D)
            {
                Point currentLocation = new Point(picturePlayer.Left, picturePlayer.Top);
                Point nextLocation = playerMover.GetNextPosition(currentLocation, e.KeyCode);
                picturePlayer.Left = nextLocation.X;
                picturePlayer.Top = nextLocation.Y;

                lastKeyPressed = e.KeyCode;
            }
            picturePlayer.BringToFront();
            CheckTraps();
            if (picturePlayer.Bounds.IntersectsWith(pictureFinish.Bounds))
            {
                FinishAndReturnMainMenu();
            }
        }
        private void TogglePause()
        {
            if (gameState.isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        private void PauseGame()
        {
            // Zamanlayıcıları durdur
            gameTimer.Stop();
            mysteryTimer.Stop();
            gameState.isPaused = true;
            lblGameStatus.Text = "Game Stopped";
        }

        private void ResumeGame()
        {
            // Zamanlayıcıları başlat
            gameTimer.Start();
            mysteryTimer.Start();
            gameState.isPaused = false;
            lblGameStatus.Text = "Game Resumed";
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            lblTimer.Text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
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
        private void FinishAndReturnMainMenu()
        {
            // Timer'ı durdur ve mevcut oyun durumunu kaydet
        
            if (levelTransitioning) return;
            levelTransitioning = true;
            gameTimer.Stop();
            SoldierTimer.Stop();
            MoveTimer.Stop();
            CurrentGameState = new GameState(UpdateLives,PauseGame,ResumeGame)
            {
                Lives = gameState.Lives, // Can sayısını artır
                TimeElapsed = TimeSpan.FromSeconds(elapsedTime), // Mevcut süreyi sakla
                Level = 3, // Seviyeyi güncelle
            };
            int remainingLives = CurrentGameState.Lives;
            int elapsedTimeInSeconds = (int)gameState.TimeElapsed.TotalSeconds; // TimeSpan'i int'e dönüştür
            int score = remainingLives * 500 + (1000 - elapsedTimeInSeconds);
            using (StreamWriter writer = new StreamWriter("skor.txt", true))
            {
                writer.WriteLine("Oyuncu Adı :"+PlayerName+" Skor :"+score);
            }
            this.Close(); // Mevcut formu kapat
            this.Dispose();
            FrmMainMenu mainMenu = new FrmMainMenu();
            mainMenu.Show();
        }
        private void CheckTraps()
        {
            // Oyuncu her hareket ettiğinde tuzak kontrolü yap
            foreach (var soldier in enemySoldiers)
            {
                if (soldier.IsActive && picturePlayer.Bounds.IntersectsWith(soldier.SoldierPictureBox.Bounds))
                {
                    soldier.IsActive = false; // Askeri devre dışı bırak
                    soldier.SoldierPictureBox.Visible = false;
                    gameState.Lives--; // Oyuncunun canını azalt
                    lblCan.Text = gameState.Lives.ToString();
                    if (gameState.Lives <= 0)
                    {
                        gameTimer.Stop();
                        TogglePause(); // Kullanıcı OK basmazsa arkada işlemlerin devamını engellemek amaçlı
                        MessageBox.Show("Game Over!");
                        this.Dispose();
                        this.Close();
                    }
                    break; // Döngüden çık, çünkü birden fazla asker aynı anda etkin olamaz
                }
            }
        }
        private void CreateSoldiers()
        {
            if (gameState.isPaused) return;

            Image soldierImage = Properties.Resources.asker; // Askere ait resmi belirtin
            Random rnd = new Random();

            Soldier enemySoldier = new Soldier();
            enemySoldier.SoldierPictureBox.Size = new Size(91, 68); // Boyutu ayarla
            enemySoldier.SoldierPictureBox.Image = soldierImage;
            enemySoldier.SoldierPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Point randomLocation = GetRandomLocation(rnd); // Rastgele konumu al
            enemySoldier.SoldierPictureBox.Location = randomLocation;

            this.Controls.Add(enemySoldier.SoldierPictureBox);
            enemySoldier.SoldierPictureBox.BringToFront();
            enemySoldiers.Add(enemySoldier); // Askerleri listeye ekle
        }

        private void SoldierTimer_Tick(object sender, EventArgs e)
        {
            CreateSoldiers();
            //if (gameState.isPaused) return;
            //Image soldierImage = Properties.Resources.asker;
            //Random rnd = new Random();
            //PictureBox enemySoldierBox = new PictureBox
            //{
            //    Size = new Size(91, 68), // Assuming this is the size of your grid
            //    Image = soldierImage,
            //    SizeMode = PictureBoxSizeMode.StretchImage
            //};
            //Point randomLocation = GetRandomLocation(rnd); // Rastgele konumu al
            //enemySoldierBox.Location = randomLocation;
            //traps.Add(new Trap { TrapPictureBox = enemySoldierBox, IsActive = true });
            //this.Controls.Add(enemySoldierBox);
            //enemySoldierBox.BringToFront(); // Askerleri taşların önüne getir
            //enemySoldiers.Add(enemySoldierBox);
        }
        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (gameState.isPaused) return;

            foreach (var soldier in enemySoldiers.ToList())
            {
                if (!soldier.IsActive) continue;

                Point currentLocation = new Point(soldier.SoldierPictureBox.Left, soldier.SoldierPictureBox.Top);
                Point nextLocation = Trap.TrapsLocations().Where(p => p.Y == currentLocation.Y && p.X < currentLocation.X)
                                                .OrderByDescending(p => p.X)
                                                .FirstOrDefault();

                if (nextLocation == Point.Empty || soldier.SoldierPictureBox.Left <= 242)
                {
                    this.Controls.Remove(soldier.SoldierPictureBox);
                    soldier.SoldierPictureBox.Dispose();
                    soldier.IsActive = false;
                }
                else
                {
                    soldier.SoldierPictureBox.Left = nextLocation.X;
                    soldier.SoldierPictureBox.Top = nextLocation.Y;
                }
            }
            CheckTraps();

            //if (gameState.isPaused) return;
            //List<Point> gridLocations = Trap.TrapsLocations();

            //foreach (var soldier in enemySoldiers.ToList()) // ToList() ile listenin bir kopyası üzerinde işlem yap
            //{
            //    Point currentLocation = new Point(soldier.Left, soldier.Top);
            //    Point nextLocation = gridLocations.Where(p => p.Y == currentLocation.Y && p.X < currentLocation.X)
            //                                      .OrderByDescending(p => p.X)
            //                                      .FirstOrDefault();

            //    if (nextLocation != Point.Empty)
            //    {
            //        soldier.Left = nextLocation.X;
            //        soldier.Top = nextLocation.Y;
            //    }

            //    if (nextLocation == Point.Empty || soldier.Left <= 242 && (soldier.Top == 166 || soldier.Top == 240 || soldier.Top == 314))
            //    {
            //        // Ekranın sol tarafına ulaştıysa askeri sil
            //        this.Controls.Remove(soldier);
            //        soldier.Dispose();
            //        enemySoldiers.Remove(soldier); // Listeden çıkar

            //    }
            //}

            //CheckTraps(); // Tuzakları kontrol etme fonksiyonunu çağır
        }
        //private void MoveTimer_Tick(object sender, EventArgs e)
        //{
        //    if (gameState.isPaused) return;
        //    List<Point> gridLocations = Trap.TrapsLocations();

        //    foreach (var soldier in enemySoldiers.ToList()) // ToList() ile listenin bir kopyası üzerinde işlem yap
        //    {
        //        // Mevcut asker konumunu bul
        //        Point currentLocation = new Point(soldier.Left, soldier.Top);



        //            Point nextLocation = Trap.TrapsLocations().Where(p => p.Y == currentLocation.Y && p.X < currentLocation.X)
        //                                        .OrderByDescending(p => p.X)
        //                                        .FirstOrDefault();
        //            soldier.Left = nextLocation.X;
        //            soldier.Top = nextLocation.Y;
        //        if(nextLocation==Point.Empty)
        //        {
        //            this.Controls.Remove(soldier);
        //            soldier.Dispose();
        //            enemySoldiers.Remove(soldier);
        //        }
        //        if(soldier.Left<=242 && (soldier.Top==166 || soldier.Top == 240 || soldier.Top == 314))
        //        {

        //            // Ekranın sol tarafına ulaştıysa askeri sil
        //            this.Controls.Remove(soldier);
        //            soldier.Dispose();
        //            enemySoldiers.Remove(soldier); // Listeden çıkar
        //        }
        //        CheckTraps();
        //    }
        //}
        private void ApplyMysteryBoxEffect()
        {
            if (isRunning)
            {
                isRunning = false;
                return;
            }
            int chance = rnd.Next(100); // 0 ile 99 arasında rastgele bir sayı üretir

            if (chance < 80) // %80 ihtimal
            {
                gameState.Lives++; // Can ekle
                isRunning = true;
            }
            else // %20 ihtimal
            {
                gameState.Lives--; // Can azalt
                lblCan.Text = gameState.Lives.ToString();
                if (gameState.Lives <= 0)
                {
                    timer1.Stop();
                    mysteryTimer.Stop();
                    TogglePause();
                    MessageBox.Show("Game Over!");
                    this.Close();
                    this.Dispose();
                }
            }
            lblCan.Text = gameState.Lives.ToString(); // Güncellenmiş can sayısını etikete yaz
            isRunning = true;
        }
        private void frmLevel3_Load(object sender, EventArgs e)
        {
            lblOyuncuAd.Text = PlayerName;
            lblScore.Text = Score.ToString();
        }

        private void frmLevel3_KeyUp(object sender, KeyEventArgs e)
        {
            lastKeyPressed = Keys.None;
        }

        private void mysteryTimer_Tick(object sender, EventArgs e)
        {
            ApplyMysteryBoxEffect();
        }
    }
}
