using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class PlayerMover
    {
        private GameState gameState;

        public PlayerMover(GameState gameState)
        {
            this.gameState = gameState;
        }

        public Point GetNextPosition(Point currentLocation, Keys direction)
        {
            Point nextLocation = currentLocation;
            var gridLocations = gameState.GetGridLocations();

            switch (direction)
            {
                case Keys.W:
                    nextLocation = gridLocations.Where(p => p.X == currentLocation.X && p.Y < currentLocation.Y)
                                                .OrderByDescending(p => p.Y)
                                                .FirstOrDefault();
                    break;
                case Keys.A:
                    nextLocation = gridLocations.Where(p => p.Y == currentLocation.Y && p.X < currentLocation.X)
                                                .OrderByDescending(p => p.X)
                                                .FirstOrDefault();
                    break;
                case Keys.S:
                    nextLocation = gridLocations.Where(p => p.X == currentLocation.X && p.Y > currentLocation.Y)
                                                .OrderBy(p => p.Y)
                                                .FirstOrDefault();
                    break;
                case Keys.D:
                    nextLocation = gridLocations.Where(p => p.Y == currentLocation.Y && p.X > currentLocation.X)
                                                .OrderBy(p => p.X)
                                                .FirstOrDefault();
                    break;
            }

            return nextLocation != Point.Empty ? nextLocation : currentLocation;
        }
    }
}
