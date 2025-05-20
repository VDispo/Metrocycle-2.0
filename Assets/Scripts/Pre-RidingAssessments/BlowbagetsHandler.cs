using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;

/// <summary>
/// Serves as a helper for <see cref="MinigameSequenceSetup"/>. 
/// If the plan is to use this alone (i.e., not via the Sequence version), manually set the on-click listeners of the start button
/// or whatever starter trigger is used.
/// </summary>
[Serializable]
public class MinigameSetup
{
    public GameObject prefab;
    public CinemachineVirtualCamera camera;
}

/// <summary>
/// Note that the <see cref="Button"/> reference enables <see cref="BlowbagetsHandler"/> 
/// to programatically set the button's on-click listeners or actions (i.e., starting the minigame). 
/// </summary>
[Serializable]
public class MinigameSequenceSetup
{
    public MinigameSetup[] minigames;

    [Tooltip("BlowbagetsHandler script programmatically sets the on-click listeners of these buttons (if set properly)")]
    public Button startButton;
}

public class BlowbagetsHandler : MonoBehaviour
{
    public static BlowbagetsHandler Instance;

    [Header("Minigames")]
    [SerializeField] private Transform minigamesParentTransform; // the parent tranform to spawn under
    [SerializedDictionary("Blowbagets", "Setup")]
    public SerializedDictionary<Blowbagets, MinigameSequenceSetup> allMinigames;
    
    // caching
    [SerializedDictionary] private SerializedDictionary<Blowbagets, GameObject[]> activeMinigames;
    private GameObject latestMinigame;
    private uint latestSequenceIdx = 0;

    [Header("Other Refs")]
    //[Tooltip("in order; this auto-sets the button listeners as well")] public Button[] blowbagetsButtons;
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    public enum Blowbagets {
        Battery = 0,
        Lights = 1, 
        Oil = 2,
        Water = 3,
        Brakes = 4,
        Air = 5,
        Gas = 6,
        Engine = 7,
        Tires = 8,
        Self = 9
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        // initialize activeMinigames
        activeMinigames = new();
        foreach (Blowbagets acronym in allMinigames.Keys)
        {
            activeMinigames.Add(acronym, new GameObject[allMinigames[acronym].minigames.Length]);
        }

        // initialize blowbagets buttons
        for (int i = 0; i < allMinigames.Count; i++)
        {
            Button button = allMinigames[(Blowbagets)i].startButton;
            if (button)
            {
                button.interactable = false;

                int _i = i; // cache to avoid closure issues
                button.onClick.AddListener(() => SelectBlowbagets(_i));
            }
        }
        EnableNextButton(0);
    }

    /// <summary>
    /// Assumes that buttons have 2 texts (as children gameobjects), with the first one being the full text.
    /// </summary>
    private void EnableNextButton(int nextButtonIdx) 
    {
        if (nextButtonIdx < allMinigames.Count)
        {
            allMinigames[(Blowbagets)nextButtonIdx].startButton.interactable = true; // enable interaction
            allMinigames[(Blowbagets)nextButtonIdx].startButton.GetComponentInChildren<TextMeshProUGUI>().enabled = true; // show full text
        }
    }

