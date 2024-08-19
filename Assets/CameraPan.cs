using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPan : MonoBehaviour
{
    public Camera cam;

    public float ZoomMin;
    public float ZoomMax;

    private Vector3 touchPos;
    Vector3 direction;

    private void Awake()
    {

    }

    private void Zoom(float increment)
    {
        cam.orthographicSize += increment;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize,ZoomMin,ZoomMax);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                float distTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
                float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

                float difference = currentDistTouch - distTouch;
                Zoom(-difference * 0.01f);
            }
            else
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchPos = cam.ScreenToWorldPoint(touch.position);
                        break;
                    case TouchPhase.Moved:
                        direction = touchPos - cam.ScreenToWorldPoint(touch.position);
                        transform.position += direction;
                        break;
                    case TouchPhase.Ended:
                        break;
                }
            }

        }
        else
        {
            if (direction != Vector3.zero)
            {
                direction = Vector3.Lerp(direction, Vector3.zero, .2f);
                transform.position += direction;
                //Debug.Log(direction);
            }
        }
    }
}
