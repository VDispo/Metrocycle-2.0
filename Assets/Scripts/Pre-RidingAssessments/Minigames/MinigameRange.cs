using UnityEngine;
using UnityEngine.UI;

public class MinigameRange : MinigameBase
{
    public override MinigameType Type => MinigameType.Range;

    private Sprite defaultMinigameImage; // for smooth increase/decrease, deprecated 
    [Header("Range Minigame")]
    [SerializeField] private Sprite activeMinigameImage;
    [SerializeField] private Button startMinigameButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Slider rangeSlider;
    private float targetValue = 0f; // for smooth increase/decrease, deprecated 
    private Coroutine latestCoroutine = null; // for smooth increase/decrease, deprecated 

    [Space(10)]
    [Tooltip("manually check with sprite")][SerializeField][Range(0,1)] private float visualMinVal = 0.325f;
    [Tooltip("manually check with sprite")][SerializeField][Range(0, 1)] private float visualMaxVal = 0.75f;

    [Space(10)]
    [SerializeField][Range(0, 0.5f)] private float minRandomValue = 0.05f;
    [SerializeField][Range(0, 0.5f)] private float maxRandomValue = 0.15f;

    protected override void Start2()
    {
        if (startMinigameButton == null) StartRangeMinigame();
        defaultMinigameImage = mainImage.sprite; // for smooth increase/decrease, deprecated 
        rangeSlider.value = 0;
    }

    public void StartRangeMinigame()
    {
        rangeSlider.gameObject.SetActive(true);
        ShowControlButtons(true);
        ShowActiveSprite(activeMinigameImage);

        rangeSlider.value = Random.Range(0f, 1f);
        CheckRangeValidity();
    }

    public void IncreaseRandom()
    {
        rangeSlider.value += Random.Range(minRandomValue, maxRandomValue);
        //targetValue += Random.Range(minRandomValue, maxRandomValue); // for smooth increase/decrease, deprecated 
        //latestCoroutine ??= StartCoroutine(nameof(UpdateSlider)); // for smooth increase/decrease, deprecated 
        CheckRangeValidity();
    }

    public void DecreaseRandom()
    {
        rangeSlider.value -= Random.Range(minRandomValue, maxRandomValue);
        //targetValue -= Random.Range(minRandomValue, maxRandomValue); // for smooth increase/decrease, deprecated 
        //latestCoroutine ??= StartCoroutine(nameof(UpdateSlider)); // for smooth increase/decrease, deprecated 
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
        mechanicBtn.SetActive(false);
    }

    private void ShowControlButtons(bool show)
    {
        increaseButton.gameObject.SetActive(show);
        decreaseButton.gameObject.SetActive(show);
    }

    // for smooth increase/decrease, deprecated 
    //private IEnumerator UpdateSlider()
    //{
    //    while (!Mathf.Approximately(rangeSlider.value, targetValue))
    //    {
    //        yield return null;
    //        int dir = rangeSlider.value < targetValue ? 1 : -1;
    //        rangeSlider.value += Time.deltaTime * dir;
    //    }
    //    latestCoroutine = null;
    //}
}
