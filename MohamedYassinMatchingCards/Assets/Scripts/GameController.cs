using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    private SaveData loadedData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(DelayedSetup());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
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

        List<CardData> deck = new List<CardData>();

        if (loadedData != null && loadedData.orderedCardIds != null && loadedData.orderedCardIds.Count == totalCards)
        {
            foreach (string cardId in loadedData.orderedCardIds)
            {
                var card = cardPool.Find(c => c.cardId == cardId);
                if (card != null)
                {
                    deck.Add(card);
                }
            }

            while (deck.Count < totalCards)
            {
                var fallback = cardPool[UnityEngine.Random.Range(0, cardPool.Count)];
                deck.Add(fallback);
            }
        }
        else
        {
            List<CardData> selectedCards = new List<CardData>();
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
        }

        foreach (var card in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            display.SetData(card);
        }

        flippedCards.Clear();
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

    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            rows = rows,
            columns = columns,
            score = score,
            turnsTaken = turnsTaken,
            comboStreak = comboStreak,
            matchedCardIds = new List<string>(),
            orderedCardIds = new List<string>()
        };

        foreach (Transform child in cardGrid)
        {
            CardDisplay display = child.GetComponent<CardDisplay>();
            if (display != null)
            {
                data.orderedCardIds.Add(display.GetData().cardId);

                if (display.IsLocked())
                {
                    data.matchedCardIds.Add(display.GetData().cardId);
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveGame", json);
        PlayerPrefs.Save();
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveGame"))
        {
            Debug.Log("No saved game found.");
            return;
        }

        string json = PlayerPrefs.GetString("SaveGame");
        loadedData = JsonUtility.FromJson<SaveData>(json);

        rows = loadedData.rows;
        columns = loadedData.columns;

        score = loadedData.score;
        turnsTaken = loadedData.turnsTaken;
        comboStreak = loadedData.comboStreak;

        SetupGame(rows, columns); // This will now use loadedData.orderedCardIds

        UIController.Instance.UpdateScore(score);
        UIController.Instance.UpdateTurns(turnsTaken);
        UIController.Instance.ShowCombo(comboStreak);

        int matchedCount = 0;
        foreach (Transform child in cardGrid)
        {
            CardDisplay display = child.GetComponent<CardDisplay>();
            if (display != null && loadedData.matchedCardIds.Contains(display.GetData().cardId))
            {
                display.ForceMatch();
                matchedCount++;
            }
        }

        Debug.Log($"Game loaded. Matched cards restored: {matchedCount}/{loadedData.matchedCardIds.Count}");

        loadedData = null; // cleanup to avoid stale reuse
    }
}
