using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is responsible for hiding/showing UI elements & camera priorities for the Pre-Riding_Assessment scene.
/// </summary>
public class PreRidingAssessmentUiHandler : MonoBehaviour
{
    public static PreRidingAssessmentUiHandler Instance;

    [Header("Canvas Refs")]
    [SerializeField] private BlowbagetsHandler blowbagetsHandler;
    [SerializeField] private CharacterCustomizationHandler customizationHandler;
    [SerializeField] private Transform minigameTransform; // the root parent, this is used to show the whole minigame; in contrast, BlowbagetHandler's minigameParent is just the parent to spawn to
    public Button finishSceneBtn;

    [Space(10)] // in CharacterCustomization
    [SerializeField] private Button goToBlowbagetsBtn; 
    [SerializeField] private Button quitBtn; 

    [Space(10)] // in Blowbagets
    [SerializeField] private Button goToCustomizationBtn; 

    [Header("Camera Refs")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera blowbagetsCam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera customizationCam;

    [Header("Values")]
    [SerializeField][Tooltip("in sec")][Range(0, 1.2f)] private float delayBeforeOpeningMinigame = 0.4f;
    [SerializeField] private string startScreenName = "Start Screen";

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;

        finishSceneBtn.onClick.AddListener(() => CustomSceneManager.SwitchScene(CustomSceneManager.SelectedScene));
    }

    private void Start()
    {
        // Start at blowbagets sub scene
        blowbagetsHandler.gameObject.SetActive(true);
        quitBtn.gameObject.SetActive(true);

        customizationHandler.gameObject.SetActive(false);
        goToBlowbagetsBtn.gameObject.SetActive(false);
        finishSceneBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// Via the forward or Self buttons. During Blowbagets.
    /// </summary>
    public void GoToCharacterCustomization() => SwitchSubScene(goForward:true);

    /// <summary>
    /// Via the back button. During CharacterCustomization.
    /// </summary>
    public void GoToBlowbagets() => SwitchSubScene(goForward:false);

    private void SwitchSubScene(bool goForward)
    {
        quitBtn.gameObject.SetActive(!goForward); // toggle forward button
        goToBlowbagetsBtn.gameObject.SetActive(goForward); // toggle back button

        blowbagetsHandler.FinishBlowbagetsMinigame(); // ensure main blowbagets camera is solo live (prio 10, all subcameras' 0)

        blowbagetsHandler.gameObject.SetActive(!goForward); // toggle blowbagets
        customizationHandler.gameObject.SetActive(goForward); // toggle customization
        finishSceneBtn.gameObject.SetActive(goForward); // toggle finish scene

        // switch camera
        customizationCam.Priority = goForward ? 10 : 0;
        blowbagetsCam.Priority = goForward ? 0 : 10;
    }

    /// <summary>
    /// Function to hide the blowbagets buttons and show the minigame panel, with delay defined in <see cref="delayBeforeOpeningMinigame"/>.
    /// <br/><br/>
    /// Call via <c>StartCoroutine(<seealso cref="ShowMinigameUi(bool, bool)"/>)</c>
    /// </summary>
    /// <param name="show"></param>
    public IEnumerator ShowMinigameUi(bool show, bool withDelay = false)
    {
        // Hide buttons
        foreach (MinigameSequenceSetup sequence in blowbagetsHandler.allMinigames.Values)
            sequence.startButton.gameObject.SetActive(!show);
        quitBtn.gameObject.SetActive(!show);

        // Delay
        if (withDelay) 
            yield return new WaitForSecondsRealtime(delayBeforeOpeningMinigame);

        // Show minigame panel
        minigameTransform.gameObject.SetActive(show);
    }

    public void BackToStartScreen() => CustomSceneManager.SwitchScene(startScreenName);
}
