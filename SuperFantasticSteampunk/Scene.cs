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
            {
                pushNextScene();
                Current.update(gameTime);
            }
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

        #region Instance Properties
        public List<Entity> Entities;
        public List<Scenery> SceneryEntities;
        #endregion

        #region Instance Fields
        private List<Entity> entitiesToAdd;
        private bool finished;
        #endregion

        #region Constructors
        protected Scene()
        {
            Entities = new List<Entity>();
            SceneryEntities = new List<Scenery>();
            entitiesToAdd = new List<Entity>();
            finished = false;
            nextScene = this;
            if (Current == null)
                pushNextScene();
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
            Scenery scenery = entity as Scenery;
            if (scenery != null)
                SceneryEntities.Add(scenery);
        }

        protected virtual void update(GameTime gameTime)
        {
            if (entitiesToAdd.Count > 0)
            {
                Entities.AddRange(entitiesToAdd);
                entitiesToAdd.Clear();
            }

            foreach (Entity entity in Entities)
                entity.Update(gameTime);

            removeDeadEntities();
        }

        protected virtual void draw(Renderer renderer)
        {
            Entities.Sort((a, b) => a.ZIndex == b.ZIndex ? a.Position.Y.CompareTo(b.Position.Y) : b.ZIndex.CompareTo(a.ZIndex));
            foreach (Entity entity in Entities)
                entity.Draw(renderer);
        }

        protected virtual void finishCleanup()
        {
            Entities.Clear();
        }

        private void removeDeadEntities()
        {
            for (int i = Entities.Count - 1; i >= 0; --i)
            {
                if (!Entities[i].Alive)
                    Entities.RemoveAt(i);
            }

            for (int i = SceneryEntities.Count - 1; i >= 0; --i)
            {
                if (!SceneryEntities[i].Alive)
                    SceneryEntities.RemoveAt(i);
            }
        }
        #endregion
    }
}
