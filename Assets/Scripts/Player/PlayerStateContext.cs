using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Thiruvizha.Player.States;
using Thiruvizha.Grids;
using UnityEngine.Tilemaps;

namespace Thiruvizha.Player
{
    public class PlayerStateContext : MonoBehaviour
    {
        //Properties
        public Camera cam;
        public float ZoomMin { get; private set; } = 2;
        public float ZoomMax { get; private set; } = 8;

        //States
        private PlayerLookState lookState;
        private PlayerMoveState moveState;

        //Runtime Changable Values
        private PlayerBaseState currentState;
        public BaseBuilding selectedBuilding;

        //For states
        public LayerMask ignoreBuildings;


        public enum PlayerState
        {
            notThere,
            look,
            move
        }


        private void Awake()
        {
            lookState = new PlayerLookState();
            moveState = new PlayerMoveState();
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
