using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Soldier
    {
        public PictureBox SoldierPictureBox { get; set; }
        public bool IsActive { get; set; }
        private List<PictureBox> enemySoldiers = new List<PictureBox>();
        public Soldier()
        {
            IsActive = true;
            SoldierPictureBox = new PictureBox
            {
                Size = new Size(91, 68), // Grid boyutuna göre ayarlayın
                SizeMode = PictureBoxSizeMode.StretchImage
            };
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
    }

}
