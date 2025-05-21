using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MinigameButton : MinigameBase
{
    public override MinigameType Type => MinigameType.Button;

    [Header("Button Minigame")]
    [SerializeField] private Button button;
    [SerializeField][Range(0, 1)] private float chanceToPass = 0.6f;

    [Space(10)]
    [Tooltip("If true, requires SpriteRandomizer component to be in the same GameObject as this")]
    [SerializeField] private bool randomizeSpriteUponActivation = false;
    protected SpriteRandomizer spriteRandomizer;

    protected override void Awake2()
    {
        spriteRandomizer = GetComponent<SpriteRandomizer>();
    }

    public void PressButton()
    {
        button.gameObject.SetActive(false);

        bool passing = Random.Range(0, 100) < (chanceToPass * 100);
        if (passing) Pass();
        else Fail();

        if (randomizeSpriteUponActivation)
        {
            Sprite newSprite = spriteRandomizer.SelectRandomSprite(passing);
            ShowActiveSprite(newSprite);
        }
    }

    /// <summary>
    /// Force update to a passing sprite.
    /// </summary>
    protected override void UpdateUiBasedOnState2()
    {
        if (randomizeSpriteUponActivation && state == MinigameState.Passed)
        {
            Sprite newSprite = spriteRandomizer.SelectRandomSprite(true);
            ShowActiveSprite(newSprite);
        }
    }
}
