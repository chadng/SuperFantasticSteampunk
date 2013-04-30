using System;

namespace SuperFantasticSteampunk
{
    static class MiscExtensions
    {
        public static T Tap<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
    }
}
