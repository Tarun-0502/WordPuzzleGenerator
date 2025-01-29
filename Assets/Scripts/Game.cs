using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] int ExtraWordsCount;

    #endregion

    [SerializeField] internal bool DailyChallenges,MissingLetters,Timer;
    [SerializeField] internal List<Transform> GetCell_Word;
    [SerializeField] Cell CurrentFirstCell;
    [SerializeField] int Badges = 3;
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

    [SerializeField] private int Total_Coins;
    [SerializeField] private TextMeshProUGUI coinsText;
    public Transform CoinPosition;
    public Transform hintPosition;
    public Transform hintParent;

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

    [SerializeField] AudioSource AudioSource;
    [SerializeField] internal AudioClip LevelComplete, CoinCollect, WordComplete, Hint,tap;

    [SerializeField] Sprite[] Paris; // Sprites for Paris Theme
    [SerializeField] Sprite[] NewYork; // Sprites for New York Theme
    [SerializeField] Sprite[] Tokyo; // Sprites for Tokyo Theme
    [SerializeField] Sprite[] Egypt; // Sprites for Egypt Theme
    [SerializeField] Sprite[] Sydney; // Sprites for Sydney Theme

    [SerializeField] Sprite Sound_Onn, Sound_off,Music_onn,Music_Off;
    [SerializeField] Image Sound_Img, Music_Img;

    [SerializeField] AudioSource Music_Source;

    [SerializeField] int HighestLevel;
    [SerializeField] int CurrentLevel;

    [SerializeField] List<string> ColorCodes;

    [SerializeField] int randomNumber;

    [SerializeField] internal bool Game_;

    #endregion


    #region METHODS

    void Start()
    {
        int soundSetting = PlayerPrefs.GetInt("Sound", 1); // Default to 1 if not set
        if (soundSetting == 1)
        {
            Sound_Img.sprite = Sound_Onn;
            AudioSource.volume = 1;
        }
        else
        {
            Sound_Img.sprite = Sound_off;
            AudioSource.volume = 0;
        }
        // Initialize Music Setting
        int musicSetting = PlayerPrefs.GetInt("Music", 1); // Default to 1 if not set
        if (musicSetting == 1)
        {
            Music_Img.sprite = Music_onn;
            Music_Source.volume = 1; // Assuming MusicSource is the AudioSource for music
        }
        else
        {
            Music_Img.sprite = Music_Off;
            Music_Source.volume = 0;
        }


        if (gameLevelWords==null)
        {
            gameLevelWords = FindObjectOfType<GameLevelWords>();
        }

        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins",Total_Coins);
        }

        Total_Coins = PlayerPrefs.GetInt("Coins");

        mainCamera = Camera.main;
        GameObject lineRe = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, LineParent);
        lineRenderer = lineRe.GetComponent<LineRenderer>();
        lineRenderer.gameObject.SetActive(false);

        if (NewFontAsset != null)
        {
            TextPreview.font = Incircle;
            coinsText.font = NewFontAsset;
        }

        if (!DailyChallenges)
        {
            ThemeSelection();
        }

        LoadSavedData();


        if (!Game_)
        {
            switch (PlayerPrefs.GetInt("Daily"))
            {
                case 1:
                    DailyChallenges = true;
                    MissingLetters = false;
                    Timer = false;
                    break;

                case 2:
                    DailyChallenges = false;
                    MissingLetters = true;
                    Timer = false;
                    break;

                case 3:
                    DailyChallenges = false;
                    MissingLetters = false;
                    Timer = true;
                    break;

                default:
                    DailyChallenges = true;
                    MissingLetters = false;
                    Timer = false;
                    break;
            }
        }

        HighestLevel = PlayerPrefs.GetInt("HighestLevel");
        CurrentLevel = PlayerPrefs.GetInt("SelectedLevel");
    }

    public void ThemeSelection()
    {
        int level_No = PlayerPrefs.GetInt("SelectedLevel", 1);

        // Define all themes and their corresponding sprites
        List<Sprite[]> themes = new List<Sprite[]>
        {
          Paris,  // Theme for Paris
          NewYork, // Theme for New York
          Tokyo,  // Theme for Tokyo
          Egypt, // Theme for Egypt
          Sydney  // Theme for Sydney
        };

        // Define the theme names
        List<string> themeNames = new List<string>
        {
          "Paris",
          "New York",
          "Tokyo",
          "Egypt",
          "Sydney"
         };

        // Determine the cyclic theme index
        int themeIndex = ((level_No - 1) / 20) % themes.Count;

        // Get the active theme
        Sprite[] activeTheme = themes[themeIndex];

        // Change the color based on the theme index
        ChangeColr(ColorCodes[themeIndex]);

        // Set the place text to the current theme's name
        Place_Text.text = themeNames[themeIndex];


        // Calculate the relative level within the 20-level block
        int relativeLevel = (level_No - 1) % 20 + 1;

        // Determine which sprite to use based on 5-level sub-blocks
        if (relativeLevel >= 1 && relativeLevel <= 5)
        {
            bg.sprite = activeTheme[0];
        }
        else if (relativeLevel >= 6 && relativeLevel <= 10)
        {
            bg.sprite = activeTheme[1];
        }
        else if (relativeLevel >= 11 && relativeLevel <= 15)
        {
            bg.sprite = activeTheme[2];
        }
        else
        {
            bg.sprite = activeTheme[3];
        }
    }

    void ChangeColr(string newColor)
    {
        colorCode = newColor;
        SetLineColor(colorCode);
        ChangeDotColor(colorCode);
    }

    public void CurrentLevelButton(int Level,List<char> characters)
    {

        //Debug.Log(characters.Count.ToString());
        //for (int i = 0; i < characters.Count; i++)
        //{
        //    Debug.Log(characters[i].ToString());
        //}

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

        for (int i = 0; i < characters.Count; i++)
        {
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().changeText_(characters[i].ToString());
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().ChangeColor(Game.Instance.colorCode);
        }

        LoadGame();
        InstiateDots();
        if (DailyChallenges)
        {
            //DailyChallenge1(true);
        }
    }

    void Update()
    {
        TextPreview.text = CurrentWord;
        Total_Coins = PlayerPrefs.GetInt("Coins");
        coinsText.text = Total_Coins.ToString();
        if (!SettingsScreen.activeInHierarchy)
        {
            Controls();
            Time.timeScale = 1;
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

    #region Hint And Shuffle

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
                    word.GetComponent<LineWord>().Hint();
                    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - coins);
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
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            // Assign the parsed color to the LineRenderer
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
    }
    #endregion

    #region LEVEL COMPLETE

    public void LevelCompleted()
    {
        DOVirtual.DelayedCall(0.3f, () =>
        {
            SnowEffect.gameObject.SetActive(false);
            Dust.gameObject.SetActive(false);
            CompletedWords = 0;
            CurrentLevelCircle.gameObject.SetActive(false);
            levelComplete_Screen.gameObject.SetActive(true);
            LevelComplete_Level_No.text = PlayerPrefs.GetInt("SelectedLevel", 1).ToString();

            if (PlayerPrefs.GetInt("SelectedLevel") >= HighestLevel)
            {
                HighestLevel = PlayerPrefs.GetInt("SelectedLevel");
                PlayerPrefs.SetInt("HighestLevel", PlayerPrefs.GetInt("HighestLevel") + 1);
            }
            //extraWords.SaveData();

            // Scale to Vector3.one over 0.35 seconds.. Apply the OutBack easing function
            levelComplete_Screen.GetChild(0).transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    int index = i; // Create a local copy of i
                    DOVirtual.DelayedCall(0.1f * index, () =>
                    {
                        Transform star = levelComplete_Screen.GetChild(0).GetChild(index);
                        star.transform.DOScale(Vector3.one, 0.25f);
                    });
                }
            });
            PlaySound(LevelComplete);
        });
    }

    public void NextButton(int scene)
    {
        PlaySound(tap);
        if (!PlayerPrefs.HasKey("Count"))
        {
            PlayerPrefs.SetInt("Count", 1);
        }
        if (PlayerPrefs.GetInt("Count")<5)
        {
            PlayerPrefs.SetInt("SelectedLevel", PlayerPrefs.GetInt("SelectedLevel") + 1);
            PlayerPrefs.SetInt("Count", PlayerPrefs.GetInt("Count")+1);
            SceneManager.LoadScene(scene);
        }
        else
        {
            SceneManager.LoadScene(0);
            PlayerPrefs.SetInt("Count", 1);
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
            Total_Coins -= coins;
            PlayerPrefs.SetInt("Coins", Total_Coins);
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
                    selectedCell.Hint();
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
            Total_Coins -= coins;
            PlayerPrefs.SetInt("Coins", Total_Coins);
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
            Total_Coins -= coins;
            PlayerPrefs.SetInt("Coins", Total_Coins);
            PlaySound(tap);

            for (int i = 0; i < LineWords.Count; i++)
            {
                if (!LineWords[i].GetComponent<LineWord>().AnswerChecked)
                {
                    for (int j = 0; j < LineWords[i].GetComponent<LineWord>().Cells.Count; j++)
                    {
                        LineWords[i].GetComponent<LineWord>().Cells[j].GetComponent<Cell>().showText = true;
                        LineWords[i].GetComponent<LineWord>().Cells[j].GetComponent<Cell>().Hint();
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
        if (Badges>0)
        {
            randomNumber = Random.Range(0, GetCell_Word.Count);
            if (First)
            {
                CurrentFirstCell = GetCell_Word[randomNumber].GetComponent<Cell>();
                CurrentFirstCell.Star.gameObject.SetActive(true);
                Debug.LogWarning(CurrentFirstCell.name);
                GetCell_Word.RemoveAt(randomNumber);
            }
            else
            {
                if(CurrentFirstCell.showText)
                {
                    CurrentFirstCell = GetCell_Word[randomNumber].GetComponent<Cell>();
                    CurrentFirstCell.Star.gameObject.SetActive(true);
                    Debug.LogWarning(CurrentFirstCell.name);
                    GetCell_Word.RemoveAt(randomNumber);
                    Badges--;
                }
            }
            badgesText.text = (3 - Badges).ToString();
        }
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

    #endregion

    
}
