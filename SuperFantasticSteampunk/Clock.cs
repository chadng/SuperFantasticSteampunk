using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    static class Clock
    {
        #region Constants
        private const float hoursPerSecond = 0.2f;
        #endregion

        #region Instance Fields
        private static float time;
        private static Color[] colors;
        #endregion

        #region Instance Properties
        public static int Hour
        {
            get { return (int)Math.Floor(time); }
        }

        public static bool IsDay
        {
            get { return Hour > 4 && Hour < 4; }
        }

        public static bool IsNight
        {
            get { return !IsDay; }
        }
        #endregion

        #region Static Methods
        public static void Init(int hour)
        {
            time = hour;
            colors = new Color[] {
                new Color(0.2f, 0.2f, 0.3f),
                new Color(0.2f, 0.2f, 0.3f),
                new Color(0.85f, 0.4f, 0.63f),
                new Color(0.85f, 0.4f, 0.63f),
                new Color(1.0f, 0.5f, 0.7f),
                new Color(1.0f, 0.8f, 0.5f),
                
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),

                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),
                new Color(1.0f, 1.0f, 1.0f),

                new Color(1.0f, 0.8f, 0.5f),
                new Color(1.0f, 0.5f, 0.7f),
                new Color(0.85f, 0.4f, 0.63f),
                new Color(0.85f, 0.4f, 0.63f),
                new Color(0.2f, 0.2f, 0.3f),
                new Color(0.2f, 0.2f, 0.3f)
            };
        }

        public static void Update(Delta delta)
        {
            Update(hoursPerSecond * delta.Time);
        }

        public static void Update(float hours)
        {
            time += hours;
            while (time >= 24.0f)
                time -= 24.0f;
        }

        public static Color GetCurrentColor()
        {
            int nextHour = Hour == 23 ? 0 : Hour + 1;
            return Color.Lerp(colors[Hour], colors[nextHour], time % 1.0f);
        }
        #endregion
    }
}
