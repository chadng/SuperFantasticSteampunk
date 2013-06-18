using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Run : NpcMover
    {
        #region Constructors
        public Run(Party party, Party partyToAvoid, Overworld overworld)
            : base(party, overworld)
        {
        }
        #endregion

        #region Instance Methods
        public override void Finish()
        {
        }
        #endregion
    }
}
