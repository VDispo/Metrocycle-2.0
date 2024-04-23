using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkerCheck : MonoBehaviour
{
    public Direction whichBlinker;
    private blinkers blinkerScript;

    public PopupType popupType = PopupType.PROMPT;
    [TextArea(3, 10)] public string popupTitle;
    [TextArea(3, 10)] public string popupText;

    void Start() {
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
    }

    void OnTriggerEnter (Collider other) {
        if ((whichBlinker == Direction.RIGHT && blinkerScript.rightStatus == 0)
            || (whichBlinker == Direction.LEFT && blinkerScript.leftStatus == 0)
        ) {
            GameManager.Instance.PopupSystem.popWithType(popupType, popupTitle, popupText);
        }

        gameObject.SetActive(false);
    }
}
