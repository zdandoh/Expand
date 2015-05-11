using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class GUI: GameObject
    {
        public int last_scroll = 0;
        public long last_scroll_time = 0;
        public int special_additive = 0;
        public Texture2D hotbar;
        public Texture2D hotbar_selector;
        public Texture2D tool1_icon;

        public GUI()
        {
            hotbar = Program.game.textures["gui\\hotbar.png"];
            hotbar_selector = Program.game.textures["gui\\hotbar_selector.png"];
            tool1_icon = Program.game.textures["gui\\icon\\mining_laser.png"];
        }

        public override void update()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.ScrollWheelValue != last_scroll)
            {
                // Check and update scroll position
                if (last_scroll < mouse.ScrollWheelValue)
                {
                    Program.game.ship.tool.setTool(Program.game.ship.tool.getTool() + 1);
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }
                else if (last_scroll > mouse.ScrollWheelValue)
                {
                    Program.game.ship.tool.setTool(Program.game.ship.tool.getTool() - 1);
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }

                // Do a stupid thing to make the selector align correctly
                if (Program.game.ship.tool.getTool() > 1)
                {
                    special_additive = 1;
                }
                else
                {
                    special_additive = 0;
                }
                last_scroll = mouse.ScrollWheelValue;
            }

            // Check num keys to move selector
            Keys[] num_codes = { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6 };
            for (int num_counter = 0; num_counter < num_codes.Length; num_counter++)
            {
                if (Keyboard.GetState().IsKeyDown(num_codes[num_counter]))
                {
                    Program.game.ship.tool.setTool(num_counter + 1);
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }
            }

        }

        public override void draw()
        {
            if (Program.game.game_time.ElapsedMilliseconds - last_scroll_time < 2000)
            {
                Vector2 pos_vector = new Vector2(Program.game.screen_size[0] / 2 - hotbar.Width / 2, Program.game.screen_size[1] - hotbar.Height);
                Vector2 selector_pos = new Vector2(pos_vector.X + 54 * Program.game.ship.tool.getTool() - 51 + special_additive, pos_vector.Y + 2);
                Vector2 origin = new Vector2(0, 0);
                Program.game.spriteBatch.Draw(hotbar, pos_vector, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0.99f);
                Program.game.spriteBatch.Draw(hotbar_selector, selector_pos, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 1f);
                Program.game.spriteBatch.Draw(tool1_icon, new Vector2(pos_vector.X + 4, pos_vector.Y + 4), null, Color.White, 0f, origin, 1f, SpriteEffects.None, 1f);
                //Program.game.spriteBatch.Draw(Program.game.textures["gui\\icon\\builder.png"], new Vector2(pos_vector.X + 60, pos_vector.Y + 6), null, Color.White, 0f, origin, 1f, SpriteEffects.None, 1f);
            }

            int[] text_pos = { 10, 5 };
            int[] sector_text_pos = { 10, 20 };
            Program.game.drawText(Program.game.ship.minerals.ToString(), text_pos, Color.White);
            Program.game.drawText(Program.game.space.getPlayerSector()[0] + ", " + Program.game.space.getPlayerSector()[1], sector_text_pos, Color.White);
        }
    }
}
