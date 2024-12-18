using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Networking;

public class Levels : MonoBehaviour
{
    #region SINGLETON

    public static Levels Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public string fileName = "Levels"; // File name without extension
    public List<string> levelWords = new List<string>(); // List to store the words

    public void LoadLevelData(string fileName, string levelName)
    {
        // Load the text file as a TextAsset
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            // Get the text content
            string fileContent = textAsset.text;

            // Split the file into lines
            string[] lines = fileContent.Split('\n');

            // Find the level and get its words
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals(levelName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < lines.Length) // Ensure there is a next line
                    {
                        // Split the next line into words and store them in the list
                        string wordsLine = lines[i + 1].Trim();
                        levelWords = new List<string>(wordsLine.Split(','));

                        Debug.Log($"Loaded words for {levelName}: {string.Join(", ", levelWords)}");
                        return;
                    }
                }
            }

            Debug.LogWarning($"Level '{levelName}' not found or has no words!");
        }
        else
        {
            Debug.LogError($"File '{fileName}' not found in Resources folder!");
        }
    }
}
