using System.Collections;
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
    private float targetValue = 0f;
    private Coroutine latestCoroutine = null;

    [Space(10)]
    [Tooltip("manually check with sprite")][SerializeField][Range(0,1)] private float visualMinVal = 0.325f;
    [Tooltip("manually check with sprite")][SerializeField][Range(0, 1)] private float visualMaxVal = 0.75f;

    [Space(10)]
    [SerializeField][Range(0, 0.5f)] private float minRandomValue = 0.05f;
    [SerializeField][Range(0, 0.5f)] private float maxRandomValue = 0.15f;

    protected override void Start2()
    {
        if (startMinigameButton == null) StartRangeMinigame();
        defaultMinigameImage = mainImage.sprite;
        rangeSlider.value = 0;
    }

    public void StartRangeMinigame()
    {
        if (startMinigameButton) startMinigameButton.gameObject.SetActive(false);
        rangeSlider.gameObject.SetActive(true);
        ShowControlButtons(true);
        ShowActiveSprite(activeMinigameImage);

        rangeSlider.value = Random.Range(0f, 1f);
        CheckRangeValidity();
    }

    public void IncreaseRandom()
    {
        rangeSlider.value += Random.Range(minRandomValue, maxRandomValue);
        //targetValue += Random.Range(minRandomValue, maxRandomValue);
        //latestCoroutine ??= StartCoroutine(nameof(UpdateSlider));
        CheckRangeValidity();
    }

    public void DecreaseRandom()
    {
        rangeSlider.value -= Random.Range(minRandomValue, maxRandomValue);
        //targetValue -= Random.Range(minRandomValue, maxRandomValue);
        //latestCoroutine ??= StartCoroutine(nameof(UpdateSlider));
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

    private void ShowControlButtons(bool show)
    {
        increaseButton.gameObject.SetActive(show);
        decreaseButton.gameObject.SetActive(show);
    }
}
