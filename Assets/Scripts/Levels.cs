using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Networking;
using System.Linq;

public class Levels : MonoBehaviour
{
    #region SINGLETON

    public static Levels Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<string> levelWords = new List<string>(); // List to store the words
    public TextAsset levelText;

    public void LoadLevelData(string fileName, string levelName)
    {
        // Load the text file as a TextAsset
        TextAsset textAsset = levelText;

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
                        // Split the next line into words, remove extra spaces, and store them in the list
                        string wordsLine = lines[i + 1].Trim();
                        levelWords = new List<string>(wordsLine.Split(',')
                                                  .Select(word => word.Trim()) // Remove extra spaces
                                                  .Where(word => !string.IsNullOrEmpty(word)) // Remove empty words
                        );

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
