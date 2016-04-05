using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Expand.core
{
    class Dialog: GameObject
    {
        Texture2D box_texture = Program.game.textures["gui\\prompt\\box.png"];
        Texture2D button_texture = Program.game.textures["gui\\prompt\\button.png"];
        String prompt;
        public Dialog(String prompt)
        {
            this.prompt = Util.wrapText(prompt, 35);
        }

        public override void draw()
        {
            Vector2 box_origin = new Vector2(box_texture.Width / 2, box_texture.Height / 2);
            Vector2 button_origin = new Vector2(0, 0);
            Vector2 draw_pos = new Vector2(Program.game.ship.draw_location[0], Program.game.ship.draw_location[1]);
            Vector2 button_pos = new Vector2(draw_pos.X - 20, draw_pos.Y + 123);
            Vector2 text_pos = new Vector2(draw_pos.X + 6, draw_pos.Y + 6);
            Program.game.spriteBatch.Draw(box_texture, draw_pos, null, Color.White, 0F, box_origin, 1F, SpriteEffects.None, 0.95F);
            Program.game.spriteBatch.Draw(button_texture, button_pos, null, Color.White, 0F, button_origin, 1F, SpriteEffects.None, 0.96F);
            Program.game.drawText("Okay", new int[] {(int)button_pos.X + 12, (int)button_pos.Y}, Color.White, layer: 0.97F);
            Program.game.drawText(this.prompt, new int[] { (int)draw_pos.X - box_texture.Width / 2 + 50, (int)draw_pos.Y - box_texture.Height / 2 + 50}, Color.White, layer: 0.96F);
        }

        public override void update()
        {
            if (Program.game.mouse.LeftButton == ButtonState.Pressed)
            {
                Rectangle button_box = button_texture.Bounds;
                Point mouse_point = new Point(Program.game.mouse.Position.X, Program.game.mouse.Position.Y);
                button_box.X = Program.game.ship.draw_location[0] - 20;
                button_box.Y = Program.game.ship.draw_location[1] + 123;
                if (button_box.Contains(mouse_point))
                {
                    this.setDead();
                }
            }
        }
    }
}
