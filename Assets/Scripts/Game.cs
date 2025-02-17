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

[System.Serializable]
public class StorePanel
{
    public GameObject Root;
    public List<GameObject> Coins,Gems;
    public RectTransform Coins_Btn, Gems_Btn;
    public Transform coin_pos, coin_rot;
    public Transform gem_pos,gem_rot;
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

    [HideInInspector]
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

    [SerializeField, HideInInspector] Sprite[] Paris;
    [SerializeField, HideInInspector] Sprite[] Egypt;
    [SerializeField, HideInInspector] Sprite[] London;
    [SerializeField, HideInInspector] Sprite[] Tokyo;
    [SerializeField, HideInInspector] Sprite[] SanFrancisco;
    [SerializeField, HideInInspector] Sprite[] LosAngeles;
    [SerializeField, HideInInspector] Sprite[] Toronto;
    [SerializeField, HideInInspector] Sprite[] Dubai;
    [SerializeField, HideInInspector] Sprite[] NewYork;
    [SerializeField, HideInInspector] Sprite[] Berlin;
    [SerializeField, HideInInspector] Sprite[] Barcelona;
    [SerializeField, HideInInspector] Sprite[] Bangkok;
    [SerializeField, HideInInspector] Sprite[] MexicoCity;
    [SerializeField, HideInInspector] Sprite[] KualaLumpur;
    [SerializeField, HideInInspector] Sprite[] Shanghai;
    [SerializeField, HideInInspector] Sprite[] Rome;
    [SerializeField, HideInInspector] Sprite[] Mumbai;
    [SerializeField, HideInInspector] Sprite[] SaoPaulo;
    [SerializeField, HideInInspector] Sprite[] Istanbul;
    [SerializeField, HideInInspector] Sprite[] CapeTown;

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

        //LoadAllCitySprites();

        if (!DailyChallenges)
        {
            //ThemeSelection();
        }

        LoadSavedData();

        //Coins_Gems_Text_Update();

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

    public void Pop_Up_Close()
    {
        Pop_Up_Panel_Parent.gameObject.SetActive(false);
        if (Pop_Up_Panel.sprite == Timer_PopUp)
        {
            Time.timeScale = 1;
            Timer = true;
        }
    }

    public void LoadAllCitySprites()
    {
        StartCoroutine(LoadCitySpritesForLevel(PlayerPrefs.GetInt("SelectedLevel"), () =>
        {
            // Once the necessary city sprites are loaded, call ThemeSelection
            ThemeSelection();
        }));
    }

    // Coroutine to load all city sprites
    private IEnumerator LoadCitySpritesForLevel(int selectedLevel, System.Action onComplete)
    {
        // Define the cities to load
        string[] cities = new string[] {
        "Paris", "Egypt", "London", "Tokyo", "SanFrancisco", "LosAngeles", "Toronto", "Dubai", "NewYork",
        "Berlin", "Barcelona", "Bangkok", "MexicoCity", "KualaLumpur", "Shanghai", "Rome", "Mumbai",
        "SaoPaulo", "Istanbul", "CapeTown"
    };

        yield return new WaitForSeconds(5f);

        // Define the base path to the extracted folder
        string basePath = DownLoadThemes.instance.ExtractedPath;

        // Calculate the city index based on the level
        int cityIndex = (selectedLevel - 1) / 20; // Each city is associated with 20 levels

        // Ensure cityIndex stays within the bounds of available cities
        if (cityIndex >= cities.Length)
        {
            cityIndex = cities.Length - 1;
        }

        // Determine the city based on the level range
        string selectedCity = cities[cityIndex];

        // Wait for a moment before loading the sprites (simulate loading time if needed)
        yield return new WaitForSeconds(1f);

        // Load sprites for the selected city
        Sprite[] sprites = LoadSpritesFromPath(selectedCity, basePath);

        // Assign the loaded sprites to the respective city variable
        AssignCitySprites(selectedCity, sprites);

        Debug.Log("Sprites loaded for city: " + selectedCity);

        // Call the onComplete action after loading the necessary sprites
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    public Sprite[] LoadSpritesFromResources(string cityName)
    {
        // Path inside the Resources folder
        string resourcePath = $"Themes/{cityName}";

        Debug.Log("Loading sprites from Resources path: " + resourcePath);

        // Load all sprites from the specified path
        Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("No sprites found in Resources at path: " + resourcePath);
            return null;
        }

        Debug.Log($"Loaded {sprites.Length} sprites from {resourcePath}");

        return sprites;
    }

