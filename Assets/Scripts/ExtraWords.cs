using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Networking;
using System.Diagnostics.SymbolStore;

public class ExtraWords : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] GameObject wordFound;
    [SerializeField] internal List<string> FoundedExtraWords = new List<string>();
    private List<Transform> Cells = new List<Transform>();
    private int TotalCells;
    private List<Transform> CurrentCells = new List<Transform>();
    private List<string> currentWord = new List<string>();
    [SerializeField] GameObject letterPrefab;
    [SerializeField] internal List<string> extraWordsFromFile;
    [SerializeField] TextAsset TextAsset;

    private string apiUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";

    #endregion

    private void Start()
    {
        LoadSavedExtraWords();
        extraWordsFromFile = LoadWordsFromTextAsset();
        //SaveExtraWords.ClearData();
        //FoundedExtraWords.Clear();
    }

    public void InstiateCells()
    {
        foreach (string word in extraWordsFromFile)
        {
            if (word.Length > TotalCells)
            {
                TotalCells = word.Length;
            }
        }

        for (int i = 0; i < TotalCells; i++)
        {
            InstiateExtraCell();
        }
    }

    void InstiateExtraCell()
    {
        GameObject go = Instantiate(letterPrefab, this.transform);
        go.transform.GetComponent<Letter>().Text.text = "";
        go.transform.GetComponent<Letter>().showText = false;
        go.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        go.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        go.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        go.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Cells.Add(go.transform);
    }

    //public void CheckExtraWord(string answer)
    //{
    //    // Check if the answer is an extra word and if it hasn't been found yet
    //    if (extraWords.Words.Contains(answer) && !FoundedExtraWords.Contains(answer))
    //    {
    //        FoundedExtraWords.Add(answer);
    //        SaveData();

    //        // Ensure Cells has enough elements to avoid out-of-bounds error
    //        if (Cells.Count < answer.Length)
    //        { 
    //            int count = answer.Length-Cells.Count;
    //            for (int i = 0;i < count;i++)
    //            {
    //                InstiateExtraCell();
    //            }
    //           //return;
    //        }

    //        // Clear CurrentCells and currentWord before using
    //        CurrentCells.Clear();
    //        currentWord.Clear();

    //        // Populate CurrentCells and currentWord at the same time
    //        for (int i = 0; i < answer.Length; i++)
    //        {
    //            CurrentCells.Add(Cells[i]);
    //            currentWord.Add(answer[i] + "");
    //        }

    //        // Update the letter property of each cell
    //        for (int i = 0; i < CurrentCells.Count; i++)
    //        {
    //            CurrentCells[i].GetComponent<Letter>().letter = currentWord[i];
    //        }

    //        // Animate the cells using delayed calls
    //        float delay = 0.2f;  // Delay between each iteration
    //        for (int i = 0; i < CurrentCells.Count; i++)
    //        {
    //            Transform t = CurrentCells[i];
    //            DOVirtual.DelayedCall(i * delay, () =>
    //            {
    //                var cell = t.GetComponent<Letter>();
    //                cell.showText = true;
    //                cell.FlyText(this.transform); // Perform the animation
    //            });
    //        }
    //    }
    //    else
    //    {
    //        // Animation when the word has already been found
    //        wordFound.transform.DOScale(Vector3.one, 0.25f);
    //        DOVirtual.DelayedCall(1.25f, () =>
    //        {
    //            wordFound.transform.DOScale(Vector3.zero, 0.25f);
    //        });
    //    }
    //}

    public void CheckExtraWord(string answer)
    {
        // Check if the answer is an extra word and if it hasn't been found yet
        if (!FoundedExtraWords.Contains(answer))
        {
            FoundedExtraWords.Add(answer);
            SaveData();

            // Ensure Cells has enough elements to avoid out-of-bounds error
            if (Cells.Count < answer.Length)
            {
                int count = answer.Length - Cells.Count;
                for (int i = 0; i < count; i++)
                {
                    InstiateExtraCell();
                }
            }

            // Clear CurrentCells and currentWord before using
            CurrentCells.Clear();
            currentWord.Clear();

            // Populate CurrentCells and currentWord at the same time
            for (int i = 0; i < answer.Length; i++)
            {
                CurrentCells.Add(Cells[i]);
                currentWord.Add(answer[i] + "");
            }

            // Update the letter property of each cell
            for (int i = 0; i < CurrentCells.Count; i++)
            {
                CurrentCells[i].GetComponent<Letter>().letter = currentWord[i];
            }

            // Animate the cells using delayed calls
            float delay = 0.2f; // Delay between each iteration
            for (int i = 0; i < CurrentCells.Count; i++)
            {
                Transform t = CurrentCells[i];
                DOVirtual.DelayedCall(i * delay, () =>
                {
                    var cell = t.GetComponent<Letter>();
                    cell.showText = true;
                    cell.FlyText(this.transform); // Perform the animation
                });
            }
        }
        else
        {
            Debug.Log("Extra Word Found");
            // Animation when the word has already been found
            wordFound.transform.DOScale(Vector3.one, 0.25f);
            DOVirtual.DelayedCall(1.25f, () =>
            {
                wordFound.transform.DOScale(Vector3.zero, 0.25f);
            });
        }
    }

    public bool CheckWord(string word)
    {
        bool isWord = false;

        StartCoroutine(CheckWordCoroutine(word, (isValid) =>
        {
            if (isValid)
            {
                Debug.Log("The word is valid." + isValid);
                CheckExtraWord(word);
                isWord = isValid;
            }
            else
            {
                Debug.Log("The word is invalid.");
                isWord = isValid;
            }
        }));
        return isWord;
    }

    private IEnumerator CheckWordCoroutine(string word, System.Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + word))
        {
            request.timeout = 10; // Set timeout
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                callback?.Invoke(false); // Return false for errors
                yield break;
            }

            if (request.responseCode == 200)
            {
                // Assume the API response implies the word is valid
                callback?.Invoke(true); // Return true for valid words
            }
            else
            {
                Debug.Log($"'{word}' is not a valid word.");
                callback?.Invoke(false); // Return false for invalid words
            }
        }
    }

    public List<string> LoadWordsFromTextAsset()
    {
        // Load the TextAsset from the Resources folder
        TextAsset textAsset = TextAsset;
        if (textAsset == null)
        {
            Debug.LogError("TextAsset 'ExtraWords' not found in Resources folder.");
            return new List<string>();
        }

        // Split the content into words using spaces, tabs, and newlines as delimiters
        List<string> words = textAsset.text
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        return words;
    }

    public void Deactivate()
    {
        CurrentCells.Clear();
        currentWord.Clear();
        foreach (Transform t in Cells)
        {
            t.GetComponent<Letter>().Text.text = "";
        }
    }

    void SaveData()
    {
        SaveExtraWords.SavePlayerData(this);
    }

    void LoadSavedExtraWords()
    {
        PlayerData data = SaveExtraWords.LoadData();


        if (data !=null)
        {
            foreach (string word in data.wordsCollected)
            {
                FoundedExtraWords.Add(word);
            }
        }
    }

}
