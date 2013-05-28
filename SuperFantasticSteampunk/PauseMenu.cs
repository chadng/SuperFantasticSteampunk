using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class PauseMenu : Scene
    {
        #region Instance Fields
        private TextureData pixelTextureData;
        private List<string> options;
        private int currentOptionIndex;
        private InputButtonListener inputButtonListener;
        #endregion

        #region Constructors
        public PauseMenu()
        {
            pixelTextureData = ResourceManager.GetTextureData("white_pixel");
            options = new List<string> {
                "Resume",
                "Exit"
            };
            currentOptionIndex = 0;

            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Down, new ButtonEventHandlers(down: nextOption) },
                { InputButton.Up, new ButtonEventHandlers(down: previousOption) },
                { InputButton.A, new ButtonEventHandlers(down: selectOption) }
            });
        }
        #endregion

        #region Instance Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);
            inputButtonListener.Update(gameTime);
        }

        protected override void draw(Renderer renderer)
        {
            sceneStack.Pop();
            Scene.DrawCurrent(renderer);
            sceneStack.Push(this);

            const float overlayAlpha = 0.7f;
            renderer.Draw(pixelTextureData, Vector2.Zero, Color.Black * overlayAlpha, 0.0f, Game1.ScreenSize);

            for (int i = 0; i < options.Count; ++i)
            {
                Color color = currentOptionIndex == i ? Color.Blue : Color.White;
                renderer.DrawText(options[i], new Vector2(200.0f, 200.0f + (i * 50.0f)), color, 0.0f, Vector2.Zero, Vector2.One);
            }
        }

        protected override void pause()
        {
            Finish();
        }

        private void nextOption()
        {
            if (++currentOptionIndex >= options.Count)
                currentOptionIndex = options.Count - 1;
        }

        private void previousOption()
        {
            if (--currentOptionIndex < 0)
                currentOptionIndex = 0;
        }

        private void selectOption()
        {
            switch (options[currentOptionIndex])
            {
            case "Resume": Finish(); break;
            case "Exit": Game1.ExitGame(); break;
            }
        }
        #endregion
    }
}
