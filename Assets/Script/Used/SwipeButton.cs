using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;

public class SwipeButton : MonoBehaviour
{
    public static SwipeButton activeTarget;

    public GameObject[] modelVariants;
    public string[] modelDescriptions;
    public TextMeshProUGUI descriptionText;
    public float fadeDuration = 0.3f;

    private int currentIndex = 0;
    private bool isTargetTracked = false;

    private ObserverBehaviour observer;

    // Store original scale per model
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        CacheOriginalScales();
        ShowModelInstant(0);
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
                activeTarget = this;
                ShowModelInstant(currentIndex);
                Debug.Log($"[{gameObject.name}] Target Tracked and Activated");
            }
        }
        else
        {
            if (activeTarget == this)
            {
                activeTarget = null;
                HideAllModels();
                Debug.Log($"[{gameObject.name}] Target Lost and Deactivated");
            }
        }
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
        if (!isTargetTracked) return;
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
            Debug.Log($"[{gameObject.name}] Switched to model {currentIndex}: {modelDescriptions[currentIndex]}");
            yield return StartCoroutine(FadeText(1f));
        }

        // Reset model's local transform but restore correct scale
        // ResetModelTransform(nextModel);
    }

    void SetModelVisibility(GameObject model, bool visible)
    {
        if (model != null) model.SetActive(visible);
    }

    // void ResetModelTransform(GameObject model)
    // {
    //     model.transform.localPosition = Vector3.zero;
    //     model.transform.localRotation = Quaternion.identity;

    //     if (originalScales.ContainsKey(model))
    //         model.transform.localScale = originalScales[model]; // restore original scale
    // }

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

    void ShowModelInstant(int index)
    {
        for (int i = 0; i < modelVariants.Length; i++)
            modelVariants[i].SetActive(i == index);

        if (descriptionText != null && index < modelDescriptions.Length)
        {
            descriptionText.text = modelDescriptions[index];
            Color color = descriptionText.color;
            color.a = 1f;
            descriptionText.color = color;

            // ResetModelTransform(modelVariants[index]);
            Debug.Log($"[{gameObject.name}] Instant show: {modelDescriptions[index]}");
        }
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

    void CacheOriginalScales()
    {
        foreach (var model in modelVariants)
        {
            if (model != null && !originalScales.ContainsKey(model))
            {
                originalScales[model] = model.transform.localScale;
            }
        }
    }
}

