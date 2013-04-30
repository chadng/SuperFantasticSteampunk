using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk
{
    class InputButtonDelayer
    {
        #region Types
        public delegate void KeyPressEventhandler();
        #endregion

        #region Constants
        private const float keyPressDelay = 0.2f;
        #endregion

        #region Instance Fields
        private Dictionary<InputButton, KeyPressEventhandler> eventBindings;
        private float timeSinceKeyPressed;
        #endregion

        #region Constructors
        public InputButtonDelayer(Dictionary<InputButton, KeyPressEventhandler> eventBindings)
        {
            this.eventBindings = eventBindings;
            timeSinceKeyPressed = 0;
        }

        public void Update(GameTime gameTime)
        {
            timeSinceKeyPressed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceKeyPressed >= keyPressDelay)
            {
                foreach (KeyValuePair<InputButton, KeyPressEventhandler> eventBinding in eventBindings)
                {
                    if (Input.ButtonPressed(eventBinding.Key))
                    {
                        timeSinceKeyPressed = 0.0f;
                        eventBinding.Value();
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
