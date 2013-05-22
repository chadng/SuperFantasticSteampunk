using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Follow : NpcMover
    {
        #region Instance Fields
        private Entity entityToFollow;
        #endregion

        #region Constructors
        public Follow(Entity entity, Entity entityToFollow, Overworld overworld)
            : base(entity, overworld)
        {
            this.entityToFollow = entityToFollow;
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
        }
        #endregion
    }
}
