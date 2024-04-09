using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public popUp PopupSystem = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        PopupSystem = GameObject.Find("/popUp").GetComponent<popUp>();
        Debug.Log("PopupSystem Initialized " + PopupSystem);
    }
}
