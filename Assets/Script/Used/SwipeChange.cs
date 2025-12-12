using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;

public class SwipeChange : MonoBehaviour
{
      public static SwipeChange activeTarget;

    public GameObject[] modelVariants;
    public string[] modelDescriptions;
    public TextMeshProUGUI descriptionText;
    public float fadeDuration = 0.3f;

    private int currentIndex = 0;
    private bool isTargetTracked = false;

    private ObserverBehaviour observer;

    void Start()
    {
        ForceShowCurrentModel();  // Show first model safely
        PositionTextAtTop();

        observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnDestroy()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
       isTargetTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

    if (isTargetTracked)
    {
        if (activeTarget != this)
        {
            // Deactivate the previous target if it's not this
            if (activeTarget != null)
            {
                activeTarget.HideAllModels();
            }

            activeTarget = this;
            ForceShowCurrentModel();
            Debug.Log($"[{gameObject.name}] Target Tracked and Activated");
        }
    }
    else
    {
        if (activeTarget == this)
        {
            HideAllModels(); // Ensure this model is hidden when target is lost
            activeTarget = null;
            Debug.Log($"[{gameObject.name}] Target Lost and Deactivated");
        }
    }
    }

    IEnumerator DelayedShowModel()
    {
        yield return new WaitForSeconds(0.1f);  // Slight delay allows Vuforia to stabilize
        ForceShowCurrentModel();
    }

    void HideAllModels()
    {
        foreach (var model in modelVariants)
        {
            if (model != null) model.SetActive(false);
        }

        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }

    public void SwitchModelExternally(int direction)
    {
        if (!isTargetTracked || modelVariants.Length == 0) return;

        if (!modelVariants[currentIndex].activeInHierarchy)
            modelVariants[currentIndex].SetActive(true);

        StartCoroutine(SwitchModel(direction));
    }

    IEnumerator SwitchModel(int direction)
    {
        GameObject currentModel = modelVariants[currentIndex];
        SetModelVisibility(currentModel, false);

        if (descriptionText != null)
            yield return StartCoroutine(FadeText(0f));

        currentIndex = (currentIndex + direction + modelVariants.Length) % modelVariants.Length;

        GameObject nextModel = modelVariants[currentIndex];
        SetModelVisibility(nextModel, true);

        if (descriptionText != null)
        {
            descriptionText.text = modelDescriptions[currentIndex];
            yield return StartCoroutine(FadeText(1f));
        }

        ResetModelTransform(nextModel);
        Debug.Log($"Switched to model index: {currentIndex}, active: {nextModel.activeSelf}, name: {nextModel.name}");
    }

    void SetModelVisibility(GameObject model, bool visible)
    {
        if (model != null) model.SetActive(visible);
    }

    void ResetModelTransform(GameObject model)
    {
        model.transform.localPosition = Vector3.zero;
        // model.transform.localRotation = Quaternion.identity;
        // Commented out: Don't override scale
        // model.transform.localScale = Vector3.one;
    }

    IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = descriptionText.color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            Color color = descriptionText.color;
            color.a = alpha;
            descriptionText.color = color;
            yield return null;
        }

        Color finalColor = descriptionText.color;
        finalColor.a = targetAlpha;
        descriptionText.color = finalColor;
    }
// Old one
    // void ForceShowCurrentModel()
    // {
    //     for (int i = 0; i < modelVariants.Length; i++)
    //         modelVariants[i].SetActive(i == currentIndex);

    //     if (descriptionText != null && currentIndex < modelDescriptions.Length)
    //     {
    //         descriptionText.text = modelDescriptions[currentIndex];
    //         Color color = descriptionText.color;
    //         color.a = 1f;
    //         descriptionText.color = color;

    //         ResetModelTransform(modelVariants[currentIndex]);

    //         Debug.Log($"Force show model: {modelVariants[currentIndex].name} | Text: {modelDescriptions[currentIndex]}");
    //     }
    // }

// New one
    void ForceShowCurrentModel()
{
    // Reactivate current model and deactivate others
    for (int i = 0; i < modelVariants.Length; i++)
    {
        if (modelVariants[i] != null)
            modelVariants[i].SetActive(i == currentIndex);
    }

    // Ensure current model is parented to the image target
    GameObject currentModel = modelVariants[currentIndex];
    if (currentModel != null && currentModel.transform.parent != this.transform)
    {
        currentModel.transform.SetParent(this.transform, false);
    }

    // Optional: Only set transform if needed — you can remove this line if you don't want transform reset
    // currentModel.transform.localPosition = Vector3.zero;
    // currentModel.transform.localRotation = Quaternion.identity;
    // currentModel.transform.localScale = yourDesiredScale; // use only if needed

    // Update text if available
    if (descriptionText != null && currentIndex < modelDescriptions.Length)
    {
        descriptionText.text = modelDescriptions[currentIndex];
        Color color = descriptionText.color;
        color.a = 1f;
        descriptionText.color = color;
    }

    Debug.Log($"Force show model: {currentModel.name} | Text: {modelDescriptions[currentIndex]}");
}


    void PositionTextAtTop()
    {
        if (descriptionText != null)
        {
            RectTransform rect = descriptionText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -50f);
        }
    }
}

