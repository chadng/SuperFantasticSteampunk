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
            handleWallCollision(entity, gameTime, ref velocity);
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

        private void handleWallCollision(Entity entity, GameTime gameTime, ref Vector2 velocity)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newPosition = entity.Position + (velocity * deltaT);
            Rectangle bounds = entity.GetBoundingBoxAt(newPosition);

            Point topLeftTileCoord = new Point(bounds.Left / Map.TileSize, bounds.Top / Map.TileSize);
            Point bottomRightTileCoord = new Point(bounds.Right / Map.TileSize, bounds.Bottom / Map.TileSize);

            for (int x = topLeftTileCoord.X; x <= bottomRightTileCoord.X; ++x)
            {
                for (int y = topLeftTileCoord.Y; y <= bottomRightTileCoord.Y; ++y)
                {
                    if (Overworld.Map.CollisionMap[x, y])
                        handleWallTileCollision(entity, deltaT, x, y, ref velocity);
                }
            }
        }

        private void handleWallTileCollision(Entity entity, float deltaT, int tileX, int tileY, ref Vector2 velocity)
        {
            Rectangle tileRect = new Rectangle(tileX * Map.TileSize, tileY * Map.TileSize, Map.TileSize, Map.TileSize);

            Vector2 intersectionAmount;
            Vector2 tempPosition = entity.Position + (velocity * deltaT);
            if (tileRect.EIntersects(entity.GetBoundingBoxAt(tempPosition)))
            {
                tempPosition = entity.Position;
                tempPosition.X += velocity.X * deltaT;
                if (entity.GetBoundingBoxAt(tempPosition).Intersects(tileRect, out intersectionAmount))
                {
                    bool positive = velocity.X >= 0;
                    velocity.X -= intersectionAmount.X / deltaT;
                    if ((positive && velocity.X < 0) || (!positive && velocity.X > 0))
                        velocity.X = 0;
                }

                tempPosition = entity.Position;
                tempPosition.Y += velocity.Y * deltaT;
                if (entity.GetBoundingBoxAt(tempPosition).Intersects(tileRect, out intersectionAmount))
                {
                    bool positive = velocity.Y >= 0;
                    velocity.Y -= intersectionAmount.Y / deltaT;
                    if ((positive && velocity.Y < 0) || (!positive && velocity.Y > 0))
                        velocity.Y = 0;
                }
            }
        }
        #endregion
    }
}
