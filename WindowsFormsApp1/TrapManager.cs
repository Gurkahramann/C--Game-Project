using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class TrapManager
    {
        private List<Trap> traps = new List<Trap>();
        private HashSet<Point> usedLocations = new HashSet<Point>();
        private Random rnd = new Random();

        public List<Trap> Traps => traps;

        public void InitializeTraps()
        {
            Image[] trapImages = {
            Properties.Resources.tuzakAlev,
            Properties.Resources.tuzakKapan,
            Properties.Resources.tuzakDiken
        };

            for (int i = 0; i < 10; i++)
            {
                PictureBox trapPictureBox = new PictureBox
                {
                    Size = new Size(91, 68),
                    Image = trapImages[rnd.Next(trapImages.Length)],
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                Point location = GetRandomLocation();
                if (location != Point.Empty)
                {
                    trapPictureBox.Location = location;
                    Trap trap = new Trap
                    {
                        ObjectPictureBox = trapPictureBox,
                        IsActive = true
                    };

                    traps.Add(trap);
                }
            }
        }
        public void ClearBomb()
        {
            foreach (var bomb in traps.ToList()) // Listenin bir kopyası üzerinden yineleme yapın
            {
                bomb.ObjectPictureBox.Visible = false; // Bombayı görünmez yap
                bomb.IsActive = false; // Bombayı devre dışı bırak
                traps.Remove(bomb); // Listeden çıkar
            }
        }

        public void DropBombs()
        {
            Image bombImage = Properties.Resources.bombapng;

            ClearBomb();
            for (int i = 0; i < 10; i++)
            {
                PictureBox bombPictureBox = new PictureBox
                {
                    Size = new Size(91, 68),
                    Image = bombImage,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                Point location = GetRandomLocation();
                if (location != Point.Empty)
                {
                    bombPictureBox.Location = location;
                    Trap trap = new Trap
                    {
                        ObjectPictureBox = bombPictureBox,
                        IsActive = true
                    };

                    traps.Add(trap);
                }
            }
        }
        private Point GetRandomLocation()
        {
            List<Point> availableLocations = Trap.TrapsLocations()
                                                  .Except(usedLocations)
                                                  .ToList();

            if (availableLocations.Count == 0)
            {
                return Point.Empty; // Tüm konumlar kullanıldıysa boş bir konum döndür
            }

            int index = rnd.Next(availableLocations.Count);
            Point selectedLocation = availableLocations[index];
            usedLocations.Add(selectedLocation); // Seçilen konumu kullanılmış olarak işaretle

            return selectedLocation;
        }
        public void CheckTraps(PictureBox playerPictureBox, Action gameOverAction)
        {
            foreach (var trap in traps)
            {
                if (trap.IsActive && playerPictureBox.Bounds.IntersectsWith(trap.ObjectPictureBox.Bounds))
                {
                    trap.IsActive = false;
                    trap.ObjectPictureBox.Visible = false;
                    gameOverAction.Invoke();
                    break;
                }
            }
        }
    }
}
