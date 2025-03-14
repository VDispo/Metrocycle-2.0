using UnityEngine;

public class ForbiddenLaneChangeCheck : MonoBehaviour
{
    private bool hasCrossedLine;

    void Start() {
        hasCrossedLine = false;
    }

    public void enteredLane(GameObject lane) {
        if (hasCrossedLine) {
            Debug.Log("Illegal lane change!!!!");

            string title = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "solidLineTitle");
            string text = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "solidLineDescription");

            GameManager.Instance.PopupSystem.popError(title, text);

            GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.LANECHANGE_NOTALLOWED);
        }

        hasCrossedLine = false;
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log($"Solid line crossed {other}");
        hasCrossedLine = true;
    }
}
