using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTargetRenderer : BattleStateRenderer
    {
        #region Constants
        private const float colorAlphaSpeed = 1.0f;
        private const float minColorAlpha = 0.0f;
        private const float maxColorAlpha = 0.8f;
        #endregion

        #region Instance Fields
        TextureData arrowTextureData;
        Effect colorTextureShader;
        float colorAlpha;
        bool incColorAlpha;
        PartyMember currentlySelectedPartyMember;
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
            colorTextureShader = ResourceManager.GetShader("ColorTexture");
            colorAlpha = 0.0f;
            incColorAlpha = true;
            currentlySelectedPartyMember = null;
        }
        #endregion

        #region Instance Methods
        public override void Pause()
        {
            nullifyCurrentlySelectedPartyMember();
            base.Pause();
        }

        public override void Finish()
        {
            nullifyCurrentlySelectedPartyMember();
            base.Finish();
        }

        public override void Update(Delta delta)
        {
            updateColorAlpha(delta);
            updateCurrentlySelectedPartyMember();
        }

        public override void BeforeDraw(Renderer renderer)
        {
            colorTextureShader.Parameters["TexColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, colorAlpha));
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawArrowOverPotentialTarget(renderer);
        }

        private void updateColorAlpha(Delta delta)
        {
            if (incColorAlpha)
            {
                colorAlpha += colorAlphaSpeed * delta.Time;
                if (colorAlpha >= maxColorAlpha)
                {
                    colorAlpha = maxColorAlpha;
                    incColorAlpha = false;
                }
            }
            else
            {
                colorAlpha -= colorAlphaSpeed * delta.Time;
                if (colorAlpha <= minColorAlpha)
                {
                    colorAlpha = minColorAlpha;
                    incColorAlpha = true;
                }
            }
        }

        private void updateCurrentlySelectedPartyMember()
        {
            if (currentlySelectedPartyMember != battleState.PotentialTarget)
            {
                if (currentlySelectedPartyMember != null)
                    currentlySelectedPartyMember.BattleEntity.Shader = null;
                currentlySelectedPartyMember = battleState.PotentialTarget;
                currentlySelectedPartyMember.BattleEntity.Shader = colorTextureShader;
                colorAlpha = 0.0f;
                incColorAlpha = true;
            }
        }

        private void drawArrowOverPotentialTarget(Renderer renderer)
        {
            Vector2 position = battleState.PotentialTarget.BattleEntity.Position;
            position.Y -= 400.0f;
            Color color = battleState.Actor.FearsPartyMember(battleState.PotentialTarget) ? Color.Black : Color.White;
            renderer.Draw(arrowTextureData, position, color);
        }

        private void nullifyCurrentlySelectedPartyMember()
        {
            if (currentlySelectedPartyMember == null)
                return;

            if (currentlySelectedPartyMember.BattleEntity.Shader == colorTextureShader)
                currentlySelectedPartyMember.BattleEntity.Shader = null;
            currentlySelectedPartyMember = null;
        }
        #endregion
    }
}
