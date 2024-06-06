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

            GameManager.setErrorReason(
                direction == Direction.LEFT
                ? Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK
                : Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK
            );
        }

        gameObject.SetActive(false);
    }
}
