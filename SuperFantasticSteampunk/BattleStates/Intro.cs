using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    class Intro : BattleState
    {
        #region Constructors
        public Intro(Battle battle)
            : base(battle)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Intro sequence setup, party member positioning
        }

        public override void Finish()
        {
            ChangeState(new Think(Battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Intro sequence update
            Finish();
        }
        #endregion
    }
}
