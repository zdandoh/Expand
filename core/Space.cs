using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace Expand.core
{
    /// <summary>
    /// Keeps track of which Sectors to keep loaded as the player moves through space.
    /// </summary>
    public class Space: GameObject
    {
        public Color space_color;
        public Texture2D star_texture1 = Program.game.textures["space\\star1.png"];
        public Texture2D star_texture2 = Program.game.textures["space\\star2.png"];
        public Texture2D asteroid_texture = Program.game.textures["space\\asteroid.png"];
        public const int SECTOR_SIZE = 5000;
        public const bool CLEAR_SECTOR = false; // Resets the sector files at runtime if true
        public bool first_load = true; // DO NOT CHANGE THIS UNLESS YOU MEAN IT
        private Sector[,] loaded_sectors;
        private int[] player_sector = {0, 0};
        /// <summary>
        /// Loads all Sectors that are adjacent to the player sector.
        /// </summary>
        public Space()
        {
            player_sector = getSector(Program.game.ship.pos[0], Program.game.ship.pos[1]);
            if (CLEAR_SECTOR)
            {
                Space.clearSpace();
            }
            space_color = new Color(0, 0, 10);
            loaded_sectors = getAdjacentSectors();
            this.player_sector[0] += -1;
            this.player_sector[1] += -1;
        }

        /// <summary>
        /// Checks if player has moved to a new sector, if they have, unload and load any old/new sectors.
        /// </summary>
        public override void update()
        {
            if (!player_sector.SequenceEqual(getSector(Program.game.ship.pos[0], Program.game.ship.pos[1])))
            {
                // Save the sector you were just in
                if (this.first_load)
                {
                    this.first_load = false;
                }
                else
                {
                    this.loaded_sectors[1, 1].saveAsync();
                }
                player_sector = (int[])getSector(Program.game.ship.pos[0], Program.game.ship.pos[1]).Clone();
                ThreadStart thread_target = new ThreadStart(loadAdjacentSectors);
                Thread sector_loader = new Thread(thread_target);
                sector_loader.IsBackground = true;
                sector_loader.Name = "Sector Loader";
                Program.game.object_handler.middle_list_locked = true;
                sector_loader.Start();
            }
        }

        /// <summary>
        /// Deletes all saved sector files
        /// </summary>
        private static void clearSpace()
        {
            DirectoryInfo space_dir = new DirectoryInfo("Content//space//sectors");
            foreach (FileInfo sector_file in space_dir.GetFiles())
            {
                if (sector_file.Name.EndsWith(".json"))
                {
                    sector_file.Delete();
                }
            }
        }

        /// <summary>
        /// Returns a sector from memory. Sector must first be loaded in memory.
        /// </summary>
        /// <param name="x">Sector coordinate X.</param>
        /// <param name="y">Sector coordinate Y.</param>
        /// <returns></returns>
        public Sector findLoadedSector(int x, int y)
        {
            return findSector(x, y, load: false);
        }

        /// <summary>
        /// Obtains sector by: getting from memory, generating new sector, or loading from file
        /// </summary>
        /// <param name="x">Sector coordinate X.</param>
        /// <param name="y">Sector coordinate Y.</param>
        /// <param name="load">Boolean whether or not to load the sector from disk if it is not already in memory.</param>
        /// <returns></returns>
        public Sector findSector(int x, int y, bool load = true)
        {
            Sector return_sector;
            // Check if sector already is loaded
            foreach (Sector loaded_sector in loaded_sectors)
            {
                if (loaded_sector == null)
                {
                    // We unloaded this sector earlier this update
                    continue;
                }
                else if (loaded_sector.coords[0] == x && loaded_sector.coords[1] == y && loaded_sector.is_loaded)
                {
                    return loaded_sector;
                }
            }

            if (Sector.exists(x, y) && load)
            {
                Sector saved_sector = Sector.load(x, y);
                return saved_sector;
            }
            else if(load)
            {
                return_sector = new Sector(x, y);
                return_sector.generate();
            }
            else
            {
                throw new ArgumentOutOfRangeException("Sector not loaded");
            }
            return return_sector;
        }

        /// <summary>
        /// Checks if there is room for a SpaceObject to be placed. 
        /// </summary>
        /// <param name="item_to_place">SpaceObject to be placed in sector.</param>
        /// <returns>Boolean whether or not there is a place for the SpaceObject.</returns>
        public bool canPlace(dynamic item_to_place)
        {
            bool can_place = true;
            int[] sector_coords = getSector(item_to_place.pos[0], item_to_place.pos[1]);
            Sector place_sector = findLoadedSector(sector_coords[0], sector_coords[1]);
            foreach (dynamic sector_item in place_sector.space_objects)
            {
                if (Collider.intersects(item_to_place.getCollideShape(), sector_item.getCollideShape()) && !item_to_place.pos.Equals(sector_item.pos))
                {
                    can_place = false;
                    break;
                }
            }
            return can_place;
        }

        /// <summary>
        /// Checks whether a given sector is still adjacent to the sector that the player is currently in.
        /// </summary>
        /// <param name="old_sector">Sector that may or may not be adjacent.</param>
        /// <param name="adjacent_sectors">Currently adjacent sectors.</param>
        /// <returns></returns>
        public bool sectorStillAdjacent(Sector old_sector, Sector[,] adjacent_sectors){
            bool still_adjacent = false;
            for (int sector_row = 0; sector_row < adjacent_sectors.GetLength(0); sector_row++)
            {
                for (int sector_x = 0; sector_x < adjacent_sectors.GetLength(1); sector_x++)
                {
                    if (old_sector == null)
                    {
                        still_adjacent = false;
                    }
                    else if (old_sector.coords.SequenceEqual(adjacent_sectors[sector_row, sector_x].coords))
                    {
                        still_adjacent = true;
                    }
                }
            }
            return still_adjacent;
        }

        /// <summary>
        /// Loads and unloads new/old sectors after the player moves to a new sector.
        /// </summary>
        public void loadAdjacentSectors()
        {
            Sector[,] adjacent_sectors = getAdjacentSectors();

            // Find sectors that need to be unloaded
            for (int sector_row = 0; sector_row < adjacent_sectors.GetLength(0); sector_row++)
            {
                for (int sector_x = 0; sector_x < adjacent_sectors.GetLength(1); sector_x++)
                {
                    if (!this.sectorStillAdjacent(loaded_sectors[sector_row, sector_x], adjacent_sectors) && loaded_sectors[sector_row, sector_x] != null)
                    {
                        // Sometimes sectors go null and would cause an error if not for the check in the if.
                        // Sector needs to be unloaded
                        loaded_sectors[sector_row, sector_x].unload();
                        loaded_sectors[sector_row, sector_x].space_objects = null;
                        loaded_sectors[sector_row, sector_x] = null;
                    }
                }
            }

            // Load data the adjacent sector array
            for (int sector_row = 0; sector_row < adjacent_sectors.GetLength(0); sector_row++)
            {
                for (int sector_x = 0; sector_x < adjacent_sectors.GetLength(1); sector_x++)
                {
                    int[] sector_coords = adjacent_sectors[sector_row, sector_x].coords;
                    Sector next_sector = findSector(sector_coords[0], sector_coords[1]);
                    adjacent_sectors[sector_row, sector_x] = next_sector;
                    int middle_list_quity = Program.game.object_handler.middle_list.Count();
                }
            }
            loaded_sectors = adjacent_sectors;
            Program.game.object_handler.middle_list_locked = false;
        }

        /// <summary>
        /// Returns the coordinates of the sector that the player is currently in.
        /// </summary>
        /// <returns>Coordinates of sector that player is currently in.</returns>
        public int[] getPlayerSector()
        {
            return getSector(Program.game.ship.pos[0], Program.game.ship.pos[1]);
        }

        /// <summary>
        /// Given space level coordinates, returns the coordinates of the sector that that point resides in.
        /// </summary>
        /// <param name="x">Space level coordinate X.</param>
        /// <param name="y">Space level coordinate Y.</param>
        /// <returns>Sector level coordinate pair.</returns>
        public static int[] getSector(int x, int y)
        {
            // Tracks what sector the player is in, rather clumsily
            int[] sector = {0, 0};
            if (x < 5000 && x > 0 && y < 5000 && y > 0)
            {
                sector[0] = 0;
                sector[1] = 0;
            }
            if (x > 5000)
            {
                sector[0] = x / 5000;
            }
            if (x < 0)
            {
                sector[0] = x / 5000 - 1;
            }
            if (x > 5000)
            {
                sector[1] = y / 5000;
            }
            if (y < 0)
            {
                sector[1] = y / 5000 - 1;
            }
            return sector;
        }

        /// <summary>
        /// Returns 3x3 array of empty sectors next to player that are initialized but not loaded.
        /// </summary>
        /// <returns>3x3 array of initialized but unloaded sectors.</returns>
        public Sector[,] getAdjacentSectors()
        {
            Sector[,] adjacent_sectors = new Sector[3, 3];
            int[] player_sector = getPlayerSector();
            adjacent_sectors[0, 0] = new Sector(player_sector[0] - 1, player_sector[1] - 1);
            adjacent_sectors[0, 1] = new Sector(player_sector[0], player_sector[1] - 1);
            adjacent_sectors[0, 2] = new Sector(player_sector[0] + 1, player_sector[1] - 1);
            adjacent_sectors[1, 0] = new Sector(player_sector[0] - 1, player_sector[1]);
            adjacent_sectors[1, 1] = new Sector(player_sector[0], player_sector[1]);
            adjacent_sectors[1, 2] = new Sector(player_sector[0] + 1, player_sector[1]);
            adjacent_sectors[2, 0] = new Sector(player_sector[0] - 1, player_sector[1] + 1);
            adjacent_sectors[2, 1] = new Sector(player_sector[0], player_sector[1] + 1);
            adjacent_sectors[2, 2] = new Sector(player_sector[0] + 1, player_sector[1] + 1);
            return adjacent_sectors;
        }

        /// <summary>
        /// Checks if a number is positive.
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns>Returns 0 if number = 0, -1 if number < 0, and 1 if number > 0</returns>
        public int isPositive(int number)
        {
            if (number > 0)
            {
                return 1;
            }
            else if (number < 0)
            {
                return -1;
            }
            else{
                return 0;
            }
        }
    }

    /// <summary>
    /// A "star" SpaceObject. The most common, fills most of space.
    /// </summary>
    public class Star: SpaceObject
    {
        public int texture_number;
        public static int MAX_SIZE = 20;
        public static int PER_SECTOR = 5000;

        public Star(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.texture_number = Program.game.rand_gen.Next(1, 3);
            this.addToSector(sector_inside);
        }

        /// <summary>
        /// Returns star collideshape, which is always false.
        /// </summary>
        /// <returns>false</returns>
        public override Object getCollideShape()
        {
            return false;
        }

        /// <summary>
        /// Draws a star texture in space.
        /// </summary>
        public override void draw()
        {
            Program.game.drawSprite(this.getTexture(texture_number), pos[0], pos[1], layer: 0f);
        }

        /// <summary>
        /// Returns one of the star textures. Based on the value that is saved with the star.
        /// </summary>
        /// <param name="number">Texture number of star.</param>
        /// <returns>A Texture2D of a star.</returns>
        public Texture2D getTexture(int number)
        {
            Texture2D return_texture = Program.game.space.star_texture1;
            if (number == 1)
            {
                return_texture = Program.game.space.star_texture1;
            }
            else if (number == 2)
            {
                return_texture = Program.game.space.star_texture2;
            }
            return return_texture;
        }
    }

    /// <summary>
    /// Mineral giving SpaceObject.
    /// </summary>
    public class Asteroid: SpaceObject
    {
        public int diameter;
        public float minerals;
        public static int MAX_SIZE = 100;
        public static int MIN_SIZE = 25;
        public static int PER_SECTOR = 15;
        public static int PADDING_DISTANCE = 15;
        public int[] center_point = {0, 0};

        public Asteroid(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.diameter = Program.game.rand_gen.Next(MIN_SIZE, MAX_SIZE);
            this.minerals = this.diameter * 5;
            this.center_point[0] = this.pos[0] + this.diameter / 2;
            this.center_point[1] = this.pos[1] + this.diameter / 2;
            this.addToSector(sector_inside);
        }

        /// <summary>
        /// Returns circular collideshape of asteroid
        /// </summary>
        /// <returns>Circle collideShape</returns>
        public override Object getCollideShape()
        {
            return new Circle(pos[0], pos[1], this.diameter / 2);
        }

        /// <summary>
        /// Scales and draws asteroid based on diameter and level of unmined minerals.
        /// </summary>
        public override void draw()
        {
            float scale = diameter / 50f;
            Vector2 pos_vector = new Vector2(pos[0], pos[1]);
            int green_level = (int)((float)this.minerals / (float)(this.diameter * 5) * 80);
            Color asteroid_color = new Color(0, green_level + 47, 14);
            Vector2 origin = new Vector2(0, 0);
            Program.game.drawSprite(Program.game.space.asteroid_texture, pos[0], pos[1], scale: scale, color: asteroid_color, layer: 0.1f);
        }

        /// <summary>
        /// Attempts to remove minerals from asteroid.
        /// </summary>
        /// <param name="count">Number of minerals to remove.</param>
        /// <returns>Boolean whether or not the minerals could be removed.</returns>
        public int harvestMinerals(int count = 1)
        {
            if (this.minerals > 0)
            {
                this.minerals -= count;
                Program.game.ship.minerals += count;
            }
            else
            {
                return 0;
            }
            return count;
        }

        /// <summary>
        /// Increases the mineral count by a small amount.
        /// </summary>
        public void replenishMinerals()
        {
            if (this.minerals < this.diameter * 5)
            {
                this.minerals += 0.02F;
            }
        }

        /// <summary>
        /// Handles collision detection, mineral regeneration, and mining.
        /// </summary>
        public override void update()
        {
            if (Util.distance(Program.game.ship.pos[0], Program.game.ship.pos[1], this.center_point[0], this.center_point[1]) <= this.diameter / 2 + Asteroid.PADDING_DISTANCE)
            {
                // Stop the player from moving closer to asteroid
                Program.game.ship.reverse();
            }
            int[] asteroid_draw_pos = Program.game.drawOffset(this.center_point[0], this.center_point[1]);
            if (Program.game.ship.tool.getTool() == 1 && Program.game.inView(this.center_point[0], this.center_point[1]) && Program.game.ship.collideLaser(asteroid_draw_pos[0], asteroid_draw_pos[1], this.diameter / 2))
            {
                this.harvestMinerals();
            }
            this.replenishMinerals();
        }
    }
}
