using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    abstract class OverworldState
    {
        #region Instance Properties
        public Overworld Overworld { get; private set; }
        public OverworldStateRenderer OverworldStateRenderer { get; protected set; }
        #endregion

        #region Constructors
        protected OverworldState(Overworld overworld)
        {
            if (overworld == null)
                throw new Exception("Overworld cannot be null");
            Overworld = overworld;
        }
        #endregion

        #region Instance Methods
        public abstract void Start();

        public virtual void Finish()
        {
            if (OverworldStateRenderer != null)
                OverworldStateRenderer.Finish();
        }

        public virtual void Pause()
        {
            if (OverworldStateRenderer != null)
                OverworldStateRenderer.Pause();
        }

        public virtual void Resume(OverworldState previousOverworldState)
        {
            if (OverworldStateRenderer != null)
                OverworldStateRenderer.Resume();
        }

        public abstract void Update(Delta delta);

        public void ChangeState(OverworldState state)
        {
            if (Overworld.CurrentOverworldState == this)
                Overworld.ChangeState(state);
        }

        public void PushState(OverworldState state)
        {
            if (Overworld.CurrentOverworldState == this)
                Overworld.PushState(state);
        }

        public void PopState()
        {
            if (Overworld.CurrentOverworldState == this)
                Overworld.PopState();
        }
        #endregion
    }
}
