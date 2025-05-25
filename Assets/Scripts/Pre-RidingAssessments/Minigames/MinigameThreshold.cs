using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Could derive from <see cref="MinigameRange"/> but it just sounds weird since Range should be the step up from Threshold.
/// (Bad architecture, maybe because late decision to add this). TODO would be to reconcile this architecture.
/// </summary>
public class MinigameThreshold : MinigameBase
{
    public override MinigameType Type => MinigameType.Threshold;

    [Header("Threshold Minigame")]
    [SerializeField] private Sprite activeMinigameImage;
    [SerializeField] private Button startMinigameButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private GameObject progressBackground;
    [SerializeField] private Transform progressIndicator;
    private float progress = 0; // % value, meaning 0-1 range

    [Space(10)]
    [Tooltip("assume z axis")][SerializeField] private float minRot = 0; // serves as offset
    [Tooltip("assume z axis")][SerializeField] private float maxRot = -110;

    [Space(10)]
    [Tooltip("manually check with sprite; % value")][SerializeField][Range(0, 1)] private float thresholdProgress = 0.5f;

    [Space(10)]
    [Tooltip("% value")][SerializeField][Range(0, 1)] private float minRandomValue = 0.05f;
    [Tooltip("% value")][SerializeField][Range(0, 1)] private float maxRandomValue = 0.15f;

    protected override void Start2()
    {
        if (startMinigameButton == null) StartThresholdMinigame();
    }

    public void StartThresholdMinigame()
    {
        progressBackground.SetActive(true);
        progressIndicator.gameObject.SetActive(true);
        ShowControlButtons(true);
        ShowActiveSprite(activeMinigameImage);

        progress = Random.Range(0f, 1f);
        CheckRangeValidity();
    }

    public void IncreaseRandom()
    {
        progress += Random.Range(minRandomValue, maxRandomValue);
        CheckRangeValidity();
    }

    private void CheckRangeValidity()
    {
        UpdateProgressIndicator();
        if (progress >= thresholdProgress)
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

    private void UpdateProgressIndicator()
    {
        Vector3 currRot = progressIndicator.rotation.eulerAngles;
        currRot.z = (progress * maxRot) + minRot;
        progressIndicator.rotation = Quaternion.Euler(currRot.x, currRot.y, currRot.z);
    }

    private void ShowControlButtons(bool show) => increaseButton.gameObject.SetActive(show);
}
