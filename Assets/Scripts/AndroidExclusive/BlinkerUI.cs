using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkerUI : MonoBehaviour
{
    [SerializeField] private Slider blinkerSlider;
    // Start is called before the first frame updat
    public bool isBlinkerActive = false;
    public bool leftBlinkerActive = false;
    void Start()
    {
        isBlinkerActive = false;
        blinkerSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(blinkerSlider.progress);
        if (blinkerSlider.value == 0 && !isBlinkerActive)
        {
            isBlinkerActive = true;
            leftBlinkerActive = true;
        }
        else if (blinkerSlider.value == 2 && !isBlinkerActive)
        {
            isBlinkerActive = true;
            leftBlinkerActive = false;
        } else {
            isBlinkerActive = false;
        }
    }
}
