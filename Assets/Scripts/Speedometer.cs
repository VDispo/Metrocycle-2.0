using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

    public GameObject bike;
    Rigidbody rb;

    private const float MAX_SPEED_ANGLE = -20;
    private const float ZERO_SPEED_ANGLE = 230;

    private Transform needleTranform;
    private Transform speedLabelTemplateTransform;
    public float speedLabelDistance;

    private float speedMax;
    private float speed;

    private double speedTotal;
    private int numSpeedSamples;

    private void Awake() {
        rb = bike.GetComponent<Rigidbody>();

        needleTranform = transform.Find("needle");
        speedLabelTemplateTransform = transform.Find("speedLabelTemplate2");
        speedLabelTemplateTransform.gameObject.SetActive(false);

        speedLabelDistance = (speedLabelTemplateTransform.Find("speedText").transform.position
                              - speedLabelTemplateTransform.Find("dashImage").transform.position).magnitude;

        speed = 0f;
        speedMax = 120f;
        
        CreateSpeedLabels();
    }

    private void Update() {
        Vector3 vel = rb.velocity;
        // Debug.Log(vel.magnitude);

        speed = vel.magnitude*3;
        if (speed > speedMax) speed = speedMax;

        needleTranform.eulerAngles = new Vector3(0,0,GetSpeedRotation());

        speedTotal += speed;
        ++numSpeedSamples;
    }

    private void CreateSpeedLabels() {
        int labelAmount=6;
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        for (int i=0;i <= labelAmount; i++){
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE - labelSpeedNormalized * totalAngleSize;

            Transform speedText = speedLabelTransform.Find("speedText");

            speedLabelTransform.eulerAngles = new Vector3(0,0,speedLabelAngle);
            speedText.GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalized * speedMax).ToString();
            speedText.eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);

            speedText.transform.position = Vector3.MoveTowards(speedText.transform.position, needleTranform.position, speedLabelDistance);
        }   
        needleTranform.SetAsLastSibling();
    }

    private float GetSpeedRotation() {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = speed / speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }

    public double GetAvgSpeed()
    {
        return speedTotal / ((double) numSpeedSamples);
    }
}
