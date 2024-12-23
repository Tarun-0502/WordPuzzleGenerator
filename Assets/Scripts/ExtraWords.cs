using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Networking;

public class ExtraWords : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] private GameObject wordFound;
    [SerializeField] internal List<string> FoundedExtraWords = new List<string>();
    private List<Transform> Cells = new List<Transform>();
    private int TotalCells;
    private List<Transform> CurrentCells = new List<Transform>();
    private List<string> currentWord = new List<string>();
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] internal List<string> extraWordsFromFile;
    [SerializeField] private TextAsset TextAsset;

    private string apiUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";

    #endregion

    private void Start()
    {
        //LoadSavedExtraWords();
        //SaveExtraWords.ClearData();
        //extraWordsFromFile = LoadWordsFromTextAsset();
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
        Cells.Add(go.transform);
    }

    public void CheckExtraWord(string answer)
    {
        if (!FoundedExtraWords.Contains(answer))
        {
            FoundedExtraWords.Add(answer);
            SaveData();

            if (Cells.Count < answer.Length)
            {
                int count = answer.Length - Cells.Count;
                for (int i = 0; i < count; i++)
                {
                    InstiateExtraCell();
                }
            }

            CurrentCells.Clear();
            currentWord.Clear();

            for (int i = 0; i < answer.Length; i++)
            {
                CurrentCells.Add(Cells[i]);
                currentWord.Add(answer[i] + "");
            }

            for (int i = 0; i < CurrentCells.Count; i++)
            {
                CurrentCells[i].GetComponent<Letter>().letter = currentWord[i];
            }

            float delay = 0.2f;
            for (int i = 0; i < CurrentCells.Count; i++)
            {
                Transform t = CurrentCells[i];
                DOVirtual.DelayedCall(i * delay, () =>
                {
                    var cell = t.GetComponent<Letter>();
                    cell.showText = true;
                    cell.FlyText(this.transform);
                });
            }
        }
        else
        {
            Debug.Log("Extra Word Found");
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
                Debug.Log($"The word '{word}' is valid.");
                CheckExtraWord(word);
                isWord = isValid;
            }
            else
            {
                Debug.Log($"The word '{word}' is invalid.");
                isWord = isValid;
            }
        }));
        return isWord;
    }

    private IEnumerator CheckWordCoroutine(string word, System.Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + word))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error checking word '{word}': {request.error}");
                callback?.Invoke(false);
                yield break;
            }

            callback?.Invoke(request.responseCode == 200);
        }
    }

    public List<string> LoadWordsFromTextAsset()
    {
        if (TextAsset == null)
        {
            Debug.LogError("TextAsset is null. Cannot load words.");
            return new List<string>();
        }

        return TextAsset.text
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
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

    int GetCurrentLevel()
    {
        int level = PlayerPrefs.GetInt("SelectedLevel");
        return level;
    }

    public void SaveData()
    {
        int currentLevelToPlay = GetCurrentLevel(); // Replace this with your logic to get the current level.
        SaveExtraWords.SavePlayerData(new PlayerData(this, currentLevelToPlay));
    }

    //void LoadSavedExtraWords()
    //{
    //    PlayerData data = SaveExtraWords.LoadData();
    //    if (data != null)
    //    {
    //        if (data.wordsCollected != null)
    //        {
    //            FoundedExtraWords.AddRange(data.wordsCollected);
    //        }

    //        // Set the current level to play from the saved data
    //        int levelToPlay = data.LevelToPlay;
    //        Debug.Log($"Loaded level to play: {levelToPlay}");
    //    }
    //}

}
