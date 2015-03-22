using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Expand
{
    public class Space: GameObject
    {
        public Color space_color;
        public Texture2D star_texture1 = Program.game.loadTexture("space//star1.png");
        public Texture2D star_texture2 = Program.game.loadTexture("space//star2.png");
        private List<Sector> loaded_sectors = new List<Sector>();
        private int[] player_sector = {-1, -1};
        public Space()
        {
            space_color = new Color(0, 0, 10);
        }

        public override void update()
        {
            if (!player_sector.SequenceEqual(getPlayerSector()))
            {
                player_sector = (int[]) getPlayerSector().Clone();
                Console.WriteLine("Now in sector " + player_sector[0] + " " + player_sector[1] + " LOADING ADJACENTS!");
                loadAdjacentSectors();
            }
        }

        public Sector findSector(int x, int y)
        {
            // Checks if sector is loaded, generates new sector, or loads from file
            Sector return_sector;

            // Check if sector already is loaded
            foreach (Sector loaded_sector in loaded_sectors)
            {
                if (loaded_sector.coords[0] == x && loaded_sector.coords[1] == y)
                {
                    return loaded_sector;
                }
            }

            if (Sector.exists(x, y))
            {
                return Sector.load(x, y);
            }
            else
            {
                return_sector = new Sector(x, y);
                return_sector.generate();
            }
            return return_sector;
        }

        public void loadAdjacentSectors()
        {
            Sector[] adjacent_sectors = getAdjacentSectors();

            // Find sectors that need to be unloaded
            for(int sector_counter = 0; sector_counter < loaded_sectors.Count(); sector_counter++)
            {
                bool keep_sector = false;
                if (loaded_sectors[sector_counter] == null)
                {
                    continue;
                }
                foreach(Sector new_sector in adjacent_sectors)
                {
                    if (loaded_sectors[sector_counter].coords.SequenceEqual(new_sector.coords))
                    {
                        Console.WriteLine("setting " + loaded_sectors[sector_counter].formatName() + " to keep");
                        keep_sector = true;
                    }
                }
                if (!keep_sector)
                {
                    // Kill all unneeded sectors
                    Console.WriteLine("UNLOADING SECTOR " + loaded_sectors[sector_counter].formatName());
                    loaded_sectors[sector_counter].unload();
                    loaded_sectors[sector_counter] = null;
                }
            }

            Sector[] temp_sectors = (Sector[]) adjacent_sectors.Clone();
            for(int sector_counter = 0; sector_counter < adjacent_sectors.Length; sector_counter++)
            {
                Sector new_loaded_sector = this.findSector(adjacent_sectors[sector_counter].coords[0], adjacent_sectors[sector_counter].coords[1]);
                temp_sectors[sector_counter] = new_loaded_sector;
            }

            loaded_sectors.Clear();
            foreach (Sector new_sector in temp_sectors)
            {
                this.loaded_sectors.Add(new_sector);
            }
            Debug.Assert(loaded_sectors.Count() < 10);
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

        public Sector[] getAdjacentSectors()
        {
            Sector[] adjacent_sectors = new Sector[9];
            int[] player_sector = getPlayerSector();
            adjacent_sectors[0] = new Sector(player_sector[0] + 1, player_sector[1]);
            adjacent_sectors[1] = new Sector(player_sector[0] - 1, player_sector[1]);
            adjacent_sectors[2] = new Sector(player_sector[0], player_sector[1] + 1);
            adjacent_sectors[3] = new Sector(player_sector[0], player_sector[1] - 1);
            adjacent_sectors[4] = new Sector(player_sector[0] + 1, player_sector[1] + 1);
            adjacent_sectors[5] = new Sector(player_sector[0] + 1, player_sector[1] - 1);
            adjacent_sectors[6] = new Sector(player_sector[0] - 1, player_sector[1] + 1);
            adjacent_sectors[7] = new Sector(player_sector[0] - 1, player_sector[1] - 1);
            adjacent_sectors[8] = new Sector(player_sector[0], player_sector[1]);
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

        public Star(int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.texture_number = Program.game.rand_gen.Next(1, 3);
        }

        public override void draw()
        {
            Program.game.drawSprite(this.getTexture(texture_number), pos[0], pos[1]);
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
}
