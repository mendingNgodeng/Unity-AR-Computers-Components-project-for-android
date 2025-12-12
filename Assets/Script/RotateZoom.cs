using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZoom : MonoBehaviour
{
    private Vector3 initialScale;
    private float zoomSpeed = 0.005f;
    private float rotationSpeed = 0.2f;

    private bool isTouchingModel = false;
    private Touch touch;
    private Vector2 previousTouchPos;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Deteksi sentuhan ke objek hanya saat touch mulai
            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        isTouchingModel = true;
                        previousTouchPos = touch.position;
                    }
                }
            }

            // Putar objek saat dragging jika sedang menyentuh model BISA PLEASWEEE!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (touch.phase == TouchPhase.Moved && isTouchingModel)
            {
                Vector2 delta = touch.position - previousTouchPos;
                float rotationY = -delta.x * rotationSpeed;
                transform.Rotate(0, rotationY, 0, Space.World);
                previousTouchPos = touch.position;
            }

            // Reset flag ketika touch selesai
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouchingModel = false;
            }
        }

        // Zoom masih pakai dua jari
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

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
