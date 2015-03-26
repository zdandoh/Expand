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
        public Texture2D hotbar = Program.game.loadTexture("gui//hotbar.png");
        public Texture2D hotbar_selector = Program.game.loadTexture("gui//hotbar_selector.png");

        public GUI()
        {

        }

        public override void update()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.ScrollWheelValue != last_scroll)
            {
                // Check and update scroll position
                if (last_scroll < mouse.ScrollWheelValue)
                {
                    Program.game.ship.tool_selected += 1;
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }
                else if (last_scroll > mouse.ScrollWheelValue)
                {
                    Program.game.ship.tool_selected -= 1;
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }

                // Prevent tool selector from going over
                if (Program.game.ship.tool_selected > 6)
                {
                    Program.game.ship.tool_selected = 1;
                }
                else if (Program.game.ship.tool_selected < 1)
                {
                    Program.game.ship.tool_selected = 6;
                }

                // Do a stupid thing to make the selector align correctly
                if (Program.game.ship.tool_selected > 1)
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
                    Program.game.ship.tool_selected = num_counter + 1;
                    last_scroll_time = Program.game.game_time.ElapsedMilliseconds;
                }
            }

        }

        public override void draw()
        {
            if (Program.game.game_time.ElapsedMilliseconds - last_scroll_time < 2000)
            {
                Vector2 pos_vector = new Vector2(Program.game.screen_size[0] / 2 - hotbar.Width / 2, Program.game.screen_size[1] - hotbar.Height);
                Vector2 selector_pos = new Vector2(pos_vector.X + 54 * Program.game.ship.tool_selected - 51 + special_additive, pos_vector.Y + 2);
                Program.game.spriteBatch.Draw(hotbar, pos_vector);
                Program.game.spriteBatch.Draw(hotbar_selector, selector_pos);
            }
        }
    }
}
