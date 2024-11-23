using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleCoroutine;

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
        scaleCoroutine = StartCoroutine(ScaleOverTime(transform, targetScale, duration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleOverTime(transform, originalScale, duration));
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