using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.NpcMovers;

namespace SuperFantasticSteampunk
{
    abstract class NpcMover
    {
        #region Static Methods
        public static NpcMover Create(OverworldMovementType movementType, Entity entity, Entity playerEntity, Overworld overworld)
        {
            switch (movementType)
            {
            case OverworldMovementType.Wander: return new Wander(entity, overworld);
            case OverworldMovementType.Follow: return new Follow(entity, playerEntity, overworld);
            case OverworldMovementType.Run: return new Run(entity, playerEntity, overworld);
            default: return null;
            }
        }
        #endregion

        #region Instance Properties
        protected Entity entity { get; private set; }
        protected Overworld overworld { get; private set; }
        #endregion

        #region Constructors
        public NpcMover(Entity entity, Overworld overworld)
        {
            this.entity = entity;
            this.overworld = overworld;
        }
        #endregion

        #region Instance Methods
        public abstract void Update(GameTime gameTime);
        #endregion
    }
}
