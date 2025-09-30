using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Card Match/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Grid Settings")]
    public int rows = 4;
    public int columns = 4;

    [Header("Scoring")]
    public int matchScore = 100;
    public int mismatchPenalty = 10;
    public int comboMultiplier = 50;

    [Header("Timing")]
    public float mismatchDelay = 1.0f;

    [Header("Card Sprites")]
    public Sprite[] cardSprites;

    public int TotalCards => rows * columns;
    public int TotalPairs => TotalCards / 2;

    public void Validate()
    {
        if (TotalCards % 2 != 0)
        {
            Debug.LogError("Total cards must be even!");
        }

        if (cardSprites.Length < TotalPairs)
        {
            Debug.LogError($"Need at least {TotalPairs} card sprites!");
        }
    }
}