using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SuperFantasticSteampunk.BattleStates
{
    using InventoryItemList = List<KeyValuePair<string, int>>;

    class Think : BattleState
    {
        #region Instance Fields
        private InputButtonDelayer inputButtonDelayer;
        #endregion

        #region Constructors
        public Think(Battle battle)
            : base(battle)
        {
            inputButtonDelayer = new InputButtonDelayer(new Dictionary<InputButton, InputButtonDelayer.KeyPressEventhandler> {
                { InputButton.Up, buttonUpPressed },
                { InputButton.Down, buttonDownPressed },
                { InputButton.A, buttonAPressed },
                { InputButton.B, buttonBPressed }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            //TODO: Set up data structures for storing position data and move selections
        }

        public override void Finish()
        {
            ChangeState(new Act(battle));
        }

        public override void Update(GameTime gameTime)
        {
            //TODO: Allow player to select from a menu to reposition and choose moves for party
            inputButtonDelayer.Update(gameTime);
        }

        private void buttonUpPressed()
        {
            System.Console.WriteLine("Up");
        }

        private void buttonDownPressed()
        {
            System.Console.WriteLine("Down");
        }

        private void buttonAPressed()
        {
            System.Console.WriteLine("A");
        }

        private void buttonBPressed()
        {
            System.Console.WriteLine("B");
        }
        #endregion
    }
}
