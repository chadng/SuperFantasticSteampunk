using Microsoft.Xna.Framework;
using SuperFantasticSteampunk.NpcMovers;

namespace SuperFantasticSteampunk
{
    abstract class NpcMover
    {
        #region Static Methods
        public static NpcMover Create(OverworldMovementType movementType, Party party, Party playerParty, Overworld overworld)
        {
            switch (movementType)
            {
            case OverworldMovementType.Wander: return new Wander(party, overworld);
            case OverworldMovementType.Follow: return new Follow(party, playerParty, overworld);
            case OverworldMovementType.Run: return new Run(party, playerParty, overworld);
            default: return null;
            }
        }
        #endregion

        #region Instance Properties
        protected Party party { get; private set; }
        protected Overworld overworld { get; private set; }
        #endregion

        #region Constructors
        public NpcMover(Party party, Overworld overworld)
        {
            this.party = party;
            this.overworld = overworld;
        }
        #endregion

        #region Instance Methods
        public virtual void Update(Delta delta)
        {
            overworld.SetEntityAnimationForVelocity(party.PrimaryPartyMember.OverworldEntity);
        }

        public abstract void Finish();

        protected bool entityCollidesWithOtherEnemy()
        {
            Entity entity = party.PrimaryPartyMember.OverworldEntity;
            foreach (Party enemyParty in overworld.EnemyParties)
            {
                Entity otherEntity = enemyParty.PrimaryPartyMember.OverworldEntity;
                if (entity != otherEntity && entity.CollidesWith(otherEntity))
                    return true;
            }
            return false;
        }

        protected bool entityCollidesWithScenery()
        {
            Entity entity = party.PrimaryPartyMember.OverworldEntity;
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
