using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    delegate void ButtonEventHandler();

    struct ButtonEventHandlers
    {
        #region Instance Fields
        public readonly ButtonEventHandler ButtonPress;
        public readonly ButtonEventHandler ButtonDown;
        public readonly ButtonEventHandler ButtonUp;
        #endregion

        #region Constructors
        public ButtonEventHandlers(ButtonEventHandler press = null, ButtonEventHandler down = null, ButtonEventHandler up = null)
        {
            ButtonPress = press;
            ButtonDown = down;
            ButtonUp = up;
        }
        #endregion
    }

    class InputButtonListener
    {
        #region Instance Fields
        private Dictionary<InputButton, ButtonEventHandlers> eventBindings;
        private Dictionary<InputButton, bool> buttonPressedLastFrame;
        #endregion

        #region Constructors
        public InputButtonListener(Dictionary<InputButton, ButtonEventHandlers> eventBindings)
        {
            this.eventBindings = eventBindings;
            buttonPressedLastFrame = new Dictionary<InputButton, bool>(eventBindings.Count);
            foreach (InputButton button in eventBindings.Keys)
                buttonPressedLastFrame.Add(button, false);
        }

        public void Update(Delta delta)
        {
            foreach (InputButton button in eventBindings.Keys)
            {
                bool thisButtonPressedThisFrame = Input.ButtonPressed(button);
                bool thisButtonPressedLastFrame = buttonPressedLastFrame[button];
                ButtonEventHandler eventHandler = null;

                if (thisButtonPressedThisFrame)
                {
                    eventHandler = thisButtonPressedLastFrame ? eventBindings[button].ButtonPress : eventBindings[button].ButtonDown;
                    buttonPressedLastFrame[button] = true;
                }
                else
                {
                    if (thisButtonPressedLastFrame)
                        eventHandler = eventBindings[button].ButtonUp;
                    buttonPressedLastFrame[button] = false;
                }

                if (eventHandler != null)
                    eventHandler();
            }
        }
        #endregion
    }
}
