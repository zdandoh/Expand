using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Expand
{
    class Builder: SpaceObject
    {
        public Circle bounds;
        public List<Selector> selectors = new List<Selector>(); 
        private bool gui_shown = false;
        public Builder(Sector sector_inside, int x, int y)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            int r = Program.game.textures["structure\\base.png"].Height/2;
            bounds = new Circle(x, y, r);
        }

        public override void setDead()
        {
            base.setDead();
            foreach (Selector select in selectors)
            {
                select.setDead();
            }
        }

        public override void update()
        {
            Vector2 mouse_space_pos = Program.game.spacePos(Program.game.mouse.X - bounds.r, Program.game.mouse.Y - bounds.r);
            if(bounds.Contains(new Point((int)mouse_space_pos.X, (int)mouse_space_pos.Y)) && Program.game.mouse.LeftButton == ButtonState.Pressed && this.alive)
            {
                gui_shown = true;
                createSelectorsAround(this.pos);
            }
            else if(Program.game.mouse.LeftButton == ButtonState.Pressed)
            {
                gui_shown = false;
                foreach(Selector select in selectors)
                {
                    select.setDead();
                }
                this.selectors.Clear();
            }
            if (this.bounds.getDistance(Program.game.ship.pos[0], Program.game.ship.pos[1]) < this.bounds.r + 5)
            {
                Program.game.ship.reverse();
            }
        }

        public override Object getCollideShape()
        {
            return this.bounds;
        }

        public void createSelectorsAround(int[] pos)
        {
            Texture2D selector = Program.game.textures["gui\\icon\\selector_circle.png"];
            int[] selector_dims = { selector.Width, selector.Height };
            int PADDING = 5;
            selectors.Add(new Selector(pos[0] + selector_dims[0] / 2 + PADDING, pos[1] - bounds.r, TechTree.MINING));
            selectors.Add(new Selector(pos[0] - selector_dims[0] / 2 - bounds.r * 2 - PADDING, pos[1] - bounds.r, TechTree.COMBAT));
            selectors.Add(new Selector(pos[0] - bounds.r, pos[1] - selector_dims[1] - PADDING, TechTree.NONE));
            selectors.Add(new Selector(pos[0] - bounds.r, pos[1] + selector_dims[1] / 2 + PADDING, TechTree.NONE));
        }

        public override void draw()
        {
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0], pos[1], layer: 0.1f, color: Color.Gold);
        }
    }

    public class Selector: GameObject
    {
        public int tree;
        public Selector(int x, int y, int tree)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.tree = tree;
        }

        public override void draw()
        {
            Program.game.drawSprite(Program.game.textures["gui\\icon\\selector_circle.png"], pos[0], pos[1], layer: 0.2f);
        }
    }
}
