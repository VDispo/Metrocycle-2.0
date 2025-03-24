using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkerCheck : MonoBehaviour
{
    public Direction whichBlinker;
    private blinkers blinkerScript;

    public PopupType popupType = PopupType.PROMPT;
    public string PopupTitle { get; set; }
    public string PopupText { get; set; }

    public int progress_order = 0;
    public int progress_total = 0;

    void Start() {
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
    }

    void OnTriggerEnter (Collider other) {
        if ((whichBlinker == Direction.RIGHT && blinkerScript.rightStatus == 0)
            || (whichBlinker == Direction.LEFT && blinkerScript.leftStatus == 0)
        ) {
            GameManager.Instance.PopupSystem.popWithType(popupType, PopupTitle, PopupText);

            GameManager.Instance.setErrorReason(
                whichBlinker == Direction.LEFT
                    ? Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER
                    : Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER
            );
        }

        gameObject.SetActive(false);

        if (progress_order > 0 && progress_total > 0) {
            GameManager.Instance.updateProgressBar(progress_order, progress_total);
        }
    }
}
