using System;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    public class Game1 : Game
    {
        #region Constants
        public static readonly Color SkyColor = new Color(140, 197, 217);
        public static readonly Color GrassColor = new Color(190, 255, 153);
        public static readonly Vector2 ReferenceScreenSize = new Vector2(1920, 1080);
        #endregion
        
        #region Static Fields
        private static Game1 instance;
        #endregion

        #region Static Properties
        public static Random Random { get; private set; }
        public static Color BackgroundColor { get; set; }
        public static Vector2 ScreenSize { get; private set; }

        public static Vector2 ScreenScaleFactor
        {
            get { return ScreenSize / ReferenceScreenSize; }
        }
        #endregion

        #region Static Constructors
        static Game1()
        {
            Random = new Random();
            BackgroundColor = SkyColor;
        }
        #endregion

        #region Static Methods
        public static void ExitGame()
        {
            Logger.Log("Game exited");
            instance.Exit();
        }
        #endregion

        #region Instance Fields
        private Settings settings;
        private GraphicsDeviceManager graphics;
        private Renderer renderer;
        private bool skipNextUpdate;
        #endregion

        #region Constructors
        public Game1()
            : base()
        {
            instance = this;
            Content.RootDirectory = "Content";
            settings = new Settings("Settings.txt", Content);
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = settings.GetSetting<bool>("fullscreen");
            ScreenSize = new Vector2(settings.GetSetting<int>("screen_width"), settings.GetSetting<int>("screen_height"));
            graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            IsFixedTimeStep = false;
            skipNextUpdate = false;
        }
        #endregion

        #region Instance Methods
        protected override void Initialize()
        {
            base.Initialize();

#if WINDOWS
            try
            {
                OpenTK.NativeWindow window = typeof(OpenTKGameWindow).GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(((OpenTKGameWindow)Window)) as OpenTK.NativeWindow;
                if (window != null)
                    window.Move += (object sender, EventArgs e) => skipNextUpdate = true;
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
#endif
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
            playerParty.AddPartyMember(new PartyMember(ResourceManager.GetPartyMemberData("medic")));

            Overworld overworld = new Overworld(playerParty, ResourceManager.GetNewArea("forest"));

            Clock.Init(12);
        }

        protected override void UnloadContent()
        {
            ResourceManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (skipNextUpdate)
            {
                skipNextUpdate = false;
                return;
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
