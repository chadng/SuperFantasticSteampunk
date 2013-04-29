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
            get { return sceneStack.Peek(); }
        }
        #endregion

        #region Static Constructors
        static Scene()
        {
            sceneStack = new Stack<Scene>();
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
        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Entity entity in entities)
                entity.Update(gameTime);
        }

        public void Draw(SkeletonRenderer skeletonRenderer)
        {
            skeletonRenderer.Begin();
            foreach (Entity entity in entities)
                entity.Draw(skeletonRenderer);
            skeletonRenderer.End();
        }

        public void Finish()
        {
            entities.Clear();
            if (Scene.Current == this)
                sceneStack.Pop();
        }
        #endregion
    }
}
