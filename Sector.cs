using Newtonsoft.Json;
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
        public String sector_name;
        public String sector_file_location;
        public bool is_loaded = false;
        public int[] coords = new int[2];
        public Sector(int x, int y)
        {
            sector_name = "sector" + x + "." + y;
            sector_file_location = getSectorFileName(x, y);
            coords[0] = x;
            coords[1] = y;
        }

        public void generate()
        {
            int SECTOR_SIZE = 5000;
            int STAR_COUNT = 5000;
            for (int counter = 0; counter < STAR_COUNT; counter++)
            {
                int x_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                int y_coord = Program.game.rand_gen.Next(20, SECTOR_SIZE - 20);
                Star new_star = new Star(x_coord + 5000*coords[0], y_coord + 5000*coords[1]);
                this.stars.Add(new_star);
            }
            this.is_loaded = true;
            this.save();
            Console.WriteLine("Generated " + sector_name);
        }

        public bool exists()
        {
            // Checks if the sector has already been generated and saved.
            return File.Exists(sector_file_location);
        }

        public static bool exists(int x, int y)
        {
            return File.Exists(getSectorFileName(x, y));
        }

        public String formatName()
        {
            return coords[0] + " " + coords[1];
        }

        public void unload()
        {
            foreach (SpaceObject space_object in stars)
            {
                space_object.setDead();
            }
            this.is_loaded = false;
        }

        public void save()
        {
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            System.IO.StreamWriter json_fi = new System.IO.StreamWriter(sector_file_location);
            json_fi.Write(json);
            json_fi.Close();
        }

        public static String getSectorFileName(int x, int y)
        {
            String sector_name = "sector" + x + "." + y;
            return "space//" + sector_name + ".json";
        }

        public void reload()
        {
            // Loads all space objects into the game again
            foreach (SpaceObject space_object in this.stars)
            {
                Program.game.object_handler.addObject(space_object);
            }
        }

        public static Sector load(int x, int y)
        {
            // Loads all the sector data from a file into this sector
            String sector_file = File.ReadAllText(getSectorFileName(x, y));
            //JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            Sector loaded_sector = (Sector) Newtonsoft.Json.JsonConvert.DeserializeObject<Sector>(sector_file);
            return loaded_sector;
        }
    }
}
