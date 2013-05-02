using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class EndTurn : BattleState
    {
        #region Constructors
        public EndTurn(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Handle any status effects
        }

        public override void Finish()
        {
            if (Battle.PlayerParty.Count == 0)
                ChangeState(new Lose(Battle));
            else if (Battle.EnemyParty.Count == 0)
                ChangeState(new Win(Battle));
            else
                ChangeState(new Think(Battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Update entities for any status effects
            Finish();
        }
        #endregion
    }
}
