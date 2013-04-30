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
            throw new System.NotImplementedException();
        }

        public override void Finish()
        {
            if (battle.PlayerParty.Count == 0)
                ChangeState(new Lose(battle));
            else if (battle.EnemyParty.Count == 0)
                ChangeState(new Win(battle));
            else
                ChangeState(new Think(battle));
        }

        public override void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
