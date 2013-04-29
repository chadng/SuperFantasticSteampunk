﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Spine;
#endregion

namespace SuperFantasticSteampunk
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SkeletonRenderer skeletonRenderer;
        private Entity e;
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ResourceManager.Initialize(Content, GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            skeletonRenderer = new SkeletonRenderer(GraphicsDevice);

            new Scene();
            Scene.AddEntity(e = new Entity("spineboy", 500, 500));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Pause())
                Exit();
            if (Input.Y()) e.Y -= 10;
            if (Input.A()) e.Y += 10;
            if (Input.X()) e.X -= 10;
            if (Input.B()) e.X += 10;
            Scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Scene.Draw(skeletonRenderer);

            base.Draw(gameTime);
        }
    }
}
