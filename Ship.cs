using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public class Ship: GameObject
    {
        private Texture2D ship_texture;
        public int[] pos = {Program.game.screen_size[0] / 2, Program.game.screen_size[1] / 2};
        public int[] draw_location;
        private float radians = 0;
        private bool preserve_rotation = false;
        private int y_velocity = 0;
        private int x_velocity = 0;
        private double y_velocity_change = 0;
        private double x_velocity_change = 0;
        private double space_velocity_change = 0;
        private int VELOCITY_COOLDOWN = 100;

        public Ship()
        {
            ship_texture = Program.game.loadTexture("ships//ship.png");
            this.pos[0] -= ship_texture.Width / 2;
            this.pos[1] -= ship_texture.Height / 2;
            this.draw_location = (int[]) this.pos.Clone();
        }

        public int moveCloserToZero(int number)
        {
            if (number < 0)
            {
                return number + 1;
            }
            else if (number > 0)
            {
                return number - 1;
            }
            else
            {
                return 0;
            }
        }

        public override void update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Program.game.game_time.ElapsedMilliseconds - y_velocity_change > VELOCITY_COOLDOWN)
            {
                y_velocity -= 1;
                y_velocity_change = Program.game.game_time.ElapsedMilliseconds;
                this.preserve_rotation = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) && Program.game.game_time.ElapsedMilliseconds - y_velocity_change > VELOCITY_COOLDOWN)
            {
                y_velocity += 1;
                y_velocity_change = Program.game.game_time.ElapsedMilliseconds;
                this.preserve_rotation = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A) && Program.game.game_time.ElapsedMilliseconds - x_velocity_change > VELOCITY_COOLDOWN)
            {
                x_velocity -= 1;
                x_velocity_change = Program.game.game_time.ElapsedMilliseconds;
                this.preserve_rotation = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D) && Program.game.game_time.ElapsedMilliseconds - x_velocity_change > VELOCITY_COOLDOWN)
            {
                x_velocity += 1;
                x_velocity_change = Program.game.game_time.ElapsedMilliseconds;
                this.preserve_rotation = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && Program.game.game_time.ElapsedMilliseconds - space_velocity_change > VELOCITY_COOLDOWN)
            {
                y_velocity = moveCloserToZero(y_velocity);
                x_velocity = moveCloserToZero(x_velocity);
                space_velocity_change = Program.game.game_time.ElapsedMilliseconds;
                this.preserve_rotation = true;
            }

            if (y_velocity > 5)
            {
                y_velocity = 5;
            }
            else if (y_velocity < -5)
            {
                y_velocity = -5;
            }
            if (x_velocity > 5)
            {
                x_velocity = 5;
            }
            else if (x_velocity < -5)
            {
                x_velocity = -5;
            }

            this.pos[1] += y_velocity;
            this.pos[0] += x_velocity;

            if (!preserve_rotation)
            {
                radians = (float)(Math.Atan2(y_velocity, x_velocity));
            }
        }

        public override void draw()
        {
            Vector2 pos_vector = new Vector2(draw_location[0], draw_location[1]);
            Vector2 origin = new Vector2(10, 10);
            Program.game.spriteBatch.Draw(ship_texture, pos_vector, null, Color.White, radians, origin, 1, SpriteEffects.None, 1);
        }
    }
}
