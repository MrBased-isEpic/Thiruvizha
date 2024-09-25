using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Thiruvizha.Player.States;
using Thiruvizha.Grids;

namespace Thiruvizha.Player
{
    public class PlayerStateContext : MonoBehaviour
    {
        //Properties
        public Camera cam;
        public float ZoomMin { get; private set; } = 2;
        public float ZoomMax { get; private set; } = 8;

        public float pickDetectionTime;
        public float pickTime;


        //States
        private PlayerLookState lookState;
        private PlayerMoveState moveState;
        private PlayerUIState uiState;

        //Runtime Changable Values
        private PlayerBaseState currentState;
        public BaseBuilding selectedBuilding;

        //For states
        public LayerMask ignoreBuildings;
        public LayerMask UI;


        public enum PlayerState
        {
            notThere,
            look,
            move,
            ui
        }


        private void Awake()
        {
            lookState = new PlayerLookState();
            moveState = new PlayerMoveState();
            uiState = new PlayerUIState();
            currentState = lookState;
            EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            currentState.UpdateState(this);
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        public void SwitchState(PlayerState state)
        {
            currentState.EndState(this);

            switch (state)
            {
                case PlayerState.look:
                    currentState = lookState;
                    break;
                case PlayerState.move:
                    currentState = moveState;
                    break;
                case PlayerState.ui:
                    currentState = uiState;
                    break;
            }
            currentState.EnterState(this);
        }

        public PlayerState GetPlayerState()
        {
            switch (currentState)
            {
                case PlayerLookState:
                    return PlayerState.look;
                case PlayerMoveState:
                    return PlayerState.move;
            }
            return PlayerState.notThere;
        }
    }
}
