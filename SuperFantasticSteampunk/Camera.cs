using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Camera
    {
        #region Instance Properties
        public Entity Target { get; set; }
        public Vector2 Size { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 TargetPosition { get; set; }
        public Vector2 TargetScale { get; set; }
        #endregion

        #region Constructors
        public Camera(Vector2 size)
        {
            Target = null;
            Size = size;
            Position = Vector2.Zero;
            Scale = Vector2.One;
            TargetScale = Vector2.One;
            TargetPosition = Position;
        }
        #endregion

        #region Instance Methods
        public Vector2 TranslateVector(Vector2 vector)
        {
            return ((vector - Position) * Scale) + (Size / 2.0f);
        }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle(
                (int)((Position.X * Scale.X) - (Size.X / 2.0f)),
                (int)((Position.Y * Scale.Y) - (Size.Y / 2.0f)),
                (int)Size.X,
                (int)Size.Y
            );
        }

        public void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                Position = new Vector2((float)Math.Round(Target.Position.X), (float)Math.Round(Target.Position.Y));
                TargetPosition = Position;
            }

            Overworld overworld = Scene.Current as Overworld;
            if (overworld != null)
            {
                Rectangle boundingBox = GetBoundingBox();
                Vector2 newPosition = Position;

                if (boundingBox.Left < 0)
                    newPosition.X -= boundingBox.Left;
                if (boundingBox.Top < 0)
                    newPosition.Y -= boundingBox.Top;
                if (boundingBox.Right > overworld.Map.Width)
                    newPosition.X -= boundingBox.Right - overworld.Map.Width;
                if (boundingBox.Bottom > overworld.Map.Height)
                    newPosition.Y -= boundingBox.Bottom - overworld.Map.Height;

                Position = newPosition;
                TargetPosition = Position;
            }

            Position = updateVectorToTarget(Position, TargetPosition, gameTime);
            Scale = updateVectorToTarget(Scale, TargetScale, gameTime);
        }

        private Vector2 updateVectorToTarget(Vector2 vector, Vector2 target, GameTime gameTime)
        {
            if (vector == target)
                return vector;

            float newX = moveFloatTowardsValue(vector.X, target.X, gameTime);
            float newY = moveFloatTowardsValue(vector.Y, target.Y, gameTime);

            return new Vector2(newX, newY);
        }

        private float moveFloatTowardsValue(float value, float targetValue, GameTime gameTime)
        {
            return value + ((targetValue - value) * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        #endregion
    }
}
