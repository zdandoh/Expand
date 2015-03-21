using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Sector
    {
        public List<Star> stars = new List<Star>();
        public List<GameObject> space_objects = new List<GameObject>();
        public Sector()
        {

        }

        public void generate()
        {
            int SECTOR_SIZE = 5000;
            int STAR_COUNT = 5000;
            for (int counter = 0; counter < STAR_COUNT; counter++)
            {
                int x_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                int y_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                Star new_star = new Star(x_coord, y_coord);
                this.stars.Add(new_star);
            }
        }
    }
}
