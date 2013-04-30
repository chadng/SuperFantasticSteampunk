#region Using Statements
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

            new Battle(new Party().Tap(p => p.Add(new PartyMember())), new Party());
            Scene.AddEntity(new Entity("spineboy", 500, 500));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Pause())
                Exit();

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
