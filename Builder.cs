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
    /// <summary>
    /// The basic builder core that is always intially placed when creating a new structure.
    /// </summary>
    public class Builder: SpaceObject
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

        /// <summary>
        /// Kills both adjacent selectors and self
        /// </summary>
        public override void setDead()
        {
            base.setDead();
            foreach (Selector select in selectors)
            {
                select.setDead();
            }
        }

        /// <summary>
        /// Basic builder update function. Checks whether a selector is being clicked, spawns adjacent selectors, and performs collision detection.
        /// </summary>
        public override void update()
        {
            if (Program.game.mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (Selector select in this.selectors)
                {
                    if (select.collideCursor())
                    {
                        if (select.build(this))
                        {
                            this.setDead();
                        }
                    }
                }
                Vector2 mouse_space_pos = Program.game.spacePos(Program.game.mouse.X - bounds.r, Program.game.mouse.Y - bounds.r);
                if (bounds.Contains(new Point((int)mouse_space_pos.X, (int)mouse_space_pos.Y)) && this.alive)
                {
                    createSelectorsAround(this.pos);
                }
                else if (Program.game.mouse.LeftButton == ButtonState.Pressed)
                {
                    gui_shown = false;
                    foreach (Selector select in selectors)
                    {
                        select.setDead();
                    }
                    this.selectors.Clear();
                }
            }
            if (this.bounds.getDistance(Program.game.ship.pos[0], Program.game.ship.pos[1]) < this.bounds.r + 5)
            {
                Program.game.ship.reverse();
            }
        }

        /// <summary>
        /// Used for collision detection in sectors.
        /// </summary>
        /// <returns>Instance of Circle</returns>
        public override Object getCollideShape()
        {
            return this.bounds;
        }

        /// <summary>
        /// Initializes new Selectors that surround position with constant PADDING. 
        /// </summary>
        /// <param name="pos">Position to surround</param>
        public void createSelectorsAround(int[] pos)
        {
            if (!this.gui_shown) {
                Texture2D selector = Program.game.textures["gui\\icon\\selector_circle.png"];
                int[] selector_dims = { selector.Width, selector.Height };
                int PADDING = 5;
                selectors.Add(new Selector(pos[0] + selector_dims[0] / 2 + PADDING, pos[1] - bounds.r, TechTree.MINING));
                selectors.Add(new Selector(pos[0] - selector_dims[0] / 2 - bounds.r * 2 - PADDING, pos[1] - bounds.r, TechTree.COMBAT));
                selectors.Add(new Selector(pos[0] - bounds.r, pos[1] - selector_dims[1] - PADDING, TechTree.NONE));
                selectors.Add(new Selector(pos[0] - bounds.r, pos[1] + selector_dims[1] / 2 + PADDING, TechTree.NONE));
                this.gui_shown = true;
            }
        }

        /// <summary>
        /// Draws texture of build base.
        /// </summary>
        public override void draw()
        {
            Program.game.drawSprite(Program.game.textures["structure\\base.png"], pos[0], pos[1], layer: 0.1f, color: Color.Gold);
        }
    }

    /// <summary>
    /// A GameObject that is meant to be clicked on to interface with other items and perform actions.
    /// </summary>
    public class Selector: GameObject
    {
        public int tree;
        public Selector(int x, int y, int tree)
        {
            this.pos[0] = x;
            this.pos[1] = y;
            this.tree = tree;
        }

        /// <summary>
        /// Checks if the cursor collides with the selector circle.
        /// </summary>
        /// <returns>Bool true if cursor collides, false otherwise.</returns>
        public bool collideCursor()
        {
            bool collides = false;
            Circle bounding_box = new Circle(pos[0], pos[1], Program.game.textures["gui\\icon\\selector_circle.png"].Width / 2);

            if(bounding_box.Contains(Program.game.space_mouse))
            {
                collides = true;
            }
            return collides;
        }

        /// <summary>
        /// General purpose function that is called when the selector is clicked on.
        /// </summary>
        /// <param name="child">The Builder object that this selector is connected to.s</param>
        /// <returns>Boolean whether or not the build action was "possible"</returns>
        public bool build(Builder child)
        {
            bool possible = false;
            this.setDead();
            return possible;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void update()
        {
            if (this.collideCursor())
            {
                if (Program.game.mouse.LeftButton == ButtonState.Pressed) {
                    this.setDead();
                }
            }
        }

        /// <summary>
        /// Draws the texture of the Selector object.
        /// </summary>
        public override void draw()
        {
            int[] yolo = Util.screenPosToSpacePos(Program.game.mouse.X, Program.game.mouse.Y);
            Program.game.drawDebugSquare(pos[0], pos[1]);
            Program.game.drawDebugSquare(yolo[0], yolo[1]);
            Program.game.drawSprite(Program.game.textures["gui\\icon\\selector_circle.png"], pos[0], pos[1], layer: 0.2f);
        }
    }
}
