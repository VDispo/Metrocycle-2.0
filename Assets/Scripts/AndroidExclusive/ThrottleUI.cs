using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThrottleUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Slider throttleSlider;

    [Header("Values")]
    [SerializeField][Range(0,10)] private float unthrottleRate;
    private bool isThrottling;

    private void Update()
    {
        if (throttleSlider.value > 0 && !isThrottling)
            throttleSlider.value -= unthrottleRate * Time.deltaTime;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isThrottling = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isThrottling = false;
    }

    public void ResetThrottle()
    {
        isThrottling = false;
    }
}
