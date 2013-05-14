using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Camera
    {
        #region Instance Properties
        public Entity Target { get; set; }
        public Vector2 Size { get; private set; }
        public Vector2 Position { get; set; }
        #endregion

        #region Constructors
        public Camera(Vector2 size)
        {
            Target = null;
            Size = size;
            Position = Vector2.Zero;
        }
        #endregion

        #region Instance Methods
        public Vector2 TranslateVector(Vector2 vector)
        {
            return vector - (Position - (Size / 2.0f));
        }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle(
                (int)(Position.X - (Size.X / 2.0f)),
                (int)(Position.Y - (Size.Y / 2.0f)),
                (int)Size.X,
                (int)Size.Y
            );
        }

        public void Update(GameTime gameTime)
        {
            if (Target != null)
                Position = Target.Position;

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
            }
        }
        #endregion
    }
}
