using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using C3.XNA;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace Expand
{
    public class Space: GameObject
    {
        public Color space_color;
        public Texture2D star_texture1 = Program.game.loadTexture("space//star1.png");
        public Texture2D star_texture2 = Program.game.loadTexture("space//star2.png");
        public Texture2D asteroid_texture = Program.game.loadTexture("space//asteroid.png");
        public const int SECTOR_SIZE = 5000;
        public const bool CLEAR_SECTOR = true; // Resets the sector files at runtime if true
        private Sector[,] loaded_sectors;
        private int[] player_sector = {0, 0};
        public Space()
        {
            if (CLEAR_SECTOR)
            {
                Space.clearSpace();
            }
            space_color = new Color(0, 0, 10);
            loaded_sectors = getAdjacentSectors();
            this.player_sector[0] = -1;
            this.player_sector[1] = -1;
        }

        public override void update()
        {
            if (!player_sector.SequenceEqual(getPlayerSector()))
            {
                player_sector = (int[]) getPlayerSector().Clone();
                Console.WriteLine("Now in sector " + player_sector[0] + " " + player_sector[1] + " LOADING ADJACENTS!");
                ThreadStart thread_target = new ThreadStart(loadAdjacentSectors);
                Thread sector_loader = new Thread(thread_target);
                sector_loader.IsBackground = true;
                sector_loader.Name = "Sector Loader";
                Program.game.object_handler.middle_list_locked = true;
                sector_loader.Start();
            }
        }

        private static void clearSpace()
        {
            // Deletes all saved sector files
            DirectoryInfo space_dir = new DirectoryInfo("space");
            foreach (FileInfo sector_file in space_dir.GetFiles())
            {
                sector_file.Delete();
            }
        }

        public Sector findSector(int x, int y)
        {
            // Checks if sector is loaded, generates new sector, or loads from file
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

            if (Sector.exists(x, y))
            {
                Sector saved_sector = Sector.load(x, y);
                return saved_sector;
            }
            else
            {
                return_sector = new Sector(x, y);
                return_sector.generate();
            }
            return return_sector;
        }

        public bool sectorStillAdjacent(Sector old_sector, Sector[,] adjacent_sectors){
            bool still_adjacent = false;
            for (int sector_row = 0; sector_row < adjacent_sectors.GetLength(0); sector_row++)
            {
                for (int sector_x = 0; sector_x < adjacent_sectors.GetLength(1); sector_x++)
                {
                    if (old_sector.coords.SequenceEqual(adjacent_sectors[sector_row, sector_x].coords))
                    {
                        still_adjacent = true;
                    }
                }
            }
            return still_adjacent;
        }

        public void loadAdjacentSectors()
        {
            Sector[,] adjacent_sectors = getAdjacentSectors();

            // Find sectors that need to be unloaded
            for (int sector_row = 0; sector_row < adjacent_sectors.GetLength(0); sector_row++)
            {
                for (int sector_x = 0; sector_x < adjacent_sectors.GetLength(1); sector_x++)
                {
                    if (!this.sectorStillAdjacent(loaded_sectors[sector_row, sector_x], adjacent_sectors))
                    {
                        // Sector needs to be unloaded
                        Console.WriteLine("UNLOADING SECTOR " + loaded_sectors[sector_row, sector_x].formatName());
                        loaded_sectors[sector_row, sector_x].unload();
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
                    adjacent_sectors[sector_row, sector_x] = findSector(sector_coords[0], sector_coords[1]);
                }
            }
            loaded_sectors = adjacent_sectors;
            Program.game.object_handler.middle_list_locked = false;
        }

        public int[] getPlayerSector()
        {
            // Tracks what sector the player is in, rather clumsily
            int[] player_sector = {0, 0};
            if (Program.game.ship.pos[0] < 5000 && Program.game.ship.pos[0] > 0 && Program.game.ship.pos[1] < 5000 && Program.game.ship.pos[1] > 0)
            {
                player_sector[0] = 0;
                player_sector[1] = 0;
            }
            if (Program.game.ship.pos[0] > 5000)
            {
                player_sector[0] = Program.game.ship.pos[0] / 5000;
            }
            if (Program.game.ship.pos[0] < 0)
            {
                player_sector[0] = Program.game.ship.pos[0] / 5000 - 1;
            }
            if (Program.game.ship.pos[1] > 5000)
            {
                player_sector[1] = Program.game.ship.pos[1] / 5000;
            }
            if (Program.game.ship.pos[1] < 0)
            {
                player_sector[1] = Program.game.ship.pos[1] / 5000 - 1;
            }
            return player_sector;
        }

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

    public class Star: SpaceObject
    {
        public int[] pos = {-1, -1};
        public int texture_number;
        public static int MAX_SIZE = 20;
        public static int PER_SECTOR = 5000;

        public Star(int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.texture_number = Program.game.rand_gen.Next(1, 3);
        }

        public override void draw()
        {
            Program.game.drawSprite(this.getTexture(texture_number), pos[0], pos[1], layer: 0f);
        }

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

    public class Asteroid: SpaceObject
    {
        public int diameter;
        public int minerals;
        public static int MAX_SIZE = 100;
        public static int MIN_SIZE = 25;
        public static int PER_SECTOR = 15;
        public static int PADDING_DISTANCE = 15;
        public int[] pos = {0, 0};
        public int[] center_point = {0, 0};

        public Asteroid(int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.diameter = Program.game.rand_gen.Next(MIN_SIZE, MAX_SIZE);
            this.minerals = this.diameter * 5;
            this.center_point[0] = this.pos[0] + this.diameter / 2;
            this.center_point[1] = this.pos[1] + this.diameter / 2;
        }

        public override void draw()
        {
            float scale = diameter / 50f;
            Program.game.drawSprite(Program.game.space.asteroid_texture, pos[0], pos[1], scale: scale, layer: 0.1f);
        }

        public int harvestMinerals(int count = 1)
        {
            if (this.minerals > 0)
            {
                this.minerals -= count;
            }
            else
            {
                return 0;
            }
            return count;
        }

        public override void update()
        {
            if (Util.distance(Program.game.ship.pos[0], Program.game.ship.pos[1], this.center_point[0], this.center_point[1]) <= this.diameter / 2 + Asteroid.PADDING_DISTANCE)
            {
                // Stop the player from moving closer to asteroid
                Program.game.ship.reverse();
            }
            int[] asteroid_draw_pos = Program.game.drawOffset(this.center_point[0], this.center_point[1]);
            if (Program.game.inView(this.center_point[0], this.center_point[1]) && Program.game.ship.collideLaser(asteroid_draw_pos[0], asteroid_draw_pos[1], this.diameter / 2))
            {
                this.harvestMinerals();
            }
        }
    }
}
