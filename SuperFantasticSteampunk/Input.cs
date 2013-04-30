using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        }
        #endregion

        #region Static Methods
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
            return keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);
        }

        private static bool gamePadUp()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadUp) || gamePadState.ThumbSticks.Left.Y > thumbStickDeadZone;
        }

        private static bool keyboardDown()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S);
        }

        private static bool gamePadDown()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadDown) || gamePadState.ThumbSticks.Left.Y < -thumbStickDeadZone;
        }

        private static bool keyboardLeft()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A);
        }

        private static bool gamePadLeft()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadLeft) || gamePadState.ThumbSticks.Left.X < -thumbStickDeadZone;
        }

        private static bool keyboardRight()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D);
        }

        private static bool gamePadRight()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.ThumbSticks.Left.X > thumbStickDeadZone;
        }

        private static bool keyboardA()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.Z);
        }

        private static bool gamePadA()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.A);
        }

        private static bool keyboardB()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.X);
        }

        private static bool gamePadB()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.B);
        }

        private static bool keyboardX()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.C);
        }

        private static bool gamePadX()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.X);
        }

        private static bool keyboardY()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.V);
        }

        private static bool gamePadY()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.Y);
        }

        private static bool keyboardPause()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.Escape);
        }

        private static bool gamePadPause()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            return gamePadState.IsButtonDown(Buttons.Back) || gamePadState.IsButtonDown(Buttons.Start);
        }
        #endregion
    }
}
