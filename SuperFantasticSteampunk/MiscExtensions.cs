using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Wrapper<T>
    {
        #region Instance Properties
        public T Value { get; set; }
        #endregion

        #region Constructors
        public Wrapper(T value)
        {
            Value = value;
        }
        #endregion
    }

    static class MiscExtensions
    {
        #region Constants
        private static readonly int[] regionAttachmentXIndexes = new int[] {
            RegionAttachment.X1,
            RegionAttachment.X2,
            RegionAttachment.X3,
            RegionAttachment.X4
        };

        private static readonly int[] regionAttachmentYIndexes = new int[] {
            RegionAttachment.Y1,
            RegionAttachment.Y2,
            RegionAttachment.Y3,
            RegionAttachment.Y4
        };
        #endregion

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

        public static T Sample<T>(this IList<T> self)
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

        public static List<T2> Map<T1, T2>(this List<T1> self, Func<T1, T2> func)
        {
            List<T2> result = new List<T2>(self.Count);
            foreach (T1 item in self)
                result.Add(func(item));
            return result;
        }

        public static bool Contains<T>(this IList<T> self, T item)
            where T : class
        {
            foreach (T element in self)
            {
                if (element == item)
                    return true;
            }
            return false;
        }

        public static int IndexOf<T>(this T[] self, T item)
        {
            for (int i = 0; i < self.Length; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(self[i], item))
                    return i;
            }
            return -1;
        }

        public static Rectangle GenerateBoundingBox(this SkeletonData self)
        {
            Skeleton skeleton = new Skeleton(self);
            skeleton.UpdateWorldTransform();

            Vector2 minPoint = new Vector2();
            Vector2 maxPoint = new Vector2();
            bool firstSlot = true;
            float[] vertices = new float[8];

            foreach (Slot slot in skeleton.DrawOrder)
            {
                RegionAttachment regionAttachment = slot.Attachment as RegionAttachment;
                if (regionAttachment == null)
                    continue;

                regionAttachment.ComputeVertices(skeleton.X, skeleton.Y, slot.Bone, vertices);

                if (firstSlot)
                {
                    minPoint.X = maxPoint.X = vertices[RegionAttachment.X1];
                    minPoint.Y = maxPoint.Y = vertices[RegionAttachment.Y1];
                    firstSlot = false;
                }

                foreach (int index in regionAttachmentXIndexes)
                {
                    float value = vertices[index];
                    if (value < minPoint.X)
                        minPoint.X = value;
                    else if (value > maxPoint.X)
                        maxPoint.X = value;
                }

                foreach (int index in regionAttachmentYIndexes)
                {
                    float value = vertices[index];
                    if (value < minPoint.Y)
                        minPoint.Y = value;
                    else if (value > maxPoint.Y)
                        maxPoint.Y = value;
                }
            }

            return new Rectangle(
                (int)(minPoint.X),
                (int)(minPoint.Y - Math.Abs(maxPoint.Y)),
                (int)(Math.Abs(minPoint.X) + Math.Abs(maxPoint.X)),
                (int)(Math.Abs(minPoint.Y) + Math.Abs(maxPoint.Y))
            );
        }
    }
}
