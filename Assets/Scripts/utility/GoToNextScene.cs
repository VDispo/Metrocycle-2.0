using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public static nextScene Instance;

    private void Start()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }


    /// <summary>
    /// Loads a precursor/intermediate scene (i.e., blowbagets/character customization) 
    /// AND saves the previously selected scene as the next scene.
    /// </summary>
    public void LoadIntermediateScene()
    {
        // Avatar Customization
        // Blowbagets
    }

    /// <summary>
    /// Loads the saved selected scene from <see cref="LoadIntermediateScene"/> with the appropriate customizations selected.
    /// </summary>
    public void LoadSelectedScene()
    {
        /// Avatar Customization
        // option A: list of all customizations references to assets THEN

        /// Blowbagets
        // pass or not (interlock: must pass first before playing)
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
