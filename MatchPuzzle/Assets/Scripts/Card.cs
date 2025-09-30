using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Setup")]
    public int cardID;                        // ID for matching

    [SerializeField]
    private Image cardImage;                  // image for this ID

    private bool isFlipped = false;
    private bool isMatched = false;

    private void Awake()
    {
        ShowBack();
    }

    public void CardInit(int id)
    {
        cardID = id;
        //TODO : assign sprite based on this id
    }

    public void OnCardClicked()
    {
        if (isMatched || isFlipped) return;

        FlipCard();
    }

    public void FlipCard()
    {
        //TODO: fliping Animations and matching logic 
        isFlipped = true;
    }

    public void ShowBack()
    {
        isFlipped = false;
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public bool IsFlipped() => isFlipped;
    public bool IsMatched() => isMatched;
}
