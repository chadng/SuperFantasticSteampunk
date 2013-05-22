using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Run : NpcMover
    {
        #region Instance Fields
        private Entity entityToAvoid;
        #endregion

        #region Constructors
        public Run(Entity entity, Entity entityToAvoid, Overworld overworld)
            : base(entity, overworld)
        {
            this.entityToAvoid = entityToAvoid;
        }
        #endregion

        #region Instance Methods
        public override void Update(GameTime gameTime)
        {
        }
        #endregion
    }
}
