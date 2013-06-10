using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Run : NpcMover
    {

        #region Constructors
        public Run(Entity entity, Entity entityToAvoid, Overworld overworld)
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
