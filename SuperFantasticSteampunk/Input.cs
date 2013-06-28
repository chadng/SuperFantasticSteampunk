using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SuperFantasticSteampunk
{
    enum InputButton { Up, Down, Left, Right, AltUp, AltDown, AltLeft, AltRight, A, B, X, Y, Pause, LeftTrigger }

    static class Input
    {
        #region Types
        private delegate bool buttonCheckMethod();
        #endregion

        #region Constants
        private const float thumbStickDeadZone = 0.6f;
        private const float triggerDeadZone = 0.2f;
        #endregion

        #region Static Fields
        private static Dictionary<InputButton, buttonCheckMethod> buttonCheckBindings;
        private static KeyboardState keyboardState;
        private static GamePadState gamePadState;
        #endregion

        #region Static Properties
        public static Dictionary<InputButton, Keys> KeyboardMapping { get; private set; }
        public static bool GamePadUsedLast { get; private set; }
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
            buttonCheckBindings.Add(InputButton.LeftTrigger, LeftTrigger);
            buttonCheckBindings.Add(InputButton.Pause, Pause);

            KeyboardMapping = new Dictionary<InputButton, Keys>();
        }
        #endregion

        #region Static Methods
        public static void Initialize(ContentManager contentManager)
        {
            foreach (string line in File.ReadAllLines(contentManager.RootDirectory + "/KeyboardMapping.txt"))
            {
                string[] parts = line.Split(':');
                KeyboardMapping.Add((InputButton)ResourceManager.ParseItemData<InputButton>(parts[0]), (Keys)ResourceManager.ParseItemData<Keys>(parts[1]));
            }
            GamePadUsedLast = false;
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

        public static bool AnyActionButton()
        {
            return A() || B() || X() || Y();
        }

        public static bool Up()
        {
            if (keyboardUp())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadUp())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool Down()
        {
            if (keyboardDown())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadDown())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool Left()
        {
            if (keyboardLeft())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadLeft())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool Right()
        {
            if (keyboardRight())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadRight())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool AltUp()
        {
            if (keyboardAltUp())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadAltUp())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool AltDown()
        {
            if (keyboardAltDown())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadAltDown())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool AltLeft()
        {
            if (keyboardAltLeft())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadAltLeft())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool AltRight()
        {
            if (keyboardAltRight())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadAltRight())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool A()
        {
            if (keyboardA())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadA())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool B()
        {
            if (keyboardB())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadB())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool X()
        {
            if (keyboardX())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadX())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool Y()
        {
            if (keyboardY())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadY())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        public static bool LeftTrigger()
        {
            return LeftTriggerAmount() > 0.0f;
        }

        public static float LeftTriggerAmount()
        {
            float result = keyboardLeftTriggerAmount();
            if (result != 0.0f)
                GamePadUsedLast = false;
            else
            {
                result = gamePadLeftTriggerAmount();
                if (result != 0.0f)
                    GamePadUsedLast = true;
            }
            return result;
        }

        public static bool Pause()
        {
            if (keyboardPause())
            {
                GamePadUsedLast = false;
                return true;
            }
            if (gamePadPause())
            {
                GamePadUsedLast = true;
                return true;
            }
            return false;
        }

        private static bool keyboardUp()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Up]);
        }

        private static bool keyboardDown()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Down]);
        }

        private static bool keyboardLeft()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Left]);
        }

        private static bool keyboardRight()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Right]);
        }

        private static bool keyboardAltUp()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.AltUp]);
        }

        private static bool keyboardAltDown()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.AltDown]);
        }

        private static bool keyboardAltLeft()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.AltLeft]);
        }

        private static bool keyboardAltRight()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.AltRight]);
        }

        private static bool keyboardA()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.A]);
        }

        private static bool keyboardB()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.B]);
        }

        private static bool keyboardX()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.X]);
        }

        private static bool keyboardY()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Y]);
        }

        private static float keyboardLeftTriggerAmount()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.LeftTrigger]) ? 1.0f : 0.0f;
        }

        private static bool keyboardPause()
        {
            return keyboardState.IsKeyDown(KeyboardMapping[InputButton.Pause]);
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

        private static float gamePadLeftTriggerAmount()
        {
            if (gamePadState.IsButtonDown(Buttons.LeftShoulder))
                return 1.0f;
            if (gamePadState.Triggers.Left > triggerDeadZone)
                return (gamePadState.Triggers.Left - triggerDeadZone) / (1.0f - triggerDeadZone);
            return 0.0f;
        }

        private static bool gamePadPause()
        {
            return gamePadState.IsButtonDown(Buttons.Back) || gamePadState.IsButtonDown(Buttons.Start);
        }
        #endregion
    }
}
