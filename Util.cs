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
    }
}
