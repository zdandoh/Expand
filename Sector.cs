using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Expand
{
    public class Sector
    {
        public List<Star> stars = new List<Star>();
        public List<Asteroid> asteroids = new List<Asteroid>();
        public String sector_name;
        public String sector_file_location;
        public const int SECTOR_SIZE = Space.SECTOR_SIZE;
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
            for (int counter = 0; counter < Star.PER_SECTOR; counter++)
            {
                int[] star_coords = getNewObjectPos(Star.MAX_SIZE);
                Star new_star = new Star(star_coords[0], star_coords[1]);
                this.stars.Add(new_star);
            }
            for (int asteroid_counter = 0; asteroid_counter < Asteroid.PER_SECTOR; asteroid_counter++)
            {
                int[] asteroid_coords = getNewObjectPos(Asteroid.MAX_SIZE);
                Asteroid new_asteroid = new Asteroid(asteroid_coords[0], asteroid_coords[1]);
                this.asteroids.Add(new_asteroid);
            }
            this.save();
            this.is_loaded = true;
            Console.WriteLine("Generated " + sector_name);
        }

        public int[] getNewObjectPos(int size_offset = 0)
        {
            int[] pos = { 0, 0 };
            pos[0] = Program.game.rand_gen.Next(size_offset, SECTOR_SIZE - size_offset) + SECTOR_SIZE*this.coords[0];
            pos[1] = Program.game.rand_gen.Next(size_offset, SECTOR_SIZE - size_offset) + SECTOR_SIZE * this.coords[1];
            return pos;
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
            foreach (SpaceObject space_object in asteroids)
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
