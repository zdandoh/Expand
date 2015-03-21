using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Expand
{
    public class Sector
    {
        public List<Star> stars = new List<Star>();
        public List<GameObject> space_objects = new List<GameObject>();
        public String sector_name;
        public int[] sector_coords = {0, 0};
        public Sector(int x, int y)
        {
            sector_name = "sector-" + x + "-" + y;
            sector_coords[0] = x;
            sector_coords[1] = y;
        }

        public void generate()
        {
            int SECTOR_SIZE = 5000;
            int STAR_COUNT = 5000;
            for (int counter = 0; counter < STAR_COUNT; counter++)
            {
                int x_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                int y_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                Star new_star = new Star(x_coord + 5000*sector_coords[0], y_coord + 5000*sector_coords[1]);
                this.stars.Add(new_star);
            }
            Console.WriteLine("Generated " + sector_name);
        }

        public bool exists()
        {
            // Checks if the sector has already been generated and saved.
            return File.Exists("space//" + sector_name + ".json");
        }

        public void unload()
        {
            foreach (Star star in stars)
            {
                star.setDead();
            }
        }

        public void save()
        {
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            System.IO.StreamWriter json_fi = new System.IO.StreamWriter("space//" + this.sector_name + ".json");
            json_fi.Write(json);
            json_fi.Close();
        }

        public static Sector load(int x, int y)
        {
            // Loads all the sector data from a file into this sector
            String sector_name = "sector-" + x + "-" + y;
            String sector_file = File.ReadAllText("space//" + sector_name + ".json");
            Sector loaded_sector = (Sector) Newtonsoft.Json.JsonConvert.DeserializeObject<Sector>(sector_file);
            return loaded_sector;
        }
    }
}
