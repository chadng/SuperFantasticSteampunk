using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Projectile : Entity
    {
        #region Instance Fields
        private readonly PartyMember actor;
        private readonly PartyMember target;
        private readonly Action<Projectile> collisionCallback;
        private readonly float distance;
        private bool collided;
        #endregion

        #region Constructors
        public Projectile(Sprite sprite, Vector2 position, bool facingRight, PartyMember actor, PartyMember target, float speed, Action<Projectile> collisionCallback)
            : base(sprite, position)
        {
            if (actor == null)
                throw new Exception("PartyMember actor cannot be null");
            if (target == null)
                throw new Exception("PartyMember target cannot be null");
            if (collisionCallback == null)
                throw new Exception("Action<Projectile> collisionCallback cannot be null");
            this.actor = actor;
            this.target = target;
            this.collisionCallback = collisionCallback;

            Vector2 directionVector = target.BattleEntity.GetCenter() - GetCenter();
            distance = directionVector.Length();
            directionVector.Normalize();
            Velocity = speed * directionVector;
            Rotation = (facingRight ? 0.0f : MathHelper.Pi) + (float)Math.Atan2(directionVector.Y, directionVector.X);

            collided = false;
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);
            if ((target.BattleEntity.GetCenter() - GetCenter()).Length() > distance / 2.0f)
                DepthOverride = actor.BattleEntity.Position.Y - 0.1f;
            else
                DepthOverride = target.BattleEntity.Position.Y + 5.0f;
            if (!collided && CollidesWith(target.BattleEntity))
            {
                collisionCallback(this);
                collided = false;
            }
        }
        #endregion
    }
}
