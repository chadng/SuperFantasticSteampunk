using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    class Scene
    {
        #region Static Fields
        private static Stack<Scene> sceneStack;
        private static Scene nextScene;
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
            nextScene = null;
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
                {
                    if (nextScene != null)
                        pushNextScene();
                    Current.update(gameTime);
                }
            }
            else if (nextScene != null)
                pushNextScene();
        }

        public static void DrawCurrent(Renderer renderer)
        {
            if (Current != null)
                Current.draw(renderer);
        }

        private static void pushNextScene()
        {
            sceneStack.Push(nextScene);
            nextScene = null;
        }
        #endregion

        #region Instance Fields
        private List<Entity> entities;
        private List<Entity> entitiesToAdd;
        private List<Entity> entitiesToRemove;
        private bool finished;
        #endregion

        #region Constructors
        protected Scene()
        {
            entities = new List<Entity>();
            entitiesToAdd = new List<Entity>();
            entitiesToRemove = new List<Entity>();
            finished = false;
            nextScene = this;
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
            entitiesToAdd.Add(entity);
        }

        protected virtual void removeEntity(Entity entity)
        {
            entitiesToRemove.Add(entity);
            entitiesToAdd.Remove(entity);
        }

        protected virtual void update(GameTime gameTime)
        {
            if (entitiesToAdd.Count > 0)
            {
                entities.AddRange(entitiesToAdd);
                entitiesToAdd.Clear();
            }

            foreach (Entity entity in entities)
                entity.Update(gameTime);

            if (entitiesToRemove.Count > 0)
            {
                foreach (Entity entity in entitiesToRemove)
                    entities.Remove(entity);
                entitiesToRemove.Clear();
            }
        }

        protected virtual void draw(Renderer renderer)
        {
            entities.Sort((a, b) => a.ZIndex == b.ZIndex ? a.Position.Y.CompareTo(b.Position.Y) : b.ZIndex.CompareTo(a.ZIndex));
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
