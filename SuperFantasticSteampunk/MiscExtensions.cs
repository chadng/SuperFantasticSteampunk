using System;
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
    }
}
