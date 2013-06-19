using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class MoveActorRenderer : BattleStateRenderer
    {
        #region Instance Fields
        private ThinkRenderer thinkRenderer;
        #endregion

        #region Instance Properties
        protected new MoveActor battleState
        {
            get { return base.battleState as MoveActor; }
        }
        #endregion

        #region Constructors
        public MoveActorRenderer(BattleState battleState, ThinkRenderer thinkRenderer)
            : base(battleState)
        {
            tintOtherPartyMembers(new Color(Color.White.ToVector3() * 0.7f));
            this.thinkRenderer = thinkRenderer;
        }
        #endregion

        #region Instance Methods
        public override void Finish()
        {
            tintOtherPartyMembers(Color.White);
            base.Finish();
        }

        public override void Update(Delta delta)
        {
            thinkRenderer.Update(delta);
        }

        public override void BeforeDraw(Renderer renderer)
        {
            thinkRenderer.BeforeDraw(renderer);
        }

        public override void AfterDraw(Renderer renderer)
        {
            thinkRenderer.AfterDraw(renderer);
        }

        private void tintOtherPartyMembers(Color color)
        {
            foreach (PartyMember partyMember in battleState.Battle.PlayerParty)
            {
                if (partyMember != this.battleState.Actor)
                    partyMember.BattleEntity.Tint = color;
            }
        }
        #endregion
    }
}
