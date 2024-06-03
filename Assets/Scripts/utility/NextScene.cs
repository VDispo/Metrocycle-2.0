using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown("n"))
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1) % 3);
    }
}
