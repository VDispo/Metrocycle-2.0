using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void PlayMotorcycleTutorial()
    {
        SceneManager.LoadSceneAsync(0);
    }
        public void PlayBicycleTutorial()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
