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
    [SerializeField] private GameObject defaultText_Blowbagets;
    [SerializeField] private GameObject defaultText; // customization
    [SerializeField] private GameObject passText; // customization
    [SerializeField] private GameObject failText; // customization
    [SerializeField] private Button exitTextPanelBtn;
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

        // TODO clean
        ShowBlowbagetsButtons(false);
        ShowBlowbagetsDefaultText();
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
    public System.Collections.IEnumerator ShowMinigameUi(bool show, bool withDelay = false)
    {
        // Hide buttons
        ShowBlowbagetsButtons(!show);
        quitBtn.SetActive(!show);

        // Delay
        if (withDelay) 
            yield return new WaitForSecondsRealtime(delayBeforeOpeningMinigame);

        // Show minigame panel
        minigameTransform.gameObject.SetActive(show);
    }

    public void ShowBlowbagetsButtons(bool show)
    {
        foreach (MinigameSequenceSetup sequence in blowbagetsHandler.allMinigames.Values)
            sequence.startButton.gameObject.SetActive(show);
    }

    // TODO mega clean DHJSSHJD
    public void ShowCustomizationSelectionButtons(bool show)
    {
        foreach (CustomizationAssetSetSelector selector in customizationHandler.selectors)
            if (selector) selector.gameObject.SetActive(show);
    }

    // TODO clean
    public void ShowBlowbagetsDefaultText()
    {
        failText.SetActive(false);
        passText.SetActive(false);
        finishSceneBtn.SetActive(false);
        defaultText.SetActive(false);
        
        defaultText_Blowbagets.SetActive(true);
        exitTextPanelBtn.gameObject.SetActive(true);
        textPanel.SetActive(true);

        exitTextPanelBtn.onClick.AddListener(ExitBlowbagetsDefaultText);
    }

    // TODO clean
    private void ExitBlowbagetsDefaultText()
    {
        ShowBlowbagetsButtons(true);
        exitTextPanelBtn.onClick.RemoveListener(ExitBlowbagetsDefaultText);
    }

    // TODO clean
    public void ShowCustomizationDefaultText()
    {
        failText.SetActive(false);
        passText.SetActive(false);
        finishSceneBtn.SetActive(false);
        defaultText_Blowbagets.SetActive(false);

        defaultText.SetActive(true);
        exitTextPanelBtn.gameObject.SetActive(true);
        textPanel.SetActive(true);

        ShowCustomizationSelectionButtons(false);
    }

    // TODO clean
    private void ExitCustomizationText()
    {
        ShowBlowbagetsButtons(true);
        exitTextPanelBtn.onClick.RemoveListener(ExitCustomizationText);
     
        ShowCustomizationSelectionButtons(true);
    }

    // TODO clean
    /// <summary>
    /// Assigned to finisbSceneBtn as the blowbagetS minigame (pass-fail mechanic) for Self.
    /// </summary>
    public void CheckValidGear()
    {
        defaultText_Blowbagets.SetActive(false);
        defaultText.SetActive(false);
        failText.SetActive(false);
        passText.SetActive(false);
        exitTextPanelBtn.gameObject.SetActive(false);
        finishSceneBtn.SetActive(false);
        ShowCustomizationSelectionButtons(false);

        textPanel.SetActive(true);
        if (CustomizationAssetsSelected.Instance.AllGearsValid())
        {
            passText.SetActive(true);
            finishSceneBtn.SetActive(true);
        }
        else
        {
            failText.SetActive(true);
            exitTextPanelBtn.gameObject.SetActive(true);
        }
    }

    public void FinishScene() => CustomSceneManager.SwitchScene(CustomSceneManager.SelectedScene);
}
