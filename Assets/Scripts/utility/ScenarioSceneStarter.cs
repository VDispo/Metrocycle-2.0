using UnityEngine;

/// <summary>
/// A non-static script to be able to place listeners to button on-clicks!
/// </summary>
public class ScenarioSceneStarter : MonoBehaviour
{
    /// <summary>
    /// For scene switching later on.
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