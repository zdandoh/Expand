using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Util
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
    }
}
