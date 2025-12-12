using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGrab : MonoBehaviour
{
    //need to have RigidBody 3d in the component
    private Camera arCam;
    private Transform grabbedObject;
    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        arCam = Camera.main;
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
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        grabbedObject = hit.transform;
                        zCoord = arCam.WorldToScreenPoint(grabbedObject.position).z;
                        Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, zCoord);
                        offset = grabbedObject.position - arCam.ScreenToWorldPoint(touchPos);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && grabbedObject != null)
            {
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, zCoord);
                Vector3 worldPos = arCam.ScreenToWorldPoint(touchPos) + offset;
                grabbedObject.position = worldPos;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                grabbedObject = null;
            }
        }
    }
}
