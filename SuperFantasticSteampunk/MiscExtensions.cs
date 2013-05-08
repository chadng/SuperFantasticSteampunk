using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    }
}
