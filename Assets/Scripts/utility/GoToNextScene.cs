using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public static nextScene Instance;
    public static string SelectedScene = string.Empty;

    private void Start()
    {
        Debug.Log($"[{GetType().FullName}] starrttt with \'{SelectedScene}\' selected");

        Instance = this;
        if (SelectedScene != string.Empty)
            PreRidingAssessmentUiHandler.Instance.finishSceneBtn.
                onClick.AddListener(() => LoadSelectedScene(SelectedScene));
    }

    /// <summary>
    /// Loads a precursor/intermediate scene (i.e., blowbagets/character customization) 
    /// AND saves the previously selected scene as the next scene.
    /// </summary>
    public void LoadIntermediateScene()
    {
        SceneManager.LoadScene("BLOWBAGETS");
    }

    /// <summary>
    /// Loads the saved selected scene from <see cref="LoadIntermediateScene"/> with the appropriate customizations selected.
    /// </summary>
    public void LoadSelectedScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        // TODO: for now, insert here, but must be renamed and reimplemented properly
        if (sceneName.Contains("Motorcycle"))
        {
            SelectedScene = sceneName;
            LoadIntermediateScene();
        }
        else LoadSelectedScene(sceneName);
    }   
}
