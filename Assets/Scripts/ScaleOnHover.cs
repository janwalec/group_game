using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleCoroutine;
    private GameObject hoverCopy; // Store the copy of the hovered element

    public float scaleMultiplier = 1.8f; // The multiplier for the scale
    public float duration = 0.1f; // Duration of the scaling animation

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Create a copy of the hovered element
        hoverCopy = Instantiate(gameObject, transform.parent.parent);
        hoverCopy.transform.SetAsLastSibling(); // Make sure it's on top of other elements

        // Set the position and scale of the copy to match the original
        RectTransform originalRect = GetComponent<RectTransform>();
        RectTransform copyRect = hoverCopy.GetComponent<RectTransform>();
        copyRect.position = originalRect.position;
        copyRect.rotation = originalRect.rotation;
        copyRect.sizeDelta = originalRect.sizeDelta;

        // Disable the ScaleOnHover script on the copy to prevent recursive behavior
        ScaleOnHover copyScript = hoverCopy.GetComponent<ScaleOnHover>();
        if (copyScript != null)
        {
            copyScript.enabled = false;
        }

        // Make the copy non-interactable to prevent it from intercepting events
        CanvasGroup canvasGroup = hoverCopy.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Start scaling the copy
        scaleCoroutine = StartCoroutine(ScaleOverTime(copyRect, targetScale, duration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        if (hoverCopy != null)
        {
            // Destroy the copy
            Destroy(hoverCopy);
        }

        // Optionally, you can add some logic here if you want to scale back the original element
        // scaleCoroutine = StartCoroutine(ScaleOverTime(transform, originalScale, duration));
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 toScale, float overTime)
    {
        Vector3 startScale = target.localScale;
        float startTime = Time.time;

        while (Time.time < startTime + overTime)
        {
            target.localScale = Vector3.Lerp(startScale, toScale, (Time.time - startTime) / overTime);
            yield return null;
        }

        target.localScale = toScale;
    }
}
