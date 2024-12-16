using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
   public List<string> wordsCollected = new List<string>();

    public PlayerData(ExtraWords words)
    {
       foreach (var word in words.FoundedExtraWords)
       {
            wordsCollected.Add(word);
       }
    }
}
