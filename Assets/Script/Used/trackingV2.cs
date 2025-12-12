using System.Collections;
using UnityEngine;
using Vuforia;

public class trackingV2 : MonoBehaviour
{
    private DefaultObserverEventHandler observerHandler;
    private Transform content;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;

    private bool isTracking = false;

    [Header("Smoothing Settings")]
    [Range(0.01f, 1f)] public float positionSmoothSpeed = 0.15f;
    [Range(0.01f, 1f)] public float rotationSmoothSpeed = 0.1f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        observerHandler = GetComponent<DefaultObserverEventHandler>();
        content = transform.GetChild(0);

        if (content)
        {
            initialLocalPosition = content.localPosition;
            initialLocalRotation = content.localRotation;
            initialLocalScale = content.localScale;

            targetPosition = content.localPosition;
            targetRotation = content.localRotation;
        }

        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;
    }

    void OnDestroy()
    {
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.World.OnObserverCreated -= OnObserverCreated;
        }
    }

    void OnObserverCreated(ObserverBehaviour observer)
    {
        if (observer.TargetName == gameObject.name)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        isTracking = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

        if (content)
        {

               if(isTracking)
        {
            content.localPosition = initialLocalPosition;
            content.localRotation = initialLocalRotation;
            content.gameObject.SetActive(isTracking);
        }
        }
    }

    void Update()
    {
        if (!isTracking || content == null)
            return;

        // Target transform relative to the parent (not world position!)
        targetPosition = Vector3.Lerp(content.localPosition, initialLocalPosition, positionSmoothSpeed);
        targetRotation = Quaternion.Slerp(content.localRotation, initialLocalRotation, rotationSmoothSpeed);

        content.localPosition = targetPosition;
        content.localRotation = targetRotation;
    }

    public void ResetObject()
    {
        if (content)
        {
            content.localPosition = initialLocalPosition;
            content.localRotation = initialLocalRotation;
            content.localScale = initialLocalScale;
        }
    }
}
