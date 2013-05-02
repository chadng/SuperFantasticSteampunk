using System;
using Microsoft.Xna.Framework;
using Spine;

namespace SuperFantasticSteampunk
{
    abstract class BattleStateRenderer
    {
        #region Instance Fields
        private BattleState battleState;
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
        public abstract void Update(GameTime gameTime);

        public abstract void BeforeDraw(SkeletonRenderer skeletonRenderer);

        public abstract void AfterDraw(SkeletonRenderer skeletonRenderer);
        #endregion
    }
}
