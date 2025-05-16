using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MinigameHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image mainImage;
    [SerializeField] private Image imageBorder;

    [Header("Button Minigame")]
    [SerializeField] private Button button;
    [SerializeField][Range(0,1)] private float buttonChanceToFail = 0.4f;

    [Header("HighLow Minigame")]
    [SerializeField] private Slider highLowSlider;
    [SerializeField] private float minVal = 0.325f;
    [SerializeField] private float maxVal = 0.75f;
    [SerializeField] private float randRange = 0.2f;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    private static float LatestValue = 0f;

    [Header("Pass & Fail Values")]
    [SerializeField] private Color passColor = Color.green; // #99FF99
    [SerializeField] private Color failColor = Color.red; // #FF9B9B
    [SerializeField] private GameObject defaultText;
    [SerializeField] private GameObject passText;
    [SerializeField] private GameObject failText;
    [SerializeField] private Sprite passSprite; // optional
    [SerializeField] private Sprite failSprite; // optional

    [Header("Other Values")]
    [SerializeField] private MinigameType minigameType;
    [SerializeField] private BlowbagetsHandler.Blowbagets blowbagetsMinigameType;
    [SerializeField] private bool hasNextMinigame; // if multi-part

    private enum MinigameType
    {
        Button,
        HighLow,
        LeftRight
    }

    // Minigame States
    private enum MinigameState
    {
        NotPlayed,
        Passed,
        Failed
    }

    private static Dictionary<BlowbagetsHandler.Blowbagets, MinigameState> states;

    private void Awake()
    { 
        // initialize if needed
        states ??= new();
        if (!states.ContainsKey(blowbagetsMinigameType))
        {
            states.Add(blowbagetsMinigameType, MinigameState.NotPlayed);
        }
    }

    private void Start()
    {
        defaultText.SetActive(true);
        passText.SetActive(false);
        failText.SetActive(false);

        if (minigameType == MinigameType.HighLow)
            highLowSlider.value = 0;

        switch (states[blowbagetsMinigameType])
        {
            case MinigameState.NotPlayed:
                // proceed to play
                break;
            case MinigameState.Failed:
                // proceed to play (100% pass the next try)
                if (minigameType == MinigameType.Button)
                    buttonChanceToFail = 0;
                break;
            case MinigameState.Passed:
                // skip playing (& revert previous state)
                Pass(); 
                if (minigameType == MinigameType.HighLow)
                    highLowSlider.value = LatestValue;
                break;
            default:
                Debug.LogWarning($"[{GetType().FullName}] invalid MinigameState");
                break;
        }
    }

    public void PressButton()
    {
        int rand = Random.Range(0, 100);
        if (rand < (buttonChanceToFail * 100)) 
        {
            Debug.Log($"[{GetType().FullName}] rolled a {rand}, something wrong!");
            Fail();
        }
        else
        {
            Debug.Log($"[{GetType().FullName}] rolled a {rand}, nothing wrong!");
            Pass();
        }

        if (hasNextMinigame) 
            Debug.Log($"[{GetType().FullName}] has next minigame!");
    }

    private void Pass()
    {
        states[blowbagetsMinigameType] = MinigameState.Passed;

        if (button) button.gameObject.SetActive(false);
        imageBorder.color = passColor;
        passText.SetActive(true);
        failText.SetActive(false);
        defaultText.SetActive(false);

        if (minigameType == MinigameType.HighLow) 
            ToggleHighLowMiniGame(false);

        if (passSprite && (mainImage.sprite != passSprite || imageBorder.sprite != passSprite))
        {
            mainImage.sprite = passSprite;
            imageBorder.sprite = passSprite;
        }
    }

    private void Fail()
    {
        states[blowbagetsMinigameType] = MinigameState.Failed;

        if (button) button.gameObject.SetActive(false);
        imageBorder.color = failColor;
        passText.SetActive(false);
        failText.SetActive(true);
        defaultText.SetActive(false);

        if (failSprite && (mainImage.sprite != failSprite || imageBorder.sprite != failSprite))
        {
            mainImage.sprite = failSprite;
            imageBorder.sprite = failSprite;
        }
    }

    public void ToggleHighLowMiniGame(bool turnOn)
    {
        // first call
        button.gameObject.SetActive(false);
        if (highLowSlider.value == 0) { 
            highLowSlider.value = Random.Range(0f, 1f); 
            CheckHighLowPassFail();
        }

        // toggling part (all other calls)
        increaseButton.gameObject.SetActive(turnOn);
        decreaseButton.gameObject.SetActive(turnOn);
    }

    public void IncreaseRandom()
    {
        highLowSlider.value += Random.Range(0.01f, randRange + 0.01f);
        CheckHighLowPassFail();
    }

    public void DecreaseRandom() 
    {
        highLowSlider.value -= Random.Range(0.01f, randRange + 0.01f);
        CheckHighLowPassFail();
    }

    private void CheckHighLowPassFail()
    {
        LatestValue = highLowSlider.value;
        if (LatestValue >= minVal && LatestValue <= maxVal) Pass();
        else Fail();
    }
}
