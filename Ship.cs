using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Expand
{
    /// <summary>
    /// Controls player spaceship and provides helper functions.
    /// </summary>
    public class Ship: GameObject
    {
        private readonly Texture2D ship_texture;
        public Tool tool;
        public int[] draw_location;
        public int minerals = 0;
        private float radians = 0;
        private bool preserve_rotation = false;
        private int y_velocity = 0;
        private int x_velocity = 0;
        private double y_velocity_change = 0;
        private double x_velocity_change;
        private double space_velocity_change = 0;
        private const int VELOCITY_COOLDOWN = 100;

        public Ship()
        {
            pos[0] = Program.game.screen_size[0] / 2;
            pos[1] = Program.game.screen_size[1] / 2;
            ship_texture = Program.game.textures["ships\\ship.png"];
            tool = new Tool();
            this.pos[0] -= ship_texture.Width / 2;
            this.pos[1] -= ship_texture.Height / 2;
            this.draw_location = (int[]) this.pos.Clone();
        }

        /// <summary>
        /// Moves a number closer to 0.
        /// </summary>
        /// <param name="number">Any int.</param>
        /// <returns>Moves number +-1 closer to 0. Does nothing if number is already at 0.</returns>
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

        /// <summary>
        /// Inverts velocity vector of player. Used for bounding the played off of things.
        /// </summary>
        public void reverse()
        {
            this.x_velocity = -this.x_velocity;
            this.y_velocity = -this.y_velocity;
        }

        /// <summary>
        /// Checks if mining laser collides with circle.
        /// </summary>
        /// <param name="a">Circle X coordinate.</param>
        /// <param name="b">Circle Y coordinate.</param>
        /// <param name="r">Radius of circle.</param>
        /// <returns>Boolean whether or not laser collides with circle.</returns>
        public bool collideLaser(int a, int b, int r)
        {
            bool does_collide = false;
            int x1 = Program.game.ship.draw_location[0];
            int y1 = Program.game.ship.draw_location[1];
            int x2 = Mouse.GetState().Position.X;
            int y2 = Mouse.GetState().Position.Y;
            double line_distance = Util.distance(x1, y1, x2, y2);
            //double distance_from_ship = Util.distance(x1, y1, a, b);

            double direction_x = (x2 - x1) / line_distance;
            double direction_y = (y2 - y1) / line_distance;

            double t = direction_x * (a - x1) + direction_y * (b - y1);

            double intersect_x = t * direction_x + x1;
            double intersect_y = t * direction_y + y1;

            double center_to_line = Util.distance(a, b, intersect_x, intersect_y);
            if (center_to_line < r && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                double distance_from_circle = Util.distance(r, center_to_line);
                int int_point1_x = (int) ((t - distance_from_circle) * direction_x + x1);
                int int_point1_y = (int)((t - distance_from_circle) * direction_y + y1);
                double d1 = Util.distance(x1, y1, int_point1_x, int_point1_y);
                double d2 = Util.distance(x2, y2, int_point1_x, int_point1_y);
                int added = (int) (d1 + d2);

                if (added == (int) line_distance)
                {
                    this.tool.tool_line_end[0] = int_point1_x;
                    this.tool.tool_line_end[1] = int_point1_y;
                    does_collide = true;
                }
            }
            return does_collide;
        }

        /// <summary>
        /// Saves all ship data to .json file.
        /// </summary>
        public void save()
        {
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            StreamWriter json_fi = new StreamWriter("Content//ships//ship_data.json");
            json_fi.Write(json);
            json_fi.Close();
        }

        /// <summary>
        /// Loads all data from ship .json file.
        /// </summary>
        /// <returns>Instance of Ship class, with data loaded from json file.</returns>
        public static Ship load()
        {
            Ship loaded_ship;
            if (File.Exists("Content//ships//ship_data.json"))
            {
                String ship_data = File.ReadAllText("Content//ships//ship_data.json");
                loaded_ship = (Ship)Newtonsoft.Json.JsonConvert.DeserializeObject<Ship>(ship_data);
            }
            else
            {
                loaded_ship = new Ship();
            }
            return loaded_ship;
        }

        /// <summary>
        /// Removes minerals from ship if there are enough.
        /// </summary>
        /// <param name="count">Number of minerals to remove from ship.</param>
        /// <returns>Boolean whether or not there were enough minerals to remove the specificed amount.</returns>
        public bool removeMinerals(int count)
        {
            if (this.minerals >= count)
            {
                this.minerals -= count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates player position from user input. Rotates player sprite accordingly.
        /// </summary>
        public void updatePosition()
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

        /// <summary>
        /// Main update function for player.
        /// </summary>
        public override void update()
        {
            this.updatePosition();
        }

        /// <summary>
        /// Draws player in the center of the screen.
        /// </summary>
        public override void draw()
        {
            // Draw ship
            Vector2 pos_vector = new Vector2(draw_location[0], draw_location[1]);
            Vector2 origin = new Vector2(10, 10);
            Program.game.spriteBatch.Draw(ship_texture, pos_vector, null, Color.White, radians, origin, 1, SpriteEffects.None, 0.9f);
        }
    }

    /// <summary>
    /// Class that specifies the currently selected tool that the player will use when clicking.
    /// </summary>
    public class Tool: GameObject
    {
        private int tool_number;
        public int[] tool_line_end = new int[2];
        public Tool()
        {
            tool_line_end[0] = -1;
            tool_line_end[1] = -1;
            this.tool_number = 1;
        }

        /// <summary>
        /// Sets the tool the player currently has selected. 
        /// </summary>
        /// <param name="tool_no">The number of the tool on the hotbar, from 1-6.</param>
        public void setTool(int tool_no)
        {
            if (tool_no > 6)
            {
                this.tool_number = 6;
            }
            else if (tool_no < 1)
            {
                this.tool_number = 1;
            }
            else
            {
                this.tool_number = tool_no;
            }
        }

        /// <summary>
        /// Returns the current tool number.
        /// </summary>
        /// <returns>Number of currently selected tool.</returns>
        public int getTool()
        {
            return this.tool_number;
        }

        /// <summary>
        /// Main update function for tool. Changes functionality depending on what tool is selected.
        /// </summary>
        public override void update()
        {
            if (this.tool_number == 1)
            {
                this.tool_line_end[0] = Mouse.GetState().Position.X;
                this.tool_line_end[1] = Mouse.GetState().Position.Y;
            }
            else if (this.tool_number == 2)
            {
                if (Program.game.mouse.LeftButton == ButtonState.Pressed)
                {
                    // Try to place a new core
                    Texture2D base_tex = Program.game.textures["structure\\base.png"];
                    int[] real_pos = Util.screenPosToSpacePos(Program.game.mouse.X - base_tex.Width / 2, Program.game.mouse.Y - base_tex.Width / 2);
                    int[] builder_sector_pos = Space.getSector(real_pos[0], real_pos[1]);
                    Sector builder_sector = Program.game.space.findLoadedSector(builder_sector_pos[0], builder_sector_pos[1]);
                    Builder new_build = new Builder(builder_sector, real_pos[0], real_pos[1]);
                    if (Program.game.space.canPlace(new_build) && Program.game.ship.removeMinerals(BuildCosts.BASE))
                    {
                        new_build.addToSector(new_build.getContainingSector());
                    }
                    else
                    {
                        new_build.setDead();
                    }
                }
            }
        }

        /// <summary>
        /// Draws the texture of the currently selected tool.
        /// </summary>
        public override void draw()
        {
            if (this.tool_number == 1)
            {
                // Utility laser
                if (Program.game.mouse.LeftButton == ButtonState.Pressed)
                {
                    Program.game.drawLine(Program.game.ship.draw_location[0], Program.game.ship.draw_location[1], this.tool_line_end[0], this.tool_line_end[1], 2, color: Color.Blue);
                    Program.game.drawLine(Program.game.ship.draw_location[0] + 1, Program.game.ship.draw_location[1] + 1, this.tool_line_end[0], this.tool_line_end[1], 2, color: Color.Blue);
                    Program.game.drawLine(Program.game.ship.draw_location[0] - 1, Program.game.ship.draw_location[1] - 1, this.tool_line_end[0], this.tool_line_end[1], 2, color: Color.Blue);
                }
            }
            if (this.tool_number == 2)
            {
                if (Program.game.mouse.LeftButton == ButtonState.Pressed)
                {

                }
            }
        }
    }
}
