using System.Collections.Generic;
using Spine;

namespace SuperFantasticSteampunk
{
    static class ResourceManager
    {
        #region Static Fields
        private static SortedDictionary<string, SkeletonData> skeletonDataDictionary;
        #endregion

        #region Static Methods
        public static void Initialize()
        {
            skeletonDataDictionary = new SortedDictionary<string, SkeletonData>();
        }

        public static Skeleton GetSkeleton(string name)
        {
            SkeletonData skeletonData;
            if (skeletonDataDictionary.TryGetValue(name, out skeletonData))
                return new Skeleton(skeletonData);
            return null;
        }
        #endregion
    }
}
