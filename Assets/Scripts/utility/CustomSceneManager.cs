using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is mainly responsible for (1) loading scenes with loading screen via async operations (see <see cref="LoadingManager"/>),
/// and (2) loading intermediary scenes (i.e., blowbagets).
/// </summary>
public static class CustomSceneManager
{
    public const string intermediateSceneName = "BLOWBAGETS";
    public static string SelectedScene = string.Empty;

    /// <summary>
    /// Load scene (async) with loading screen if available.
    /// </summary>
    public static void SwitchScene(string sceneName) => _ = LoadSceneAsync(sceneName);
    //public static void SwitchScene(string sceneName) => LoadingManager.Instance.StartLoadSceneAsync(sceneName);

    /// <summary>
    /// Save a selected scene for a specific button onclick (see <see cref="PreRidingAssessmentUiHandler.Awake"/>), 
    /// and intermediately play a set scene (<see cref="intermediateSceneName"/>).
    /// </summary>
    public static void SwitchIntermediateScene() => SwitchScene(intermediateSceneName);

    /// <summary>
    /// Restart the active scene.
    /// </summary>
    public static void ReloadActiveScene() => SwitchScene(SceneManager.GetActiveScene().name);

    /// <summary>
    /// Allows loading of scene with a loading screen.
    /// </summary>
    private static async Task LoadSceneAsync(string name, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (LoadingManager.Instance) LoadingManager.Instance.StartLoading();
        try
        {
            AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(name, mode)
                ?? throw new Exception($"Could not retrieve scene via '{name}'");

            sceneLoadOp.allowSceneActivation = false;

            float targetProgress;
            float previousTargetProgress = -1f;

            while (!sceneLoadOp.isDone)
            {
                await Task.Yield();

                targetProgress =
                    sceneLoadOp.progress == 0 ? 0.05f :
                    sceneLoadOp.progress < 0.9f ? sceneLoadOp.progress : 1f;

                if (targetProgress != previousTargetProgress)
                {
                    if (LoadingManager.Instance) LoadingManager.Instance.Loading(targetProgress);
                    previousTargetProgress = targetProgress;
                }

                if (sceneLoadOp.progress >= 0.9f)
                    sceneLoadOp.allowSceneActivation = true;
            }
        }
        finally
        {
            if (LoadingManager.Instance) LoadingManager.Instance.EndLoading();
        }
    }
}
