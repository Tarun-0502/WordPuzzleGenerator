using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public List<string> wordsCollected = new List<string>();
    public int LevelToPlay;

    public PlayerData(ExtraWords words, int levelToPlay = 0)
    {
        if (words != null && words.FoundedExtraWords != null)
        {
            wordsCollected.AddRange(words.FoundedExtraWords);
        }

        LevelToPlay = levelToPlay;
    }

}
