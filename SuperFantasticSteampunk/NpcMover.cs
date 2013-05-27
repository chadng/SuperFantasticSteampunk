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
        public virtual void Update(GameTime gameTime)
        {
            overworld.SetEntitySpriteAnimationForVelocity(entity);
        }

        public abstract void Finish();

        protected bool entityCollidesWithOtherEnemy()
        {
            foreach (Party party in overworld.EnemyParties)
            {
                Entity otherEntity = party.PrimaryPartyMember.OverworldEntity;
                if (entity != otherEntity && entity.CollidesWith(otherEntity))
                    return true;
            }
            return false;
        }

        protected bool entityCollidesWithScenery()
        {
            foreach (Scenery scenery in overworld.SceneryEntities)
            {
                if (entity.CollidesWith(scenery))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
