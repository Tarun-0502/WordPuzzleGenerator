using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform Content;
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Image Bar;

    // Start is called before the first frame update
    void Start()
    {
        SetLevels(Content);
    }

    public void SetLevels(Transform levelsParent)
    {
        //Transform levelsParent = levelSelectionScreen.GetChild(0);

        int levelIndex = 1;
        for (int i = 0; i < levelsParent.childCount; i++)
        {
            Transform Level = levelsParent.GetChild(i);
            if (Level != null)
            {
                for (int j = 0; j < Level.childCount; j++)
                {
                    Transform Theme = Level.GetChild(j);
                    if (Theme != null)
                    {
                        for (int k = 0; k < Theme.childCount; k++)
                        {
                            Transform Line = Theme.GetChild(k);
                            if (Line != null)
                            {
                                for (int l = 0; l < Line.childCount; l++)
                                {
                                    Transform child = Line.GetChild(l);
                                    if (child != null)
                                    {
                                        // Set the name of the GameObject
                                        child.gameObject.name = "Level" + levelIndex;

                                        // Capture the current levelIndex in a local variable
                                        int currentLevelIndex = levelIndex;

                                        // Set the button's onClick listener
                                        Button button = child.GetComponent<Button>();
                                        if (button != null)
                                        {
                                            button.onClick.AddListener(() => Chooselevel(currentLevelIndex));
                                        }

                                        // Set the text of the child
                                        TextMeshProUGUI text = child.GetChild(0).GetComponent<TextMeshProUGUI>();
                                        if (text != null)
                                        {
                                            text.text = levelIndex.ToString();
                                        }

                                        // Increment the levelIndex
                                        levelIndex++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
    void Chooselevel(int current)
        {
            PlayerPrefs.SetInt("SelectedLevel", current);
            LoadingScreen.SetActive(true);
            LoadSceneWithProgress("GameScene");
            //SceneManager.LoadScene(1);
        }

    public void LoadSceneWithProgress(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false; // Prevent automatic activation

        // Update the progress bar fill amount
        while (!asyncOperation.isDone)
        {
            // Progress is between 0.0 and 0.9; normalize it to 0.0 - 1.0 for UI
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Bar.fillAmount = progress;

            // Activate the scene when fully loaded (optional: wait for user input or delay)
            if (asyncOperation.progress >= 0.9f)
            {
                // Optional: Add a delay or wait for input here
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
