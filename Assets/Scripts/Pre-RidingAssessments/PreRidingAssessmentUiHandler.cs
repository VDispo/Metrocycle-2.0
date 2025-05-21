using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

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

    [Space(10)]
    public Button checkGearBtn;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject passText;
    [SerializeField] private GameObject failText;
    [SerializeField] private GameObject exitTextPanelBtn;
    [SerializeField] private GameObject finishSceneBtn;

    [Space(10)] // in CharacterCustomization
    [SerializeField] private GameObject goToBlowbagetsBtn; 
    [SerializeField] private GameObject quitBtn; 

    [Header("Camera Refs")]
    [SerializeField] private CinemachineVirtualCamera blowbagetsCam;
    [SerializeField] private CinemachineVirtualCamera customizationCam;

    [Header("Values")]
    [SerializeField][Tooltip("in sec")][Range(0, 1.2f)] private float delayBeforeOpeningMinigame = 0.4f;
    [SerializeField] private string startScreenName = "Start Screen";

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        // Start at blowbagets sub scene
        blowbagetsHandler.gameObject.SetActive(true);
        quitBtn.SetActive(true);

        customizationHandler.gameObject.SetActive(false);
        goToBlowbagetsBtn.SetActive(false);
        checkGearBtn.gameObject.SetActive(false);
    }

    public void BackToStartScreen() => CustomSceneManager.SwitchScene(startScreenName);

    /// <summary>
    /// Via the Self button. During Blowbagets.
    /// </summary>
    public void GoToCharacterCustomization() => SwitchSubScene(goForward:true);

    /// <summary>
    /// Via the back button. During CharacterCustomization.
    /// </summary>
    public void GoToBlowbagets() => SwitchSubScene(goForward:false);

    /// <summary>
    /// Forward: go to Customization subscene.<br/>
    /// Backward: go to Blowbagets subscene.
    /// </summary>
    private void SwitchSubScene(bool goForward)
    {
        // switch camera
        blowbagetsHandler.SwitchCamera(goForward ? customizationCam : blowbagetsCam); // set all cameras to 0

        blowbagetsHandler.gameObject.SetActive(!goForward); // toggle blowbagets
        customizationHandler.gameObject.SetActive(goForward); // toggle customization
        checkGearBtn.gameObject.SetActive(goForward); // toggle finish scene

        quitBtn.SetActive(!goForward); // toggle quit button (note that FinishBlowbagetsMinigame turns this on in this function)
        goToBlowbagetsBtn.SetActive(goForward); // toggle back button
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
        quitBtn.SetActive(!show);

        // Delay
        if (withDelay) 
            yield return new WaitForSecondsRealtime(delayBeforeOpeningMinigame);

        // Show minigame panel
        minigameTransform.gameObject.SetActive(show);
    }

    /// <summary>
    /// Assigned to finisbSceneBtn as the blowbagetS minigame (pass-fail mechanic) for Self.
    /// </summary>
    public void CheckValidGear()
    {
        failText.SetActive(false);
        passText.SetActive(false);
        exitTextPanelBtn.SetActive(false);
        finishSceneBtn.SetActive(false);

        textPanel.SetActive(true);
        if (CustomizationAssetsSelected.Instance.AllGearsValid())
        {
            passText.SetActive(true);
            finishSceneBtn.SetActive(true);
        }
        else
        {
            failText.SetActive(true);
            exitTextPanelBtn.SetActive(true);
        }
    }

    public void FinishScene() => CustomSceneManager.SwitchScene(CustomSceneManager.SelectedScene);
}
