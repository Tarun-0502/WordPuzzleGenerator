using System;
using System.IO;
using UnityEngine;

public static class SaveExtraWords
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "PlayerFoundedExtraWords.json");

    public static void SavePlayerData(PlayerData playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(FilePath, json);
            Debug.Log($"Player data (including level) saved successfully at: {FilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save player data: {ex.Message}");
        }
    }

    public static PlayerData LoadData()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                Debug.Log(FilePath);
                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load player data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"No save file found at: {FilePath}");
        }
        return null;
    }

    public static void ClearData()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                File.WriteAllText(FilePath, JsonUtility.ToJson(new PlayerData(null), true));
                Debug.Log("Player data cleared successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clear player data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No player data file found to clear.");
        }
    }
}
