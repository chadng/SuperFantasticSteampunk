using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTargetRenderer : BattleStateRenderer
    {
        #region Instance Fields
        TextureData arrowTextureData;
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
            arrowTextureData = ResourceManager.GetTextureData("arrow_down");
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
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
            Color color = battleState.Actor.FearsPartyMember(battleState.PotentialTarget) ? Color.Black : Color.White;
            renderer.Draw(arrowTextureData, position, color);
        }
        #endregion
    }
}
