using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SuperFantasticSteampunk
{
    enum InputButton { Up, Down, Left, Right, A, B, X, Y, Pause }

    static class Input
    {
        #region Types
        private delegate bool buttonCheckMethod();
        #endregion

        #region Constants
        private const float thumbStickDeadZone = 0.6f;
        #endregion

        #region Static Fields
        private static Dictionary<InputButton, buttonCheckMethod> buttonCheckBindings;
        private static Dictionary<InputButton, Keys> keyboardMapping;
        #endregion

        #region Static Constructors
        static Input()
        {
            buttonCheckBindings = new Dictionary<InputButton, buttonCheckMethod>();
            buttonCheckBindings.Add(InputButton.Up, Up);
            buttonCheckBindings.Add(InputButton.Down, Down);
            buttonCheckBindings.Add(InputButton.Left, Left);
            buttonCheckBindings.Add(InputButton.Right, Right);
            buttonCheckBindings.Add(InputButton.A, A);
            buttonCheckBindings.Add(InputButton.B, B);
            buttonCheckBindings.Add(InputButton.X, X);
            buttonCheckBindings.Add(InputButton.Y, Y);
            buttonCheckBindings.Add(InputButton.Pause, Pause);

            keyboardMapping = new Dictionary<InputButton, Keys>();
        }
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager)
        {
            foreach (string line in File.ReadAllLines(contentManager.RootDirectory + "/KeyboardMapping.txt"))
            {
                string[] parts = line.Split(':');
                keyboardMapping.Add((InputButton)ResourceManager.ParseItemData<InputButton>(parts[0]), (Keys)ResourceManager.ParseItemData<Keys>(parts[1]));
            }
        }

        public static bool ButtonPressed(InputButton button)
        {
            return buttonCheckBindings[button]();
        }

        public static bool Up()
        {
            return keyboardUp() || gamePadUp();
        }

        public static bool Down()
        {
            return keyboardDown() || gamePadDown();
        }

        public static bool Left()
        {
            return keyboardLeft() || gamePadLeft();
        }

        public static bool Right()
        {
            return keyboardRight() || gamePadRight();
        }

        public static bool A()
        {
            return keyboardA() || gamePadA();
        }

        public static bool B()
        {
            return keyboardB() || gamePadB();
        }

        public static bool X()
        {
            return keyboardX() || gamePadX();
        }

        public static bool Y()
        {
            return keyboardY() || gamePadY();
        }

        public static bool Pause()
        {
            return keyboardPause() || gamePadPause();
        }

        private static bool keyboardUp()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Up]);
        }

        private static bool keyboardDown()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Down]);
        }

        private static bool keyboardLeft()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Left]);
        }

        private static bool keyboardRight()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Right]);
        }

        private static bool keyboardA()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.A]);
        }

        private static bool keyboardB()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.B]);
        }

        private static bool keyboardX()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.X]);
        }

        private static bool keyboardY()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Y]);
        }

        private static bool keyboardPause()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Pause]);
        }

        private static bool gamePadUp()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadUp) || gamePadState.ThumbSticks.Left.Y > thumbStickDeadZone;
        }

        private static bool gamePadDown()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadDown) || gamePadState.ThumbSticks.Left.Y < -thumbStickDeadZone;
        }

        private static bool gamePadLeft()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadLeft) || gamePadState.ThumbSticks.Left.X < -thumbStickDeadZone;
        }

        private static bool gamePadRight()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.ThumbSticks.Left.X > thumbStickDeadZone;
        }

        private static bool gamePadA()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.A);
        }

        private static bool gamePadB()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.B);
        }

        private static bool gamePadX()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.X);
        }

        private static bool gamePadY()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.Y);
        }

        private static bool gamePadPause()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.Back) || gamePadState.IsButtonDown(Buttons.Start);
        }
        #endregion
    }
}
