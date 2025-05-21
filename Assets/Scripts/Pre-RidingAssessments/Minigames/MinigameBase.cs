using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public abstract class MinigameBase : MonoBehaviour
{
    public abstract MinigameType Type { get; }
    protected MinigameState state = MinigameState.NotPlayed;

    [Header("General")]
    public BlowbagetsHandler.Blowbagets blowbagets;
    [SerializeField] protected GameObject nextPartBtn;
    [SerializeField] protected GameObject mechanicBtn;
    [SerializeField] protected GameObject finishBtn;

    [Header("Pass & Fail")]
    [SerializeField] protected Image mainImage;
    [SerializeField] protected Image imageBorder;

    [Space(10)]
    [SerializeField] protected GameObject defaultText;
    [SerializeField] protected GameObject passText;
    [SerializeField] protected GameObject failText;

    [Space(10)]
    [SerializeField] protected Color passColor = Color.green; // #99FF99
    [SerializeField] protected Color failColor = Color.red; // #FF9B9B
    protected Color defaultColor; // auto-set
    
    [Space(10)]
    [SerializeField][Tooltip("Optional")] protected Sprite passSprite;
    [SerializeField][Tooltip("Optional")] protected Sprite failSprite;
    protected Sprite defaultSprite; // auto-set

    [Header("FadeToBlack")]
    [SerializeField] protected Image fadeToBlackPanel;
    [SerializeField][Range(0,2)][Tooltip("in sec")] protected float fadeToBlackDuration = 0.8f;
    [SerializeField][Range(0,2)][Tooltip("in sec")] protected float totalBlacknessDuration = 0.2f;
    [SerializeField][Range(0,2)][Tooltip("in sec")] protected float fadeFromBlackDuration = 0.8f;
    private float currFadeDuration = 0f;

    /// <summary>
    /// <paramref name="Button"/> minigame 
    ///     just requires pressing of a button placed on the screen.<br/>
    /// <paramref name="Threshold"/> minigame 
    ///     requires reaching a treshold progress (one-way).<br/>
    /// <paramref name="Range"/> minigame 
    ///     requires keeping a static progress in between a high and low threshold (two-way).<br/>
    /// <paramref name="Pump"/> minigame 
    ///     is the same as <paramref name="Range"/> except the progress is constantly erratic (moving).
    /// </summary>
    public enum MinigameType { Button, Threshold, Range, Pump }

    public enum MinigameState { NotPlayed, Passed, Failed }

    /// <summary> Unity's <c>Awake</c> function for child classes to override and use. </summary>
    protected virtual void Awake2() { }
    protected void Awake()
    {
        defaultColor = imageBorder.color;
        defaultSprite = mainImage.sprite;
        state = MinigameState.NotPlayed;
        Awake2();
    }

    /// <summary> Unity's <c>Start</c> function for child classes to override and use. </summary>
    protected virtual void Start2() { }
    protected void Start()
    {
        UpdateUiBasedOnState();
        Start2();
    }

    protected void Pass()
    {
        state = MinigameState.Passed;
        UpdateUiBasedOnState();
    }

    protected void Fail()
    {
        state = MinigameState.Failed;
        UpdateUiBasedOnState();
    }

    /// <summary>
    /// Updates active or set text, border color, and images based on the 
    /// the <see cref="state"/> global variable. Can be extended via <see cref="UpdateUiBasedOnState2"/>.
    /// </summary>
    private void UpdateUiBasedOnState()
    {
        defaultText.SetActive(false);
        passText.SetActive(false);
        failText.SetActive(false);

        bool isFinalPart = BlowbagetsHandler.Instance.IsFinalPart(blowbagets);
        switch (state)
        {
            case MinigameState.NotPlayed:
                nextPartBtn.SetActive(false);
                finishBtn.SetActive(false);
                mechanicBtn.SetActive(false);

                defaultText.SetActive(true);
                imageBorder.color = defaultColor;
                mainImage.sprite = defaultSprite;
                imageBorder.sprite = defaultSprite;
                break;

            case MinigameState.Passed:
                nextPartBtn.SetActive(!isFinalPart);
                finishBtn.SetActive(isFinalPart);
                mechanicBtn.SetActive(false);

                passText.SetActive(true);
                imageBorder.color = passColor;
                if (passSprite)
                {
                    mainImage.sprite = passSprite;
                    imageBorder.sprite = passSprite;
                }
                break;

            case MinigameState.Failed:
                nextPartBtn.SetActive(false);
                finishBtn.SetActive(false);
                mechanicBtn.SetActive(true);

                failText.SetActive(true);
                imageBorder.color = failColor;
                if (failSprite)
                {
                    mainImage.sprite = failSprite;
                    imageBorder.sprite = failSprite;
                }
                break; 

            default:
                nextPartBtn.SetActive(false);
                mechanicBtn.SetActive(true);
                finishBtn.SetActive(true);
                Debug.LogError($"[{GetType().FullName}] invalid minigame state \'{state}\'");
                return;
        }
        UpdateUiBasedOnState2();
    }
    protected virtual void UpdateUiBasedOnState2() { }

    /// <summary>
    /// "Take to mechanic" mechanic. Has a fade to black.
    /// </summary>
    public void InstantPass() => StartCoroutine(nameof(InstantPassCoroutine)); 
    private IEnumerator InstantPassCoroutine()
    {
        mechanicBtn.SetActive(false);
        fadeToBlackPanel.gameObject.SetActive(true);

        // Fade to Black animation
        Color black = Color.black;
        black.a = 0;

        void UpdateAlpha(float newAlpha)
        {
            black.a = newAlpha;
            fadeToBlackPanel.color = black;
        }

        // Fade to black (alpha to 1)
        while (currFadeDuration < fadeToBlackDuration)
        {
            UpdateAlpha(currFadeDuration / fadeToBlackDuration);
            currFadeDuration += Time.unscaledDeltaTime;
            yield return null;
        }
        UpdateAlpha(1);
        Pass();

        // Stay black
        yield return new WaitForSecondsRealtime(totalBlacknessDuration);

        // Fade from black (alpha to 0)
        black.a = 1;
        currFadeDuration = fadeFromBlackDuration;
        while (currFadeDuration >= 0)
        {
            UpdateAlpha(currFadeDuration / fadeFromBlackDuration);
            currFadeDuration -= Time.unscaledDeltaTime;
            yield return null;
        }
        UpdateAlpha(0);
        fadeToBlackPanel.gameObject.SetActive(false);
        currFadeDuration = 0;
    }

    public void PlayNextPart() => BlowbagetsHandler.Instance.StartNextMinigamePart(blowbagets);

    public void EndMinigame() => BlowbagetsHandler.Instance.FinishBlowbagetsMinigame();

    /// <summary> 
    /// Change sprite upon activation of minigame 
    /// (not necessarily the start of the minigame; instead, trigger-based, like via a button). 
    /// </summary>
    protected virtual void ShowActiveSprite(Sprite sprite = null) 
    {
        if (!sprite) return;
        mainImage.sprite = sprite;
        imageBorder.sprite = sprite;
    }
}
