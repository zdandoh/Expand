using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expand
{
    class Util
    {
        public static double distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
    }
}
