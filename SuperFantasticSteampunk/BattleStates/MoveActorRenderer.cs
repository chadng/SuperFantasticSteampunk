using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class MoveActorRenderer : BattleStateRenderer
    {
        #region Instance Properties
        protected new MoveActor battleState
        {
            get { return base.battleState as MoveActor; }
        }
        #endregion

        #region Constructors
        public MoveActorRenderer(BattleState battleState)
            : base(battleState)
        {
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
        }

        public override void BeforeDraw(Renderer renderer)
        {
        }

        public override void AfterDraw(Renderer renderer)
        {
            battleState.Battle.DrawArrowOverPartyMember(battleState.Actor, Color.White, renderer);
        }
        #endregion
    }
}
