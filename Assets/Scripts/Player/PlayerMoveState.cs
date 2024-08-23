using System.Collections;
using System.Collections.Generic;
using Thiruvizha.Grids;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ENTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Thiruvizha.Player.States
{
    public class PlayerMoveState : PlayerBaseState
    {
        public override void EnterState(PlayerStateContext player)
        {
            Debug.Log("entered move state");
        }
        public override void UpdateState(PlayerStateContext player)
        {
            if(ENTouch.Touch.activeTouches.Count > 0)
            {
                ENTouch.Touch touch = ENTouch.Touch.activeTouches[0];
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        Ray ray = player.cam.ScreenPointToRay(touch.screenPosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray,out hit,100, player.ignoreBuildings))
                            player.selectedBuilding.transform.position = hit.point;
                        if(!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))
                        {
                            Debug.Log("Can't Place Building here");
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Ended:
                        GridManager.instance.PlaceBaseBuilding(player.selectedBuilding);
                        player.SwitchState(PlayerStateContext.PlayerState.look);
                        break;
                }
            }
        }

        public override void EndState(PlayerStateContext player)
        {
            Debug.Log("Exited MoveState");
        }
    }
}
