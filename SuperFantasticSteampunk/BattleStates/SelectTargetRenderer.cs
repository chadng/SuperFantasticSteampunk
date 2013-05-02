using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTargetRenderer : BattleStateRenderer
    {
        #region Instance Fields
        Texture2D arrowTexture;
        #endregion

        #region Instance Properties
        protected new SelectTarget battleState
        {
            get { return base.battleState as SelectTarget; }
        }
        #endregion

        #region Constructors
        public SelectTargetRenderer(BattleState battleState)
            : base(battleState)
        {
            arrowTexture = ResourceManager.GetTexture("arrow_down");
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
            // Update transitions and easing
        }

        public override void BeforeDraw(Renderer renderer)
        {
            // Draw elements under party members
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawArrowOverPotentialTarget(renderer);
        }

        private void drawArrowOverPotentialTarget(Renderer renderer)
        {
            Vector2 position = battleState.PotentialTarget.BattleEntity.Position;
            position.Y -= 400.0f;
            renderer.Draw(arrowTexture, position, Color.White);
        }
        #endregion
    }
}
