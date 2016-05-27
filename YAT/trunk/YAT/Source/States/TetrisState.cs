using System;

using Axiom;
using Axiom.Input;

namespace YAT
{
    public abstract class TetrisState : State
    {
        #region Fields
        protected Game game;
        protected KeyState escapeKey;
        protected KeyState enterKey;
        protected KeyState downKey;
        protected KeyState upKey;
        protected KeyState leftKey;
        protected KeyState rightKey;
        protected KeyState spaceKey;
        protected KeyState zKey;
        protected KeyState xKey;
        #endregion

        #region Constructors
        public TetrisState()
        {
            escapeKey = new KeyState( Axiom.Input.KeyCodes.Escape );
            enterKey = new KeyState( Axiom.Input.KeyCodes.Enter );
            downKey = new KeyState( Axiom.Input.KeyCodes.Down );
            upKey = new KeyState( Axiom.Input.KeyCodes.Up );
            leftKey = new KeyState( Axiom.Input.KeyCodes.Left );
            rightKey = new KeyState( Axiom.Input.KeyCodes.Right );
            spaceKey = new KeyState( Axiom.Input.KeyCodes.Space );
            zKey = new KeyState(Axiom.Input.KeyCodes.Z);
            xKey = new KeyState(Axiom.Input.KeyCodes.X);
        }
        #endregion

        #region Public Methods
        // State overrides
        public override void Initialize()
        {
            game = Game.Instance;
        }
        public override void KeyPressed( KeyEventArgs e )
        {
            // Handle keys common to all game states
            switch ( e.Key )
            {
                case Axiom.Input.KeyCodes.F12:
                    {
                        TetrisApplication.Instance.showDebugOverlay = !TetrisApplication.Instance.showDebugOverlay;
                    }
                    break;
            }
        }
        public override void FrameStarted( float dt )
        {
            // Update game
            game.Update( dt );
        }
        public override void FrameEnded( float dt )
        {
            TetrisApplication.Instance.UpdateStats();
        }
        #endregion

    }
}