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

        public static void FinishCurrent()
        {
            if (Current != null)
                Current.Finish();
        }

        public static void UpdateCurrent(GameTime gameTime)
        {
            if (Current != null)
            {
                if (Current.finished)
                {
                    Current.finishCleanup();
                    sceneStack.Pop();
                }
                else
                    Current.update(gameTime);
            }
        }

        public static void DrawCurrent(Renderer renderer)
        {
            if (Current != null)
                Current.draw(renderer);
        }
        #endregion

        #region Instance Fields
        private List<Entity> entities;
        private bool finished;
        #endregion

        #region Constructors
        protected Scene()
        {
            entities = new List<Entity>();
            finished = false;
            sceneStack.Push(this);
        }
        #endregion

        #region Instance Methods
        public void Finish()
        {
            Logger.Log("Finishing scene");
            finished = true;
        }

        protected virtual void addEntity(Entity entity)
        {
            entities.Add(entity);
        }

        protected virtual void removeEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        protected virtual void update(GameTime gameTime)
        {
            foreach (Entity entity in entities)
                entity.Update(gameTime);
        }

        protected virtual void draw(Renderer renderer)
        {
            entities.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
            foreach (Entity entity in entities)
                entity.Draw(renderer);
        }

        protected virtual void finishCleanup()
        {
            entities.Clear();
        }
        #endregion
    }
}
