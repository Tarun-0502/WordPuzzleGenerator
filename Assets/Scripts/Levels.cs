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

    public List<GameLevels> stringListDataList; // List of GameLevels ScriptableObjects

    void WriteDataToFile()
    {
        if (stringListDataList == null || stringListDataList.Count == 0)
        {
            Debug.LogWarning("The GameLevels list is null or empty!");
            return;
        }

        // Define the file path
        string path = Application.dataPath + "/MyLevels.txt";

        // Prepare the content
        using (StreamWriter writer = new StreamWriter(path))
        {
            for (int i = 0; i < stringListDataList.Count; i++)
            {
                var gameLevel = stringListDataList[i];

                // Check for null or empty words in the current GameLevels object
                if (gameLevel == null || gameLevel.Words.Count == 0)
                {
                    Debug.LogWarning($"GameLevel at index {i} is null or has no words!");
                    continue;
                }

                // Get the ScriptableObject name and combine its words into a single line
                string scriptableObjectName = gameLevel.name;
                string data = string.Join(",", gameLevel.Words);

                // Write to file
                writer.WriteLine("Level-"+(301+i)); // Write the name of the ScriptableObject
                writer.WriteLine(data); // Write the words separated by commas
                writer.WriteLine(); // Add a blank line for better readability
            }
        }

        Debug.Log($"Data written to file: {path}");
    }

    public string fileName = "Levels"; // File name without extension
    public List<string> levelWords = new List<string>(); // List to store the words

    void Start()
    {
      
    }

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
