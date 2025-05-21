using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FadeButton : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float visibleDuration = 1f;

    private Image buttonImage;
    private Color originalColor;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;

        StartCoroutine(FadeLoop());
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FadeImage(0f, 1f));
            yield return new WaitForSeconds(visibleDuration);
            yield return StartCoroutine(FadeImage(1f, 0f));
        }
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}