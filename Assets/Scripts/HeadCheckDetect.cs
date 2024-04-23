using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheckDetect : MonoBehaviour
{
    public Direction direction;

    public PopupType popupType;
    [TextArea(3, 10)] public string popupTitle;
    [TextArea(3, 10)] public string popupText;

    void OnTriggerEnter (Collider other) {
        bool isValid = isValid = GameManager.Instance.isDoingHeadCheck(direction);

        if (!isValid) {
            GameManager.Instance.PopupSystem.popWithType(popupType, popupTitle, popupText);
        }

        gameObject.SetActive(false);
    }
}
