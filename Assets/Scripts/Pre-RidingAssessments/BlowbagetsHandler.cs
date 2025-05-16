using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class BlowbagetsHandler : MonoBehaviour
{
    public static BlowbagetsHandler Instance;

    [Header("Minigames")]
    [SerializeField] private GameObject[] minigames;
    [SerializeField] private Transform minigamesParentTransform; // the parent tranform to spawn under
    [SerializeField][Tooltip("in sec")] private float delayBeforeOpeningMinigame = 1; 
    private GameObject latestMinigame;

    [Header("Buttons")]
    [Tooltip("in order; this auto-sets the button listeners as well")] public Button[] blowbagetsButtons;
    [SerializeField] private Button backToSelectionButton;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    // Mapping of each Blowbagets enum to the correct Camera Location
    [SerializedDictionary("Blowbagets, CameraType")]
    [SerializeField] private SerializedDictionary<Blowbagets, CinemachineVirtualCamera[]> blowbagetsCameraLocation;

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
        for (int i = 0; i < blowbagetsButtons.Length; i++)
        {
            Button button = blowbagetsButtons[i];
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
        if (nextButtonIdx < blowbagetsButtons.Length)
        {
            blowbagetsButtons[nextButtonIdx].interactable = true; // enable interaction
            blowbagetsButtons[nextButtonIdx].GetComponentInChildren<TextMeshProUGUI>().enabled = true; // show full text
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

    private void StartBlowbagetsMinigame(Blowbagets idx)
    {
        // Switch camera
        mainCamera.Priority = 0;
        foreach (CinemachineVirtualCamera[] cams in blowbagetsCameraLocation.Values)
            foreach (CinemachineVirtualCamera cam in cams)
                cam.Priority = 0;
        blowbagetsCameraLocation[idx][0].Priority = 10;

        // Start blowbagetsMinigameType
        latestMinigame = Instantiate(minigames[(int)idx], parent: minigamesParentTransform); // initialize
        StartCoroutine(nameof(ShowMiniGameWithDelay));
    }

    private IEnumerator ShowMiniGameWithDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeOpeningMinigame);
        PreRidingAssessmentUiHandler.Instance.ToggleMinigameUI(true);
    }

    public void FinishBlowbagetsMinigame()
    {
        // End blowbagetsMinigameType
        PreRidingAssessmentUiHandler.Instance.ToggleMinigameUI(false);
        if (latestMinigame) Destroy(latestMinigame); // cleanup

        // Switch camera
        mainCamera.Priority = 10;
        foreach (CinemachineVirtualCamera[] cams in blowbagetsCameraLocation.Values)
            foreach (CinemachineVirtualCamera cam in cams)
                cam.Priority = 0;
    }

    public void FinishScene()
    {
        FinishBlowbagetsMinigame();
        //nextScene.Instance.LoadScene();
        // todo switch scene (reinitialize assets)
    }
}
