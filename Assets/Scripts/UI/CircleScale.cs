using System.Collections;
using UnityEngine;

public class CircleScale : MonoBehaviour
{
    public RectTransform outerCircle;
    public float maxSize = 400f;
    public float minSize = 60f;

    public float fadeDuration = 1.0f;
    public CanvasGroup canvasGroup;

    private void OnEnable()
    {
        FadeIn();
    }
    public void ScaleCircle(float startDist, float remainingDist)
    {
        // Normalize far to close
        float t = Mathf.InverseLerp(startDist, 0f, remainingDist);

        float size = Mathf.Lerp(maxSize, minSize, t);
        outerCircle.sizeDelta = new Vector2(size, size);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvas(canvasGroup, 0f, 1f, fadeDuration));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
    }
}