    // Load sprites from the path for a given city
    private Sprite[] LoadSpritesFromPath(string cityName, string basePath)
    {
        // Ensure basePath does not end with a separator
        if (basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            basePath = basePath.TrimEnd(Path.DirectorySeparatorChar);
        }

        Debug.LogWarning("BasePath : "+ basePath);

        // Use Path.Combine to correctly build the directory path
        string directoryPath = Path.Combine(basePath, "Themes", cityName); // Full path for city

        // Debug log the directory path
        Debug.Log("Checking directory path: " + directoryPath);

        // If directory doesn't exist, try lowercase city name
        if (!Directory.Exists(directoryPath))
        {
            directoryPath = Path.Combine(basePath, "Themes", cityName.ToLower());
            Debug.Log("Directory not found. Trying lowercase path: " + directoryPath);
        }

        // Check if directory exists after both attempts
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError("Directory not found: " + directoryPath);
            return null;
        }

        // Get all PNG and JPG files in the directory
        string[] pngFiles = Directory.GetFiles(directoryPath, "*.png");
        string[] jpgFiles = Directory.GetFiles(directoryPath, "*.jpg");

        // Combine both arrays
        string[] filePaths = pngFiles.Concat(jpgFiles).ToArray();
        if (filePaths.Length == 0)
        {
            Debug.LogError("No PNG files found in directory: " + directoryPath);
        }
        else
        {
            foreach (string filePath in filePaths)
            {
                //Debug.Log(filePath);
            }
        }

        // Load sprites from the files
        Sprite[] sprites = new Sprite[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            byte[] imageData = File.ReadAllBytes(filePaths[i]);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                sprites[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load texture: " + filePaths[i]);
            }
        }

