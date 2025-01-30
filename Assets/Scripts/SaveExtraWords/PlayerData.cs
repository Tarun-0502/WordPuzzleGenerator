using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public List<string> wordsCollected = new List<string>();
    public int LevelToPlay;

    public PlayerData(ExtraWords words)
    {
        if (words != null && words.FoundedExtraWords != null)
        {
            wordsCollected.AddRange(words.FoundedExtraWords);
        }
    }

}
