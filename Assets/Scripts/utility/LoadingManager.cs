using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for the loading screen. Only use via <see cref="CustomSceneManager"/>. Also sets the target framerate of the app to 60.
/// </summary>
public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("Refs")]
    [SerializeField] private GameObject loadingScreen; // Canvas
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Values")]
    [SerializeField][Range(0, 10)] private float smoothLoadingSpeed = 2f;
    private Coroutine currLoadingAnimCoroutine;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EndLoading();

            Application.targetFrameRate = 60;
        }
        else Destroy(gameObject); // dont allow recreation / reset of this script, for consistency
    }

    public void StartLoading() => loadingScreen.SetActive(true);

    public void Loading(float target)
    {
        if (currLoadingAnimCoroutine != null) StopCoroutine(currLoadingAnimCoroutine);
        currLoadingAnimCoroutine = StartCoroutine(AnimateLoadingAndUpdateUi(target));
    }

    private IEnumerator AnimateLoadingAndUpdateUi(float target)
    {
        while (loadingBar.value != target)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, target, smoothLoadingSpeed * Time.unscaledDeltaTime);
            loadingText.text = (loadingBar.value * 100).ToString("F0") + "%";
            yield return null;
        }
    }

    public void EndLoading()
    {
        loadingScreen.SetActive(false);
        loadingBar.value = 0;
        loadingText.text = "0%";
        AudioSourceManager.PlayBgmBasedOnScene();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        StopAllCoroutines();
    }
}
