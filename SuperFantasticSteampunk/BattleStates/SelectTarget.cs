using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class SelectTarget : BattleState
    {
        #region Instance Fields
        ThinkAction thinkAction;
        #endregion

        #region Constructors
        public SelectTarget(Battle battle, ThinkAction thinkAction)
            : base(battle)
        {
            this.thinkAction = thinkAction;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void Finish()
        {
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
