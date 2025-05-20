using UnityEngine;
using UnityEngine.UI;

public class MinigameButton : MinigameBase
{
    public override MinigameType Type => MinigameType.Button;

    [Header("Button Minigame")]
    [SerializeField] private Button button;
    [SerializeField][Range(0, 1)] private float buttonChanceToFail = 0.4f;

    public void PressButton()
    {
        button.gameObject.SetActive(false);
        
        int rand = Random.Range(0, 100);
        if (rand < (buttonChanceToFail * 100))
        {
            Debug.Log($"[{GetType().FullName}] rolled a {rand}, something wrong!");
            Fail();
        }
        else
        {
            Debug.Log($"[{GetType().FullName}] rolled a {rand}, nothing wrong!");
            Pass();
        }
    }
}
