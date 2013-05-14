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
        #region Constants
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
        #endregion

        #region Static Properties
        public static Random Random { get; private set; }
        #endregion

        #region Static Constructors
        static Game1()
        {
            Random = new Random();
        }
        #endregion

        #region Instance Fields
        private GraphicsDeviceManager graphics;
        private Renderer renderer;
        #endregion

        #region Constructors
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Instance Methods
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Input.Initialize(Content);
            ResourceManager.Initialize(Content, GraphicsDevice);
            renderer = new Renderer(GraphicsDevice);
            renderer.SpriteFont = ResourceManager.GetSpriteFont("verdana10");

            Party playerParty = new Party();
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("player1")));
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("player1")));
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("player1")));
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("player1")));

            //new Battle(playerParty, enemyParty);
            new Overworld(playerParty);
        }

        protected override void UnloadContent()
        {
            ResourceManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Pause())
            {
                Logger.Log("Game exited");
                Exit();
            }

            Scene.UpdateCurrent(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Scene.DrawCurrent(renderer);
            renderer.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
