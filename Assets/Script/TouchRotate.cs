using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchRotate : MonoBehaviour
{
      private Camera arCam;
    private Transform grabbedObject;
    private Vector3 offset;
    private float zCoord;
    private float rotationSpeed = 0.3f;
    private float pinchZoomSpeed = 0.005f;
    private Vector3 initialScale;

    void Start()
    {
        arCam = Camera.main;
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = arCam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    grabbedObject = hit.transform;
                    zCoord = arCam.WorldToScreenPoint(grabbedObject.position).z;
                    Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, zCoord);
                    offset = grabbedObject.position - arCam.ScreenToWorldPoint(touchPos);
                }
            }
            else if (touch.phase == TouchPhase.Moved && grabbedObject != null)
            {
                // MOVE ONLY (no rotation)
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, zCoord);
                Vector3 worldPos = arCam.ScreenToWorldPoint(touchPos) + offset;
                grabbedObject.position = worldPos;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                grabbedObject = null;
            }
        }
        else if (Input.touchCount == 2 && grabbedObject != null)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // ROTATE IN PLACE - please work
            Vector2 prevDir = (touchZero.position - touchZero.deltaPosition) - (touchOne.position - touchOne.deltaPosition);
            Vector2 currDir = touchZero.position - touchOne.position;

            float angle = Vector2.SignedAngle(prevDir, currDir);
            grabbedObject.Rotate(Vector3.up, -angle * rotationSpeed, Space.World);

            // ZOOM this is working!  yay!
            float prevMag = (touchZero.position - touchZero.deltaPosition - (touchOne.position - touchOne.deltaPosition)).magnitude;
            float currMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMag = currMag - prevMag;

            Vector3 scale = grabbedObject.localScale + Vector3.one * deltaMag * pinchZoomSpeed;
            scale = Vector3.Max(scale, initialScale * 0.5f); // Min zoom
            scale = Vector3.Min(scale, initialScale * 2f);   // Max zoom

            grabbedObject.localScale = scale;
        }
    }
}
