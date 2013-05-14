using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    enum EncounterState { Pending, Won, Lost }

    class Encounter : OverworldState
    {
        #region Instance Fields
        Party enemyParty;
        Entity primaryEnemyPartyMemberEntity;
        #endregion

        #region Instance Properties
        public EncounterState State { get; set; }
        #endregion

        #region Constructors
        public Encounter(Overworld overworld, Party enemyParty)
            : base(overworld)
        {
            this.enemyParty = enemyParty;
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            primaryEnemyPartyMemberEntity = enemyParty.PrimaryPartyMember.OverworldEntity;
            State = EncounterState.Pending;
            new Battle(Overworld.PlayerParty, enemyParty, this);
        }

        public override void Finish()
        {
            if (State == EncounterState.Won)
            {
                Overworld.EnemyParties.Remove(enemyParty);
                Overworld.RemoveEntity(primaryEnemyPartyMemberEntity);
            }
            else if (State == EncounterState.Lost)
            {
                //TODO: Lose game
            }
            PopState();
        }

        public override void Update(GameTime gameTime)
        {
            if (State != EncounterState.Pending)
                Finish();
        }
        #endregion
    }
}
