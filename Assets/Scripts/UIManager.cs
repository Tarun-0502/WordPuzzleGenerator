using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class Stamps
{
    public Transform Puzzle1;
    public Transform Puzzle2;
    public Transform Puzzle3;
    public Transform Puzzle4;

    public Transform MainImage;
    public TextMeshProUGUI cityName;
}
[System.Serializable]
public class CityStamp
{
    public Stamps Stamp;
}

public class UIManager : MonoBehaviour
{
    #region REFERENCES

    [SerializeField] Transform Content;
    [SerializeField] GameObject LoadingScreen,MainScreen,LevelSelectionScreen;
    [SerializeField] Image Bar,LevelBg;
    [SerializeField] List<Transform> levels;

    [SerializeField] int Highestlevel;
    [SerializeField] float position;

    [SerializeField] Sprite Unlocked, Locked,LockPng;
    [SerializeField] TextMeshProUGUI ThemeName,Left_arrow,Right_Arrow;
    [SerializeField] List<string> ThemeNames;
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

    [SerializeField] Transform Stamps;
    [SerializeField] List<Transform> StampPages;
    [SerializeField] int CurrentPage,previousPage,NextPage;

    [SerializeField] List<Sprite> Level_bg_Sprites;


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

        currentTheme = 0;
        LevelBg.sprite = Level_bg_Sprites[currentTheme];
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
        LevelWasLoaded(Highestlevel + 1);
        Button_Sound();
        //LevelWasLoaded(400);
    }

    #region LEVELS_SELECTION

    public void SetLevels(Transform levelsParent)
    {
        if (levelsParent == null)
        {
            Debug.LogError("levelsParent is null!");
            return;
        }

        int levelIndex = 1;
        levels.Clear();  // Clear any existing levels

        for (int i = 0; i < levelsParent.childCount; i++)
        {
            Transform levelContainer = levelsParent.GetChild(i);
            if (levelContainer == null) continue;

            for (int j = 0; j < levelContainer.childCount; j++)
            {
                Transform child = levelContainer.GetChild(j);
                if (child == null) continue;

                child.localScale = Vector3.zero;  // Optionally, set to active instead of scale
                levels.Add(child);

                // Set GameObject name
                child.gameObject.name = "Level" + levelIndex;

                // Add button click listener
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    int currentLevel = levelIndex;  // Capture current index for closure
                    button.onClick.AddListener(() => Chooselevel(currentLevel));
                }

                // Set text to show the level index
                TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = levelIndex.ToString();
                }

                levelIndex++;
            }
        }
    }

    private void LevelWasLoaded(int level_)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            var level = levels[i];
            bool isUnlocked = i < level_;

            level.GetComponent<Image>().sprite = isUnlocked ? Unlocked : Locked;
            level.GetComponent<Button>().interactable = isUnlocked;
            level.GetChild(1).GetComponent<Image>().sprite = LockPng;
            level.GetChild(1).GetComponent<Image>().SetNativeSize();
            level.GetChild(1).gameObject.SetActive(!isUnlocked);
            level.GetChild(0).gameObject.SetActive(isUnlocked);



            // Animate the scale smoothly with DoTween
            level.transform.localScale = Vector3.zero; // Start from zero
            DOVirtual.DelayedCall(0.09f * i, () =>
            {
                level.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            });
        }
    }

    void Chooselevel(int current)
    {
        Button_Sound();
        Debug.LogWarning("SELECTED LEVEL - "+ current);
        PlayerPrefs.SetInt("SelectedLevel", current);
        LevelSelectionScreen.SetActive(false);
        LoadingScreen.SetActive(true);
        Facts_();
        LoadSceneWithProgress("GameScene");
        //SceneManager.LoadScene(1);   
    }

    int currentTheme = 0;
    [SerializeField] float[] Contentpositions;
    [SerializeField]RectTransform contentTransform;

    public void Right(bool right)
    {
        if (right)
        {
            if (currentTheme < Level_bg_Sprites.Count - 1)
            {
                currentTheme++;
                position = Contentpositions[currentTheme];
                LevelBg.sprite = Level_bg_Sprites[currentTheme];
                contentTransform.DOAnchorPos(new Vector2(-position, contentTransform.anchoredPosition.y), 0.5f)
                                .SetEase(Ease.OutCubic); // Smooth easing

                AnimateThemeChildren(currentTheme);
            }
        }
        else
        {
            if (currentTheme > 0)
            {
                currentTheme--;
                position = Contentpositions[currentTheme];
                LevelBg.sprite = Level_bg_Sprites[currentTheme];
                contentTransform.DOAnchorPos(new Vector2(-position, contentTransform.anchoredPosition.y), 0.5f)
                                .SetEase(Ease.OutCubic); // Smooth easing

                AnimateThemeChildren(currentTheme);
            }
        }
    }

    private void AnimateThemeChildren(int themeIndex)
    {
        for (int i = 0; i < 20; i++)
        {
            Transform levelTransform = Content.GetChild(themeIndex).GetChild(i);
            levelTransform.localScale = Vector3.zero;  // Start from zero scale

            levelTransform.DOScale(Vector3.one, 0.3f)  // Animate to full scale
                          .SetEase(Ease.OutBack)
                          .SetDelay(0.05f * i);  // Add staggered delay for each child
        }
    }

    #endregion

    #region LOADING-PROGESS

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

    #endregion

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

    #region STAMPS

    [SerializeField] private List<Transform> Stamps_;
    [SerializeField] private List<CityStamp> stamps;

    void getStamps()
    {
        for (int i = 0; i < Stamps.childCount; i++)
        {
            CityStamp newStamp = new CityStamp();
            newStamp.Stamp = new Stamps();  // Initialize the Stamps object

            newStamp.Stamp.Puzzle1 = Stamps_[i].Find("Puzzle_1");
            newStamp.Stamp.Puzzle2 = Stamps_[i].Find("Puzzle_2");
            newStamp.Stamp.Puzzle3 = Stamps_[i].Find("Puzzle_3");
            newStamp.Stamp.Puzzle4 = Stamps_[i].Find("Puzzle_4");

            newStamp.Stamp.MainImage = Stamps_[i].Find("Main_");
            newStamp.Stamp.cityName = Stamps_[i].GetComponentInChildren<TextMeshProUGUI>();

            stamps.Add(newStamp);  // Add after setting up newStamp
        }
    }

    public void Stamps_Display()
    {

    }

    private bool isAnimating = false;  // Animation lock

    public void Stamps_Update(bool Left)
    {
        if (isAnimating) return;  // Prevent multiple clicks during animation
        isAnimating = true;        // Lock animation

        if (Left)
        {
            if (CurrentPage > 0)
            {
                previousPage = CurrentPage - 1;

                StampPages[previousPage].transform.DOScale(0.8f, 0.25f).SetEase(Ease.InOutSine)
                    .OnComplete(() => StampPages[previousPage].transform.DOScale(1f, 0.25f).SetEase(Ease.InOutSine));

                StampPages[previousPage].transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360)
                    .OnComplete(() =>
                    {
                        CurrentPage--;
                        isAnimating = false;  // Unlock after animation completes
                    });
            }
            else
            {
                isAnimating = false;  // Unlock if no page flip happens
            }
        }
        else
        {
            if (CurrentPage < StampPages.Count - 1)
            {
                previousPage = CurrentPage;

                StampPages[CurrentPage].transform.DOScale(0.8f, 0.25f).SetEase(Ease.InOutSine)
                    .OnComplete(() => StampPages[CurrentPage].transform.DOScale(1f, 0.25f).SetEase(Ease.InOutSine));

                StampPages[CurrentPage].transform.DORotate(new Vector3(0, 180, 0), 0.5f, RotateMode.FastBeyond360)
                    .OnComplete(() =>
                    {
                        CurrentPage++;
                        isAnimating = false;  // Unlock after animation completes
                    });
            }
            else
            {
                isAnimating = false;  // Unlock if no page flip happens
            }
        }
    }

    #endregion

}
