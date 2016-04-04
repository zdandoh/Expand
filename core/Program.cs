#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Expand
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static Expand game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            game = new Expand();
            using (game)
                game.Run();
        }
    }
#endif
}
