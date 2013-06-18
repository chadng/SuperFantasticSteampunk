using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.NpcMovers
{
    class Wander : NpcMover
    {
        #region Constants
        private const float thinkDelay = 1.0f;
        private const float movementVelocity = 250.0f;
        #endregion

        #region Instance Fields
        private float thinkTime;
        private Vector2 destination;
        #endregion

        #region Instance Properties
        private Entity entity
        {
            get { return party.PrimaryPartyMember.OverworldEntity; }
        }
        #endregion

        #region Constructors
        public Wander(Party party, Overworld overworld)
            : base(party, overworld)
        {
            thinkTime = 0.0f;
            destination = entity.Position;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            if (destination == entity.Position)
            {
                thinkTime += delta.Time;
                if (thinkTime > thinkDelay)
                {
                    chooseNewDestination(delta);
                    thinkTime = 0.0f;
                }
            }
            else if (entityPositionCloseEnoughToDestination(delta))
            {
                destination = entity.Position;
                entity.Velocity = Vector2.Zero;
            }

            base.Update(delta);
        }

        public override void Finish()
        {
            entity.Velocity = Vector2.Zero;
        }

        private void chooseNewDestination(Delta delta)
        {
            const float maxDistance = 200.0f;
            Vector2 originalPosition = entity.Position;

            do
            {
                entity.Position = originalPosition;

                double distance = Game1.Random.NextDouble() * maxDistance;
                double angle = Game1.Random.NextDouble() * MathHelper.TwoPi;
                Vector2 relativePosition = new Vector2((float)(Math.Cos(angle) * distance), (float)(Math.Sin(angle) * distance));
                destination = entity.Position + relativePosition;
                relativePosition.Normalize();
                entity.Velocity = relativePosition * movementVelocity;

                entity.Position += entity.Velocity * delta.Time;
            } while (entityPositionCloseEnoughToDestination(delta));

            entity.Position = originalPosition;
        }

        private bool entityPositionCloseEnoughToDestination(Delta delta)
        {
            const float tolerance = 10.0f;
            bool result = entity.Position.X > destination.X - tolerance && entity.Position.X < destination.X + tolerance && entity.Position.Y > destination.Y - tolerance && entity.Position.Y < destination.Y + tolerance;
            if (result)
                return result;

            Vector2 previousPosition = entity.Position;
            entity.Position += entity.Velocity * delta.Time;
            result = entity.CollidesWith(overworld.Map) || entityCollidesWithOtherEnemy() || entityCollidesWithScenery();
            entity.Position = previousPosition;

            return result;
        }
        #endregion
    }
}
