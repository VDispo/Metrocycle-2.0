using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple randomizer for picking random sprites. Additionally stores sprites as pass or fail, 
/// aligned with passing or failing a minigame. (Initial pass/fail randomness should be provided 
/// by <see cref="MinigameBase"/> or children).
/// </summary>
public class SpriteRandomizer : MonoBehaviour
{
    [Header("Refs")]
    public List<Sprite> passingSprites;
    public List<Sprite> failingSprites;

    public Sprite SelectRandomSprite(bool passing)
    {
        return passing ?
            passingSprites[Random.Range(0, passingSprites.Count)] :
            failingSprites[Random.Range(0, failingSprites.Count)];
    }
}

//public Sprite SelectRandomSprite_SelfRandomize()
//{
//    int randomIdx = Random.Range(0, passingSprites.Count + failingSprites.Count);
//    Sprite randomSprite = randomIdx < passingSprites.Count ? 
//        passingSprites[randomIdx] : failingSprites[randomIdx];

//    if (randomSprite) return randomSprite;
//    else return SelectRandomSprite_SelfRandomize(); // try again if empty sprite
//}