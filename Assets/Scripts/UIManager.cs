using DG.Tweening;
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
    [SerializeField] List<Transform> levels;

    [SerializeField] int Highestlevel;
    [SerializeField] int position = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("HighestLevel"))
        {
            SaveExtraWords.ClearData();
        }
        SetLevels(Content);
        Highestlevel = PlayerPrefs.GetInt("HighestLevel");
        //LevelsUnlocked(Highestlevel);
        //LevelsUnlocked(levels.Count-1);
    }

    public void Play()
    {
        LevelsUnlocked(Highestlevel+1);
    }

    public void SetLevels(Transform levelsParent)
    {
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
                                    child.localScale = Vector3.zero;
                                    levels.Add(child);
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

    private void LevelsUnlocked(int Levels)
    {
        position = 0;
        for (int i = 0; i < Levels; i++)
        {
            Transform Level = levels[i];

            // Add to position after every 5 iterations
            if ((i + 1) % 5 == 0)
            {
                position += 3020;
                Debug.Log(i + " POsition ");
            }

            if (i == Levels - 1)
            {
                RectTransform rectTransform = Content.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Use DOTween to animate anchoredPosition smoothly
                    float y = rectTransform.anchoredPosition.y - position;
                    rectTransform.DOAnchorPos(new Vector2(0, y), 1.2f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        // Animate the last level
                        Level.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutQuad);
                        Debug.Log(i + " - " + y);
                        Debug.Log(rectTransform.anchoredPosition.y + position);
                    });

                    //// Animate the last level
                    //Level.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutQuad).SetDelay(1.2f);
                    //Debug.Log(i + " - " + y);
                    //Debug.Log(rectTransform.anchoredPosition.y + position);
                }
                else
                {
                    Debug.LogError("Content does not have a RectTransform component!");
                }
            }
            else
            {
                // No delay for other iterations
                Level.transform.localScale = Vector3.one;
            }
        }

    }
}
