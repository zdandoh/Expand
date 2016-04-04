using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Expand.core.gui
{
    public class TechTree: GameObject
    {
        private bool shown;
        private Texture2D tech_outline;
        public TechTree()
        {
            tech_outline = Program.game.textures["gui\\hotbar.png"];
            shown = false;
        }

        public override void update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.B)) {
                shown = Util.toggle(shown);
            }
        }

        public override void draw()
        {
            if (shown)
            {
                Vector2 origin = new Vector2(0, 0);
                Vector2 outline_pos = new Vector2(100, 100);
                Program.game.spriteBatch.Draw(tech_outline, outline_pos, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0.99f);
            }
        }
    }
}
