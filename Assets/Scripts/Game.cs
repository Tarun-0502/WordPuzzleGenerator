using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using static DG.DemiLib.External.DeHierarchyComponent;
using UnityEngine.XR;


   #region USER-DEFINED-CLASS


   [System.Serializable]
   public class StorePanel
   {
    public GameObject Root;
    public List<GameObject> Coins, Gems;
    public RectTransform Coins_Btn, Gems_Btn;
    public Transform coin_pos, coin_rot;
    public Transform gem_pos, gem_rot;
   }

   [System.Serializable]
   public class Power_up
   {
    public Transform hintPosition;
    public Transform hintParent;
    public Transform MultiHintPosition;
    public Transform MultiHintParent;
    public Transform SpotLightPosition;
    public Transform SpotLightParent;
    public Transform WordChainPosition;
    public Transform WordChainParent;
   }

   [System.Serializable]
   public class Coins_Gems_Text
   {
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI coinsText_LevelComplete;
    public TextMeshProUGUI coinsText_Store;
    public TextMeshProUGUI coinsText_Extra;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI gemsText_LevelComplete;
    public TextMeshProUGUI gemsText_Store;
    public TextMeshProUGUI gemsText_Extra;
   }

   [System.Serializable]
   public class powerUpImage
   {
    public Transform PowerUp_Pop, Bg;
    public Image BaseImage;
    public TextMeshProUGUI textPreview;
    public Sprite Hint;
    public Sprite MultipleHints;
    public Sprite SpotLight;
    public Sprite WordChain;
    public Transform hint, multiHint, spotLight, wordChain, extraWords;
   }

#endregion

public class Game : MonoBehaviour
{

    #region Singleton Reference
    public static Game Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion


    #region REFERENCES

    #region HIDE_In_Inspector

    [HideInInspector]
    [SerializeField] private GameLevelWords gameLevelWords;

    [HideInInspector]
    [SerializeField] internal string CurrentWord;

    [HideInInspector]
    [SerializeField] List<Cell> Cells_In_GNR;

    [HideInInspector]
    [SerializeField] internal List<Transform> LineWords = new List<Transform>();

    [HideInInspector]
    [SerializeField] List<Transform> LettersUsedInLevel = new List<Transform>();

    //[HideInInspector]
    [SerializeField] internal int CompletedWords;

    [HideInInspector]
    [SerializeField] int maxLetters;

    [HideInInspector]
    [SerializeField] int ExtraWordsCount;

    #endregion

    [SerializeField] internal Power_up Power_up;

    [SerializeField] internal bool DailyChallenges,MissingLetters,Timer;
    [SerializeField] Cell CurrentFirstCell;
    [SerializeField] int TotalBadges = 3;
    [SerializeField] internal int CollectedBadges = 0;
    [SerializeField] TextMeshProUGUI TimerText,badgesText;
    [SerializeField] float RemaningTime;

    [SerializeField] internal TextMeshProUGUI TextPreview;
    [SerializeField] private Transform Circle;
    [SerializeField] private Transform LineParent;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform DotParent;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private ExtraWords extraWords;

    private Camera mainCamera;
    private LineRenderer lineRenderer;
    private bool isDrawing;

    private int dotId;
    private List<GameObject> Letters = new List<GameObject>();
    private List<Vector3> drawPositions = new List<Vector3>();
    private List<GameObject> Dots = new List<GameObject>();

    // Shuffle helper
    private List<Transform> letters = new List<Transform>();
    private List<Vector3> letterPositions = new List<Vector3>();

    private Transform CurrentLevelCircle;

    [SerializeField] private int Total_Coins,Total_Gems;
    
    public Transform CoinPosition;
    

    [SerializeField] GameObject SettingsScreen;

    [SerializeField] TextMeshProUGUI levelText,Level_Text_Settings,Place_Text;

    [Tooltip("Font is Used EveryWhere in Game")]
    [SerializeField] internal TMP_FontAsset NewFontAsset,Incircle;

    [Tooltip("ColorCode")]
    [SerializeField] internal string colorCode;

    [SerializeField] ParticleSystem SnowEffect,Dust;
    [SerializeField] Transform levelComplete_Screen;

    [SerializeField] Image bg;

    [SerializeField] Image TextImage;
    [SerializeField] TextMeshProUGUI LevelComplete_Level_No;
    [SerializeField] Image LevelComplete_FillingBar;

    [SerializeField] AudioSource AudioSource;
    [SerializeField] internal AudioClip LevelComplete, CoinCollect, WordComplete, Hint,tap;

    #region THEMES_SPRITES

    [SerializeField] Sprite[] Paris;
    [SerializeField] Sprite[] Egypt;
    [SerializeField] Sprite[] London;
    [SerializeField] Sprite[] Tokyo;
    [SerializeField] Sprite[] SanFrancisco;
    [SerializeField] Sprite[] LosAngeles;
    [SerializeField] Sprite[] Toronto;
    [SerializeField] Sprite[] Dubai;
    [SerializeField] Sprite[] NewYork;
    [SerializeField] Sprite[] Berlin;
    [SerializeField] Sprite[] Barcelona;
    [SerializeField] Sprite[] Bangkok;
    [SerializeField] Sprite[] MexicoCity;
    [SerializeField] Sprite[] KualaLumpur;
    [SerializeField] Sprite[] Shanghai;
    [SerializeField] Sprite[] Rome;
    [SerializeField] Sprite[] Mumbai;
    [SerializeField] Sprite[] SaoPaulo;
    [SerializeField] Sprite[] Istanbul;
    [SerializeField] Sprite[] CapeTown;

