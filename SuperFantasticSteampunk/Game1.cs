using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    public class Game1 : Game
    {
        #region Constants
        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;
        public static readonly Color SkyColor = new Color(140, 197, 217);
        public static readonly Color GrassColor = new Color(190, 255, 153);
        #endregion

        #region Static Properties
        public static Random Random { get; private set; }
        public static Color BackgroundColor { get; set; }
        #endregion

        #region Static Constructors
        static Game1()
        {
            Random = new Random();
            BackgroundColor = SkyColor;
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

            Overworld overworld = new Overworld(playerParty, ResourceManager.GetNewArea("forest"));

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
            GraphicsDevice.Clear(new Color(Clock.GetCurrentColor().ToVector4() * BackgroundColor.ToVector4()));

            Scene.DrawCurrent(renderer);
            renderer.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
