using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Expand
{
    class Sector
    {
        public List<Star> stars = new List<Star>();
        public List<GameObject> space_objects = new List<GameObject>();
        public String sector_name;
        public Sector(int x, int y)
        {
            sector_name = "sector-" + x + "-" + y;
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
            Console.WriteLine("Generated " + sector_name);
        }

        public void save()
        {
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            System.IO.StreamWriter json_fi = new System.IO.StreamWriter("space//" + this.sector_name + ".json");
            json_fi.Write(json);
            json_fi.Close();
        }

        public static Sector load(String sector_name)
        {
            // Loads all the sector data from a file into this sector
            String sector_file = File.ReadAllText("space//" + sector_name + ".json");
            Sector loaded_sector = (Sector) Newtonsoft.Json.JsonConvert.DeserializeObject(sector_file);
            return loaded_sector;
        }
    }
}
