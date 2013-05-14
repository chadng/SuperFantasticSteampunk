using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class Play : OverworldState
    {
        #region Constants
        private const float movementVelocity = 300.0f;
        #endregion

        #region Constructors
        public Play(Overworld overworld)
            : base(overworld)
        {
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
        }

        public override void Finish()
        {
        }

        public override void Update(GameTime gameTime)
        {
            handlePlayerMovement(gameTime);
            handleEnemyCollisions();
        }

        private void handlePlayerMovement(GameTime gameTime)
        {
            Vector2 velocity = Vector2.Zero;
            if (Input.Up())
                velocity.Y -= 1.0f;
            if (Input.Down())
                velocity.Y += 1.0f;
            if (Input.Left())
                velocity.X -= 1.0f;
            if (Input.Right())
                velocity.X += 1.0f;

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
                velocity *= movementVelocity;
            }

            Entity entity = Overworld.PlayerParty.PrimaryPartyMember.OverworldEntity;
            limitVelocityForCollisionWithWall(entity, gameTime, ref velocity);
            entity.Velocity = velocity;
        }

        private void handleEnemyCollisions()
        {
            foreach (Party party in Overworld.EnemyParties)
            {
                Entity playerEntity = Overworld.PlayerParty.PrimaryPartyMember.OverworldEntity;
                Entity enemyEntity = party.PrimaryPartyMember.OverworldEntity;

                if (playerEntity.CollidesWith(enemyEntity))
                {
                    PushState(new Encounter(Overworld, party));
                    break;
                }
            }
        }

        private void limitVelocityForCollisionWithWall(Entity entity, GameTime gameTime, ref Vector2 velocity)
        {
            if (!collidesWithWall(entity, gameTime, velocity))
                return;

            Vector2 newVelocity = new Vector2(velocity.X, 0.0f);
            if (!collidesWithWall(entity, gameTime, newVelocity))
            {
                velocity.Y = 0.0f;
                return;
            }

            newVelocity = new Vector2(0.0f, velocity.Y);
            if (!collidesWithWall(entity, gameTime, newVelocity))
            {
                velocity.X = 0.0f;
                return;
            }

            velocity = Vector2.Zero;
        }

        private bool collidesWithWall(Entity entity, GameTime gameTime, Vector2 velocity)
        {
            Vector2 newPosition = entity.Position + (velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Rectangle bounds = entity.GetBoundingBox();
            bounds.X += (int)(newPosition.X - entity.Position.X);
            bounds.Y += (int)(newPosition.Y - entity.Position.Y);

            Point topLeftTileCoord = new Point(bounds.Left / Map.TileSize, bounds.Top / Map.TileSize);
            Point bottomRightTileCoord = new Point(bounds.Right / Map.TileSize, bounds.Bottom / Map.TileSize);

            for (int x = topLeftTileCoord.X; x <= bottomRightTileCoord.X; ++x)
            {
                for (int y = topLeftTileCoord.Y; y <= bottomRightTileCoord.Y; ++y)
                {
                    if (Overworld.Map.CollisionMap[x, y])
                        return true;
                }
            }

            return false;
        }
        #endregion
    }
}
