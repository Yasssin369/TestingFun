using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GridLayoutController gridLayout;
    public GameObject cardPrefab;
    public Transform cardGrid;
    public List<CardData> cardPool;

    public int rows = 2;
    public int columns = 2;

  
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
}
