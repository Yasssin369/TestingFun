using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int rows;
    public int columns;
    public int score;
    public int turnsTaken;
    public int comboStreak;

    public List<string> matchedCardIds = new List<string>();
    public List<string> orderedCardIds;
}