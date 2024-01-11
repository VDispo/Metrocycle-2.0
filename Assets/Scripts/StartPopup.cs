using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPopup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // pause game at start
        Time.timeScale = 0;
    }

    public void startGame()
    {
        Time.timeScale = 1;
    }
}
