using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using PrimeTween;
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GridLayoutController gridLayout;
    public GameObject cardPrefab;
    public Transform cardGrid;
    public List<CardData> cardPool;
    public int rows = 2;
    public int columns = 2;

    private List<CardDisplay> flippedCards = new List<CardDisplay>();
    private bool isCheckingMatch;

    public TextMeshProUGUI scoreText; 
    private int score = 0;
    private int matchedPairs = 0;
    private int totalPairs = 0;

    public TextMeshProUGUI turnsText;
    private int turnsTaken = 0;
    public TextMeshProUGUI comboText;
    private int comboStreak = 0;
    private Tween comboTween;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(DelayedSetup());
    }

    private System.Collections.IEnumerator DelayedSetup()
    {
        yield return new WaitForEndOfFrame();
        SetupGame(rows, columns);
    }

    public void SetupGame(int rowCount, int columnCount)
    {
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        gridLayout.ConfigureGrid(rowCount, columnCount);

        int totalCards = rowCount * columnCount;
        int pairsNeeded = totalCards / 2;

        totalPairs = totalCards / 2;
        matchedPairs = 0;
        score = 0;
        UpdateScoreUI();

        List<CardData> selectedCards = new List<CardData>();
        List<CardData> deck = new List<CardData>();

       
        for (int i = 0; i < pairsNeeded; i++)
        {
            var card = cardPool[i % cardPool.Count];
            selectedCards.Add(card);
        }

        foreach (var card in selectedCards)
        {
            deck.Add(card);
            deck.Add(card);
        }

        Shuffle(deck);

        foreach (var card in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            display.SetData(card);
        }

        flippedCards.Clear();
        isCheckingMatch = false;
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    public void OnCardSelected(CardDisplay card)
    {
        if (isCheckingMatch || flippedCards.Contains(card))
            return;

        card.FlipToFront();
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            turnsTaken++;
            UpdateTurnsUI();
            isCheckingMatch = true;
            StartCoroutine(CheckMatch());
        }
    }

    private System.Collections.IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.6f);

        if (flippedCards[0].GetData().cardId == flippedCards[1].GetData().cardId)
        {
            comboStreak++;
            ShowCombo(comboStreak);
            flippedCards[0].Lock();
            flippedCards[1].Lock();

            //score += 100;
            score += 100 + comboStreak * 10;
            matchedPairs++;
            UpdateScoreUI();
        }
        else
        {
            comboStreak = 0;
            HideCombo();
            flippedCards[0].FlipToBack();
            flippedCards[1].FlipToBack();
        }

        flippedCards.Clear();
        isCheckingMatch = false;
    }
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score\n{score}";
    }
    private void UpdateTurnsUI()
    {
        turnsText.text = $"Turns:\n{turnsTaken}";
    }
    private void ShowCombo(int combo)
    {
        if (combo <= 1)
            return;

        comboText.text = $"Combo x{combo}!";
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
        comboText.transform.localScale = Vector3.one;

        Sequence.Create()
            .Group(Tween.Color(comboText, new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f), 0.2f))
            .Group(Tween.Scale(comboText.transform, Vector3.one * 1.3f, 0.2f, Ease.OutBack))
            .ChainDelay(1.0f)
            .Chain(Tween.Color(comboText, new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f), 0.3f));
    }
    private void HideCombo()
    {
        comboText.text = "";
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
    }
}
