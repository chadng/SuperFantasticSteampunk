using System.Collections.Generic;

namespace SuperFantasticSteampunk.OverworldStates
{
    class Menu : OverworldState
    {
        #region Constants
        public static readonly string[] MenuOptions = { "Next battle", "Restart game" };
        private const int NEXT_BATTLE = 0;
        private const int RESTART_GAME = 1;
        #endregion

        #region Instance Fields
        private InputButtonListener inputButtonListener;
        #endregion

        #region Instance Properties
        public int CurrentMenuOptionIndex { get; private set; }

        public override bool RenderWorld
        {
            get { return false; }
        }
        #endregion

        #region Constructors
        public Menu(Overworld overworld)
            : base(overworld)
        {
            inputButtonListener = new InputButtonListener(new Dictionary<InputButton, ButtonEventHandlers> {
                { InputButton.Up, new ButtonEventHandlers(down: previousOption) },
                { InputButton.Down, new ButtonEventHandlers(down: nextOption) },
                { InputButton.A, new ButtonEventHandlers(up: selectOption) }
            });
        }
        #endregion

        #region Instance Methods
        public override void Start()
        {
            OverworldStateRenderer = new MenuRenderer(this);
            CurrentMenuOptionIndex = 0;
        }

        public override void Update(Delta delta)
        {
            inputButtonListener.Update(delta);
        }

        private void previousOption()
        {
            if (--CurrentMenuOptionIndex < 0)
                CurrentMenuOptionIndex = 0;
        }

        private void nextOption()
        {
            if (++CurrentMenuOptionIndex >= MenuOptions.Length)
                CurrentMenuOptionIndex = MenuOptions.Length - 1;
        }

        private void selectOption()
        {
            switch (CurrentMenuOptionIndex)
            {
            case NEXT_BATTLE: PushState(new Encounter(Overworld, Overworld.GenerateEnemyParty())); break;
            case RESTART_GAME: Game1.RestartGame(); break;
            default: break;
            }
        }
        #endregion
    }
}