    public void SelectBlowbagets(int blowbagetsButtonIdx)
    {
        EnableNextButton(blowbagetsButtonIdx + 1); // enable next button
        
        // do logic for each
        Blowbagets idx = (Blowbagets)blowbagetsButtonIdx;
        switch (idx) 
        {
            case Blowbagets.Battery:
                Debug.Log($"[{GetType().FullName}] battery!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Lights:
                Debug.Log($"[{GetType().FullName}] lights!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Oil:
                Debug.Log($"[{GetType().FullName}] oil!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Water:
                Debug.Log($"[{GetType().FullName}] water!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Brakes:
                Debug.Log($"[{GetType().FullName}] brakes!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Air:
                Debug.Log($"[{GetType().FullName}] air!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Gas:
                Debug.Log($"[{GetType().FullName}] gas!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Engine:
                Debug.Log($"[{GetType().FullName}] engine!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Tires:
                Debug.Log($"[{GetType().FullName}] tires!");
                StartBlowbagetsMinigame(idx);
                break;
            case Blowbagets.Self:
                Debug.Log($"[{GetType().FullName}] self!");
                PreRidingAssessmentUiHandler.Instance.GoToCharacterCustomization();
                return;
            default:
                Debug.LogWarning($"[{GetType().FullName}] argument for BlowbagetsButtonClick is invalid");
                break;
        }
    }

    /// <summary>
    /// Function for starting a minigame, with <paramref name="blowbagetsIdx"/> for the minigame (or minigame sequence) 
    /// based on the BLOWBAGETS acronym (defined in <see cref="Blowbagets"/> enum), and <paramref name="sequenceIdx"/> 
    /// for the sequence index of the minigame (if more than one minigame for the acronym).
    /// </summary>
    /// <param name="blowbagetsIdx"></param>
    /// <param name="sequenceIdx"></param>
    public void StartBlowbagetsMinigame(Blowbagets blowbagetsIdx, uint sequenceIdx = 0)
    {
        if (sequenceIdx >= allMinigames[blowbagetsIdx].minigames.Length) return;

        // Switch camera to specific minigame camera
        mainCamera.Priority = 0;
        foreach (MinigameSequenceSetup sequence in allMinigames.Values)
            foreach (MinigameSetup setup in sequence.minigames)
                setup.camera.Priority = 0;
        allMinigames[blowbagetsIdx].minigames[sequenceIdx].camera.Priority = 10;

        // Start minigame
        latestSequenceIdx = sequenceIdx;
        GameObject existingMinigame = activeMinigames[blowbagetsIdx][sequenceIdx];
        if (existingMinigame) // activate if exiting
        {
            existingMinigame.SetActive(true);
            latestMinigame = existingMinigame;
        }
        else // create if not existing
        {
            latestMinigame = Instantiate(allMinigames[blowbagetsIdx].minigames[sequenceIdx].prefab, parent: minigamesParentTransform);
            activeMinigames[blowbagetsIdx][sequenceIdx] = latestMinigame;
        }
        StartCoroutine(PreRidingAssessmentUiHandler.Instance.ShowMinigameUi(show:true, withDelay:true));
    }

    /// <summary>
    /// Function for continuing a sequence of minigames.
    /// </summary>
    public void StartNextMinigamePart(Blowbagets blowbagetsIdx)
    {
        latestSequenceIdx++;
        if (latestSequenceIdx < allMinigames[blowbagetsIdx].minigames.Length)
        {
            FinishBlowbagetsMinigame();
            StartBlowbagetsMinigame(blowbagetsIdx, latestSequenceIdx);
            Debug.Log($"[{GetType().FullName}] next minigame played!");
        }
    }

    /// <summary>
    /// Useful for determining when to show the FinishMinigame button and NextPart button, 
    /// which are both done via <see cref="PreRidingAssessmentUiHandler.ShowMinigameEndgameButtons"/>
    /// </summary>
    public bool IsFinalPart(Blowbagets blowbagetsIdx) => 
        latestSequenceIdx == (allMinigames[blowbagetsIdx].minigames.Length - 1);

    /// <summary>
    /// Function to end the latest minigame.
    /// </summary>
    public void FinishBlowbagetsMinigame()
    {
        // End minigame
        StartCoroutine(PreRidingAssessmentUiHandler.Instance.ShowMinigameUi(show:false));
        latestMinigame.SetActive(false);

        // Switch camera to main
        mainCamera.Priority = 10;
        foreach (MinigameSequenceSetup sequence in allMinigames.Values)
            foreach (MinigameSetup setup in sequence.minigames)
                setup.camera.Priority = 0;
    }
}

