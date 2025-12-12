using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchInteract : MonoBehaviour
    {
    private Vector3 initialScale;
    private float rotationSpeed = 0.3f;
    private float zoomSpeed = 0.005f;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // ROTATION
            Vector2 prevDir = (touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition);
            Vector2 currDir = touch0.position - touch1.position;
            float angle = Vector2.SignedAngle(prevDir, currDir);
            transform.Rotate(Vector3.up, -angle * rotationSpeed, Space.World);

            // ZOOM
            float prevMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;
            float delta = currMag - prevMag;

            Vector3 scale = transform.localScale + Vector3.one * delta * zoomSpeed;
            scale = Vector3.Max(scale, initialScale * 0.5f); // Min zoom
            scale = Vector3.Min(scale, initialScale * 2f);   // Max zoom
            transform.localScale = scale;
        }
    }
}
