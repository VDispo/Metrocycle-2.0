using UnityEngine;

public class CollisionWithObstacles : MonoBehaviour
{
    public PopupType popupType = PopupType.ERROR;

    void OnCollisionEnter (Collision other)
    {
        // HACK: hardcode collision to only AI cars and obstacles for now
        //       may be use a layer mask parameter?
        const int layer_AITraffic = 9;
        const int layer_obstacles = 10;

        int otherLayer = other.gameObject.layer;
        if (!(otherLayer == layer_AITraffic
            || otherLayer == layer_obstacles)
        ) {
            return;
        }

        Debug.Log("Obstacle hit by Layer: " + other.gameObject.layer + other.gameObject.name);

        string otherDescription = "";
        bool isVehicle = false;
        switch (otherLayer) {
            case layer_AITraffic:
                otherDescription = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "collider3");
                isVehicle = true;

                GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.COLLISION_AIVEHICLE);
                break;
            case layer_obstacles:
                string tag = other.gameObject.tag;
                Debug.Log("TAG: " + tag);
                if (tag == "Side Obstacle") {
                    otherDescription = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "collider1");
                } else {
                    otherDescription = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "collider2");
                }

                GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.COLLISION_OBSTACLE);
                break;
            default:
                break;
        }

        string title = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "collisionTitle");
        string text1 = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "collisionDescription");
        string text2 = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "isVehicle");
        GameManager.Instance.PopupSystem.popWithType(popupType,
            title, 
            string.Format(text1, otherDescription) + (isVehicle ? text2 : "")
        );
    }
}
