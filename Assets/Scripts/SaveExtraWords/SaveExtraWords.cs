using System;
using System.IO;
using UnityEngine;

public static class SaveExtraWords
{
    // Save Player Data with ExtraWords
    public static void SavePlayerData(ExtraWords words)
    {
        try
        {
            string path = Application.persistentDataPath + "/PlayerFoundedExtraWords.json"; // Change extension to .json

            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Serialize the data to JSON
            string json = JsonUtility.ToJson(new PlayerData(words), true); // 'true' for pretty print (indentation)
            File.WriteAllText(path, json); // Save as a JSON text file
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save player data: " + ex.Message);
        }
    }

    // Load Player Data (from JSON)
    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/PlayerFoundedExtraWords.json"; // Updated to .json
        if (File.Exists(path))
        {
            try
            {
                // Read the file and deserialize it back to PlayerData
                string json = File.ReadAllText(path);
                Debug.Log(path);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load player data: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
        return null;
    }

    // Clear Player Data (without deleting file)
    public static void ClearData()
    {
        string path = Application.persistentDataPath + "/PlayerFoundedExtraWords.json"; // Same file path used for saving/loading data

        if (File.Exists(path))
        {
            try
            {
                // Serialize empty data to JSON and overwrite the file
                string json = JsonUtility.ToJson(new PlayerData(new ExtraWords()), true); // Empty data reset
                File.WriteAllText(path, json); // Save as empty JSON data

                Debug.Log("Player data cleared successfully (data reset).");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to clear player data: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No player data file found to clear.");
        }
    }

}
