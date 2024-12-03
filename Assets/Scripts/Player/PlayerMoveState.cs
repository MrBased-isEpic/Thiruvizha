using Thiruvizha.Grids;
using UnityEngine;
using ENTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Thiruvizha.Player.States
{
    public class PlayerMoveState : PlayerBaseState
    {
        private Vector3 prevPosition;
        ENTouch.Touch touch;

        bool wasRotated = false;

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

            player.OnMoveStateEntered?.Invoke();
            player.selectedBuilding.TurnOffArrow();

            //player.selectedBuilding.OnRotation += () => 
            //{
            //    wasRotated = true;
            //    while(!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))
            //    {
            //        player.selectedBuilding.RotateClockWise();
            //    }

            //    player.selectedBuilding.lastValidOrientation = player.selectedBuilding.transform.rotation.eulerAngles.y;
            //    GridManager.instance.PlaceBaseBuilding(player.selectedBuilding);

            //    Debug.Log("Orientation Stored");
            //    Debug.Log(player.selectedBuilding.lastValidOrientation);
            //};

            //player.selectedBuilding.TurnOnRotator();
            //player.selectedBuilding.lastValidOrientation = player.selectedBuilding.transform.rotation.eulerAngles.y;
        }
        public override void UpdateState(PlayerStateContext player)
        {
            if (ENTouch.Touch.activeTouches.Count <= 0) return;

            touch = ENTouch.Touch.activeTouches[0];
            //if(touch.isTap)
            //{
            //    Ray ray = player.cam.ScreenPointToRay(touch.screenPosition);
            //    RaycastHit hit;

            //    if(wasRotated)
            //    {
            //        wasRotated = false;
            //        return;
            //    }

            //    if (Physics.Raycast(ray, out hit, 100))
            //    {
            //        if(!hit.transform.GetComponentInParent<BaseBuilding>() == player.selectedBuilding)
            //        {
            //            if (!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))//If Tile is not Valid
            //            {
            //                GridManager.instance.PlaceBaseBuilding(player.selectedBuilding, new Vector3Int(player.selectedBuilding.gridPosition.x, 0, player.selectedBuilding.gridPosition.y));
            //                player.selectedBuilding.RotateY(player.selectedBuilding.lastValidOrientation);
            //            }
            //            player.SwitchState(PlayerStateContext.PlayerState.look);
            //        }
            //    }
            //    return;
            //}

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
                            //Debug.Log("Can't Place Building here");
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Ended:

                        if (!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))//If Tile is not Valid
                        {
                            GridManager.instance.PlaceBaseBuilding(player.selectedBuilding, new Vector3Int(player.selectedBuilding.gridPosition.x, 0, player.selectedBuilding.gridPosition.y));
                            //player.selectedBuilding.RotateY(player.selectedBuilding.lastValidOrientation);
                            //Debug.Log("Position was not valid");

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
            player.OnMoveStateExited?.Invoke();
            //player.selectedBuilding.TurnOffRotator();
            //player.selectedBuilding.OnRotation -= () => 
            //{
            //    wasRotated = true;
            //    while (!GridManager.instance.CheckBuildingPositionisValid(player.selectedBuilding))
            //    {
            //        player.selectedBuilding.RotateClockWise();
            //    }

            //    Debug.Log("Orientation Stored");
            //    player.selectedBuilding.lastValidOrientation = player.selectedBuilding.transform.rotation.eulerAngles.y;
            //    Debug.Log(player.selectedBuilding.lastValidOrientation);
            //    GridManager.instance.PlaceBaseBuilding(player.selectedBuilding);
            //};
            Debug.Log("Exited MoveState");
        }
    }
}
