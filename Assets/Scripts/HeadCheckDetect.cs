using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheckDetect : MonoBehaviour
{
    public Direction direction;

    public PopupType popupType;
    public string PopupTitle { get; set; }
    public string PopupText { get; set;}

    void OnTriggerEnter (Collider other) {
        bool isValid = GameManager.Instance.isDoingHeadCheck(direction);

        if (!isValid) {
            GameManager.Instance.PopupSystem.popWithType(popupType, PopupTitle, PopupText);

            GameManager.Instance.setErrorReason(
                direction == Direction.LEFT
                ? Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK
                : Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK
            );
        }

        gameObject.SetActive(false);
    }
}
