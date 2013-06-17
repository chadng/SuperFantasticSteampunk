using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SuperFantasticSteampunk
{
    enum InputButton { Up, Down, Left, Right, AltUp, AltDown, AltLeft, AltRight, A, B, X, Y, Pause, LeftShoulder }

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
        private static KeyboardState keyboardState;
        private static GamePadState gamePadState;
        #endregion

        #region Static Constructors
        static Input()
        {
            buttonCheckBindings = new Dictionary<InputButton, buttonCheckMethod>();
            buttonCheckBindings.Add(InputButton.Up, Up);
            buttonCheckBindings.Add(InputButton.Down, Down);
            buttonCheckBindings.Add(InputButton.Left, Left);
            buttonCheckBindings.Add(InputButton.Right, Right);
            buttonCheckBindings.Add(InputButton.AltUp, AltUp);
            buttonCheckBindings.Add(InputButton.AltDown, AltDown);
            buttonCheckBindings.Add(InputButton.AltLeft, AltLeft);
            buttonCheckBindings.Add(InputButton.AltRight, AltRight);
            buttonCheckBindings.Add(InputButton.A, A);
            buttonCheckBindings.Add(InputButton.B, B);
            buttonCheckBindings.Add(InputButton.X, X);
            buttonCheckBindings.Add(InputButton.Y, Y);
            buttonCheckBindings.Add(InputButton.LeftShoulder, LeftShoulder);
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

        public static void UpdateInputState()
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
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

        public static bool AltUp()
        {
            return keyboardAltUp() || gamePadAltUp();
        }

        public static bool AltDown()
        {
            return keyboardAltDown() || gamePadAltDown();
        }

        public static bool AltLeft()
        {
            return keyboardAltLeft() || gamePadAltLeft();
        }

        public static bool AltRight()
        {
            return keyboardAltRight() || gamePadAltRight();
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

        public static bool LeftShoulder()
        {
            return keyboardLeftShoulder() || gamePadLeftShoulder();
        }

        public static bool Pause()
        {
            return keyboardPause() || gamePadPause();
        }

        private static bool keyboardUp()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Up]);
        }

        private static bool keyboardDown()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Down]);
        }

        private static bool keyboardLeft()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Left]);
        }

        private static bool keyboardRight()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Right]);
        }

        private static bool keyboardAltUp()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.AltUp]);
        }

        private static bool keyboardAltDown()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.AltDown]);
        }

        private static bool keyboardAltLeft()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.AltLeft]);
        }

        private static bool keyboardAltRight()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.AltRight]);
        }

        private static bool keyboardA()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.A]);
        }

        private static bool keyboardB()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.B]);
        }

        private static bool keyboardX()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.X]);
        }

        private static bool keyboardY()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Y]);
        }

        private static bool keyboardLeftShoulder()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.LeftShoulder]);
        }

        private static bool keyboardPause()
        {
            return keyboardState.IsKeyDown(keyboardMapping[InputButton.Pause]);
        }

        private static bool gamePadUp()
        {
            return gamePadState.IsButtonDown(Buttons.DPadUp) || gamePadState.ThumbSticks.Left.Y > thumbStickDeadZone;
        }

        private static bool gamePadDown()
        {
            return gamePadState.IsButtonDown(Buttons.DPadDown) || gamePadState.ThumbSticks.Left.Y < -thumbStickDeadZone;
        }

        private static bool gamePadLeft()
        {
            return gamePadState.IsButtonDown(Buttons.DPadLeft) || gamePadState.ThumbSticks.Left.X < -thumbStickDeadZone;
        }

        private static bool gamePadRight()
        {
            return gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.ThumbSticks.Left.X > thumbStickDeadZone;
        }

        private static bool gamePadAltUp()
        {
            return gamePadState.ThumbSticks.Right.Y > thumbStickDeadZone;
        }

        private static bool gamePadAltDown()
        {
            return gamePadState.ThumbSticks.Right.Y < -thumbStickDeadZone;
        }

        private static bool gamePadAltLeft()
        {
            return gamePadState.ThumbSticks.Right.X < -thumbStickDeadZone;
        }

        private static bool gamePadAltRight()
        {
            return gamePadState.ThumbSticks.Right.X > thumbStickDeadZone;
        }

        private static bool gamePadA()
        {
            return gamePadState.IsButtonDown(Buttons.A);
        }

        private static bool gamePadB()
        {
            return gamePadState.IsButtonDown(Buttons.B);
        }

        private static bool gamePadX()
        {
            return gamePadState.IsButtonDown(Buttons.X);
        }

        private static bool gamePadY()
        {
            return gamePadState.IsButtonDown(Buttons.Y);
        }

        private static bool gamePadLeftShoulder()
        {
            return gamePadState.IsButtonDown(Buttons.LeftShoulder);
        }

        private static bool gamePadPause()
        {
            return gamePadState.IsButtonDown(Buttons.Back) || gamePadState.IsButtonDown(Buttons.Start);
        }
        #endregion
    }
}
