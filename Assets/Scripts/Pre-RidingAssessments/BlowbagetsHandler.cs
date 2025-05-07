using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlowbagetsHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField][Tooltip("in order; this auto-sets the button listeners as well")] private Button[] blowbagetsButtons;
    
    public enum Blowbagets {
        Battery = 0,
        Lights = 1, 
        Oil = 2,
        Water = 3,
        Brakes = 4,
        Air = 5,
        Gas = 6,
        Engine = 7,
        Tires = 8,
        Self = 9
    }

    private void Start()
    {
        for (int i = 0; i < blowbagetsButtons.Length; i++)
        {
            Button button = blowbagetsButtons[i];
            if (button)
            {
                button.interactable = false;

                int _i = i; // cache to avoid closure issues
                button.onClick.AddListener(() => BlowbagetsButtonClick(_i));
            }
        }
        EnableNextButton(0);
    }

    /// <summary>
    /// Assumes that buttons have 2 texts (as children gameobjects), with the first one being the full text.
    /// </summary>
    private void EnableNextButton(int nextButtonIdx) 
    {
        if (nextButtonIdx < blowbagetsButtons.Length)
        {
            blowbagetsButtons[nextButtonIdx].interactable = true; // enable interaction
            blowbagetsButtons[nextButtonIdx].GetComponentInChildren<TextMeshProUGUI>().enabled = true; // show full text
        }
    }

    public void BlowbagetsButtonClick(int blowbagetsButtonIdx)
    {
        // do logic for each
        switch ((Blowbagets)blowbagetsButtonIdx) 
        {
            case Blowbagets.Battery:
                Debug.Log($"[{GetType().FullName}] battery!");
                break;
            case Blowbagets.Lights:
                Debug.Log($"[{GetType().FullName}] lights!");
                break;
            case Blowbagets.Oil:
                Debug.Log($"[{GetType().FullName}] oil!");
                break;
            case Blowbagets.Water:
                Debug.Log($"[{GetType().FullName}] water!");
                break;
            case Blowbagets.Brakes:
                Debug.Log($"[{GetType().FullName}] brakes!");
                break;
            case Blowbagets.Air:
                Debug.Log($"[{GetType().FullName}] air!");
                break;
            case Blowbagets.Gas:
                Debug.Log($"[{GetType().FullName}] gas!");
                break;
            case Blowbagets.Engine:
                Debug.Log($"[{GetType().FullName}] engine!");
                break;
            case Blowbagets.Tires:
                Debug.Log($"[{GetType().FullName}] tires!");
                break;
            case Blowbagets.Self:
                Debug.Log($"[{GetType().FullName}] self!");
                break;
            default:
                Debug.LogWarning($"[{GetType().FullName}] argument for BlowbagetsButtonClick is invalid");
                break;

        }

        int nextIdx = ++blowbagetsButtonIdx;
        if (nextIdx < blowbagetsButtons.Length)
        {
            // enable interaction for the next button
            EnableNextButton(nextIdx);
        }
        else
        {
            // finish blowbagets, go to next scene
        }
    }
}
