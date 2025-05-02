using TMPro;
using UnityEngine;
using System.Collections;

public class FadeTMP : MonoBehaviour
{
    public float fadeDuration = 1f;      // How long the fade in/out lasts
    public float visibleDuration = 1f;   // How long text stays fully visible

    private TextMeshProUGUI tmpText;
    private Color originalColor;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        originalColor = tmpText.color;
        StartCoroutine(FadeLoop());
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            // Fade In
            yield return StartCoroutine(FadeText(0f, 1f));
            // Wait while visible
            yield return new WaitForSeconds(visibleDuration);
            // Fade Out
            yield return StartCoroutine(FadeText(1f, 0f));
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure final alpha is set
        tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}