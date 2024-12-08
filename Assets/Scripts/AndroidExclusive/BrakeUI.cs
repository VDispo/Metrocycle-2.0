using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeUI : MonoBehaviour
{
    public bool isPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonDown()
    {
        isPressed = true;
    }

    public void OnButtonUp()
    {
        isPressed = false;
    }
}
