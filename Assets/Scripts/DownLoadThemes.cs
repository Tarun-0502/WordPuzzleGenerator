using DG.Tweening;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownLoadThemes : MonoBehaviour
{
    private string url = "https://playdgamestudio.com/wp-content/uploads/2025/02/Themes.zip";  // Change this to your server URL
    public string localPath;
    public string ExtractedPath = string.Empty;
    public Image LoadingBar;
    public float time;
    public float duration = 5f; // Duration to fill the bar (in seconds)

    private float elapsedTime = 0f;
    public float rotationSpeed;
    public Transform Load;

    #region SINGLETON

    public static DownLoadThemes instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    void Start()
    {
        

        // Define the local path for saving the downloaded ZIP file
        localPath = Path.Combine(Application.persistentDataPath, "folder.zip");

        // Check if the file already exists to avoid downloading again
        if (File.Exists(localPath))
        {
            Debug.Log("File already exists, skipping download.");
            UnzipFile(localPath, Path.Combine(Application.persistentDataPath, "DownloadedFolder"));
        }
        else
        {
            StartCoroutine(DownloadFolder());
        }
    }

    // Coroutine to download the ZIP file
    IEnumerator DownloadFolder()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();

        while (!www.isDone)
        {
            if (LoadingBar != null)
            {
                LoadingBar.fillAmount = www.downloadProgress;
            }
            yield return null;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download failed: " + www.error);
        }
        else
        {
            // Save the downloaded data to the local path
            File.WriteAllBytes(localPath, www.downloadHandler.data);
            Debug.Log("Download complete!");
            Debug.Log("Downloaded data size: " + www.downloadHandler.data.Length);

            // Unzip the folder after successful download
            UnzipFile(localPath, Path.Combine(Application.persistentDataPath, "DownloadedFolder"));
        }
    }

    // Method to unzip the downloaded file
    void UnzipFile(string zipPath, string extractPath)
    {
        try
        {
            // If the extraction directory exists, delete it and all its contents
            if (Directory.Exists(extractPath))
            {
                duration = 5f;
                Directory.Delete(extractPath, true);
                Debug.Log("Existing extraction folder deleted.");
            }
            else
            {
                duration = 46f;
            }

            // Extract the ZIP file to the specified directory using a separate thread to avoid blocking the main thread
            Thread unzipThread = new Thread(() =>
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                Debug.Log("Folder extracted to: " + extractPath);
                ExtractedPath = extractPath;
            });
            unzipThread.Start();
            //Game.Instance.LoadAllCitySprites();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to unzip file: " + e.Message);
        }
    }

    private void Update()
    {
        Load.localRotation *= Quaternion.Euler(0, 0, rotationSpeed);
        if (ExtractedPath == string.Empty)
        {
            time += Time.deltaTime;
        }
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            LoadingBar.fillAmount = Mathf.Clamp01(elapsedTime / duration);
        }
    }
}
