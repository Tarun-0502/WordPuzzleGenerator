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
    [SerializeField] List<NewFacts> Cityfacts;

    [SerializeField] StorePanel StorePanel_;
    [SerializeField] List<TextMeshProUGUI> CoinsText;
    [SerializeField] List<TextMeshProUGUI> GemsText;

    [SerializeField] int Total_Coins, Total_Gems;

    [SerializeField] NewFacts Paris, Egypt, London, Tokyo, SanFrancisco,LosAngeles, Toronto, Dubai, NewYork, Berlin, Barcelona,
        Bangkok, MexicoCity, KualaLumpur, Shanghai, Rome, Mumbai, SaoPaulo, Istanbul, CapeTown;
    

    [SerializeField] TextMeshProUGUI Cityname, CityFact;

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
            PlayerPrefs.SetInt("Coins", 100);
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
        //SetLevels(Content);
        Highestlevel = PlayerPrefs.GetInt("HighestLevel");
        if (PlayerPrefs.GetInt("Count")==1)
        {
            //LevelSelectionScreen.SetActive(true);
            MainScreen.SetActive(true);
            //Play();
        }
        else
        {
            MainScreen.SetActive(true);
        }

        Coins_Gems_Text_Update(false);

        currentTheme = 0;
        LevelBg.sprite = Level_bg_Sprites[currentTheme];

        getStamps();
        Stamps_Display(PlayerPrefs.GetInt("Theme"));
    }

    void Facts_(int Level_No)
    {
        // List of theme objects (cyclic order)
        List<NewFacts> themes = new List<NewFacts>
    {
        Paris, Egypt, London, Tokyo, SanFrancisco, LosAngeles,
        Toronto, Dubai, NewYork, Berlin, Barcelona, Bangkok,
        MexicoCity, KualaLumpur, Shanghai, Rome, Mumbai,
        SaoPaulo, Istanbul, CapeTown
    };

        if (themes.Count == 0)
        {
            Debug.LogError("Themes list is empty!");
            return;
        }

        // Determine the cyclic theme index (every 20 levels)
        int themeIndex = ((Level_No - 1) / 20) % themes.Count;
        NewFacts activeTheme = themes[themeIndex];

        if (activeTheme == null)
        {
            Debug.LogError("Active theme is null!");
            return;
        }

        // Update city name
        Cityname.text = activeTheme.CityName;

        // Determine fact index within the 20-level block
        int factIndex = ((Level_No - 1) % 20) / 5;

        // Ensure the fact index is within bounds
        if (activeTheme.Facts != null && factIndex < activeTheme.Facts.Count)
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
        Chooselevel(Highestlevel+1);
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
                    //button.onClick.AddListener(() => Chooselevel(currentLevel));
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
        Facts_(current);
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

    public bool RemoveCoins(int Count)
    {
        if (PlayerPrefs.GetInt("Coins") >= Count)
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - Count);
            Coins_Gems_Text_Update(true);
            return true;
        }
        return false;
    }

    public bool RemoveGems(int Count)
    {
        if (PlayerPrefs.GetInt("Gems") >= Count)
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - Count);
            Coins_Gems_Text_Update(true);
            return true;
        }
        return false;
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

    public void BuyCoinsWithGems(int gems)
    {
        if (RemoveGems(gems))
        {
            switch (gems)
            {
                case 15:
                    AddCoins(100);
                    Move_Coins(StorePanel_.Coins[0].transform.GetChild(0));
                    break;
                case 25:
                    AddCoins(250);
                    Move_Coins(StorePanel_.Coins[1].transform.GetChild(0));
                    break;
                case 75:
                    AddCoins(500);
                    Move_Coins(StorePanel_.Coins[2].transform.GetChild(0));
                    break;
                case 150:
                    AddCoins(1000);
                    Move_Coins(StorePanel_.Coins[3].transform.GetChild(0));
                    break;
            }
        }
    }

    public void BuyCoins(int coins)
    {
        switch (coins)
        {
            case 100:
                AddCoins(100);
                Move_Coins(StorePanel_.Coins[0].transform.GetChild(0));
                break;
            case 250:
                AddCoins(250);
                Move_Coins(StorePanel_.Coins[1].transform.GetChild(0));
                break;
            case 500:
                AddCoins(500);
                Move_Coins(StorePanel_.Coins[2].transform.GetChild(0));
                break;
            case 1000:
                AddCoins(1000);
                Move_Coins(StorePanel_.Coins[3].transform.GetChild(0));
                break;
        }
    }

    public void BuyGems(int gems)
    {
        switch (gems)
        {
            case 25:
                AddGems(25);
                Move_Gems(StorePanel_.Gems[0].transform.GetChild(0));
                break;
            case 50:
                AddGems(50);
                Move_Gems(StorePanel_.Gems[1].transform.GetChild(0));
                break;
            case 100:
                AddGems(100);
                Move_Gems(StorePanel_.Gems[2].transform.GetChild(0));
                break;
            case 500:
                AddGems(500);
                Move_Gems(StorePanel_.Gems[3].transform.GetChild(0));
                break;
        }
    }

    #endregion

    #endregion

    #region STAMPS

    [SerializeField] private List<CityStamp> stamps;
    [SerializeField] private List<Transform> Stamps_;
    [SerializeField] List<Transform> StampPages;
    [SerializeField] Transform child;

    void getStamps()
    {
        stamps.Clear(); // Clear previous data if needed
        Stamps_.Clear();

        for (int i=0;i<StampPages.Count;i++)
        {
            child = StampPages[i].GetChild(0);
            for (int j = 0; j < child.childCount; j++)
            {
                Stamps_.Add(child.GetChild(j));
            }
        }

        for (int i = 0; i < Stamps_.Count; i++)
        {
            CityStamp newStamp = new CityStamp();
            newStamp.Stamp = new Stamps();  // Ensure `Stamps` is a valid class

            if (Stamps_[i] != null)
            {
                newStamp.Stamp.Puzzle1 = Stamps_[i].Find("Puzzle_1");
                newStamp.Stamp.Puzzle2 = Stamps_[i].Find("Puzzle_2");
                newStamp.Stamp.Puzzle3 = Stamps_[i].Find("Puzzle_3");
                newStamp.Stamp.Puzzle4 = Stamps_[i].Find("Puzzle_4");

                newStamp.Stamp.MainImage = Stamps_[i].Find("Main_");
                newStamp.Stamp.cityName = Stamps_[i].GetComponentInChildren<TextMeshProUGUI>();
                newStamp.Stamp.cityName.text = ThemeNames[i];

                stamps.Add(newStamp);
            }
            else
            {
                Debug.LogWarning($"Stamps_[{i}] is null!");
            }
        }
    }

    public void Stamps_Display(int Theme)
    {
        for (int i = 0;i < stamps.Count; i++)
        {
            if (i<=Theme)
            {
                ShowPuzzle(stamps[i], PlayerPrefs.GetInt("Theme" + i));
            }
        }
    }

    void ShowPuzzle(CityStamp currentStamp, int count)
    {
        // Store the puzzles in an array for easy access
        GameObject[] puzzles = new GameObject[]
        {
        currentStamp.Stamp.Puzzle1.gameObject,
        currentStamp.Stamp.Puzzle2.gameObject,
        currentStamp.Stamp.Puzzle3.gameObject,
        currentStamp.Stamp.Puzzle4.gameObject
        };

        // Loop through all puzzles and activate only up to 'count'
        for (int i = 0; i < puzzles.Length; i++)
        {
            puzzles[i].SetActive(i < count);
        }
    }


    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    private bool isAnimating = false;
    private float swipeThreshold = 50f;  // Minimum swipe distance to trigger

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endPosition = touch.position;
                float swipeDistance = endPosition.x - startPosition.x;

                if (Mathf.Abs(swipeDistance) > swipeThreshold)  // Prevent small movements from triggering
                {
                    if (swipeDistance < 0)
                        Stamps_Update(false);  // Swipe left
                    else
                        Stamps_Update(true); // Swipe right
                }
            }
        }
    }

    public void Stamps_Update(bool left)
    {
        if (isAnimating) return;
        isAnimating = true;

        if (left) // Swipe left (go to the previous page)
        {
            if (CurrentPage > 0)
            {
                previousPage = CurrentPage - 1;
                AnimatePage(previousPage, 0);
                CurrentPage--;
            }
        }
        else // Swipe right (go to the next page)
        {
            if (CurrentPage < StampPages.Count - 1)
            {
                previousPage = CurrentPage;
                AnimatePage(CurrentPage, 180);
                CurrentPage++;
            }
        }

        isAnimating = false; // Unlock after animation
    }

    private void AnimatePage(int pageIndex, float targetRotation)
    {
        StampPages[pageIndex].transform.DOComplete(); // Cancel any ongoing animations
        StampPages[pageIndex].transform.DOScale(0.8f, 0.25f).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                StampPages[pageIndex].transform.DOScale(1f, 0.25f).SetEase(Ease.InOutSine);
            });

        StampPages[pageIndex].transform.DORotate(new Vector3(0, targetRotation, 0), 0.5f, RotateMode.FastBeyond360);
    }

    #endregion

}
