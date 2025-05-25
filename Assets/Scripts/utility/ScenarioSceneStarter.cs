using UnityEngine;

/// <summary>
/// A non-static script to be able to place listeners to button on-clicks!
/// </summary>
public class ScenarioSceneStarter : MonoBehaviour
{
    /// <summary>
    /// No intermediate (blowbagets) scene. Mainly used to go from Title to Start Screen.
    /// </summary>
    public void StartSceneNoIntermediate(string sceneName) => CustomSceneManager.SwitchScene(sceneName);

    /// <summary>
    /// For direct scene switching. Saves argument to <see cref="CustomSceneManager.SelectedScene"/>.
    /// </summary>
    public void StartScene(string sceneName)
    {
        SaveSelectedScene(sceneName);
        StartSelectedScene();
    }

    /// <summary>
    /// For scene switching later on. Saves argument to <see cref="CustomSceneManager.SelectedScene"/>.
    /// </summary>
    public void SaveSelectedScene(string sceneName)
    {
        CustomSceneManager.SelectedScene = sceneName;
        Debug.Log("Selected scene with intermediate: " + CustomSceneManager.SelectedScene);
    }

    /// <summary>
    /// Note that the <see cref="CustomSceneManager.SelectedScene"/> is saved and triggerable by a particular 
    /// button in the new scene (see implementation <see cref="CustomSceneManager.SwitchIntermediateScene"/>).<br/><br/>
    /// </summary>
    public void StartSelectedScene() => CustomSceneManager.SwitchIntermediateScene(); 
}