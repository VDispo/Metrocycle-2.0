using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeUI : MonoBehaviour
{
    public bool isPressed = false;
    public bool isLeftBrake = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonDown()
    {
        isPressed = true;
        Debug.Log("Brake Pressed");

        // Get the GameObject associated with this script
        GameObject brakeObject = this.gameObject;

        if (isLeftBrake) {
            // Rotate the brake to the left
            brakeObject.transform.Rotate(new Vector3(0, 0, 20));
        } else {
            // Rotate the brake to the right
            brakeObject.transform.Rotate(new Vector3(0, 0, -20));
        }
    }

    public void OnButtonUp()
    {
        isPressed = false;
        Debug.Log("Brake Released");

        // Get the GameObject associated with this script
        GameObject brakeObject = this.gameObject;

        // Animate a rotation back to the original position
        StartCoroutine(RotateBrake(brakeObject, Quaternion.identity));
    }

    private IEnumerator RotateBrake(GameObject brakeObject, Quaternion endRotation)
    {
        float duration = 0.1f; // Duration of the rotation in seconds
        float elapsedTime = 0.0f;
        Quaternion startRotation = brakeObject.transform.rotation;

        while (elapsedTime < duration)
        {
            brakeObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        brakeObject.transform.rotation = endRotation;
    }
}
