using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HeadCheckUI : MonoBehaviour
{
    public bool isButtonPressed = false;
    public bool simulateKeyUp = false;
    public Button headCheckButton;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnButtonDown(BaseEventData eventData)
    {
        isButtonPressed = true;
    }

    public void OnButtonUp(BaseEventData eventData)
    {
        isButtonPressed = false;
        simulateKeyUp = true;
    }
}
