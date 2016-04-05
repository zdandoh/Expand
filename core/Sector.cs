using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Expand.core
{
    /// <summary>
    /// An instance of the sector class is a unit of space. Mostly just a container for SpaceObjects and helper methods for saving and loading.
    /// </summary>
    public class Sector
    {
        public List<SpaceObject> space_objects = new List<SpaceObject>();
        public String sector_name;
        public String sector_file_location;
        public const int SECTOR_SIZE = Space.SECTOR_SIZE;
        public bool is_loaded;
        public int[] coords = new int[2];
        public Sector(int x, int y)
        {
            this.is_loaded = false;
            sector_name = "sector" + x + "." + y;
            sector_file_location = getSectorFileName(x, y);
            coords[0] = x;
            coords[1] = y;
        }

        /// <summary>
        /// Generates new sector by populating internal space object list.
        /// </summary>
        public void generate()
        {
            for (int counter = 0; counter < Star.PER_SECTOR; counter++)
            {
                int[] star_coords = getNewObjectPos(Star.MAX_SIZE);
                Star new_star = new Star(this, star_coords[0], star_coords[1]);
            }
            for (int asteroid_counter = 0; asteroid_counter < Asteroid.PER_SECTOR; asteroid_counter++)
            {
                int[] asteroid_coords = getNewObjectPos(Asteroid.MAX_SIZE);
                Asteroid new_asteroid = new Asteroid(this, asteroid_coords[0], asteroid_coords[1]);
            }
            this.saveAsync();
            this.is_loaded = true;
        }

        /// <summary>
        /// Returns a random point inside the sector. Used for placing new space objects.
        /// </summary>
        /// <param name="size_offset">Two random ints inside the sector.</param>
        /// <returns></returns>
        public int[] getNewObjectPos(int size_offset = 0)
        {
            int[] pos = { 0, 0 };
            pos[0] = Program.game.rand_gen.Next(size_offset, SECTOR_SIZE - size_offset) + SECTOR_SIZE*this.coords[0];
            pos[1] = Program.game.rand_gen.Next(size_offset, SECTOR_SIZE - size_offset) + SECTOR_SIZE * this.coords[1];
            return pos;
        }

        /// <summary>
        /// Checks if the sector has already been generated and saved.
        /// </summary>
        /// <returns></returns>
        public bool exists()
        {
            return File.Exists(sector_file_location);
        }

        /// <summary>
        /// Checks if the sector has already been generated and saved.
        /// </summary>
        /// <param name="x">Sector coordinate x</param>
        /// <param name="y">Sector coordinate y</param>
        /// <returns>Boolean true if sector exists, false if it doesn't.</returns>
        public static bool exists(int x, int y)
        {
            return File.Exists(getSectorFileName(x, y));
        }

        /// <summary>
        /// Get name of sector.
        /// </summary>
        /// <returns>Returns sector name in format: "x y"</returns>
        public String formatName()
        {
            return coords[0] + " " + coords[1];
        }

        /// <summary>
        /// Kills all GameObject instances in sector
        /// </summary>
        public void unload()
        {
            int count = 0;
            foreach (SpaceObject space_object in this.space_objects)
            {
                space_object.setDead();
                count++;
            }
            this.is_loaded = false;
        }

        /// <summary>
        /// Saves sector to sector_name.json file.
        /// </summary>
        public void save()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(this, settings);
            System.IO.StreamWriter json_fi = new System.IO.StreamWriter(sector_file_location);
            json_fi.Write(json);
            json_fi.Close();
        }

        /// <summary>
        /// Threaded version of save function.
        /// </summary>
        public void saveAsync()
        {
            ThreadStart thread_target = new ThreadStart(this.save);
            Thread save_thread = new Thread(thread_target);
            save_thread.Start();
        }

        /// <summary>
        /// Gets sector name from sector coordinates
        /// </summary>
        /// <param name="x">Sector x coord</param>
        /// <param name="y">Sector y coord</param>
        /// <returns>Sector name formatted as: sector_save_location + "sectorx.y"</returns>
        public static String getSectorFileName(int x, int y)
        {
            String sector_name = "sector" + x + "." + y;
            return "Content//space//sectors//" + sector_name + ".json";
        }

        /// <summary>
        /// Re-initializes each space object that belongs to a sector.
        /// </summary>
        public void reload()
        {
            foreach (SpaceObject space_object in this.space_objects)
            {
                Program.game.object_handler.addObject(space_object);
            }
        }

        /// <summary>
        /// Loads all the sector data from a file into returned sector.
        /// </summary>
        /// <param name="x">X coordinate of sector to load.</param>
        /// <param name="y">Y coordinate of sector to load.</param>
        /// <returns></returns>
        public static Sector load(int x, int y)
        {
            String sector_file = File.ReadAllText(getSectorFileName(x, y));
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            Sector loaded_sector = (Sector) Newtonsoft.Json.JsonConvert.DeserializeObject<Sector>(sector_file, settings);
            loaded_sector.is_loaded = true;
            return loaded_sector;
        }
    }
}
