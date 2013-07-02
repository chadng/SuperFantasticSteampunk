using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTargetRenderer : BattleStateRenderer
    {

        #region Instance Fields
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
            currentlySelectedPartyMember = null;
        }
        #endregion

        #region Instance Methods
        public override void Pause()
        {
            tintOtherPartyMembers(Color.White);
            base.Pause();
        }

        public override void Finish()
        {
            tintOtherPartyMembers(Color.White);
            base.Finish();
        }

        public override void Update(Delta delta)
        {
            updateCurrentlySelectedPartyMember();
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            drawArrowOverPotentialTarget(renderer);
        }

        private void updateCurrentlySelectedPartyMember()
        {
            if (currentlySelectedPartyMember != battleState.PotentialTarget)
            {
                currentlySelectedPartyMember = battleState.PotentialTarget;
                currentlySelectedPartyMember.BattleEntity.Tint = Color.White;
                tintOtherPartyMembers(new Color(Color.White.ToVector3() * 0.5f));
            }
        }

        private void drawArrowOverPotentialTarget(Renderer renderer)
        {
            Color color = battleState.Actor.FearsPartyMember(battleState.PotentialTarget) ? Color.Black : Color.White;
            battleState.Battle.DrawArrowOverPartyMember(battleState.PotentialTarget, color, renderer);
        }

        private void tintOtherPartyMembers(Color color)
        {
            foreach (PartyMember partyMember in battleState.Battle.GetPartyForPartyMember(currentlySelectedPartyMember))
            {
                if (partyMember != currentlySelectedPartyMember)
                    partyMember.BattleEntity.Tint = color;
            }
        }
        #endregion
    }
}
