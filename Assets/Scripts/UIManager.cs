using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] Transform Content;
    [SerializeField] GameObject LoadingScreen,MainScreen,LevelSelectionScreen;
    [SerializeField] Image Bar;
    [SerializeField] List<Transform> levels;

    [SerializeField] int Highestlevel;
    [SerializeField] float position;

    [SerializeField] Sprite Filled, Opacity;
    [SerializeField] TextMeshProUGUI ThemeName,Left_arrow,Right_Arrow;
    [SerializeField] List<string> ThemeNames;
    [SerializeField] List<Sprite> ThemeSprites;
    [SerializeField] int ThemeIndex;

    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip buttonSound;

    #endregion

    #region METHODS

    void Start()
    {
        if (!PlayerPrefs.HasKey("HighestLevel"))
        {
            SaveExtraWords.ClearData();
        }
        SetLevels(Content);
        Highestlevel = PlayerPrefs.GetInt("HighestLevel");
        if (PlayerPrefs.GetInt("Count")==1)
        {
            LevelSelectionScreen.SetActive(true);
            Play();
            PlayerPrefs.SetInt("Count", 0);
        }
        else
        {
            MainScreen.SetActive(true);
        }
    }

    public void Play()
    {
        //LevelsUnlocked(Highestlevel+1);
        LevelWasLoaded(Highestlevel + 1);
        Button_Sound();
        //LevelWasLoaded(300);
    }

    public void SetLevels(Transform levelsParent)
    {
        int levelIndex = 1;
       /* for (int i = 0; i < levelsParent.childCount; i++)
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
        }*/

        for (int i = 0;i < levelsParent.childCount; i++) 
        {
            Transform transform = levelsParent.GetChild(i).transform;
            if (transform != null)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    Transform child = transform.GetChild(j);
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
                        TextMeshProUGUI text = child.GetChild(1).GetComponent<TextMeshProUGUI>();
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

    void Chooselevel(int current)
    {
        Button_Sound();
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
        //yield return new WaitForSeconds(1.05f); 
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

    private void LevelWasLoaded(int level_)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            var level = levels[i];
            bool isUnlocked = i < level_;

            // Set active states and sprites based on level status
            level.GetChild(0).gameObject.SetActive(!isUnlocked);
            level.GetChild(1).gameObject.SetActive(isUnlocked);
            level.GetComponent<Image>().sprite = isUnlocked ? Filled : Opacity;
            level.GetComponent<Button>().enabled = isUnlocked;

            //// Scale animation with delay
            //float delay = i * 0.1f; // Adjust the delay time between each level scaling
            //level.transform.DOScale(Vector3.one, 0.25f)
            //    .SetDelay(delay)
            //    .SetEase(Ease.OutQuad);
        }
        Display(0);
        //StartCoroutine(DisplayTextLetterByLetter(0));
    }

    public void LeftClick()
    {
        Button_Sound();
        if (position<0)
        {
            position += 1240f;
            ThemeIndex = -(int)(position / 1240);
            Display(ThemeIndex);
        }
    }
    public void RightClick()
    {
        Button_Sound();
        if (position>-17360)
        {
            position -= 1240f;
            ThemeIndex = -(int)(position / 1240);
            Display(ThemeIndex);
        }
    }

    private void Update()
    {
        RectTransform rectTransform = Content.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,new Vector2(position,rectTransform.anchoredPosition.y),2.5f*Time.deltaTime);
    }

    private void Display(int index)
    {
        // Ensure index is within bounds, wrap around if needed
        int themeIndex = (index + ThemeNames.Count) % ThemeNames.Count;

        if (index >= 0 && index < Content.childCount)
        {
            Transform level_1_ = Content.GetChild(index);

            // Animate each child scale
            for (int i = 0; i < level_1_.childCount; i++)
            {
                Transform child_1_ = level_1_.GetChild(i);
                child_1_.transform.localScale = Vector3.zero;
                // Scale animation with delay
                float delay = i * 0.1f; // Adjust the delay time between each level scaling
                child_1_.transform.DOScale(Vector3.one, 0.15f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutQuad);
            }

            // Calculate theme index correctly to avoid negative index
             themeIndex = (index % ThemeNames.Count + ThemeNames.Count) % ThemeNames.Count;

            // Update the theme name and sprite
            ThemeName.text = ThemeNames[themeIndex];
            LevelSelectionScreen.GetComponent<Image>().sprite = ThemeSprites[themeIndex];

            // Set the Left Arrow text (previous theme)
            int prevIndex = (index - 1 + ThemeNames.Count) % ThemeNames.Count; // Ensure previous index wraps around
            Left_arrow.text = ThemeNames[prevIndex];

            // Set the Right Arrow text (next theme)
            int nextIndex = (index + 1) % ThemeNames.Count; // Ensure next index wraps around
            Right_Arrow.text = ThemeNames[nextIndex];
            if (index==0)
            {
                Left_arrow.text = "";
            }
            else if (index==Content.childCount-1)
            {
                Right_Arrow.text = "";
            }
        }
    }

    private IEnumerator DisplayTextLetterByLetter(int index)
    {
        if (index < ThemeNames.Count)
        {
            Transform level_1_ = Content.GetChild(index);

            for (int i = 0;i<level_1_.childCount;i++)
            {
                Transform child_1_ = level_1_.GetChild(i);
                // Scale animation with delay
                float delay = i * 0.1f; // Adjust the delay time between each level scaling
                child_1_.transform.DOScale(Vector3.one, 0.25f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutQuad);
            }
        }

        ThemeName.text = ThemeNames[index]; // Clear the initial text

        string fullText = ThemeNames[index];
        float totalDuration = 2.5f; // Total duration for displaying the text
        float elapsedTime = 0f;

        int currentCharIndex = 0; // Keeps track of the current character to display
        while (currentCharIndex < fullText.Length)
        {
            // Calculate the target character index based on elapsed time
            elapsedTime += Time.deltaTime;
            int targetCharIndex = Mathf.FloorToInt((elapsedTime / totalDuration) * fullText.Length);

            // Update the visible characters up to the target index
            if (targetCharIndex > currentCharIndex)
            {
                currentCharIndex = targetCharIndex;
                ThemeName.text = fullText.Substring(0, currentCharIndex); // Display the substring
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the full text is displayed at the end
        ThemeName.text = fullText;
    }

    void Button_Sound()
    {
        AudioSource.clip = buttonSound;
        AudioSource.Play();
    }

    #endregion
}
