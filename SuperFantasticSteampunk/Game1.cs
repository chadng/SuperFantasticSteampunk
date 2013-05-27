using System;
using Microsoft.Xna.Framework;

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
            IsFixedTimeStep = false;
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
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("marksman")));
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("medic")));
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("marksman")));

            Overworld overworld = new Overworld(playerParty);
            for (int i = 2; i < 5; ++i)
            {
                Party enemyParty = new Party();
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                enemyParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("enemy")));
                overworld.AddEnemyParty(enemyParty, new Vector2(100.0f * i));
            }
            Scene.AddEntity(new Scenery(ResourceManager.GetNewSprite("arrow_down"), new Vector2(100.0f, 400.0f)));

            Clock.Init(12);
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
            Color backgroundColor = new Color(140, 197, 217);
            GraphicsDevice.Clear(new Color(Clock.GetCurrentColor().ToVector4() * backgroundColor.ToVector4()));

            Scene.DrawCurrent(renderer);
            renderer.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
