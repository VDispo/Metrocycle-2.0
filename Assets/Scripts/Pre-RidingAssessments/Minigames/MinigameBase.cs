using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class MinigameBase : MonoBehaviour
{
    public abstract MinigameType Type { get; }
    protected MinigameState state = MinigameState.NotPlayed;

    [Header("Typing")]
    public BlowbagetsHandler.Blowbagets blowbagets;

    [Header("Pass & Fail")]
    [SerializeField] protected Image mainImage;
    [SerializeField] protected Image imageBorder;
    [SerializeField][Tooltip("in sec")][Range(0, 2f)] protected float nextPartDelay = 0.5f;

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

    /// <summary>
    /// <paramref name="Button"/> minigame 
    ///     just requires pressing of a button placed on the screen.<br/>
    /// <paramref name="Range"/> minigame 
    ///     requires keeping a static value in between a high and low threshold (may be different from the visual max-min lines).<br/>
    /// <paramref name="Pump"/> minigame 
    ///     is the same as <paramref name="Range"/> except the value is constantly erratic (moving).
    /// </summary>
    public enum MinigameType { Button, Range, Pump }

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

    protected IEnumerator PlayNextPartWithDelay()
    {
        yield return new WaitForSecondsRealtime(nextPartDelay);
        BlowbagetsHandler.Instance.StartNextMinigamePart(blowbagets);
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
    /// the <see cref="state"/> global variable.
    /// </summary>
    private void UpdateUiBasedOnState()
    {
        defaultText.SetActive(false);
        passText.SetActive(false);
        failText.SetActive(false);

        switch (state)
        {
            case MinigameState.NotPlayed:
                defaultText.SetActive(true);
                imageBorder.color = defaultColor;
                mainImage.sprite = defaultSprite;
                imageBorder.sprite = defaultSprite;
                break;

            case MinigameState.Passed:
                passText.SetActive(true);
                imageBorder.color = passColor;
                if (passSprite)
                {
                    mainImage.sprite = passSprite;
                    imageBorder.sprite = passSprite;
                }
                break;

            case MinigameState.Failed:
                failText.SetActive(true);
                imageBorder.color = failColor;
                if (failSprite)
                {
                    mainImage.sprite = failSprite;
                    imageBorder.sprite = failSprite;
                }
                break; 

            default:
                Debug.LogError($"[{GetType().FullName}] invalid minigame state \'{state}\'");
                return;
        }
    }
}
