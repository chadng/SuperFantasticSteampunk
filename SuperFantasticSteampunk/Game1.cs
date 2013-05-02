using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Spine;

namespace SuperFantasticSteampunk
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Renderer renderer;
        
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ResourceManager.Initialize(Content, GraphicsDevice);
            renderer = new Renderer(GraphicsDevice);
            renderer.SpriteFont = ResourceManager.GetSpriteFont("verdana10");

            new Battle(new Party().Tap(p => p.Add(new PartyMember())).Tap(p => p.Add(new PartyMember())), new Party().Tap(p => p.Add(new PartyMember())));
            Scene.AddEntity(new Entity(ResourceManager.GetNewSkeleton("spineboy"), new Vector2(500, 500)));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Pause())
            {
                Logger.Log("Game exited");
                Exit();
            }

            Scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Scene.Draw(renderer);
            renderer.End();

            base.Draw(gameTime);
        }
    }
}
