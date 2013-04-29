using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Scene
    {
        #region Static Fields
        private static Stack<Scene> sceneStack;
        #endregion

        #region Static Properties
        public static Scene Current
        {
            get { return sceneStack.Count == 0 ? null : sceneStack.Peek(); }
        }
        #endregion

        #region Static Constructors
        static Scene()
        {
            sceneStack = new Stack<Scene>();
        }
        #endregion

        #region Static Methods
        public static void AddEntity(Entity entity)
        {
            if (Current != null)
                Current.addEntity(entity);
        }

        public static void RemoveEntity(Entity entity)
        {
            if (Current != null)
                Current.removeEntity(entity);
        }

        public static void Finish()
        {
            if (Current != null)
            {
                Current.finish();
                sceneStack.Pop();
            }
        }

        public static void Update(GameTime gameTime)
        {
            if (Current != null)
                Current.update(gameTime);
        }

        public static void Draw(SkeletonRenderer skeletonRenderer)
        {
            if (Current != null)
                Current.draw(skeletonRenderer);
        }
        #endregion

        #region Instance Fields
        private List<Entity> entities;
        #endregion

        #region Constructors
        public Scene()
        {
            entities = new List<Entity>();
            sceneStack.Push(this);
        }
        #endregion

        #region Instance Methods
        private void addEntity(Entity entity)
        {
            entities.Add(entity);
        }

        private void removeEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        private void finish()
        {
            entities.Clear();
        }

        private void update(GameTime gameTime)
        {
            foreach (Entity entity in entities)
                entity.Update(gameTime);
        }

        private void draw(SkeletonRenderer skeletonRenderer)
        {
            skeletonRenderer.Begin();
            foreach (Entity entity in entities)
                entity.Draw(skeletonRenderer);
            skeletonRenderer.End();
        }
        #endregion
    }
}
