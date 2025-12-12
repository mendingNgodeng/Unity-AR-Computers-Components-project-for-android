using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetin2Tap : MonoBehaviour
{
   private Vector3 initialScale;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private float zoomSpeed = 0.005f;
    private float rotationSpeed = 0.2f;

    private bool isTouchingModel = false;
    private Vector2 previousTouchPos;

    private float lastTapTime = 0f;
    private float doubleTapDelay = 0.3f;

    void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // ROTATE
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        isTouchingModel = true;
                        previousTouchPos = touch.position;

                        // Double tap reset
                        if (Time.time - lastTapTime < doubleTapDelay)
                        {
                            ResetTransform();
                        }
                        lastTapTime = Time.time;
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved && isTouchingModel)
            {
                Vector2 delta = touch.position - previousTouchPos;
                float rotationX = delta.y * rotationSpeed;
                float rotationY = -delta.x * rotationSpeed;

                transform.Rotate(Vector3.right, rotationX, Space.World);
                transform.Rotate(Vector3.up, rotationY, Space.World);

                previousTouchPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouchingModel = false;
            }
        }

        // ZOOM
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float prevMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;
            float delta = currMag - prevMag;

            Vector3 scale = transform.localScale + Vector3.one * delta * zoomSpeed;
            scale = Vector3.Max(scale, initialScale * 0.5f);
            scale = Vector3.Min(scale, initialScale * 2f);
            transform.localScale = scale;
        }
    }

    void ResetTransform()
    {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
        transform.localScale = initialScale;
    }
}
