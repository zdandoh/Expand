using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Builder: SpaceObject
    {
        public Rectangle bounds;
        private bool gui_shown = false;
        public Builder(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            bounds.X = x;
            bounds.Y = y;
            this.addToSector(sector_inside);
        }

        public override void update()
        {
            if(bounds.Contains(new Point(pos[0], pos[1])))
            {
                gui_shown = true;
            }
            else
            {
                gui_shown = false;
            }
        }

        public void drawSelectorAround(Rectangle pos_rect)
        {
            int[] selector_dims = {Program.game.textures["structure\\base.png"].Width, Program.game.textures["structure\\base.png"].Height};
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0] + selector_dims[0], pos[1], layer: 0.2f);
        }

        public override void draw()
        {
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0], pos[1], layer: 0.1f);
            if (gui_shown)
            {
                drawSelectorAround(this.bounds);
            }
        }
    }
}
