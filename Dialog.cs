using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Dialog: GameObject
    {
        Texture2D box_texture = Program.game.textures["gui\\prompt\\box.png"];
        Texture2D button_texture = Program.game.textures["gui\\prompt\\button.png"];
        String prompt;
        public Dialog(String prompt)
        {
            this.prompt = prompt;
        }

        public override void draw()
        {
            Vector2 box_origin = new Vector2(box_texture.Width / 2, box_texture.Height / 2);
            Vector2 button_origin = new Vector2(0, 0);
            Vector2 draw_pos = new Vector2(Program.game.ship.draw_location[0], Program.game.ship.draw_location[1]);
            Vector2 button_pos = new Vector2(draw_pos.X - 20, draw_pos.Y + 70);
            Vector2 text_pos = new Vector2(draw_pos.X + 6, draw_pos.Y + 6);
            Program.game.spriteBatch.Draw(box_texture, draw_pos, null, Color.White, 0F, box_origin, 1F, SpriteEffects.None, 0.95F);
            Program.game.spriteBatch.Draw(button_texture, button_pos, null, Color.White, 0F, button_origin, 1F, SpriteEffects.None, 0.96F);
            Program.game.drawText(this.prompt, new int[] { (int)draw_pos.X + 6, (int)draw_pos.Y + 6}, Color.White, layer: 0.96F);
        }

        public override void update()
        {
            
        }
    }
}
