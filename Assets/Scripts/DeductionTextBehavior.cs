using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeductionTextBehavior : MonoBehaviour
{
    private Text deductionText;
    private float fadeDuration = 3f;
    private float fadeTimer;
    private bool isFading;

    void Start()
    {
        deductionText = GetComponent<Text>();
        if (deductionText == null)
        {
            Debug.LogError("DeductionTextBehavior: No Text component found on this GameObject.");
        }
    }

    void Update()
    {
        if (isFading)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0)
            {
                deductionText.CrossFadeAlpha(0, 0.5f, false);
                isFading = false;
            }
        }
    }

    public void SetErrorText(string text)
    {
        if (deductionText == null) return;

        deductionText.text = text;
        deductionText.color = Color.red; // Set text color to red
        deductionText.CrossFadeAlpha(1, 0, false); // Reset alpha to fully visible
        fadeTimer = fadeDuration; // Reset the fade timer
        isFading = true;
    }

    public void SetIncrementText(string text)
    {
        if (deductionText == null) return;

        deductionText.text = text;
        deductionText.color = Color.green; // Set text color to green
        deductionText.CrossFadeAlpha(1, 0, false); // Reset alpha to fully visible
        fadeTimer = fadeDuration; // Reset the fade timer
        isFading = true;
    }
}
