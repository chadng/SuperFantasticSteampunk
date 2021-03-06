﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.OverworldStates
{
    class Play : OverworldState
    {
        #region Constants
        private const float movementVelocity = 300.0f;
        #endregion

        #region Instance Fields
        private List<NpcMover> npcMovers;
        #endregion

        #region Constructors
        public Play(Overworld overworld)
            : base(overworld)
        {
            npcMovers = new List<NpcMover>();
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            finalizeNpcMovers();
            initNpcMovers();
            OverworldStateRenderer = new PlayRenderer(this);
        }

        public override void Finish()
        {
            base.Finish();
            finalizeNpcMovers();
        }

        public override void Pause()
        {
            finalizeNpcMovers();
            base.Pause();
        }

        public override void Resume(OverworldState previousOverworldState)
        {
            base.Resume(previousOverworldState);
            initNpcMovers();
        }

        public override void Update(Delta delta)
        {
            handlePlayerMovement(delta);
            foreach (NpcMover npcMover in npcMovers)
                npcMover.Update(delta);

            if (!Overworld.PlayerIsInvincible)
                handleEnemyCollisions();

            Overworld.UpdateInvincibility(delta);
        }

        private void initNpcMovers()
        {
            foreach (Party party in Overworld.EnemyParties)
            {
                npcMovers.Add(NpcMover.Create(party.PrimaryPartyMember.Data.OverworldMovementType, party, Overworld.PlayerParty, Overworld));
            }
        }

        private void finalizeNpcMovers()
        {
            foreach (NpcMover npcMover in npcMovers)
                npcMover.Finish();
            npcMovers.Clear();
        }

        private void handlePlayerMovement(Delta delta)
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
            handleWallCollisions(entity, delta, ref velocity);
            handleSceneryCollisions(entity, delta, ref velocity);
            entity.Velocity = velocity;

            Overworld.SetEntityAnimationForVelocity(entity);
        }

        private void handleEnemyCollisions()
        {
            Entity playerEntity = Overworld.PlayerParty.PrimaryPartyMember.OverworldEntity;
            foreach (Party party in Overworld.EnemyParties)
            {
                Entity enemyEntity = party.PrimaryPartyMember.OverworldEntity;

                if (playerEntity.CollidesWith(enemyEntity))
                {
                    playerEntity.Velocity = Vector2.Zero;
                    PushState(new EncounterIntro(Overworld, party));
                    break;
                }
            }
        }

        private void handleWallCollisions(Entity entity, Delta delta, ref Vector2 velocity)
        {
            Vector2 newPosition = entity.Position + (velocity * delta.Time);
            Rectangle bounds = entity.GetBoundingBoxAt(newPosition);

            Point topLeftTileCoord = new Point(bounds.Left / Map.TileSize, bounds.Top / Map.TileSize);
            Point bottomRightTileCoord = new Point(bounds.Right / Map.TileSize, bounds.Bottom / Map.TileSize);

            for (int x = topLeftTileCoord.X; x <= bottomRightTileCoord.X; ++x)
            {
                for (int y = topLeftTileCoord.Y; y <= bottomRightTileCoord.Y; ++y)
                {
                    if (Overworld.Map.CollisionMap[x, y])
                    {
                        Rectangle tileRect = new Rectangle(x * Map.TileSize, y * Map.TileSize, Map.TileSize, Map.TileSize);
                        adjustVelocityForRectangleCollision(entity, delta.Time, tileRect, ref velocity);
                    }
                }
            }
        }

        private void handleSceneryCollisions(Entity entity, Delta delta, ref Vector2 velocity)
        {
            foreach (Scenery scenery in Overworld.SceneryEntities)
                adjustVelocityForRectangleCollision(entity, delta.Time, scenery.GetBoundingBox(), ref velocity);
        }

        private void adjustVelocityForRectangleCollision(Entity entity, float deltaT, Rectangle rect, ref Vector2 velocity)
        {
            Vector2 intersectionAmount;
            Vector2 tempPosition = entity.Position + (velocity * deltaT);
            if (rect.EIntersects(entity.GetBoundingBoxAt(tempPosition)))
            {
                tempPosition = entity.Position;
                tempPosition.X += velocity.X * deltaT;
                if (entity.GetBoundingBoxAt(tempPosition).Intersects(rect, out intersectionAmount))
                {
                    bool positive = velocity.X >= 0;
                    velocity.X -= intersectionAmount.X / deltaT;
                    if ((positive && velocity.X < 0) || (!positive && velocity.X > 0))
                        velocity.X = 0;
                }

                tempPosition = entity.Position;
                tempPosition.Y += velocity.Y * deltaT;
                if (entity.GetBoundingBoxAt(tempPosition).Intersects(rect, out intersectionAmount))
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
