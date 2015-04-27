using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    public static class Util
    {
        public static double distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static double distance(double n1, double n2)
        {
            return Math.Sqrt(n1 * n1 - n2 * n2);
        }

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

        public static int[] screenPosToSpacePos(int x, int y)
        {
            return new int[] {Program.game.ship.pos[0] + x - Program.game.ship.draw_location[0], Program.game.ship.pos[1] + y - Program.game.ship.draw_location[1]};
        }

        public static int[] mousePosToSpacePos()
        {
            return screenPosToSpacePos(Program.game.mouse.X, Program.game.mouse.Y);
        }
    }

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

        public bool Contains(Point point_inside)
        {
            if (Util.distance(x, y, point_inside.X, point_inside.Y) < this.r)
            {
                return true;
            }
            return false;
        }

        public int getDistance(int x, int y)
        {
            return (int)Util.distance(this.x + this.r, this.y + this.r, x, y);
        }

        public double getArea()
        {
            return Math.PI * Math.Pow(this.r, 2);
        }

        public double getCircumfrence()
        {
            return Math.PI * 2 * this.r;
        }
    }

    public struct CollideShape
    {
        public static int NONE = 0;
        public static int CIRCLE = 1;
        public static int RECT = 2;
        public static int ABSTRACT = 3;
    }

    public struct BuildCosts
    {
        public static int BASE = 100;
    }
}
