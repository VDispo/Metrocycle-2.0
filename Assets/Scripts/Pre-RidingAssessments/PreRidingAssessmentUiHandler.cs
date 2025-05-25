using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Metrocycle;
using UnityEngine.Rendering;

/// <summary>
/// This is responsible for hiding/showing UI elements & camera priorities for the Pre-Riding_Assessment scene.
/// This script also ties in and determines the targeted vehicle type based on the initially-set <see cref="CustomSceneManager.SelectedScene"/> (name). <br/>
/// For debug purposes (i.e., playing straight in this scene without setting <see cref="CustomSceneManager.SelectedScene"/>, use <see cref="debug_targetVehicleType"/>.
/// </summary>
public class PreRidingAssessmentUiHandler : MonoBehaviour
{
    public static PreRidingAssessmentUiHandler Instance;

    [Header("For Initial Setup")]
    [SerializeField] private GameObject motorcycle;
    [SerializeField] private GameObject bicycle;
    public static BikeType vehicleType;
#if UNITY_EDITOR
    [Tooltip("Debug and only takes effect if CustomSceneManager.SelectedScene is empty")]
    [SerializeField] private BikeType debug_targetVehicleType = BikeType.Motorcycle;
#endif
    
    [Header("General Refs")]
    [SerializeField] private BlowbagetsHandler blowbagetsHandler;
    [SerializeField] private CharacterCustomizationHandler customizationHandler;
    [Tooltip("the whole minigame root parent; BlowbagetHandler's minigameParent is the parent to spawn under")]
    [SerializeField] private Transform minigameTransform;
    [SerializeField] private GameObject blowbagetsButtonsParent;
    [SerializeField] private GameObject customizationSelectorsParent;
    [SerializeField] private Button checkGearBtn;

    [Header("Blowbagets Text Panel")]
    [SerializeField] private GameObject blowbagets_initialPrompt_bike; 
    [SerializeField] private GameObject blowbagets_initialPrompt_motor; 

    [Header("Customization Text Panel")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject defaultText_bike;
    [SerializeField] private GameObject passText_bike;
    [SerializeField] private GameObject failText_bike;
    [SerializeField] private GameObject defaultText_motor;
    [SerializeField] private GameObject passText_motor;
    [SerializeField] private GameObject failText_motor;
    private GameObject defaultText; 
    private GameObject passText;
    private GameObject failText; 
    [SerializeField] private Button exitTextPanelBtn;
    [SerializeField] private GameObject finishSceneBtn;

    [Header("Back Buttons")]
    [SerializeField] private GameObject goToBlowbagetsBtn; 
    [SerializeField] private GameObject quitBtn; 

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera blowbagetsCam;
    [SerializeField] private CinemachineVirtualCamera customizationCam;

    [Header("Values")]
    [SerializeField][Tooltip("in sec")][Range(0, 1.2f)] private float delayBeforeOpeningMinigame = 0.4f;
    [SerializeField] private string startScreenName = "Start Screen";

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;

#if UNITY_EDITOR
        if (CustomSceneManager.SelectedScene == string.Empty)
        {
            string debugSceneName = debug_targetVehicleType == BikeType.Motorcycle ? "Motorcycle" : "Bicycle";
            CustomSceneManager.SelectedScene = debugSceneName;
            Debug.Log($"[{GetType().FullName}] entered scene {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} " +
                $"without setting CustomSceneManager.SelectedScene, defaulting to debug: {debugSceneName} " +
                $"({nameof(debug_targetVehicleType)} is {debug_targetVehicleType})");
        }
#endif

        // determine bike type
        if (CustomSceneManager.SelectedScene.Contains("Motorcycle"))
        {
            vehicleType = BikeType.Motorcycle;
            motorcycle.SetActive(true);
            bicycle.SetActive(false);

            // activate initial prompt based on bike type
            blowbagets_initialPrompt_motor.SetActive(true);
            blowbagets_initialPrompt_bike.SetActive(false);
            defaultText = defaultText_motor;
            passText = passText_motor;
            failText = failText_motor;
        }
        else
        {
            vehicleType = BikeType.Bicycle;
            motorcycle.SetActive(false);
            bicycle.SetActive(true);

            // activate initial prompt based on bike type
            blowbagets_initialPrompt_motor.SetActive(false);
            blowbagets_initialPrompt_bike.SetActive(true);
            defaultText = defaultText_bike;
            passText = passText_bike;
            failText = failText_bike;
        }
    }

    public void ShowCustomizationDefaultText()
    {
        failText.SetActive(false);
        passText.SetActive(false);
        checkGearBtn.gameObject.SetActive(false);
        customizationSelectorsParent.SetActive(false);

        defaultText.SetActive(true);
        exitTextPanelBtn.gameObject.SetActive(true);
        textPanel.SetActive(true);
    }

    /// <summary>
    /// Via the Self button. During Blowbagets.
    /// </summary>
    public void GoToCharacterCustomization()
    {
        SwitchSubScene(goForward: true);
        ShowCustomizationDefaultText();
    }

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
        if (!goForward) checkGearBtn.gameObject.SetActive(false); // disable checkgear if going back

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
        blowbagetsButtonsParent.SetActive(!show);
        quitBtn.SetActive(!show);

        // Delay
        if (withDelay) 
            yield return new WaitForSecondsRealtime(delayBeforeOpeningMinigame);

        // Show minigame panel
        minigameTransform.gameObject.SetActive(show);
    }

    public void ValidGearUi()
    {
        if (vehicleType == BikeType.Bicycle)

        defaultText.SetActive(false);
        if (customizationHandler.AllGearsValid())
        {
            passText.SetActive(true);
            failText.SetActive(false);
            exitTextPanelBtn.gameObject.SetActive(true);
            finishSceneBtn.SetActive(true);
        }
        else
        {
            passText.SetActive(false);
            failText.SetActive(true);
            exitTextPanelBtn.gameObject.SetActive(true);
        }
    }

    public void BackToStartScreen() => CustomSceneManager.SwitchScene(startScreenName);

    public void FinishScene() => CustomSceneManager.SwitchScene(CustomSceneManager.SelectedScene);
}
