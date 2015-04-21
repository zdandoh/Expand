using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Builder: SpaceObject
    {
        public Circle bounds;
        private bool gui_shown = false;
        public Builder(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            bounds = new Circle(x, y, Program.game.textures["structure\\base.png"].Height / 2);
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

        public Circle getCollideShape()
        {
            return this.bounds;
        }

        public override bool collidesWith(Circle circ)
        {
            return Collider.intersects(this.getCollideShape(), circ);
        }

        public override bool collidesWith(Rectangle rect)
        {
            return Collider.intersects(this.getCollideShape(), rect);
        }

        public void drawSelectorAround(int[] pos)
        {
            int[] selector_dims = {Program.game.textures["structure\\base.png"].Width, Program.game.textures["structure\\base.png"].Height};
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0] + selector_dims[0], pos[1], layer: 0.2f);
        }

        public override void draw()
        {
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0], pos[1], layer: 0.1f);
            if (gui_shown)
            {
                drawSelectorAround(this.pos);
            }
        }
    }
}
