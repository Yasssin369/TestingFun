using UnityEngine;
using System.Collections.Generic;

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
    // private bool isCheckingMatch;

    private int score = 0;
    private int matchedPairs = 0;
    private int totalPairs = 0;

    private int turnsTaken = 0;
    private int comboStreak = 0;

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

        totalPairs = pairsNeeded;
        matchedPairs = 0;
        score = 0;
        turnsTaken = 0;
        comboStreak = 0;

        UIController.Instance.UpdateScore(score);
        UIController.Instance.UpdateTurns(turnsTaken);
        UIController.Instance.HideCombo();

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
        //isCheckingMatch = false;
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
        if (flippedCards.Contains(card))
            return;

        card.FlipToFront();
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            turnsTaken++;
            UIController.Instance.UpdateTurns(turnsTaken);

            //isCheckingMatch = true;
            StartCoroutine(CheckMatch());
        }
    }

    private System.Collections.IEnumerator CheckMatch()
    {
        var activeCards = flippedCards.FindAll(c => !c.IsLocked());
        if (activeCards.Count < 2)
            yield break;

        var card1 = activeCards[0];
        var card2 = activeCards[1];

        yield return new WaitForSeconds(0.5f); 

        if (card1.GetData().cardId == card2.GetData().cardId)
        {
            comboStreak++;
            UIController.Instance.ShowCombo(comboStreak);

            card1.Lock();
            card2.Lock();

            score += 100 + comboStreak * 10;
            matchedPairs++;
            AudioController.Instance.PlayMatch();
            UIController.Instance.UpdateScore(score);
            flippedCards.Remove(card1); 
            flippedCards.Remove(card2);
        }
        else
        {
            comboStreak = 0;
            UIController.Instance.HideCombo();
            AudioController.Instance.PlayMismatch();
            card1.FlipToBack();
            card2.FlipToBack();
            flippedCards.Remove(card1);
            flippedCards.Remove(card2);
        }

       // turnsTaken++;
        UIController.Instance.UpdateTurns(turnsTaken);

        
        if (flippedCards.FindAll(c => !c.IsLocked()).Count >= 2)
            StartCoroutine(CheckMatch());
        CheckGameOver();
    }
    private void CheckGameOver()
    {
        if (matchedPairs >= totalPairs)
        {
            UIController.Instance.ShowGameOver(score, turnsTaken);
        }
    }

}
