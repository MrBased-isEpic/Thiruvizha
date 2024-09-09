using System.Collections;
using System.Collections.Generic;
using Thiruvizha.Grids;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using ENTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Thiruvizha.Player.States
{
    public class PlayerMoveState : PlayerBaseState
    {
        private Vector3 prevPosition;
        ENTouch.Touch touch;
        public override void EnterState(PlayerStateContext player)
        {
            Debug.Log("entered move state");
            touch = ENTouch.Touch.activeTouches[0];
            Ray ray = player.cam.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, player.ignoreBuildings))
            {
                prevPosition = hit.point;
            }
        }
        public override void UpdateState(PlayerStateContext player)
        {
            touch = ENTouch.Touch.activeTouches[0];
            if (ENTouch.Touch.activeTouches.Count > 0)
            {       
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Moved:

                        // Take world position of the player's touch and make the building follow it.
                        Ray ray = player.cam.ScreenPointToRay(touch.screenPosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, player.ignoreBuildings))
                        {
                            player.selectedBuilding.transform.position = hit.point;
                        }

                        // Only Check fot Tile validity if the building is moved to another cell.
                        if(GridManager.instance.GetWorldToCellPosition(prevPosition) == GridManager.instance.GetWorldToCellPosition(player.selectedBuilding.transform.position))break;   
                        else prevPosition = player.selectedBuilding.transform.position;

                        if(!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))//If Tile is not Valid
                        {
                            Debug.Log("Can't Place Building here");
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Ended:

                        if (!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))//If Tile is not Valid
                        {
                            Debug.Log("Not gonna place here");
                            GridManager.instance.PlaceBaseBuilding(player.selectedBuilding, new Vector3Int(player.selectedBuilding.gridPosition.x, 0, player.selectedBuilding.gridPosition.y));

                        }
                        else
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
