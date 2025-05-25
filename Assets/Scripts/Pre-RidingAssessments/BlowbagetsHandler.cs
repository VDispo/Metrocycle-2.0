using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;
using Metrocycle;

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
    [SerializeField] private SerializedDictionary<Blowbagets, MinigameSequenceSetup> allMinigames;

    [Tooltip("Empty the start button to leave it uninteractible")]
    [SerializedDictionary("Blowbagets", "Setup")]
    [SerializeField] private SerializedDictionary<Blowbagets, MinigameSequenceSetup> onlyBicycleMinigames;

    // chooses which to use: allMinigames or onlyBicycleMinigames
    private SerializedDictionary<Blowbagets, MinigameSequenceSetup> relevantMinigames;
    
    // caching
    [SerializedDictionary] private SerializedDictionary<Blowbagets, GameObject[]> activeMinigames;
    private GameObject latestMinigame;
    private uint latestSequenceIdx = 0;

    [Header("Other Refs")]
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
        // choose relevant array
        relevantMinigames = PreRidingAssessmentUiHandler.vehicleType == BikeType.Motorcycle ? allMinigames : onlyBicycleMinigames;
        Debug.Log($"[{GetType().FullName}] active vehicle type: {PreRidingAssessmentUiHandler.vehicleType}");

        // initialize activeMinigames
        activeMinigames = new();
        foreach (Blowbagets acronym in relevantMinigames.Keys)
        {
            activeMinigames.Add(acronym, new GameObject[relevantMinigames[acronym].minigames.Length]);
        }

        // initialize blowbagets buttons
        for (int i = 0; i < relevantMinigames.Count; i++)
        {
            Button button = relevantMinigames[(Blowbagets)i].startButton;
            if (button)
            {
                button.interactable = false;

                int _i = i; // cache to avoid closure issues
                button.onClick.AddListener(() => SelectBlowbagets(_i));
            }
        }

        if (CustomSceneManager.SelectedScene.Contains("Tutorial")) // if tutorial
        {
            EnableNextButton(0);
        }
        else
        {
            for (int i = 0; i < relevantMinigames.Count; i++)
                EnableNextButton(i);
        }
    }

    /// <summary>
    /// Assumes that buttons have 2 texts (as children gameobjects), with the first one being the full text.
    /// In bike, enables the next button if the current is unavailable.
    /// </summary>
    private void EnableNextButton(int nextButtonIdx) 
    {
        if (nextButtonIdx >= Enum.GetValues(typeof(Blowbagets)).Length) return;

        if (!relevantMinigames[(Blowbagets)nextButtonIdx].startButton)
        {
            Debug.Log($"[{GetType().FullName}] the relevant minigames for {PreRidingAssessmentUiHandler.vehicleType} does not contain a minigame entry for {(Blowbagets)nextButtonIdx} (enabling next button instead)");
            EnableNextButton(++nextButtonIdx);
            return;
        }

        if (nextButtonIdx < relevantMinigames.Count)
        {
            relevantMinigames[(Blowbagets)nextButtonIdx].startButton.interactable = true; // enable interaction
            relevantMinigames[(Blowbagets)nextButtonIdx].startButton.GetComponentInChildren<TextMeshProUGUI>().enabled = true; // show full text
        }
    }

    public void SelectBlowbagets(int blowbagetsButtonIdx)
    {
        EnableNextButton(blowbagetsButtonIdx + 1); // enable next button
        StartBlowbagetsMinigame((Blowbagets)blowbagetsButtonIdx);
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
        if (sequenceIdx >= relevantMinigames[blowbagetsIdx].minigames.Length) return;

        // Switch camera to specific minigame camera
        SwitchCamera(relevantMinigames[blowbagetsIdx].minigames[sequenceIdx].camera);

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
            latestMinigame = Instantiate(relevantMinigames[blowbagetsIdx].minigames[sequenceIdx].prefab, parent: minigamesParentTransform);
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
        if (latestSequenceIdx < relevantMinigames[blowbagetsIdx].minigames.Length)
        {
            FinishBlowbagetsMinigame();
            StartBlowbagetsMinigame(blowbagetsIdx, latestSequenceIdx);
            Debug.Log($"[{GetType().FullName}] next {blowbagetsIdx} minigame started!");
        }
        else Debug.Log($"[{GetType().FullName}] {blowbagetsIdx} minigames finished, total of {latestSequenceIdx} minigames played!");
    }

    /// <summary>
    /// Useful for determining when to show the FinishMinigame button and NextPart button, 
    /// which are both done via <see cref="PreRidingAssessmentUiHandler.ShowMinigameEndgameButtons"/>
    /// </summary>
    public bool IsFinalPart(Blowbagets blowbagetsIdx)
        => latestSequenceIdx == (relevantMinigames[blowbagetsIdx].minigames.Length - 1);

    /// <summary>
    /// Function to end the latest minigame.
    /// </summary>
    public void FinishBlowbagetsMinigame()
    {
        // End minigame
        StartCoroutine(PreRidingAssessmentUiHandler.Instance.ShowMinigameUi(show:false));
        latestMinigame.SetActive(false);

        // Switch camera to main
        SwitchCamera(mainCamera);
    }

    /// <summary>
    /// Switch camera to the selected <paramref name="targetCamera"/>. 
    /// If <c>null</c>, all cameras are set to 0. 
    /// </summary>
    /// <param name="targetCamera"/>
    public void SwitchCamera(CinemachineVirtualCamera targetCamera)
    {
        foreach (MinigameSequenceSetup sequence in relevantMinigames.Values)
            foreach (MinigameSetup setup in sequence.minigames)
                setup.camera.Priority = 0;
        mainCamera.Priority = 0;
     
        if (targetCamera) targetCamera.Priority = 10;
    }
}

