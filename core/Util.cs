using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Expand
{
    /// <summary>
    /// Misc functions that are useful for stuff and I don't want to make a separate class for.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Get distance between two points.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Distance between point one and two.</returns>
        public static double distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static double distance(double n1, double n2)
        {
            return Math.Sqrt(n1 * n1 - n2 * n2);
        }
        
        /// <summary>
        /// Inserts newlines into text between words at certain intervals.
        /// </summary>
        /// <param name="before_text">String of unaltered texts.</param>
        /// <param name="interval">Character interval of each newline insertion.</param>
        /// <returns></returns>
        public static String wrapText(String before_text, int interval)
        {
            String[] words = before_text.Split(' ');
            String new_string = "";
            int interval_level = 0;
            foreach (String word in words)
            {
                interval_level += word.Length;
                new_string += word + " ";
                if (interval_level >= interval)
                {
                    new_string += "\n\r";
                    interval_level = 0;
                }
            }
            return new_string;
        }

        public static bool toggle(bool boolean)
        {
            if (!boolean) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts screen coordinates to space coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int[] screenPosToSpacePos(int x, int y)
        {
            return new int[] {Program.game.ship.pos[0] + x - Program.game.ship.draw_location[0], Program.game.ship.pos[1] + y - Program.game.ship.draw_location[1]};
        }

        /// <summary>
        /// Converts position of mouse on screen to position of mouse in space.
        /// </summary>
        /// <returns></returns>
        public static int[] mousePosToSpacePos()
        {
            return screenPosToSpacePos(Program.game.mouse.X, Program.game.mouse.Y);
        }
    }

    /// <summary>
    /// Houses various collision functions and their argument switched overloads.
    /// </summary>
    public static class Collider
    {
        public static bool intersects(Object doesnt_matter, bool star)
        {
            return false;
        }

        public static bool intersects(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }

        public static bool intersects(Rectangle rect, Circle circle)
        {
            return intersects(circle, rect);
        }

        public static bool intersects(Circle circle1, Circle circle2)
        {
            bool collides = false;
            int distance = (int)Util.distance(circle1.x, circle1.y, circle2.x, circle2.y);
            if (distance < circle1.r*2 + 1 || distance < circle2.r*2 + 1)
            {
                collides = true;
            }
            return collides;
        }

        public static bool intersects(Circle circle, Rectangle rect)
        {
            bool collides = false;
            int distance_x = Math.Abs(circle.x - rect.X);
            int distance_y = Math.Abs(circle.y - rect.Y);

            if (distance_x > rect.Width / 2 + circle.r || distance_y > rect.Height / 2 + circle.r)
            {
                collides = false;
            }
            else if (distance_x <= rect.Width / 2 || distance_y <= rect.Height / 2)
            {
                collides = true;
            }
            else
            {
                int corner_distance = (int)Math.Pow(distance_x - rect.Width / 2, 2) + (int)Math.Pow(distance_y - rect.Height / 2, 2);
                if (corner_distance <= Math.Pow(circle.r, 2))
                {
                    collides = true;
                }
            }
            return collides;
        }
    }

    /// <summary>
    /// a utility class for circle collisions and other operations.
    /// </summary>
    public class Circle: Object
    {
        public int x;
        public int y;
        public int r;
        public Circle(int x, int y, int r)
        {
            this.x = x;
            this.y = y;
            this.r = r;
        }

        /// <summary>
        /// Checks if circle contains point.
        /// </summary>
        /// <param name="point_inside">Point object to check the inside of circle.</param>
        /// <returns>Boolean true if point is in circle, otherwise false.</returns>
        public bool Contains(Point point_inside)
        {
            if (Util.distance(x, y, point_inside.X, point_inside.Y) < this.r)
            {
                return true;
            }
            return false;
        }

        public bool Contains(Vector2 vector_inside)
        {
            if (Util.distance(x, y, vector_inside.X, vector_inside.Y) < this.r)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates distance from center of circle to provided point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int getDistance(int x, int y)
        {
            return (int)Util.distance(this.x + this.r, this.y + this.r, x, y);
        }

        /// <summary>
        /// Returns area of circle.
        /// </summary>
        /// <returns></returns>
        public double getArea()
        {
            return Math.PI * Math.Pow(this.r, 2);
        }

        /// <summary>
        /// Returns circumfrence of circle.
        /// </summary>
        /// <returns></returns>
        public double getCircumfrence()
        {
            return Math.PI * 2 * this.r;
        }
    }

    /// <summary>
    /// Unused struct that stores different shapes of SpaceObjects.
    /// </summary>
    public struct CollideShape
    {
        public static int NONE = 0;
        public static int CIRCLE = 1;
        public static int RECT = 2;
        public static int ABSTRACT = 3;
    }

    /// <summary>
    /// Struct stores each cost for each building.
    /// </summary>
    public struct BuildCosts
    {
        public static int BASE = 100;
    }
    
    /// <summary>
    /// Struct stores numbers that each represent a branch of the tech tree.
    /// </summary>
    public struct TechTree
    {
        public static int NONE = 0;
        public static int MINING = 1;
        public static int COMBAT = 2;
    }

    /// <summary>
    /// Class that keeps track of and counts FPS.
    /// </summary>
    public class FPSCounter: GameObject
    {
        public const int GOAL_FPS = 60;
        public int last_fps = 60;
        public int fps;
        private long time_elapsed = 0;
        public FPSCounter()
        {
            fps = 0;
            time_elapsed = Program.game.game_time.ElapsedMilliseconds;
        }

        /// <summary>
        /// Doesn't actually draw anything. Just counts how many times draw has been called each second.
        /// </summary>
        public override void draw()
        {
            fps++;
            if (Program.game.game_time.ElapsedMilliseconds - time_elapsed > 1000)
            {
                last_fps = fps;
                fps = 0;
                time_elapsed = Program.game.game_time.ElapsedMilliseconds;
            }
        }
    }
}
