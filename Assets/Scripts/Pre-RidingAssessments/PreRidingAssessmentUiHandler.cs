using UnityEngine;
using UnityEngine.UI;

public class PreRidingAssessmentUiHandler : MonoBehaviour
{
    public static PreRidingAssessmentUiHandler Instance;

    [Header("Canvas Refs")]
    [SerializeField] private BlowbagetsHandler blowbagetsHandler;
    [SerializeField] private CharacterCustomizationHandler customizationHandler;
    [SerializeField] private Transform minigameTransform; // the root parent, this is used to toggle the blowbagetsMinigameType; in contrast, BlowbagetHandler's minigameParent is just the parent to spawn to
    [SerializeField] private Button goToBlowbagetsBtn;
    [SerializeField] private Button goToCustomizationBtn;
    public Button finishSceneBtn;

    [Header("Camera Refs")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera blowbagetsCam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera customizationCam;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        // Start at blowbagets sub scene
        blowbagetsHandler.gameObject.SetActive(true);
        //goToCustomizationBtn.gameObject.SetActive(true);

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
        //goToCustomizationBtn.gameObject.SetActive(!goForward); // toggle forward button
        goToBlowbagetsBtn.gameObject.SetActive(goForward); // toggle back button

        blowbagetsHandler.FinishBlowbagetsMinigame(); // ensure main blowbagets camera is solo live (prio 10, all subcameras' 0)

        blowbagetsHandler.gameObject.SetActive(!goForward); // toggle blowbagets
        customizationHandler.gameObject.SetActive(goForward); // toggle customization
        finishSceneBtn.gameObject.SetActive(goForward); // toggle finish scene

        // switch camera
        customizationCam.Priority = goForward ? 10 : 0;
        blowbagetsCam.Priority = goForward ? 0 : 10;
    }

    public void ToggleMinigameUI(bool turnOn)
    {
        // Disable buttons
        foreach (Button btn in blowbagetsHandler.blowbagetsButtons)
            btn.gameObject.SetActive(!turnOn);

        //// Disable next button
        //goToCustomizationBtn.gameObject.SetActive(!turnOn);

        // Enable blowbagetsMinigameType
        minigameTransform.gameObject.SetActive(turnOn);
    }
}
