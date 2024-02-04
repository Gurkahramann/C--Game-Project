using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Trap:IGameObject
    {
        public PictureBox ObjectPictureBox { get; set; }
        public bool IsActive { get; set; }

        public Trap()
        {
            IsActive = true;
            ObjectPictureBox = new PictureBox
            {
                Size = new Size(91, 68),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }
        public void InitializeObject()
        {
            // Tuzakları başlatmak için kullanılan kod
            // Örneğin, resim ataması ve rastgele konum belirleme
            Image[] trapImages = {
                Properties.Resources.tuzakAlev,
                Properties.Resources.tuzakKapan,
                Properties.Resources.tuzakDiken
            };
            Random rnd = new Random();
            ObjectPictureBox.Image = trapImages[rnd.Next(trapImages.Length)];
            ObjectPictureBox.Location = GetRandomLocation();
        }
        public void ClearObject()
        {
            // Tuzakları temizlemek için kullanılan kod
            ObjectPictureBox.Visible = false;
            IsActive = false;
        }
        private Point GetRandomLocation()
        {
            // Rastgele konum üretme kodunuzu buraya ekleyin
            List<Point> availableLocations = TrapsLocations();
            Random rnd = new Random();
            int index = rnd.Next(availableLocations.Count);
            return availableLocations[index];
        }
        public static List<Point> TrapsLocations()
        {
            return new List<Point>
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
        }
        public void UpdatePosition()
        {
            throw new NotImplementedException();
        }

    }
}
    


