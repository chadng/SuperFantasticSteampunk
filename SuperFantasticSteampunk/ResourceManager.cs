using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk
{
    static class ResourceManager
    {
        #region Static Fields
        private static SortedDictionary<string, SkeletonData> skeletonDataDictionary;
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            skeletonDataDictionary = new SortedDictionary<string, SkeletonData>();
            populateSkeletonDataDictionary(contentManager, graphicsDevice);
        }

        public static Skeleton GetSkeleton(string name)
        {
            SkeletonData skeletonData;
            if (skeletonDataDictionary.TryGetValue(name, out skeletonData))
                return new Skeleton(skeletonData);
            return null;
        }

        private static void populateSkeletonDataDictionary(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            string skeletonDirectory = contentManager.RootDirectory + @"\Skeletons\";
            IEnumerable<string> atlasFileNames = Directory.EnumerateFiles(skeletonDirectory, "*.atlas");
            foreach (string atlasFileName in atlasFileNames)
            {
                string skeletonName = atlasFileName.Replace(skeletonDirectory, "").Replace(".atlas", "");
                skeletonDataDictionary.Add(skeletonName, loadSkeletonData(skeletonDirectory + skeletonName, graphicsDevice));
#if DEBUG
                System.Console.WriteLine("Loaded skeleton '" + skeletonName + "'");
#endif
            }
        }

        private static SkeletonData loadSkeletonData(string skeletonPath, GraphicsDevice graphicsDevice)
        {
            Atlas atlas = new Atlas(skeletonPath + ".atlas", new XnaTextureLoader(graphicsDevice));
            SkeletonJson skeletonJson = new SkeletonJson(atlas);
            return skeletonJson.ReadSkeletonData(skeletonPath + ".json");
        }
        #endregion
    }
}
