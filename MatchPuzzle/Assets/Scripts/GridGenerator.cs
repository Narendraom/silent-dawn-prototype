using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Setup")]

    [SerializeField]
    GameObject cardPrefab;

    
    private Vector2Int gridSize = new Vector2Int(2, 2); // e.g., 2x2, 2x3, 5x6
    public Sprite[] cardSprites; // Assign unique front images

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        int totalCards = gridSize.x * gridSize.y;
        int totalPairs = totalCards / 2;

        // Create a deck with pairs
        int[] deck = new int[totalCards];
        for (int i = 0; i < totalPairs; i++)
        {
            deck[i * 2] = i;
            deck[i * 2 + 1] = i;
        }

        // Shuffle deck
        for (int i = 0; i < deck.Length; i++)
        {
            int rand = Random.Range(0, deck.Length);
            int temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }

        // Spawn cards
        for (int i = 0; i < deck.Length; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, transform);
            Card card = cardObj.GetComponent<Card>();
           // card.CardInit(deck[i], cardSprites[deck[i]]);
          
        }

        // Auto-scale with GridLayoutGroup
        var grid = GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            RectTransform rt = GetComponent<RectTransform>();
            float cellWidth = rt.rect.width / gridSize.x;
            float cellHeight = rt.rect.height / gridSize.y;
            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
