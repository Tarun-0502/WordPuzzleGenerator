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
    [SerializeField] private GameObject Network_Error;

    [SerializeField] GameObject ExtraWordsPanel;
    [SerializeField] TextMeshProUGUI WordsList;

    [SerializeField] const int ExtraWordsCollected = 5;

    private string apiUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";

    #endregion

    #region METHODS

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
        go.transform.GetComponent<ExtraWordCell>().Text.text = "";
        go.transform.GetComponent<ExtraWordCell>().showText = false;
        go.transform.localScale = Vector3.zero;
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
                CurrentCells[i].GetComponent<ExtraWordCell>().letter = currentWord[i];
            }

            for (int i = 0; i < CurrentCells.Count; i++)
            {
                float delay = 0.5f;
                Transform t = CurrentCells[i];
                DOVirtual.DelayedCall(i * delay, () =>
                {
                    var cell = t.GetComponent<ExtraWordCell>();
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

            // Handle network connection errors
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning("Network lost! Please check your internet connection.");
                Network_Error.SetActive(true);
                DOVirtual.DelayedCall(0.25f,()=>
                {
                    Network_Error.SetActive(false);
                });
                callback?.Invoke(false);
                yield break;
            }

            // Handle protocol errors (e.g., 404 - Not Found)
            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning($"Word '{word}' not found. Error: {request.error}");
                Game.Instance.TextPreview.transform.DOShakePosition(duration: 1f, strength: new Vector3(5f, 0f, 0f), vibrato: 5, randomness: 10, snapping: false, fadeOut: true);
                callback?.Invoke(false);
                yield break;
            }

            // Handle successful response
            if (request.responseCode == 200)
            {
                Debug.Log($"Word '{word}' found successfully.");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogWarning($"Unexpected response code: {request.responseCode}");
                callback?.Invoke(false);
            }
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
            t.GetComponent<ExtraWordCell>().Text.text = "";
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
        SaveExtraWords.SavePlayerData(new PlayerData(this));
    }

    public void ShowExtraWords()
    {
        ExtraWordsPanel.gameObject.SetActive(true);
        WordsList.text = string.Empty;
        for (int i = 0;i<FoundedExtraWords.Count;i++)
        {
            WordsList.text += FoundedExtraWords[i]+" ";
        }
    }

    void ExtraWords_Collected(List<string> words,int count)
    {
        if (words.Count==count)
        {
            SaveExtraWords.ClearData();
        }
    }

    #endregion

}