    #endregion

    [SerializeField] Sprite Sound_Onn, Sound_off,Music_onn,Music_Off;
    [SerializeField] Image Sound_Img, Music_Img;

    [SerializeField] AudioSource Music_Source;

    [SerializeField] int HighestLevel;

    [SerializeField] List<string> ColorCodes;

    [SerializeField] int randomNumber;

    [SerializeField] internal bool Game_;
    [SerializeField] Button Shuffele_Btn;

    [SerializeField] StorePanel StorePanel_;

    [SerializeField] Image Pop_Up_Panel, Pop_Up_Panel_Parent;
    [SerializeField] Sprite Star_PopUp, Missing_PopUp, Timer_PopUp;

    [SerializeField] int bonusLevel;

    #endregion

    [SerializeField] Coins_Gems_Text text_update;

    [SerializeField] List<GameObject> Screens;

    [SerializeField] RectTransform LevelTextBg_;

    [SerializeField] GameObject LoadingScreen;

    #region METHODS

    void Start()
    {
        if (gameLevelWords==null)
        {
            gameLevelWords = FindObjectOfType<GameLevelWords>();
        }

        if (PlayerPrefs.GetInt("BonusLevel") == 1)
        {
            DOVirtual.DelayedCall(0.35f,() =>
            {
                LevelTextBg_.DOAnchorPos(new Vector2(13, -98), 0.5f);
                LevelTextBg_.transform.DOScale(Vector3.one, 0.5f);
            });
        }
        else
        {
            LevelTextBg_.DOAnchorPos(new Vector2(13, -98), 0f);
            LevelTextBg_.transform.DOScale(Vector3.one, 0f);
        }

        Total_Coins = PlayerPrefs.GetInt("Coins");
        Total_Gems = PlayerPrefs.GetInt("Gems");

        mainCamera = Camera.main;

        GameObject lineRe = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, LineParent);
        lineRenderer = lineRe.GetComponent<LineRenderer>();
        lineRenderer.gameObject.SetActive(false);

        UpdateFont();

        ThemeSelection(DailyChallenges);

        if (!DailyChallenges)
        {
            PowerUpUnlock(PlayerPrefs.GetInt("SelectedLevel"));
            if (PlayerPrefs.GetInt("SelectedLevel")>6)
            {
                powerUpImage.extraWords.gameObject.SetActive(true);
            }
        }

        LoadSavedData();

        if (!Game_)
        {
            switch (PlayerPrefs.GetInt("Daily"))
            {
                case 1:
                    Pop_Up_Panel.sprite = Star_PopUp;
                    DailyChallenges = true;
                    MissingLetters = false;
                    Timer = false;
                    Pop_Up_Panel_Parent.gameObject.SetActive(true);
                    break;

                case 2:
                    Pop_Up_Panel.sprite = Missing_PopUp;
                    DailyChallenges = false;
                    MissingLetters = true;
                    Timer = false;
                    Pop_Up_Panel_Parent.gameObject.SetActive(true);
                    break;

                case 3:
                    Pop_Up_Panel.sprite = Timer_PopUp;
                    DailyChallenges = false;
                    MissingLetters = false;
                    Timer = true;
                    Pop_Up_Panel_Parent.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    break;

                default:
                    DailyChallenges = true;
                    MissingLetters = false;
                    Timer = false;
                    break;
            }
           
        }

        HighestLevel = PlayerPrefs.GetInt("HighestLevel");
        //CurrentLevel = PlayerPrefs.GetInt("SelectedLevel");

