using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class Space
    {
        public Color space_color;
        public Texture2D star_texture1 = Program.game.loadTexture("space//star1.png");
        public Texture2D star_texture2 = Program.game.loadTexture("space//star2.png");
        private List<Sector> sectors = new List<Sector>();
        public Space()
        {
            space_color = new Color(0, 0, 10);
            Sector new_sector = new Sector();
            new_sector.generate();
        }
    }

    public class Star: GameObject
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

        public override String saveString()
        {
            String save_string = "";
            return save_string;
        }
    }
}
