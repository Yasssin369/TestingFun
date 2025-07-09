using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "MemoryGame/CardData")]
public class CardData : ScriptableObject
{
    public string cardId;
    public Sprite image;
}
