using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheckDetect : MonoBehaviour
{
    public Direction direction;
    public bool checkLookingForward = false;

    public PopupType popupType;
    [TextArea(3, 10)] public string popupTitle;
    [TextArea(3, 10)] public string popupText;

    void OnTriggerEnter (Collider other) {
        bool isValid = false;
        if (checkLookingForward) {
            isValid = GameManager.hasDoneHeadCheck(null);
        } else {
            isValid = GameManager.hasDoneHeadCheck(direction);;
        }

        Debug.Log("isValid " + isValid + "dir " + direction + "forward " + checkLookingForward + " left " + headCheckScript.isLookingLeft() + headCheckScript.isLookingRight());
        if (!isValid) {
            GameManager.Instance.PopupSystem.popWithType(popupType, popupTitle, popupText);
        }

        gameObject.SetActive(false);
    }
}
