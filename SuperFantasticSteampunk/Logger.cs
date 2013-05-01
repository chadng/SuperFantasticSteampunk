using System;

namespace SuperFantasticSteampunk
{
    class Logger
    {
        #region Static Fields
        private static uint messageCounter;
        #endregion

        #region Static Constructors
        static Logger()
        {
            messageCounter = 0;
            //TODO: choose between file and console logging
        }
        #endregion

        #region Static Methods
        public static void Log(string message)
        {
            unchecked { Console.WriteLine(messageCounter++.ToString() + ": " + message); }
        }
        #endregion
    }
}
