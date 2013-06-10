using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Follow : NpcMover
    {
        #region Constructors
        public Follow(Entity entity, Entity entityToFollow, Overworld overworld)
            : base(entity, overworld)
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
