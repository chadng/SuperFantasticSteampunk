using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperFantasticSteampunk
{
    public static class Program
    {
        private static Game1 game;

        [STAThread]
        static void Main()
        {
            Logger.Start();
            try
            {
                game = new Game1();
                game.Run();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            Logger.Finish();
        }
    }
}
