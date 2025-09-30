using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    private GameConfig[] gameConfig;
    [SerializeField]
    private Transform cardContainer;
    [SerializeField]
    private GameObject cardPrefab;

    [Header("UI")]
    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI comboText;

    private List<Card> cards = new List<Card>();
    private Card firstCard;
    private Card secondCard;
    private GameConfig config;
    private int level;


    private int score = 0;
    private int combo = 0;
    private int matchesFound = 0;

    private bool isProcessing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        level = Random.Range(0, gameConfig.Length);
       

        if (SaveManager.Instance.HasSavedGame())
        {
            LoadGame();
        }
        else
        {
            score = 0;
            combo = 0;
            matchesFound = 0;
        }

        config = gameConfig[level];
        config.Validate();
        ClearBoard();
        GenerateCards();
        SetupGridLayout();

        UpdateUI();
    }

    private void ClearBoard()
    {
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    private void GenerateCards()
    {
        List<int> cardIds = new List<int>();

        // Create pairs
        for (int i = 0; i < config.TotalPairs; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        // Shuffle
        for (int i = 0; i < cardIds.Count; i++)
        {
            int temp = cardIds[i];
            int randomIndex = Random.Range(i, cardIds.Count);
            cardIds[i] = cardIds[randomIndex];
            cardIds[randomIndex] = temp;
        }

        // Instantiate cards
        for (int i = 0; i < cardIds.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            Card card = cardObj.GetComponent<Card>();

            int spriteIndex = cardIds[i] % config.cardSprites.Length;
            card.Initialize(cardIds[i], config.cardSprites[spriteIndex]);
            cards.Add(card);
        }
    }

    private void SetupGridLayout()
    {
        GridLayoutGroup grid = cardContainer.GetComponent<GridLayoutGroup>();

        if (grid == null)
        {
            grid = cardContainer.gameObject.AddComponent<GridLayoutGroup>();
        }

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = config.columns;

        // Calculate cell size based on container size
        RectTransform containerRect = cardContainer.GetComponent<RectTransform>();
        float width = containerRect.rect.width;
        float height = containerRect.rect.height;

        float cellWidth = (width - grid.spacing.x * (config.columns - 1)) / config.columns;
        float cellHeight = (height - grid.spacing.y * (config.rows - 1)) / config.rows;

        float cellSize = Mathf.Min(cellWidth, cellHeight) * 0.95f;
        grid.cellSize = new Vector2(cellSize, cellSize);
    }

    public void OnCardClicked(Card card)
    {
        if (isProcessing) return;

        if (firstCard == null)
        {
            firstCard = card;
            firstCard.Flip();
            AudioManager.Instance?.PlayFlipSound();
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            secondCard.Flip();
            AudioManager.Instance?.PlayFlipSound();
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isProcessing = true;

        // Wait for flip animation
        yield return new WaitForSeconds(0.5f);

        if (firstCard.CardId == secondCard.CardId)
        {
            // Match!
            firstCard.SetMatched();
            secondCard.SetMatched();

            combo++;
            score += config.matchScore + (combo * config.comboMultiplier);
            matchesFound++;

            AudioManager.Instance?.PlayMatchSound();

            if (matchesFound >= config.TotalPairs)
            {
                yield return new WaitForSeconds(0.5f);
                OnGameComplete();
            }
        }
        else
        {
            // Mismatch
            combo = 0;
            score = Mathf.Max(0, score - config.mismatchPenalty);

            AudioManager.Instance?.PlayMismatchSound();

            yield return new WaitForSeconds(config.mismatchDelay);

            firstCard.FlipBack();
            secondCard.FlipBack();
        }

        firstCard = null;
        secondCard = null;
        isProcessing = false;

        UpdateUI();
        SaveGame();
    }

    private void OnGameComplete()
    {
        AudioManager.Instance?.PlayGameOverSound();
        SaveManager.Instance.ClearSave();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (comboText != null)
            comboText.text = combo > 0 ? $"Combo: x{combo}" : "";
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            score = score,
            combo = combo,
            matchesFound = matchesFound,
            level = level,
            cardStates = cards.Select(c => new CardState
            {
                cardId = c.CardId,
                isMatched = c.IsMatched,
                isFlipped = c.IsFlipped
            }).ToList()
        };

        SaveManager.Instance.SaveGame(saveData);
    }

    public void LoadGame()
    {
        GameSaveData saveData = SaveManager.Instance.LoadGame();

        if (saveData != null)
        {
            score = saveData.score;
            combo = saveData.combo;
            matchesFound = saveData.matchesFound;
            level = saveData.level;
            for (int i = 0; i < cards.Count && i < saveData.cardStates.Count; i++)
            {
                if (saveData.cardStates[i].isMatched)
                {
                    cards[i].Flip(true);
                    cards[i].SetMatched();
                }
            }
        }
    }

    public void RestartGame()
    {
        SaveManager.Instance.ClearSave();
        InitializeGame();
    }
}