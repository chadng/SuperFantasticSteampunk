using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    static class MiscExtensions
    {
        public static T Tap<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }

        public static void Try<T>(this T self, Action<T> action)
        {
            if (self != null)
                action(self);
        }

        public static void WriteLineProperties(this Object self)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(self))
                Console.WriteLine("{0}={1}", descriptor.Name, descriptor.GetValue(self));
        }

        public static void AddOrReplace<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> self, TKey key, TValue value)
            where TKey : class
            where TValue : class
        {
            TValue tempValue;
            if (self.TryGetValue(key, out tempValue))
                self.Remove(key);
            self.Add(key, value);
        }

        public static T Sample<T>(this List<T> self)
        {
            if (self.Count == 0)
                return default(T);
            return self[Game1.Random.Next(self.Count)];
        }

        public static bool EIntersects(this Rectangle self, Rectangle otherRect)
        {
            bool xIntersection = (self.Left < otherRect.Right && self.Left > otherRect.Left) || (self.Right > otherRect.Left && self.Right < otherRect.Right);
            bool yIntersection = (self.Top < otherRect.Bottom && self.Top > otherRect.Top) || (self.Bottom > otherRect.Top && self.Bottom < otherRect.Bottom);
            bool longer = (self.Left <= otherRect.Left && self.Right >= otherRect.Right) || (otherRect.Left <= self.Left && otherRect.Right >= self.Right);
            bool taller = (self.Top <= otherRect.Top && self.Bottom >= otherRect.Bottom) || (otherRect.Top <= self.Top && otherRect.Bottom >= self.Bottom);
            return (xIntersection || longer) && (yIntersection || taller);
        }

        public static bool Intersects(this Rectangle self, Rectangle otherRect, out Vector2 intersectionAmount)
        {
            bool xIntersection = false, yIntersection = false;
            intersectionAmount = Vector2.Zero;

            if (self.Left < otherRect.Right && self.Left > otherRect.Left)
            {
                xIntersection = true;
                intersectionAmount.X = self.Left - otherRect.Right;
            }
            else if (self.Right > otherRect.Left && self.Right < otherRect.Right)
            {
                xIntersection = true;
                intersectionAmount.X = self.Right - otherRect.Left;
            }
            else if ((self.Left <= otherRect.Left && self.Right >= otherRect.Right) || (otherRect.Left <= self.Left && otherRect.Right >= self.Right))
                xIntersection = true;

            if (self.Top < otherRect.Bottom && self.Top > otherRect.Top)
            {
                yIntersection = true;
                intersectionAmount.Y = self.Top - otherRect.Bottom;
            }
            else if (self.Bottom > otherRect.Top && self.Bottom < otherRect.Bottom)
            {
                yIntersection = true;
                intersectionAmount.Y = self.Bottom - otherRect.Top;
            }
            else if ((self.Top <= otherRect.Top && self.Bottom >= otherRect.Bottom) || (otherRect.Top <= self.Top && otherRect.Bottom >= self.Bottom))
                yIntersection = true;

            if (!xIntersection || !yIntersection)
                intersectionAmount = Vector2.Zero;

            return xIntersection && yIntersection;
        }

        public static void Merge<T1, T2>(this IDictionary<T1, T2> self, IDictionary<T1, T2> other, Func<T2, T2, T2> keyExistsHandler = null)
        {
            foreach (KeyValuePair<T1, T2> pair in other)
            {
                if (self.ContainsKey(pair.Key))
                {
                    if (keyExistsHandler == null)
                        self[pair.Key] = pair.Value;
                    else
                        self[pair.Key] = keyExistsHandler(self[pair.Key], pair.Value);
                }
                else
                    self.Add(pair.Key, pair.Value);
            }
        }
    }
}
