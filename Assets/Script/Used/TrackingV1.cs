using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackingV1 : MonoBehaviour
{
       private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    void Start()
    {
        // Get the observer (image target) this script is attached to
        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        // Save original transform
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
    }

    private void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        isTracking = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;

        if (isTracking)
        {
            // You can add logic here if something should happen when tracking is found
            Debug.Log("Tracking FOUND");
        }
        else
        {
            // Optionally hide or reset object when tracking is lost
            Debug.Log("Tracking LOST");
        }
    }

    void Update()
    {
        // Optional: Smooth any behaviors while tracking
        if (!isTracking)
        {
            // If you want to hide or freeze movement when not tracked
            // gameObject.SetActive(false);
        }
        else
        {
            // gameObject.SetActive(true);
        }
    }
}
