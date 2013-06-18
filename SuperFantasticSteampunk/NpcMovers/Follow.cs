using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Follow : NpcMover
    {
        #region Constructors
        public Follow(Party party, Party partyToFollow, Overworld overworld)
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
