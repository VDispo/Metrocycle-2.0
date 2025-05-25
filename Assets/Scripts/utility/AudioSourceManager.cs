using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource buttonClick; // currently not used by any buttons via this variable (theyre all set inside the button onclicks manually)

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        Instance = this;

        DontDestroyOnLoad(this);
        PlayBgmBasedOnScene();
    }

    /// <summary>
    /// Continuously play the BGM on scenes not containing the "cycle" in the name.
    /// </summary>
    public static void PlayBgmBasedOnScene()
    {
        if (Instance)
        {
            if (SceneManager.GetActiveScene().name.Contains("cycle"))
            {
                Instance.bgm.Stop();
            }
            else
            {
                if (!Instance.bgm.isPlaying)
                    Instance.bgm.Play();
            }
        }
        else Debug.LogWarning($"[{nameof(AudioSourceManager)}-static] Instance not defined, hence audio sources may not play appropriately (be sure to load Title scene once to set)");
    }
}