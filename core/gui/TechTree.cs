using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Expand.core.gui
{
    public class TechTree: GameObject
    {
        private Texture2D tech_outline;
        public TechTree()
        {
            tech_outline = Program.game.textures["gui\\icon\\tree_icon.png"];
        }

        public override void update()
        {

        }

        public override void draw()
        {
            // Draw tech tree triangles
            Vector2 origin = new Vector2(tech_outline.Height * Expand.gui_scale, tech_outline.Width * Expand.gui_scale);
            float rotation = 0;
            for(int screen_percent = 40; screen_percent < 65; screen_percent += 5)
            {
                Program.game.drawGUI(tech_outline, screen_percent, 50, rotation);
                rotation = toggleRotation(rotation);
            }
        }

        public float toggleRotation(float rotation)
        {
            if (rotation == 0)
            {
                return (float)Math.PI;
            }
            return 0;
        }
    }
}