        return sprites;
    }

    // Assign sprites to respective city variables
    private void AssignCitySprites(string cityName, Sprite[] sprites)
    {
        switch (cityName)
        {
            case "Paris":
                Paris = sprites;
                break;
            case "Egypt":
                Egypt = sprites;
                break;
            case "London":
                London = sprites;
                break;
            case "Tokyo":
                Tokyo = sprites;
                break;
            case "SanFrancisco":
                SanFrancisco = sprites;
                break;
            case "LosAngeles":
                LosAngeles = sprites;
                break;
            case "Toronto":
                Toronto = sprites;
                break;
            case "Dubai":
                Dubai = sprites;
                break;
            case "NewYork":
                NewYork = sprites;
                break;
            case "Berlin":
                Berlin = sprites;
                break;
            case "Barcelona":
                Barcelona = sprites;
                break;
            case "Bangkok":
                Bangkok = sprites;
                break;
            case "MexicoCity":
                MexicoCity = sprites;
                break;
            case "KualaLumpur":
                KualaLumpur = sprites;
                break;
            case "Shanghai":
                Shanghai = sprites;
                break;
            case "Rome":
                Rome = sprites;
                break;
            case "Mumbai":
                Mumbai = sprites;
                break;
            case "SaoPaulo":
                SaoPaulo = sprites;
                break;
            case "Istanbul":
                Istanbul = sprites;
                break;
            case "CapeTown":
                CapeTown = sprites;
                break;
            default:
                Debug.LogError("City not found in switch: " + cityName);
                break;
        }
    }

    public void ThemeSelection()
    {
        int level_No = PlayerPrefs.GetInt("SelectedLevel", 1);

        // Define all themes and their corresponding sprites
        List<Sprite[]> themes = new List<Sprite[]>
    {
        Paris,
        Egypt,
        London,
        Tokyo,
        SanFrancisco,
        LosAngeles,
        Toronto,
        Dubai,
        NewYork,
        Berlin,
        Barcelona,
        Bangkok,
        MexicoCity,
        KualaLumpur,
        Shanghai,
        Rome,
        Mumbai,
        SaoPaulo,
        Istanbul,
        CapeTown
    };

        // Define the theme names
        List<string> themeNames = new List<string>
    {
        "Paris",
        "Egypt",
        "London",
        "Tokyo",
        "San Francisco",
        "Los Angeles",
        "Toronto",
        "Dubai",
        "New York",
        "Berlin",
        "Barcelona",
        "Bangkok",
        "Mexico City",
        "Kuala Lumpur",
        "Shanghai",
        "Rome",
        "Mumbai",
        "São Paulo",
        "Istanbul",
        "Cape Town"
    };

        int themeIndex = ((level_No - 1) / 20) % themes.Count;

        // Get the active theme
        Sprite[] activeTheme = themes[themeIndex];

        // Set the place text to the current theme's name
        Place_Text.text = themeNames[themeIndex];

        // Calculate the relative level within the 20-level block
        int relativeLevel = (level_No - 1) % 20;

        // Determine which sprite to use based on 5-level sub-blocks
        if (relativeLevel < 5)
        {
            bg.sprite = activeTheme[0];
        }
        else if (relativeLevel < 10)
        {
            bg.sprite = activeTheme[1];
        }
        else if (relativeLevel < 15)
        {
            bg.sprite = activeTheme[2];
        }
        else
        {
            bg.sprite = activeTheme[3];
        }
        LoadingScreen.SetActive(false);
    }

    #endregion

    public void SetColor(int Level)
    {
        // Calculate the color code based on theme index
        int ColorCode = (((Level - 1) / 20) % 20) / 5;  // Each block of 5 themes has the same color code

        // Ensure ColorCode stays within 0-4 range (if there are exactly 5 color codes)
        ColorCode = ColorCode % 5;

        colorCode = ColorCodes[ColorCode];

        // Change the color based on the theme index
        ChangeColr(colorCode);

    }

    void ChangeColr(string newColor)
    {
        colorCode = newColor;
        SetLineColor(colorCode);
        ChangeDotColor(colorCode);
        SetTextColor(colorCode);
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
        currentText(TextPreview.transform.parent.GetComponent<Image>(),maxLetters,CurrentWord);
        
        if (!Screens_Active(Screens))
        {
            Controls();
            if (Timer)
            {
                RemaningTime -= Time.deltaTime;
                TimerText.text = TimerUpdate(RemaningTime);
                if (RemaningTime <=0)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    bool Screens_Active(List<GameObject> Screens)
    {
        // Check if all screens are active
        return Screens.All(screen => screen.activeSelf);
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
            if (Game.Instance.DailyChallenges)
            {
                Game.Instance.DailyChallenge1();
            }
        });
    }
    #endregion

    #region LEVEL COMPLETE

    public void LevelCompleted()
    {
        DOVirtual.DelayedCall(0.3f, () =>
        {
            if (!Game_)
            {
                PlayerPrefs.SetInt("DailyLevel",PlayerPrefs.GetInt("DailyLevel")+1);
                PlayerPrefs.SetInt("DailyChallengeComplete", 1);
                if (PlayerPrefs.GetInt("DailyLevel")>70)
                {
                    PlayerPrefs.SetInt("DailyLevel", 1);
                }
            }

            else
            {
                if (PlayerPrefs.GetInt("SelectedLevel") >= HighestLevel)
                {
                    HighestLevel = PlayerPrefs.GetInt("SelectedLevel");

                    PlayerPrefs.SetInt("HighestLevel", PlayerPrefs.GetInt("HighestLevel") + 1);
                    AddCoins(20);
                    if (relativeLevel == 20)
                    {
                        AddGems(5);
                    }
                }
            }

            SnowEffect.gameObject.SetActive(false);
            Dust.gameObject.SetActive(false);
            CompletedWords = 0;
            CurrentLevelCircle.gameObject.SetActive(false);
            levelComplete_Screen.gameObject.SetActive(true);
            LevelComplete_Level_No.text = relativeLevel + "/20";
            LevelComplete_FillingBar.fillAmount += (float)relativeLevel / 20;
            
            //extraWords.SaveData();
            Coins_Gems_Text_Update(true);
            PlaySound(LevelComplete);
        });
    }

    public void NextButton()
    {
        PlaySound(tap);

        if (Game_)
        {
            // Initialize "Count" if not already set
            if (!PlayerPrefs.HasKey("Count"))
            {
                PlayerPrefs.SetInt("Count", 1);
            }

            int count = PlayerPrefs.GetInt("Count");
            int bonusLevel = PlayerPrefs.GetInt("BonusLevel");
            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel");
            int bonus = PlayerPrefs.GetInt("Bonus", 0); // Default value 0

            if (count < 5 && bonusLevel == 0)
            {
                // Update PlayerPrefs BEFORE scene transition
                PlayerPrefs.SetInt("SelectedLevel", selectedLevel + 1);
                PlayerPrefs.SetInt("Count", count + 1);
                PlayerPrefs.SetInt("BonusLevel", 0);

                Debug.LogError("COUNT " + (count + 1));
                SceneManager.LoadScene(1);
            }
            else
            {
                if (count == 5 && bonusLevel == 0)
                {
                   // Debug.LogError("Bonus-Level " + bonus);

                    // Update PlayerPrefs BEFORE scene transition
                    PlayerPrefs.SetInt("BonusLevel", 1);
                    PlayerPrefs.SetInt("Count", 1);

                    SceneManager.LoadScene(1);
                }
                else
                {
                   // Debug.LogError("Bonus-Completed " + bonus);

                    // Update PlayerPrefs BEFORE scene transition
                    PlayerPrefs.SetInt("Bonus", bonus + 1);
                    PlayerPrefs.SetInt("Count", 1);
                    PlayerPrefs.SetInt("BonusLevel", 0);

                    if (PlayerPrefs.GetInt("Bonus") >= 60)
                    {
                        PlayerPrefs.SetInt("Bonus", 0); // Reset to 0 instead of 1
                    }

                    SceneManager.LoadScene(0);
                }
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }
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
        Debug.LogError("COINS " + Count);
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + Count);
    }

    public void AddGems(int Count)
    {
        Debug.LogError("GEMS " + Count);
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + Count);
    }

    public void RemoveCoins(int Count)
    {
        if (PlayerPrefs.GetInt("Coins") > 0)
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - Count);
        }
    }

    public void RemoveGems(int Count)
    {
        if (PlayerPrefs.GetInt("Gems") > 0)
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - Count);
        }
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

    #endregion

    #endregion


}
