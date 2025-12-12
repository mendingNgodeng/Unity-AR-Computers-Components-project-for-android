using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;
public class NewBehaviourScript : MonoBehaviour
{
   public GameObject[] modelVariants;
    public string[] modelDescriptions;
    public TextMeshProUGUI descriptionText;
    public float fadeDuration = 0.3f;

    private int currentIndex = 0;
    private bool isTargetTracked = false;

    void Start()
    {
        ShowModelInstant(0);
        PositionTextAtTop();

        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnDestroy()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        isTargetTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
        gameObject.SetActive(isTargetTracked);

        if (isTargetTracked)
        {
            ShowModelInstant(currentIndex);
            Debug.Log($"[{gameObject.name}] Target Found. Showing model index: {currentIndex}");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Target Lost. Hiding models.");
        }
    }

    // === PUBLIC METHODS FOR BUTTONS ===
    public void OnClickNext()
    {
        if (isTargetTracked)
            StartCoroutine(SwitchModel(1));
    }

    public void OnClickPrevious()
    {
        if (isTargetTracked)
            StartCoroutine(SwitchModel(-1));
    }

    // === MODEL SWITCHING ===
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
    }

    void SetModelVisibility(GameObject model, bool visible)
    {
        model.SetActive(visible);
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
}
