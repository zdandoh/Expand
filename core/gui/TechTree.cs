﻿using Microsoft.Xna.Framework;
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
            tech_outline = Program.game.sprites["gui\\icon\\tree_icon.png"].frame;
        }

        public override void update()
        {

        }

        public override void draw()
        {
            // Draw tech tree triangles
            Vector2 origin = new Vector2(tech_outline.Height * Expand.gui_scale, tech_outline.Width * Expand.gui_scale);
            float rotation = 0;
            for(int screen_percent = 45; screen_percent < 60; screen_percent += 5)
            {
                Program.game.drawGUI(tech_outline, screen_percent, 50, rotation, 0.98f);
                rotation = toggleRotation(rotation);
            }

            // Draw icons on top of the triangles
            Program.game.drawGUI(Program.game.sprites["gui\\icon\\boom.png"].frame, 55, 51);
            Program.game.drawGUI(Program.game.sprites["gui\\icon\\science.png"].frame, 50, 48);
            Program.game.drawGUI(Program.game.sprites["gui\\icon\\mine.png"].frame, 45, 52);
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
