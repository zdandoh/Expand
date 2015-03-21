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
        private int[] player_sector = {0, 0};
        public Space()
        {
            space_color = new Color(0, 0, 10);
            Sector sector = new Sector(0, 0);
            sector.generate();
            sectors.Add(sector);
        }

        public void loadAdjacentSectors()
        {
        }

        public int[] getSector()
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
            Console.WriteLine(Program.game.ship.pos[0]);
            return player_sector;
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
