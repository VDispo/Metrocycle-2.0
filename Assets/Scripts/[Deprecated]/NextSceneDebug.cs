using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// For debugging (Metrocycle 1.0 development)
/// </summary>
public class NextSceneDebug : MonoBehaviour
{
#if !UNITY_ANDROID
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown("n"))
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1) % 3);
    }
#endif
}
