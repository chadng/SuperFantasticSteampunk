using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class Play : OverworldState
    {
        #region Constructors
        public Play(Overworld overworld)
            : base(overworld)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
        }

        public override void Finish()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.A())
            {
                Party enemyParty = new Party();
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                new Battle(Overworld.PlayerParty, enemyParty);
            }
        }
        #endregion
    }
}
