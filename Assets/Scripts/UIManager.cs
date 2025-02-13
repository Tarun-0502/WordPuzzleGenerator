using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
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

    [SerializeField] AudioSource AudioSource, Music_Source;
    [SerializeField] AudioClip buttonSound;

    [SerializeField] int FactsIndex;

    [SerializeField] StorePanel StorePanel_;
    [SerializeField] List<TextMeshProUGUI> CoinsText;
    [SerializeField] List<TextMeshProUGUI> GemsText;

    [SerializeField] int Total_Coins, Total_Gems;

    [SerializeField] NewFacts Paris, NewYork, Tokyo, Egypt;
    [SerializeField] TextMeshProUGUI Cityname, CityFact;

    #endregion

    #region SINGLETON

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region METHODS

    void Start()
    {
        // Initialize Music Setting
        int musicSetting = PlayerPrefs.GetInt("Music", 1); // Default to 1 if not set
        if (musicSetting == 1)
        {
            Music_Source.volume = 1; // Assuming MusicSource is the AudioSource for music
        }
        else
        {
            Music_Source.volume = 0;
        }

        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", 50);
        }

        if (!PlayerPrefs.HasKey("Gems"))
        {
            PlayerPrefs.SetInt("Gems", 5);
        }

        if (!PlayerPrefs.HasKey("HighestLevel"))
        {
            SaveExtraWords.ClearData();
            PlayerPrefs.SetInt("Bonus", 1);
        }
        SetLevels(Content);
        Highestlevel = PlayerPrefs.GetInt("HighestLevel");
        if (PlayerPrefs.GetInt("Count")==1)
        {
            LevelSelectionScreen.SetActive(true);
            Play();
        }
        else
        {
            MainScreen.SetActive(true);
        }

        Coins_Gems_Text_Update(false);
    }

    void Facts_()
    {
        int level_No = PlayerPrefs.GetInt("SelectedLevel", 1);

        // List of theme objects (cyclic order)
        List<NewFacts> themes = new List<NewFacts> { Paris, Egypt, NewYork, Tokyo };

        // Determine the cyclic theme index (every 20 levels)
        int themeIndex = ((level_No - 1) / 20) % themes.Count;
        NewFacts activeTheme = themes[themeIndex];

        // Update city name
        Cityname.text = activeTheme.CityName;

        // Determine fact index within the 20-level block
        int factIndex = ((level_No - 1) % 20) / 5;

        // Ensure the fact index is within bounds
        if (factIndex < activeTheme.Facts.Count)
        {
            CityFact.text = activeTheme.Facts[factIndex];
        }
        else
        {
            CityFact.text = "No fact available!";
        }
    }

    public void Play()
    {
        //LevelsUnlocked(Highestlevel+1);
        //LevelWasLoaded(Highestlevel + 1);
        Button_Sound();
        //LevelWasLoaded(300);
    }

    //public void SetLevels(Transform levelsParent)
    //{
    //    int levelIndex = 1;
    //   /* for (int i = 0; i < levelsParent.childCount; i++)
    //    {
    //        Transform Level = levelsParent.GetChild(i);
    //        if (Level != null)
    //        {
    //            for (int j = 0; j < Level.childCount; j++)
    //            {
    //                Transform Theme = Level.GetChild(j);
    //                if (Theme != null)
    //                {
    //                    for (int k = 0; k < Theme.childCount; k++)
    //                    {
    //                        Transform Line = Theme.GetChild(k);
    //                        if (Line != null)
    //                        {
    //                            for (int l = 0; l < Line.childCount; l++)
    //                            {
    //                                Transform child = Line.GetChild(l);
    //                                child.localScale = Vector3.zero;
    //                                levels.Add(child);
    //                                if (child != null)
    //                                {
    //                                    // Set the name of the GameObject
    //                                    child.gameObject.name = "Level" + levelIndex;

    //                                    // Capture the current levelIndex in a local variable
    //                                    int currentLevelIndex = levelIndex;

    //                                    // Set the button's onClick listener
    //                                    Button button = child.GetComponent<Button>();
    //                                    if (button != null)
    //                                    {
    //                                        button.onClick.AddListener(() => Chooselevel(currentLevelIndex));
    //                                    }

    //                                    // Set the text of the child
    //                                    TextMeshProUGUI text = child.GetChild(0).GetComponent<TextMeshProUGUI>();
    //                                    if (text != null)
    //                                    {
    //                                        text.text = levelIndex.ToString();
    //                                    }

    //                                    // Increment the levelIndex
    //                                    levelIndex++;
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }*/

    //    for (int i = 0;i < levelsParent.childCount; i++) 
    //    {
    //        Transform transform = levelsParent.GetChild(i).transform;
    //        if (transform != null)
    //        {
    //            for (int j = 0; j < transform.childCount; j++)
    //            {
    //                Transform child = transform.GetChild(j);
    //                child.localScale = Vector3.zero;
    //                levels.Add(child);
    //                if (child != null)
    //                {
    //                    // Set the name of the GameObject
    //                    child.gameObject.name = "Level" + levelIndex;

    //                    // Capture the current levelIndex in a local variable
    //                    int currentLevelIndex = levelIndex;

    //                    // Set the button's onClick listener
    //                    Button button = child.GetComponent<Button>();
    //                    if (button != null)
    //                    {
    //                        button.onClick.AddListener(() => Chooselevel(currentLevelIndex));
    //                    }

    //                    // Set the text of the child
    //                    TextMeshProUGUI text = child.GetChild(1).GetComponent<TextMeshProUGUI>();
    //                    if (text != null)
    //                    {
    //                        text.text = levelIndex.ToString();
    //                    }

    //                    // Increment the levelIndex
    //                    levelIndex++;
    //                }
    //            }
    //        }
    //    }

    //}

    public void SetLevels(Transform levelsParent)
    {
        int levelIndex = 1;

        for (int i = 0; i < levelsParent.childCount; i++)
        {
            Transform transform = levelsParent.GetChild(i).transform;
            if (transform != null)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    int currentLevelIndex = levelIndex;
                    Transform child = transform.GetChild(j);
                    levels.Add(child);
                    child.name = "Level" + levelIndex;
                    // Set the button's onClick listener
                    Button button = child.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => Chooselevel(currentLevelIndex));
                    }
                    child.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = levelIndex.ToString();
                    levelIndex++;
                }
            }
            //if (transform != null)
            //{
            //    for (int j = 0; j < transform.childCount; j++)
            //    {
            //        Transform child = transform.GetChild(j);
            //        child.localScale = Vector3.zero;
            //        levels.Add(child);
            //        if (child != null)
            //        {
            //            // Set the name of the GameObject
            //            child.gameObject.name = "Level" + levelIndex;

            //            // Capture the current levelIndex in a local variable
            //            int currentLevelIndex = levelIndex;

            //            // Set the button's onClick listener
            //            Button button = child.GetComponent<Button>();
            //            if (button != null)
            //            {
            //                button.onClick.AddListener(() => Chooselevel(currentLevelIndex));
            //            }

            //            // Set the text of the child
            //            TextMeshProUGUI text = child.GetChild(1).GetComponent<TextMeshProUGUI>();
            //            if (text != null)
            //            {
            //                text.text = levelIndex.ToString();
            //            }

            //            // Increment the levelIndex
            //            levelIndex++;
            //        }
            //    }
            //}
        }

    }

    void Chooselevel(int current)
    {
        Button_Sound();
        Debug.LogWarning("SELECTED LEVEL - "+ current);
        PlayerPrefs.SetInt("SelectedLevel", current);
        LoadingScreen.SetActive(true);
        Facts_();
        LoadSceneWithProgress("GameScene");
        //SceneManager.LoadScene(1);   
    }

    public void LoadSceneWithProgress(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(3f);
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
        //Display(0);
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

    void Button_Sound()
    {
        AudioSource.clip = buttonSound;
        AudioSource.Play();
    }

    public void DailyChallenges()
    {
        Button_Sound();

        // Ensure the key exists (fixed spelling mistake)
        if (!PlayerPrefs.HasKey("DailyChallengeComplete"))
        {
            PlayerPrefs.SetInt("DailyChallengeComplete", 0);
        }

        // Reset challenge status if it's a new day
        if (PlayerPrefs.GetInt("LastActiveDay") != DateTime.Now.DayOfYear)
        {
            PlayerPrefs.SetInt("DailyChallengeComplete", 0);
            PlayerPrefs.Save(); // Ensure changes persist
        }

        // If the challenge is not completed, proceed
        if (PlayerPrefs.GetInt("DailyChallengeComplete") == 0)
        {
            PlayerPrefs.SetInt("LastActiveDay", DateTime.Now.DayOfYear);
            PlayerPrefs.Save(); // Save to ensure persistence

            // Get the current day of the year
            int dayOfYear = DateTime.Now.DayOfYear;

            // Cycle Current_Day in the pattern 1 → 2 → 3 → 1 → 2 → 3
            int Current_Day = ((dayOfYear - 1) % 3) + 1;

            PlayerPrefs.SetInt("Daily", Current_Day);
            PlayerPrefs.Save(); // Save the current day's challenge

            // Start loading the daily challenge
            LoadingScreen.SetActive(true);
            LoadSceneWithProgress("DailyChallenge");
        }
    }

    public void Coins_Gems_Text_Update(bool anim = false)
    {
        Total_Coins = PlayerPrefs.GetInt("Coins");
        Total_Gems = PlayerPrefs.GetInt("Gems");

        int currentCoins = int.Parse(CoinsText[0].text);  // Get the current displayed coins
        int currentGems = int.Parse(GemsText[0].text);    // Get the current displayed gems

        if (anim)
        {
            // Animate Coins Count
            DOVirtual.Int(currentCoins, Total_Coins, 1.75f, value =>
            {
                for (int i = 0;i<CoinsText.Count;i++)
                {
                    CoinsText[i].text = value.ToString();
                }
            });

            // Animate Gems Count
            DOVirtual.Int(currentGems, Total_Gems, 1.75f, value =>
            {
                for(int i = 0; i < GemsText.Count; i++)
                {
                    GemsText[i].text = value.ToString();
                }
            });
        }
        else
        {
            for (int i = 0; i < CoinsText.Count; i++)
            {
                CoinsText[i].text = Total_Coins.ToString();
            }
            for (int i = 0; i < GemsText.Count; i++)
            {
                GemsText[i].text = Total_Gems.ToString();
            }
        }
    }

    #region COINS,GEMS ADD AND REMOVE

    public void AddCoins(int Count)
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + Count);
        Coins_Gems_Text_Update(true);
    }

    public void AddGems(int Count)
    {
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + Count);
        Coins_Gems_Text_Update(true);
    }

    public void RemoveCoins(int Count)
    {
        if (PlayerPrefs.GetInt("Coins") > Count)
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - Count);
        }
        Coins_Gems_Text_Update(true);
    }

    public void RemoveGems(int Count)
    {
        if (PlayerPrefs.GetInt("Gems") > Count)
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - Count);
        }
        Coins_Gems_Text_Update(true);
    }

    #endregion

    #region StorePanel

    void Store(bool Coins)
    {
        if (Coins)
        {
            foreach (var gem in StorePanel_.Gems)
            {
                gem.SetActive(false);
            }
            foreach (var coin in StorePanel_.Coins)
            {
                coin.SetActive(true);
            }
        }
        else
        {
            foreach (var gem in StorePanel_.Gems)
            {
                gem.SetActive(true);
            }
            foreach (var coin in StorePanel_.Coins)
            {
                coin.SetActive(false);
            }
        }
    }

    public void Store_Btn(bool Gems)
    {
        if (Gems)
        {
            // Animate position of Coins_Btn
            StorePanel_.Coins_Btn.DOAnchorPos(new Vector2(-220, -270), 0.5f).SetEase(Ease.OutQuad);
            // Animate position of Gems_Btn
            StorePanel_.Gems_Btn.DOAnchorPos(new Vector2(220, -259), 0.5f).SetEase(Ease.OutQuad);
            // Animate scale of Coins_Btn
            StorePanel_.Coins_Btn.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f).SetEase(Ease.OutBack);
            // Animate scale of Gems_Btn
            StorePanel_.Gems_Btn.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            Store(false);
        }
        else
        {
            // Animate position of Coins_Btn
            StorePanel_.Coins_Btn.DOAnchorPos(new Vector2(-220, -259), 0.5f).SetEase(Ease.OutQuad);
            // Animate position of Gems_Btn
            StorePanel_.Gems_Btn.DOAnchorPos(new Vector2(220, -270), 0.5f).SetEase(Ease.OutQuad);
            // Animate scale of Coins_Btn
            StorePanel_.Coins_Btn.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            // Animate scale of Gems_Btn
            StorePanel_.Gems_Btn.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f).SetEase(Ease.OutBack);
            Store(true);
        }
    }

    public void Move_Coins(Transform parent)
    {
        float delayIncrement = 0.2f; // Adjust for desired spacing
        float duration = 0.5f;

        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform coin = parent.GetChild(0);
            coin.SetParent(StorePanel_.coin_rot);
            coin.localScale = Vector3.zero;

            // Scale animation with delay
            coin.DOScale(Vector3.one, duration)
                .SetDelay(i * delayIncrement);

            // Move animation with delay
            coin.DOMove(StorePanel_.coin_pos.position, duration)
                .SetDelay(i * delayIncrement)
                .OnComplete(() =>
                {
                    coin.localScale = Vector3.zero;
                    coin.SetParent(parent);
                    coin.SetAsFirstSibling();
                    coin.localPosition = Vector3.zero;
                });
        }
    }

    public void Move_Gems(Transform parent)
    {
        float delayIncrement = 0.2f; // Adjust this value for spacing between animations
        float duration = 0.5f;

        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform gem = parent.GetChild(0);
            gem.SetParent(StorePanel_.gem_rot);
            gem.localScale = Vector3.zero;

            // Scale animation
            gem.DOScale(Vector3.one, duration)
                .SetDelay(i * delayIncrement);

            // Move animation
            gem.DOLocalMove(StorePanel_.gem_pos.localPosition, duration)
                .SetDelay(i * delayIncrement)
                .OnComplete(() =>
                {
                    gem.localScale = Vector3.zero;
                    gem.SetParent(parent);
                    gem.SetAsFirstSibling();
                    gem.localPosition = Vector3.zero;
                });
        }
    }

    #endregion

    #endregion
}
