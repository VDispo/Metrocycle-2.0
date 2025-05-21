using UnityEngine;
using Metrocycle;

/// <summary>
/// A non-static script to be able to place listeners to button on-clicks!
/// </summary>
public class ScenarioSceneStarter : MonoBehaviour
{
    [HideInInspector] public string selectedSceneNoIntermediate = string.Empty;

    /// <summary>
    /// For direct scene switching.
    /// </summary>
    public void SaveSelectedScene(string sceneName)
    {
        selectedSceneNoIntermediate = sceneName;
        CustomSceneManager.SelectedScene = string.Empty;
        Debug.Log("Selected scene: " + selectedSceneNoIntermediate);
    }

    /// <summary>
    /// For scene switching with intermdiary scene (i.e., blowbagets).
    /// </summary>
    public void SaveSelectedSceneForIntermediation(string sceneName)
    {
        selectedSceneNoIntermediate = string.Empty;
        CustomSceneManager.SelectedScene = sceneName;
        Debug.Log("Selected scene with intermediate: " + CustomSceneManager.SelectedScene);
    }

    /// <summary>
    /// Note that the <see cref="CustomSceneManager.SelectedScene"/> is saved and triggerable by a particular 
    /// button in the new scene (see implementation <see cref="CustomSceneManager.SwitchIntermediateScene"/>).<br/><br/>
    /// 
    /// As of now, only <see cref="BikeType.Motorcycle"/> should have intermediate scene.
    /// </summary>
    /// <param name="playIntermediateScene"/>
    public void StartSelectedScene()
    {
        if (!string.IsNullOrEmpty(selectedSceneNoIntermediate))
            CustomSceneManager.SwitchScene(selectedSceneNoIntermediate); // no blowbagets
        else CustomSceneManager.SwitchIntermediateScene(); // yes blowbagets
    }

    /// <summary>
    /// Version that combines saving and starting immediately; for NO blowbagets. Only used for tutorial mode.
    /// </summary>
    public void StartSceneDirect(string sceneName)
    {
        SaveSelectedScene(sceneName);
        CustomSceneManager.SwitchScene(selectedSceneNoIntermediate); 
    }

    /// <summary>
    /// Version that combines saving and starting immediately; for YES blowbagets. Only used for tutorial mode.
    /// </summary>
    public void StartSceneDirectWithIntermediation(string sceneName)
    {
        SaveSelectedSceneForIntermediation(sceneName);
        CustomSceneManager.SwitchIntermediateScene();
    }
        
}

/// Deprecated implementation since on-click listeners dont serialize functions with enum parameters:
//[Serializable]
//public class SceneNames
//{
//    [SerializedDictionary("Scenario", "Scene")]
//    public SerializedDictionary<SceneScenarioTypes, string> sceneNames;
//}
//[SerializedDictionary("Bike Type", "Scenes")] 
//[SerializeField] private SerializedDictionary<BikeType, SceneNames> sceneNamesPerBikeType;

//public void StartBicycleScene(SceneScenarioTypes scenario) => 
//    StartSelectedScene(BikeType.Bicycle, scenario, playIntermediateScene:false  );

//public void StartMotorcycleScene(SceneScenarioTypes scenario) =>
//    StartSelectedScene(BikeType.Motorcycle, scenario, playIntermediateScene:true);

///// <summary>
///// For now, only <see cref="BikeType.Motorcycle"/> has intermediate scene. 
///// </summary>
//private void StartSelectedScene(BikeType bikeType, SceneScenarioTypes scenarioType, bool playIntermediateScene = false)
//{
//    if (playIntermediateScene)
//        CustomSceneManager.SwitchIntermediateScene(sceneNamesPerBikeType[bikeType].sceneNames[scenarioType]);
//    else _ = CustomSceneManager.SwitchScene(sceneNamesPerBikeType[bikeType].sceneNames[scenarioType]);
//}