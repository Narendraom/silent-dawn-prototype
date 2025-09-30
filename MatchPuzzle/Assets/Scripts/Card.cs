using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Card : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private GameObject cardBack;
    [SerializeField]
    private GameObject cardFront;

    [Header("Settings")]
    [SerializeField]
    private float flipDuration = 0.3f;

    private int cardId;
    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;

    public int CardId => cardId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;
    public bool IsFlipping => isFlipping;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
    }

    public void Initialize(int id, Sprite frontSprite)
    {
        cardId = id;
        cardImage.sprite = frontSprite;
        isFlipped = false;
        isMatched = false;
        cardFront.SetActive(false);
        cardBack.SetActive(true);
    }

    private void OnCardClicked()
    {
       
        if (!isFlipping && !isFlipped && !isFlipped)
        {
            GameManager.Instance.OnCardClicked(this);
        }
    }

    public void Flip(bool immediate = false)
    {
        if (immediate)
        {
            isFlipped = !isFlipped;
            cardFront.SetActive(isFlipped);
            cardBack.SetActive(!isFlipped);
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            StartCoroutine(FlipAnimation());
        }
    }

    private IEnumerator FlipAnimation()
    {
        isFlipping = true;

        float elapsed = 0f;
        Quaternion startRot = transform.localRotation;
        Quaternion midRot = startRot * Quaternion.Euler(0, 90, 0);

        // First half - hide current side
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            transform.localRotation = Quaternion.Lerp(startRot, midRot, t);
            yield return null;
        }

        // Switch sprites at middle point
        isFlipped = !isFlipped;
        cardFront.SetActive(isFlipped);
        cardBack.SetActive(!isFlipped);

        // Second half - show new side
        elapsed = 0f;
        Quaternion endRot = startRot * Quaternion.Euler(0, 180, 0);
        if (!isFlipped) endRot = startRot;

        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            transform.localRotation = Quaternion.Lerp(midRot, endRot, t);
            yield return null;
        }

        transform.localRotation = endRot;
        isFlipping = false;

        AudioManager.Instance?.PlayFlipSound();
    }

    public void SetMatched()
    {
        isMatched = true;
        StartCoroutine(MatchAnimation());
    }

    private IEnumerator MatchAnimation()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 1.1f;

        // Scale up
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Scale back
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        transform.localScale = startScale;
    }

    public void FlipBack()
    {
        if (isFlipped && !isMatched)
        {
            Flip(immediate:true);
        }
    }

    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        isFlipping = false;
        cardFront.SetActive(false);
        cardBack.SetActive(true);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}