using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    abstract class BattleState
    {
        #region Instance Fields
        protected Battle battle;
        #endregion

        #region Constructors
        protected BattleState(Battle battle)
        {
            if (battle == null)
                throw new Exception("Battle cannot be null");
            this.battle = battle;
        }
        #endregion

        #region Instance Methods
        public abstract void Start();
        public abstract void Finish();

        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SkeletonRenderer skeletonRenderer)
        {
        }

        public void ChangeState(BattleState state)
        {
            battle.ChangeState(state);
        }
        #endregion
    }
}