        Coins_Gems_Text_Update();

    }

    #region THEME_SELECTION

    public int relativeLevel;

    //DailyChallenges_Pop_Close
    public void Pop_Up_Close()
    {
        Pop_Up_Panel_Parent.gameObject.SetActive(false);
        if (Pop_Up_Panel.sprite == Timer_PopUp)
        {
            Time.timeScale = 1;
            Timer = true;
        }
    }

    public void ThemeSelection(bool dailychallenges)
    {
        int level_No = 0;

        List<Sprite[]> themes = new List<Sprite[]>
    {
        Paris, Egypt, London, Tokyo, SanFrancisco, LosAngeles, Toronto, Dubai,
        NewYork, Berlin, Barcelona, Bangkok, MexicoCity, KualaLumpur, Shanghai,
        Rome, Mumbai, SaoPaulo, Istanbul, CapeTown
    };

        List<string> themeNames = new List<string>
    {
        "Paris", "Egypt", "London", "Tokyo", "San Francisco", "Los Angeles",
        "Toronto", "Dubai", "New York", "Berlin", "Barcelona", "Bangkok",
        "Mexico City", "Kuala Lumpur", "Shanghai", "Rome", "Mumbai",
        "São Paulo", "Istanbul", "Cape Town"
    };

        int themeIndex = 0;

        if (!dailychallenges)
        {
            level_No = PlayerPrefs.GetInt("SelectedLevel", 1);

            if (PlayerPrefs.GetInt("BonusLevel") == 0)
            {
                themeIndex = ((level_No - 1) / 20) % themes.Count;
                relativeLevel = (level_No - 1) % 20 + 1;
            }
            else
            {
                level_No = PlayerPrefs.GetInt("Bonus");
                themeIndex = ((level_No - 1) / 4) % themes.Count;
                relativeLevel = (level_No - 1) % 4 + 1;
            }
        }
        else
        {
            level_No = PlayerPrefs.GetInt("DailyLevel");
            themeIndex = ((level_No - 1) / 4) % themes.Count;
            relativeLevel = (level_No - 1) % 4 + 1;
        }

        Sprite[] activeTheme = themes[themeIndex];
        Place_Text.text = themeNames[themeIndex];

        // Ensure the index is within range [0, 3]
        int spriteIndex = (!dailychallenges && PlayerPrefs.GetInt("BonusLevel") == 0)
                          ? Mathf.Clamp((relativeLevel - 1) / 5, 0, 3)  // Regular levels (every 5 levels -> new sprite)
                          : Mathf.Clamp(relativeLevel - 1, 0, 3);       // Bonus & Daily levels (every 4 levels -> new sprite)

        bg.sprite = activeTheme[spriteIndex];

        LoadingScreen.SetActive(false);
        SetColor(level_No, dailychallenges);
    }

    #endregion

    public void SetColor(int Level, bool DailyChallenges)
    {
        int ColorCode;

        if (DailyChallenges || PlayerPrefs.GetInt("BonusLevel") == 0)
        {
            // Change color every 4 levels
            ColorCode = ((Level - 1) / 4) % 5;  // Dividing by 4 for non-daily challenges
        }
        else
        {
            // Original logic for daily challenges
            ColorCode = (((Level - 1) / 20) % 20) / 5;  // Every 20 levels divided into 5 blocks
        }

        ColorCode = ColorCode % 5;  // Ensure within 0-4 range
        colorCode = ColorCodes[ColorCode];

        ChangeColr(colorCode);
    }

    void ChangeColr(string newColor)
    {
        colorCode = newColor;
        SetLineColor(colorCode);
        ChangeDotColor(colorCode);
        //SetTextColor(colorCode);
    }

    public void SetTextColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color colorCode))
        {
            TextPreview.color = colorCode;
            Debug.Log($"Text color successfully set to {hexColor}");
        }
        else
        {
            Debug.LogError($"Invalid hex color: {hexColor}");
        }
    }

    public void CurrentLevelButton(int Level,List<char> characters)
    {
        Debug.Log(characters.Count.ToString());
        for (int i = 0; i < characters.Count; i++)
        {
            Debug.Log(characters[i].ToString());
        }

        //Debug.LogWarning(Circle.Find(characters.Count.ToString()));
        CurrentLevelCircle = Circle.Find(characters.Count.ToString());
        CurrentLevelCircle.gameObject.SetActive(true);

        if (NewFontAsset !=null)
        {
            levelText.font = NewFontAsset;
            Level_Text_Settings.font = NewFontAsset;
            if (badgesText!=null)
            {
                badgesText.font = NewFontAsset;
            }
        }

        if (!Game_)
        {
            if (!DailyChallenges)
            {
                if (badgesText != null)
                {
                    badgesText.transform.parent.gameObject.SetActive(false);
                }
            }
            if (!MissingLetters || !DailyChallenges)
            {
                levelText.transform.parent.gameObject.SetActive(false);
            }
            if (Timer)
            {
                levelText.transform.parent.gameObject.SetActive(true);
            }
        }
        else
        {
            levelText.text = "LEVEL - " + Level.ToString();
            Level_Text_Settings.text = "LEVEL - " + Level.ToString();
        }
        if (PlayerPrefs.GetInt("BonusLevel") ==1)
        {
            levelText.text = "BonusLevel";
            Level_Text_Settings.text = "BonusLevel";
        }

        for (int i = 0; i < characters.Count; i++)
        {
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().changeText_(characters[i].ToString());
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().ChangeColor(Game.Instance.colorCode);
        }

        LoadGame();
        InstiateDots();
        maxLetters = characters.Count;

    }

    void Update()
    {
        TextPreview.text = CurrentWord;
        currentText(TextPreview.transform.parent.GetComponent<Image>(), maxLetters, CurrentWord);

        if (!Screens.Any(screen => screen.activeSelf))  // Check if no screens are active
        {
            Controls();
            if (Timer)
            {
                RemaningTime -= Time.deltaTime;
                TimerText.text = TimerUpdate(RemaningTime);
                if (RemaningTime <= 0)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    // Loads Game On Start...
    private void LoadGame()
    {
       
        for (int i = 0; i < CurrentLevelCircle.childCount; i++)
        {
            LettersUsedInLevel.Add(CurrentLevelCircle.GetChild(i));
        }

        extraWords.InstiateCells();
    }

    private void Controls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            extraWords.Deactivate();
            var ray = GetMousePositionInWorld2D();
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 10f, targetLayerMask);

            if (hit.collider != null)
            {
                isDrawing = true;
                dotId = 0;
                Letters.Add(hit.transform.gameObject);
                PlaySound(tap);
                ActiveDot(dotId, hit.transform.localPosition);
                lineRenderer.gameObject.SetActive(true);
                CurrentWord += hit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            var ray = GetMousePositionInWorld2D();
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 10f, targetLayerMask);

            if (hit.collider != null)
            {
                var targetObject = hit.transform.gameObject;
                if (!Letters.Contains(targetObject))
                {
                    PlaySound(tap);
                    ActiveDot(dotId, targetObject.transform.localPosition);
                    Letters.Add(targetObject);
                    CurrentWord += targetObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                }
            }

            DrawLine();
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            DeActiveDrawing();
        }
    }

    private void CheckResult(string currentWord)
    {
        foreach (Transform word in LineWords)
        {
            word.GetComponent<LineWord>().CheckAnswer(currentWord);
        }
        DOVirtual.DelayedCall(2f, () =>
        {
            foreach (Transform word in LineWords)
            {
                word.GetComponent<LineWord>().CheckAllCellsFilled();
            }
        });

        //if (!gameLevelWords.Words.Contains(currentWord) && extraWords.extraWordsFromFile.Contains(currentWord))
        //{
        //    Debug.Log(currentWord);
        //    extraWords.CheckExtraWord(currentWord);
        //}

        if (!GeneratePattern.Instance.AssignedWords.Contains(currentWord) && currentWord.Length>2)
        {
            if (ExtraWordsCount<4)
            {
                extraWords.CheckWord(currentWord);
                ExtraWordsCount++;
            }
        }

        //if (extraWords.CheckWord(currentWord))
        //{
        //    Debug.Log(currentWord);
        //    extraWords.CheckExtraWord(currentWord);
        //}
    }

    #region PAUSE SCREEN
    public void home()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region DotsScript

    public void ChangeDotColor(string hexColorCode)
    {
        for (int i = 0;i<DotParent.childCount;i++)
        {
            Image dot = DotParent.GetChild(i).transform.GetComponent<Image>();

            if (dot.GetComponent<Image>() != null)
            {
                // Convert hex color to Unity's Color
                if (ColorUtility.TryParseHtmlString(hexColorCode, out Color newColor))
                {
                    // Apply the new color to the material
                    dot.GetComponent<Image>().color = newColor;
                    //Debug.LogError($"{hexColorCode}" + "DOTCOLOR");
                }
                else
                {
                    Debug.LogError("Invalid hex color code");
                }
            }
            else
            {
                Debug.LogError("Image not found!");
            }

        }

        if (TextImage.GetComponent<Image>() != null)
        {
            // Convert hex color to Unity's Color
            if (ColorUtility.TryParseHtmlString(hexColorCode, out Color newColor))
            {
                // Apply the new color to the material
                TextImage.GetComponent<Image>().color = newColor;
            }
            else
            {
                Debug.LogError("Invalid hex color code");
            }
        }
        else
        {
            Debug.LogError("Image not found!");
        }


    }

    private void InstiateDots()
    {
        for (int i = 0; i < LettersUsedInLevel.Count; i++)
        {
            GameObject dot = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, DotParent);
            dot.SetActive(false);
            Dots.Add(dot);
        }
    }

    private void ActiveDot(int currentDot, Vector3 position)
    {
        Dots[currentDot].SetActive(true);
        Dots[currentDot].transform.localPosition = position;
        dotId++;
    }

    private void DeActivateDots()
    {
        foreach (var dot in Dots)
        {
            dot.SetActive(false);
        }
    }
    #endregion

    #region LineDraw

    public void SetLineColor(string hexColor)
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component is not assigned!");
            return;
        }

        Debug.Log($"Received Hex Color: {hexColor}");

        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            Debug.Log($"Successfully changed LineRenderer color to {hexColor}");
        }
        else
        {
            Debug.LogError($"Invalid hex color: {hexColor}");
        }
    }

    public Vector2 GetMousePositionInWorld2D()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        return new Vector2(worldPosition.x, worldPosition.y);
    }

    public void DrawLine()
    {
        drawPositions.Clear();

        if (Letters.Count > 0)
        {
            foreach (var letter in Letters)
            {
                drawPositions.Add(letter.transform.position);
            }

            Vector3 inputDrawPosition = (Vector3)GetMousePositionInWorld2D();
            drawPositions.Add(inputDrawPosition);

            lineRenderer.positionCount = drawPositions.Count;
            lineRenderer.SetPositions(drawPositions.ToArray());
        }
    }

    private void DeActiveDrawing()
    {
        isDrawing = false;
        CheckResult(CurrentWord);
        Letters.Clear();
        DeActivateDots();
        lineRenderer.positionCount = 0;
        drawPositions.Clear();
        lineRenderer.gameObject.SetActive(false);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (DailyChallenges)
            {
                DailyChallenge1();
            }
        });
    }
    #endregion

    #region LEVEL COMPLETE

    [SerializeField] Transform FillingBarFrame;

    // Stores unlocked bonuses to ensure bonus levels are not repeated
    public HashSet<int> unlockedBonuses = new HashSet<int>();

    /// <summary>
    /// Handles logic when a level is completed, including checking for bonus levels.
    /// </summary>
    public void LevelCompleted()
    {
        DOVirtual.DelayedCall(0.3f, () =>
        {
            if (!Game_)
            {
                // Increment daily level and reset if it exceeds 70
                PlayerPrefs.SetInt("DailyLevel", PlayerPrefs.GetInt("DailyLevel") + 1);
                PlayerPrefs.SetInt("DailyChallengeComplete", 1);
                AddCoins(50);
                if (PlayerPrefs.GetInt("DailyLevel") > 70)
                {
                    PlayerPrefs.SetInt("DailyLevel", 1);
                }
            }
            else
            {
                if (PlayerPrefs.GetInt("BonusLevel") == 1)
                {
                    BonusLevel.Instance.BounusLevelCompleted(PlayerPrefs.GetInt("Bonus", 0));
                }
                else
                {
                    int selectedLevel = PlayerPrefs.GetInt("SelectedLevel");

                    if (selectedLevel >= HighestLevel)
                    {
                        HighestLevel = selectedLevel;
                        PlayerPrefs.SetInt("HighestLevel", PlayerPrefs.GetInt("HighestLevel") + 1);
                        AddCoins(20);

                        if (relativeLevel == 20)
                        {
                            AddGems(5);
                        }
                    }
                }
            }

            // UI and game state updates after level completion
            SnowEffect.gameObject.SetActive(false);
            Dust.gameObject.SetActive(false);
            CompletedWords = 0;
            CurrentLevelCircle.gameObject.SetActive(false);
            levelComplete_Screen.gameObject.SetActive(true);
            LevelComplete_Level_No.text = relativeLevel + "/20";
            LevelComplete_FillingBar.fillAmount += (float)relativeLevel / 20;

            Coins_Gems_Text_Update(true);
            PlaySound(LevelComplete);
        });
    }

    // This method handles what happens when the Next button is clicked, managing level progression and bonus level triggers.
    public void NextButton()
    {
        PlaySound(tap); // Play tap sound effect

        if (Game_) // Check if the game is active
        {
            if (!PlayerPrefs.HasKey("Count")) // Initialize level count if not set
            {
                PlayerPrefs.SetInt("Count", 1);
            }

            int count = PlayerPrefs.GetInt("Count"); // Get the current count
            int bonus = PlayerPrefs.GetInt("Bonus", 0); // Get the bonus count
            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel"); // Get the current level

            if (count < 5) // Check if 5 levels have not been completed yet
            {
                PlayerPrefs.SetInt("SelectedLevel", selectedLevel + 1); // Move to the next level
                PlayerPrefs.SetInt("Count", count + 1); // Increment count
                PlayerPrefs.SetInt("BonusLevel", 0); // Reset bonus level flag
                SceneManager.LoadScene(1); // Load the next level
            }
            else // When 5 levels are completed
            {
                if (!unlockedBonuses.Contains(selectedLevel) && PlayerPrefs.GetInt("BonusLevel")==0) // Check if bonus level is not already played
                {
                    unlockedBonuses.Add(selectedLevel); // Mark bonus level as unlocked
                    PlayerPrefs.SetInt("BonusLevel", 1); // Set bonus level flag
                    SceneManager.LoadScene(1); // Load the bonus level
                }
                else // If bonus level was already played
                {
                    BonusLevelCompleted(bonus); // Call method to handle post-bonus logic
                }
            }
        }
        else // If game is not active
        {
            SceneManager.LoadScene(0); // Load main menu
        }
    }

    // Handles logic after a bonus level is completed
    private void BonusLevelCompleted(int bonus)
    {
        //Debug.LogError("Bonus Level Completed!"); // Log completion
        if (FillingBarFrame !=null)
        {
            FillingBarFrame.gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt("Bonus", bonus + 1); // Increment bonus count
        PlayerPrefs.SetInt("Count", 1); // Reset level count
        PlayerPrefs.SetInt("BonusLevel", 0); // Reset bonus level flag

        if (PlayerPrefs.GetInt("Bonus") >= 60) // Reset bonus count after 60
        {
            PlayerPrefs.SetInt("Bonus", 0);
        }

        SceneManager.LoadScene(0); // Load main menu after bonus completion
    }

    #endregion

    #region SOUNDS

    public void PlaySound(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void Sound_On_Off()
    {
        PlaySound(tap);

        if (Sound_Img.sprite == Sound_Onn)
        {
            Sound_Img.sprite = Sound_off;
            PlayerPrefs.SetInt("Sound", 0);
            AudioSource.volume = 0;
        }
        else
        {
            Sound_Img.sprite = Sound_Onn;
            PlayerPrefs.SetInt("Sound", 1);
            AudioSource.volume = 1;
        }
    }

    public void Music_On_Off()
    {
        PlaySound(tap);

        if (Music_Img.sprite == Music_onn)
        {
            Music_Img.sprite = Music_Off;
            PlayerPrefs.SetInt("Music", 0);
            Music_Source.volume = 0; // Assuming MusicSource is the AudioSource for music
        }
        else
        {
            Music_Img.sprite = Music_onn;
            PlayerPrefs.SetInt("Music", 1);
            Music_Source.volume = 1;
        }
    }

    #endregion

    #region POWER-UPS

    public void GetHint(int coins)
    {
        PlaySound(tap);

        if (Total_Coins >= coins)
        {
            // First foreach loop: Find the first uncompleted word and call Hint()
            foreach (Transform word in LineWords)
            {
                if (!word.GetComponent<LineWord>().AnswerChecked)
                {
                    word.GetComponent<LineWord>().Hint(Power_up.hintParent, Power_up.hintPosition);
                    RemoveCoins(coins);
                    Coins_Gems_Text_Update(true);
                    Debug.Log("HINT-CALLED");
                    break; // Ensure only one word is processed
                }
            }

            DOVirtual.DelayedCall(0.25f, () =>
            {
                // Second foreach loop: Check all cells for completion after the first loop
                foreach (Transform word in LineWords)
                {
                    var lineWord = word.GetComponent<LineWord>();
                    if (!lineWord.AnswerChecked)
                    {
                        lineWord.CheckAllCellsFilled();
                    }
                }
            });

        }
        else
        {
            Debug.LogWarning("Not enough coins for a Hint!");
        }
    }

    public void Shuffle()
    {
        Shuffele_Btn.interactable = false;
        PlaySound(tap);
        letters.Clear();
        letterPositions.Clear();

        // Collect letters and their initial positions
        foreach (var letter in LettersUsedInLevel)
        {
            letters.Add(letter);
            letterPositions.Add(letter.localPosition);
        }

        // Shuffle positions
        List<Vector3> shuffledPositions = new List<Vector3>(letterPositions);
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = Random.Range(0, shuffledPositions.Count);
            // Swap positions in the shuffled list
            Vector3 temp = shuffledPositions[i];
            shuffledPositions[i] = shuffledPositions[randomIndex];
            shuffledPositions[randomIndex] = temp;
        }

        // Assign shuffled positions back to letters with delay and smooth movement
        float delayBetweenMoves = 0.01f; // Delay between each move
        float totalDelayTime = letters.Count * delayBetweenMoves + 0.5f; // Total time after the last move is done

        for (int i = 0; i < letters.Count; i++)
        {
            int index = i; // Capture the current index for the delayed call
            DOVirtual.DelayedCall(index * delayBetweenMoves, () =>
            {
                letters[index].transform
                    .DOLocalMove(shuffledPositions[index], 0.5f) // Move to new position over 0.5 seconds
                    .SetEase(Ease.InOutQuad); // Smooth easing
            });
        }

        // Delay the button interaction enable after the last letter move
        DOVirtual.DelayedCall(totalDelayTime, () =>
        {
            Shuffele_Btn.interactable = true; // Enable the shuffle button after all letters have moved
        });
    }

    public void Get_Multi_Hint(int coins)
    {
        get_UNRevealed();

        // Number of hints to reveal (adjustable)
        int revealed = 3;
        if (Cells_In_GNR.Count < revealed)
        {
            revealed = Cells_In_GNR.Count; // Reduce to the actual number of available cells
            Debug.Log("REVEALED " + revealed);
        }

        // Check if the player has enough coins
        if (Total_Coins >= coins)
        {
            // Deduct coins and play sound
            RemoveCoins(coins);
            Coins_Gems_Text_Update(true);
            PlaySound(tap);

            // Create a DOTween sequence for the delayed hinting process
            Sequence hintSequence = DOTween.Sequence();

            // Reveal hints one by one
            for (int i = 0; i < revealed; i++)
            {
                hintSequence.AppendCallback(() =>
                {
                    // Pick a random cell and reveal the hint
                    int randomNumber = Random.Range(0, Cells_In_GNR.Count);
                    var selectedCell = Cells_In_GNR[randomNumber];
                    selectedCell.showText = true;
                    selectedCell.Hint(Power_up.MultiHintParent,Power_up.MultiHintPosition);
                    selectedCell.ChangeColor(Game.Instance.colorCode);

                    // Remove the cell from the list to avoid duplication
                    Cells_In_GNR.RemoveAt(randomNumber);
                });

                // Add a delay between each hint
                hintSequence.AppendInterval(0.1f);
            }

            // After all hints are revealed, check all words for completion
            hintSequence.AppendCallback(() =>
            {
                foreach (Transform word in LineWords)
                {
                    var lineWord = word.GetComponent<LineWord>();
                    if (!lineWord.AnswerChecked)
                    {
                        lineWord.CheckAllCellsFilled();
                    }
                }
            });

            // Start the sequence
            hintSequence.Play();
        }
        else
        {
            Debug.LogWarning("Not enough coins for a Multi-Hint!");
        }
    }

    public void Get_Spot_Light(int coins)
    {
        get_UNRevealed();

        // Check if the player has enough coins
        if (Total_Coins >= coins)
        {
            // Deduct coins and play sound
            RemoveCoins(coins);
            Coins_Gems_Text_Update(true);
            PlaySound(tap);

            randomNumber = Random.Range(0,Cells_In_GNR.Count);
            Cells_In_GNR[randomNumber].SpotLight();
        }
        else
        {
            Debug.LogWarning("Not enough coins for a Spot-Light!");
        }

    }

    public void Get_Word_Chain(int coins)
    {
        // Check if the player has enough coins
        if (Total_Coins >= coins)
        {
            // Deduct coins and play sound
            RemoveCoins(coins);
            Coins_Gems_Text_Update(true);
            PlaySound(tap);

            for (int i = 0; i < LineWords.Count; i++)
            {
                if (!LineWords[i].GetComponent<LineWord>().AnswerChecked)
                {
                    for (int j = 0; j < LineWords[i].GetComponent<LineWord>().Cells.Count; j++)
                    {
                        LineWords[i].GetComponent<LineWord>().Cells[j].GetComponent<Cell>().showText = true;
                        LineWords[i].GetComponent<LineWord>().Cells[j].GetComponent<Cell>().Hint(Power_up.WordChainParent,Power_up.WordChainPosition);
                        LineWords[i].GetComponent<LineWord>().Cells[j].GetComponent<Cell>().ChangeColor(Game.Instance.colorCode);
                    }
                    LineWords[i].GetComponent<LineWord>().CheckAllCellsFilled();
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Not enough coins for a Word-Chain!");
        }
    }

    #endregion

    #region DAILY CHALLENGES

    public void DailyChallenge1(bool First = false)
    {
        get_UNRevealed();

        if (TotalBadges > 0) // Only execute if the player has TotalBadges remaining
        {
            // Check if there are no available cells 
            if (Cells_In_GNR.Count == 0)
            {
                Debug.LogWarning("No available cells in Cells_In_GNR.");
                return;
            }

            if (First)
            {
                // Generate a random index within the available cells
                randomNumber = Random.Range(0, Cells_In_GNR.Count);
                CurrentFirstCell = Cells_In_GNR[randomNumber].GetComponent<Cell>(); // Get Cell component

                // Activate the Star_PopUp for the first selection
                CurrentFirstCell.Star.gameObject.SetActive(true);
                TotalBadges--; // Decrease the number of available TotalBadges
            }
            else
            {
                // Ensure the star is not already active before activating again
                if (CurrentFirstCell.showText)
                {
                    // Generate a random index within the available cells
                    randomNumber = Random.Range(0, Cells_In_GNR.Count);
                    CurrentFirstCell = Cells_In_GNR[randomNumber].GetComponent<Cell>(); // Get Cell component
                    CurrentFirstCell.Star.gameObject.SetActive(true);
                    TotalBadges--; // Decrease the number of available TotalBadges
                }
            }
        }
        badgesText.text = CollectedBadges.ToString();// Update UI Text
    }

    private System.String TimerUpdate(float elapsed)
    {
        int d = (int)(elapsed * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;
        int hundredths = d % 100;
        return System.String.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #endregion

    void get_UNRevealed()
    {
        // Clear the list of eligible cells
        Cells_In_GNR.Clear();

        // Populate eligible cells that don't have their text revealed yet
        foreach (Cell t in GeneratePattern.Instance.CellsUsed)
        {
            if (t != null && !t.showText)
            {
                Cells_In_GNR.Add(t);
            }
        }
    }

    void LoadSavedData()
    {
        PlayerData data = SaveExtraWords.LoadData();
        if (data != null)
        {
            if (data.wordsCollected != null)
            {
                extraWords.FoundedExtraWords.AddRange(data.wordsCollected);
            }

            // Set the current level to play from the saved data
            //HighestLevel = data.LevelToPlay;
            Debug.Log($"Loaded level to play: {HighestLevel}");
        }
    }

    void currentText(Image preview, int maxLetters, string text)
    {
        int currentLetters = Mathf.Clamp(text.Length, 0, maxLetters); // Ensure within bounds

        float sizeFactor = (float)currentLetters / maxLetters; // Normalize between 0 and 1

        // Set min & max width values (change as needed)
        float minWidth = 0f; // Smallest width when text length = 0
        float maxWidth = 465f; // Maximum width when text length = maxLetters

        // Lerp to smoothly transition between minWidth and maxWidth
        float newWidth = Mathf.Lerp(minWidth, maxWidth, sizeFactor);

        // Apply new width while keeping the height unchanged
        preview.rectTransform.sizeDelta = new Vector2(newWidth, preview.rectTransform.sizeDelta.y);
    }

    public void Coins_Gems_Text_Update(bool anim=false)
    {
        Total_Coins = PlayerPrefs.GetInt("Coins");
        Total_Gems = PlayerPrefs.GetInt("Gems");

        int currentCoins = int.Parse(text_update.coinsText.text);  // Get the current displayed coins
        int currentGems = int.Parse(text_update.gemsText.text);    // Get the current displayed gems

        if (anim)
        {
            // Animate Coins Count
            DOVirtual.Int(currentCoins, Total_Coins, 0.75f, value =>
            {
               
                text_update.coinsText.text = value.ToString();
                text_update.coinsText_LevelComplete.text = value.ToString();
                text_update.coinsText_Store.text = value.ToString();
                text_update.coinsText_Extra.text = value.ToString();
            });

            // Animate Gems Count
            DOVirtual.Int(currentGems, Total_Gems, 0.75f, value =>
            {
                
                text_update.gemsText.text = value.ToString();
                text_update.gemsText_LevelComplete.text = value.ToString();
                text_update.gemsText_Store.text = value.ToString();
                text_update.gemsText_Extra.text = value.ToString();
            });
        }
        else
        {
            text_update.coinsText.text= Total_Coins.ToString();
            text_update.coinsText_LevelComplete.text= Total_Coins.ToString();
            text_update.coinsText_Store.text= Total_Coins.ToString();
            text_update.coinsText_Extra.text= Total_Coins.ToString();

            text_update.gemsText.text = Total_Gems.ToString();
            text_update.gemsText_LevelComplete.text = Total_Gems.ToString();
            text_update.gemsText_Store.text = Total_Gems.ToString();
            text_update.gemsText_Extra.text = Total_Gems.ToString();
        }
    }

    #region COINS,GEMS ADD AND REMOVE

    public void AddCoins(int Count)
    {
        //Debug.LogError("COINS " + Count);
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + Count);
        Coins_Gems_Text_Update(true);
    }

    public void AddGems(int Count)
    {
        //Debug.LogError("GEMS " + Count);
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

    public void UpdateFont()
    {
        if (NewFontAsset != null)
        {
            TextPreview.font = Incircle;
            text_update.coinsText_LevelComplete.font = Incircle;
            text_update.coinsText_Store.font = Incircle;
            text_update.coinsText_Extra.font = Incircle;
            text_update.coinsText.font = Incircle;
            text_update.gemsText.font = Incircle;
            text_update.gemsText_Store.font = Incircle;
            text_update.gemsText_Extra.font = Incircle;
            text_update.gemsText_LevelComplete.font = Incircle;
        }
    }

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
        Timer = false;
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

    #region POWERUP_UNLOCK

    [SerializeField] powerUpImage powerUpImage;

    void PowerUpUnlock(int level)
    {
        // Safety check to avoid null reference errors
        if (powerUpImage == null)
        {
            Debug.LogError("PowerUpUnlock: Missing powerUpImage reference.");
            return;
        }

        // Dictionary storing power-up unlock levels, descriptions, sprites, and associated transforms
        Dictionary<int, (string description, Sprite sprite, Transform powerUpTransform)> powerUpData = new Dictionary<int, (string, Sprite, Transform)>
    {
        { 3, ("Reveals a random letter in multiple words", powerUpImage.Hint, powerUpImage.hint) },
        { 5, ("Reveals multiple letters across different words", powerUpImage.MultipleHints, powerUpImage.multiHint) },
        { 7, ("Highlights a single word in the puzzle", powerUpImage.SpotLight, powerUpImage.spotLight) },
        { 9, ("Reveals the first letter of connected words", powerUpImage.WordChain, powerUpImage.wordChain) }
    };

        // If level > 9, ensure all power-ups are permanently active
        if (level > 9)
        {
            ActivatePowerUp(powerUpImage.hint);
            ActivatePowerUp(powerUpImage.multiHint);
            ActivatePowerUp(powerUpImage.spotLight);
            ActivatePowerUp(powerUpImage.wordChain);
        }
        else
        {
            // Activate all previously unlocked power-ups
            foreach (var entry in powerUpData)
            {
                if (entry.Key <= level) // Ensure that once unlocked, it stays active
                {
                    ActivatePowerUp(entry.Value.powerUpTransform);
                }
            }

            // Show pop-up only if it's the first time unlocking this power-up
            if (powerUpData.TryGetValue(level, out var data))
            {
                string key = "PowerUpShown_" + level; // Unique key for PlayerPrefs
                if (PlayerPrefs.GetInt(key, 0) == 0) // Check if pop-up was already shown
                {
                    ApplyPowerUp(data.description, data.sprite);
                    PlayerPrefs.SetInt(key, 1); // Mark pop-up as shown
                    PlayerPrefs.Save(); // Save changes to PlayerPrefs
                }
            }
        }
    }

    /**
     * Displays the power-up pop-up only once when first unlocked.
     * @param text - Description of the power-up.
     * @param sprite - Icon representing the power-up.
     */
    void ApplyPowerUp(string text, Sprite sprite)
    {
        // Update power-up description and sprite icon
        powerUpImage.textPreview.text = text;
        powerUpImage.BaseImage.sprite = sprite;

        // Animate the power-up pop-up
        powerUpImage.PowerUp_Pop.transform.localScale = Vector3.zero;
        powerUpImage.Bg.gameObject.SetActive(true);
        powerUpImage.PowerUp_Pop.gameObject.SetActive(true);
        powerUpImage.PowerUp_Pop.DOScale(Vector3.one, 0.2f);

        // Auto-hide pop-up after 2 seconds
        DOVirtual.DelayedCall(2f, () =>
        {
            powerUpImage.PowerUp_Pop.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                powerUpImage.Bg.gameObject.SetActive(false);
                powerUpImage.PowerUp_Pop.gameObject.SetActive(false);
            });
        });
    }

    /**
     * Ensures the specified power-up remains active once unlocked.
     * @param powerUp - The transform of the power-up to activate.
     */
    void ActivatePowerUp(Transform powerUp)
    {
        if (powerUp != null && !powerUp.gameObject.activeSelf) // Activate only if not already active
        {
            powerUp.gameObject.SetActive(true);
        }
    }




    #endregion


    #endregion //MethodsEnd

}




