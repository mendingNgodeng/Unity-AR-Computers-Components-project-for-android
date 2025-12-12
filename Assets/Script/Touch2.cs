using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch2 : MonoBehaviour
{
    private Vector2 touchStartPos;
    private float rotationSpeed = 0.2f;
    private float pinchZoomSpeed = 0.1f;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotX = touch.deltaPosition.y * rotationSpeed;
                float rotY = -touch.deltaPosition.x * rotationSpeed;

                transform.Rotate(Camera.main.transform.up, rotY, Space.World);
                transform.Rotate(Camera.main.transform.right, rotX, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            // Pinch Zoom
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float prevDistance = (touchZero.position - touchZero.deltaPosition - (touchOne.position - touchOne.deltaPosition)).magnitude;
            float currentDistance = (touchZero.position - touchOne.position).magnitude;

            float delta = currentDistance - prevDistance;

            Vector3 scaleChange = transform.localScale + Vector3.one * (delta * pinchZoomSpeed);
            scaleChange = Vector3.Max(scaleChange, initialScale * 0.5f); // Min scale
            scaleChange = Vector3.Min(scaleChange, initialScale * 2f);   // Max scale

            transform.localScale = scaleChange;
        }
    }
}
