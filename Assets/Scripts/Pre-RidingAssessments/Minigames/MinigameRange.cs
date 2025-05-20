using UnityEngine;
using UnityEngine.UI;

public class MinigameRange : MinigameBase
{
    public override MinigameType Type => MinigameType.Range;

    private Sprite defaultMinigameImage;
    [Header("Range Minigame")]
    [SerializeField] private Sprite activeMinigameImage;

    [SerializeField] private Button startMinigameButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Slider rangeSlider;

    [Space(10)]
    [Tooltip("manually check with sprite")][SerializeField][Range(0,1)] private float visualMinVal = 0.325f;
    [Tooltip("manually check with sprite")][SerializeField][Range(0, 1)] private float visualMaxVal = 0.75f;

    [Space(10)]
    [SerializeField][Range(0, 0.5f)] private float minRandomValue = 0.05f;
    [SerializeField][Range(0, 0.5f)] private float maxRandomValue = 0.15f;

    protected override void Start2()
    {
        defaultMinigameImage = mainImage.sprite;
        rangeSlider.value = 0;
    }

    public void StartRangeMinigame()
    {
        startMinigameButton.gameObject.SetActive(false);
        rangeSlider.gameObject.SetActive(true);
        ShowControlButtons(true);
        ShowActiveSprite(true);

        rangeSlider.value = Random.Range(0f, 1f);
        CheckRangeValidity();
    }

    public void IncreaseRandom()
    {
        rangeSlider.value += Random.Range(minRandomValue, maxRandomValue);
        CheckRangeValidity();
    }

    public void DecreaseRandom()
    {
        rangeSlider.value -= Random.Range(minRandomValue, maxRandomValue);
        CheckRangeValidity();
    }

    private void CheckRangeValidity()
    {
        if (rangeSlider.value >= visualMinVal && rangeSlider.value <= visualMaxVal)
        {
            Pass();
            ShowControlButtons(false);
        }
        else
        {
            Fail();
            ShowControlButtons(true);
        }
    }

    private void ShowControlButtons(bool show)
    {
        increaseButton.gameObject.SetActive(show);
        decreaseButton.gameObject.SetActive(show);
    }

    private void ShowActiveSprite(bool show)
    {
        if (!activeMinigameImage) return;

        if (show)
        {
            mainImage.sprite = activeMinigameImage;
            imageBorder.sprite = activeMinigameImage;
        }
        else
        {
            mainImage.sprite = defaultMinigameImage;
            imageBorder.sprite = defaultMinigameImage;
        }
    }
}
