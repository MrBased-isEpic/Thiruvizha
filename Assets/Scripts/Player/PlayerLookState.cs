using UnityEngine;
using Thiruvizha.Grids;
using EnTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Thiruvizha.Player.States
{
    public class PlayerLookState : PlayerBaseState
    {
        private Vector3 touchPos;
        Vector3 direction;

        private float holdDuration;
        private float pickTimer;

        private bool isPickingUp = false;

        private void Zoom(float increment, PlayerStateContext player)
        {
            player.cam.orthographicSize += increment;
            player.cam.orthographicSize = Mathf.Clamp(player.cam.orthographicSize, player.ZoomMin, player.ZoomMax);
        }

        public override void EnterState(PlayerStateContext player)
        {

        }
        public override void UpdateState(PlayerStateContext player)
        {
            if (EnTouch.Touch.activeFingers.Count > 0)
            {   
                if (EnTouch.Touch.activeFingers.Count == 2)
                {

                    EnTouch.Touch touch1 = EnTouch.Touch.activeTouches[0];
                    EnTouch.Touch touch2 = EnTouch.Touch.activeTouches[1];

                    Vector2 touch1lastPos = touch1.screenPosition - touch1.delta;
                    Vector2 touch2lastPos = touch2.screenPosition - touch2.delta;

                    float distTouch = (touch1lastPos - touch2lastPos).magnitude;
                    float currentDistTouch = (touch1.screenPosition - touch2.screenPosition).magnitude;


                    float difference = currentDistTouch - distTouch;
                    Zoom(-difference * 0.01f, player);
                }
                else
                {
                    EnTouch.Touch touch = EnTouch.Touch.activeTouches[0];

                    switch (touch.phase)
                    {
                        case UnityEngine.InputSystem.TouchPhase.Began:
                            holdDuration = 0;
                            pickTimer = 0;
                            isPickingUp = false;
                            touchPos = player.cam.ScreenToWorldPoint(touch.screenPosition);
                            break;
                        case UnityEngine.InputSystem.TouchPhase.Moved:
                            direction = touchPos - player.cam.ScreenToWorldPoint(touch.screenPosition);
                            player.transform.position += direction;
                            break;

                        case UnityEngine.InputSystem.TouchPhase.Stationary:
                            Debug.Log("touch is stationary updating");
                            if (!isPickingUp)
                            {
                                if (holdDuration > player.pickDetectionTime)
                                {
                                    Ray ray = player.cam.ScreenPointToRay(touch.screenPosition);
                                    RaycastHit hit;
                                    if (Physics.Raycast(ray, out hit, 100))
                                    {
                                        BaseTile tile = hit.collider.gameObject.GetComponentInParent<BaseTile>();
                                        if (tile as BaseBuilding != null)
                                        {
                                            player.selectedBuilding = tile as BaseBuilding;
                                            player.selectedBuilding.TurnOnArrow();
                                            isPickingUp = true;
                                        }
                                    }
                                    holdDuration = 0;
                                }
                                else
                                {
                                    Debug.Log("hold timer updating");
                                    holdDuration += Time.deltaTime;
                                }
                            }
                            else
                            {
                                if (pickTimer > player.pickTime)
                                {
                                    isPickingUp = false;
                                    player.SwitchState(PlayerStateContext.PlayerState.move);    
                                }
                                else
                                {
                                    pickTimer += Time.deltaTime;
                                    Debug.Log("pick timer updating");
                                    player.selectedBuilding.SetArrowFill(pickTimer,player.pickTime);
                                }

                            }

                            break;
                            case UnityEngine.InputSystem.TouchPhase.Ended:
                                if (player.selectedBuilding != null)
                                {
                                    player.selectedBuilding.TurnOffArrow();
                                }
                                break;

                    }
                }

            }
            else
            {
                if (direction != Vector3.zero)
                {
                    direction = Vector3.Lerp(direction, Vector3.zero, .2f);
                    player.transform.position += direction;
                    //Debug.Log(direction);
                }
            }
        }
        public override void EndState(PlayerStateContext player)
        {
            holdDuration = 0;
            pickTimer = 0;
            isPickingUp = false;
        }
    }
}
