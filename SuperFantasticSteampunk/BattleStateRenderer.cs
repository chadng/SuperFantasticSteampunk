using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    abstract class BattleStateRenderer
    {
        #region Instance Properties
        protected BattleState battleState { get; private set; }
        #endregion

        #region Constructors
        public BattleStateRenderer(BattleState battleState)
        {
            if (battleState == null)
                throw new Exception("BattleState cannot be null");
            this.battleState = battleState;
        }
        #endregion

        #region Instance Methods
        public virtual void Finish()
        {
        }

        public virtual void Pause()
        {
        }

        public virtual void Resume()
        {
        }

        public abstract void Update(Delta delta);

        public abstract void BeforeDraw(Renderer renderer);

        public abstract void AfterDraw(Renderer renderer);
        #endregion
    }
}
