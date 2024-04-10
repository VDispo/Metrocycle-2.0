using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheckDetect : MonoBehaviour
{
    public Blinker direction;
    public bool checkLookingForward = false;
    public HeadCheck headCheckScript;

    public PopupType popupType;
    [TextArea(3, 10)] public string popupTitle;
    [TextArea(3, 10)] public string popupText;

    void OnTriggerEnter (Collider other) {
        bool isValid = false;
        if (checkLookingForward) {
            isValid = headCheckScript.isLookingForward();
        } else {
            isValid = ((direction == Blinker.RIGHT && headCheckScript.isLookingRight())
                        || (direction == Blinker.LEFT && headCheckScript.isLookingLeft())
                      );
        }

        Debug.Log("isValid " + isValid + "dir " + direction + "forward " + checkLookingForward + " left " + headCheckScript.isLookingLeft() + headCheckScript.isLookingRight());
        if (!isValid) {
            GameManager.Instance.PopupSystem.popWithType(popupType, popupTitle, popupText);
        }

        gameObject.SetActive(false);
    }
}
