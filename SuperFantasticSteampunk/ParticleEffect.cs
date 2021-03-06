﻿using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class Particle
    {
        #region Constants
        private const float radialVelocity = MathHelper.Pi;
        #endregion

        #region Instance Fields
        private readonly float lifeTime;
        private float time;
        private Vector2 position;
        private Vector2 velocity;
        private float gravity;
        private float scale;
        private bool rotate;
        private float rotation;
        #endregion

        #region Instance Properties
        public bool Alive
        {
            get { return time < lifeTime; }
        }
        #endregion

        #region Constructors
        public Particle(Vector2 position, float speed, float gravity, float maxScale, float lifeTime, bool rotate)
        {
            time = 0.0f;
            this.position = position;
            this.gravity = gravity;
            this.lifeTime = lifeTime + (float)((Game1.Random.NextDouble() * 0.5) - 0.25);
            scale = (float)(maxScale * (1.0 - (Game1.Random.NextDouble() * 0.4)));
            velocity = new Vector2((float)((Game1.Random.NextDouble() - 0.5) * 2.0), (float)((Game1.Random.NextDouble() - 0.5) * 2.0)) * speed;
            this.rotate = rotate;
            rotation = rotate ? (float)(Game1.Random.NextDouble() * MathHelper.TwoPi) : 0.0f;
        }
        #endregion

        #region Instance Methods
        public void Update(Delta delta)
        {
            time += delta.Time;
            velocity.Y += gravity * delta.Time;
            position += velocity * delta.Time;
            if (rotate)
                rotation += radialVelocity * delta.Time;
        }

        public void Draw(Color tint, Renderer renderer, TextureData textureData)
        {
            float alpha = 1.0f - (time / lifeTime);
            renderer.Draw(textureData, position, new Color(tint.R, tint.G, tint.B, (byte)(255 * alpha)), rotation, new Vector2(scale));
        }
        #endregion
    }

    class ParticleEffect : Entity
    {
        #region Static Methods
        public static void AddExplosion(Vector2 position, Battle battle)
        {
            if (battle == null)
                throw new Exception("Battle cannot be null");

            TextureData cloudTextureData = ResourceManager.GetTextureData("particles/cloud_1");
            Scene.AddEntity(new ParticleEffect(position, Color.Red, 12, cloudTextureData, 800.0f, 500.0f, 0.8f, 0.6f, true));
            Scene.AddEntity(new ParticleEffect(position, Color.Orange, 12, cloudTextureData, 800.0f, 500.0f, 0.8f, 0.6f, true));
            Scene.AddEntity(new ParticleEffect(position, Color.White, 12, cloudTextureData, 800.0f, 500.0f, 0.8f, 0.6f, true));
            battle.Camera.Shake(new Vector2(4.0f), 0.1f);
            battle.SetCameraUpdateDelay(1.0f);
        }

        public static ParticleEffect AddSmokePuff(Vector2 position, Battle battle)
        {
            if (battle == null)
                throw new Exception("Battle cannot be null");
            ParticleEffect particleEffect = new ParticleEffect(position, Color.White, 20, ResourceManager.GetTextureData("particles/cloud_1"), 500.0f, 500.0f, 0.6f, 0.6f, true);
            Scene.AddEntity(particleEffect);
            battle.Camera.Shake(new Vector2(3.0f, 0.0f), 0.1f);
            return particleEffect;
        }
        #endregion

        #region Instance Fields
        TextureData textureData;
        private Particle[] particles;
        private Color tint;
        #endregion

        #region Constructors
        public ParticleEffect(Vector2 position, Color tint, int particleCount, TextureData textureData, float particleSpeed, float particleGravity, float maxScale, float lifeTime, bool rotate)
            : base(position)
        {
            this.textureData = textureData;
            this.tint = tint;
            particles = new Particle[particleCount];
            for (int i = 0; i < particleCount; ++i)
                particles[i] = new Particle(position, particleSpeed, particleGravity, maxScale, lifeTime, rotate);
        }
        #endregion

        #region Instance Methods
        public override void Update(Delta delta)
        {
            base.Update(delta);
            foreach (Particle particle in particles)
            {
                if (particle.Alive)
                    particle.Update(delta);
            }
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Particle particle in particles)
            {
                if (particle.Alive)
                    particle.Draw(tint, renderer, textureData);
            }
        }
        #endregion
    }
}
