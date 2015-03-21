using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class Space: GameObject
    {
        public Color space_color;
        public Texture2D star_texture1 = Program.game.loadTexture("space//star1.png");
        public Texture2D star_texture2 = Program.game.loadTexture("space//star2.png");
        private List<Sector> sectors = new List<Sector>();
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
                loadAdjacentSectors();
            }
        }

        public void loadAdjacentSectors()
        {
            Sector[] adjacent_sectors = getAdjacentSectors();
            // Kill all loaded sectors
            foreach (Sector loaded_sector in sectors)
            {
                loaded_sector.unload();
            }
            sectors.Clear();
            foreach (Sector adjacent in adjacent_sectors)
            {
                Sector new_sector = new Sector(adjacent.sector_coords[0], adjacent.sector_coords[1]);
                if (adjacent.exists())
                {
                    new_sector = Sector.load(adjacent.sector_coords[0], adjacent.sector_coords[1]);
                }
                else
                {
                    new_sector.generate();
                    new_sector.save();
                }
                sectors.Add(new_sector);
                Console.WriteLine(adjacent.sector_coords[0] + " " + adjacent.sector_coords[1]);
            }
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
