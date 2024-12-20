using DG.Tweening;
using System.Collections.Generic;
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
    [SerializeField] internal List<Transform> LineWords = new List<Transform>();

    [HideInInspector]
    [SerializeField] List<Transform> LettersUsedInLevel = new List<Transform>();

    [HideInInspector]
    [SerializeField] internal int CompletedWords;

    #endregion



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
    private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    private List<Transform> letters = new List<Transform>();

    [SerializeField] internal GameObject levelSelectionScreen;

    private Transform CurrentLevelCircle;

    [SerializeField] private int Total_Coins;
    [SerializeField] private TextMeshProUGUI coinsText;
    public Transform CoinPosition;
    public Transform hintPosition;
    public Transform hintParent;

    [SerializeField] GameObject PauseScreen;

    [SerializeField] TextMeshProUGUI levelText;

    [Tooltip("Font is Used EveryWhere in Game")]
    [SerializeField] internal TMP_FontAsset NewFontAsset,Incircle;

    [Tooltip("ColorCode")]
    [SerializeField] internal string colorCode;

    [SerializeField] ParticleSystem SnowEffect,Dust;
    [SerializeField] Transform levelComplete_Screen;
    [SerializeField] Transform[] stars;

    [SerializeField] Image bg;
    [SerializeField] Sprite bg1, bg2;

    [SerializeField] Image TextImage;
    [SerializeField] TextMeshProUGUI LevelComplete_Level_No;

    [SerializeField] AudioSource AudioSource;
    [SerializeField] internal AudioClip LevelComplete, CoinCollect, WordComplete, Hint,tap;

    [SerializeField] Sprite[] Paris_Theme; // Sprites for Paris Theme
    [SerializeField] Sprite[] NewYork_Theme; // Sprites for New York Theme
    [SerializeField] Sprite[] Tokyo_Theme; // Sprites for Tokyo Theme
    [SerializeField] Sprite[] Egypt_Theme; // Sprites for Egypt Theme
    [SerializeField] Sprite[] Sydney_Theme; // Sprites for Sydney Theme

    #endregion


    #region METHODS

    void Start()
    {
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
        //SetLineColor(colorCode);
        //ChangeDotColor(colorCode);

        if (NewFontAsset != null)
        {
            TextPreview.font = Incircle;
            coinsText.font = NewFontAsset;
        }

        SetLineColor(colorCode);
        ChangeDotColor(colorCode);
        ThemeSelection();
    }

    public void ThemeSelection()
    {
        int level_No = PlayerPrefs.GetInt("SelectedLevel", 1);

        // Define all themes and their corresponding sprites
        List<Sprite[]> themes = new List<Sprite[]>
        {
            Paris_Theme,  // Levels 1–20
            NewYork_Theme, // Levels 21–40
            Tokyo_Theme,  // Levels 41–60
            Egypt_Theme, // Levels 61–80
            Sydney_Theme  // Levels 81–100
        };

        // Determine the theme index based on the 20-level blocks
        int themeIndex = (level_No - 1) / 20;

        // Ensure the theme index does not exceed the number of available themes
        if (themeIndex >= themes.Count)
        {
            themeIndex = themes.Count - 1; // Default to the last theme if out of range
        }

        // Get the active theme
        Sprite[] activeTheme = themes[themeIndex];

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

    public void CurrentLevelButton(int Level,List<char> characters)
    {
        //Debug.Log(HighestWord + "-" + currentWordsCount);

        //Debug.Log(characters.Count.ToString());
        //for (int i = 0; i < characters.Count; i++)
        //{
        //    Debug.Log(characters[i].ToString());
        //}

        CurrentLevelCircle = Circle.Find(characters.Count.ToString());
        CurrentLevelCircle.gameObject.SetActive(true);

        levelText.font = NewFontAsset;
        levelText.text = "LEVEL - " + Level.ToString();

        for (int i = 0; i < characters.Count; i++)
        {
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().changeText_(characters[i].ToString());
            CurrentLevelCircle.GetChild(i).GetComponent<ChangeText>().ChangeColor(Game.Instance.colorCode);
        }

        LoadGame();
        InstiateDots();
        levelSelectionScreen.SetActive(false);
    }

    void Update()
    {
        TextPreview.text = CurrentWord;
        Total_Coins = PlayerPrefs.GetInt("Coins");
        coinsText.text = Total_Coins.ToString();
        if (!PauseScreen.activeInHierarchy)
        {
            Controls();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }

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

        if (!GeneratePattern.Instance.AssignedWords.Contains(currentWord))
        {
            extraWords.CheckWord(currentWord);
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

    public void Resume()
    {
        PauseScreen.SetActive(!PauseScreen.activeInHierarchy);
    }

    #endregion

    #region Hint And Shuffle

    public void GetHint(int hintCoins)
    {
        if (Total_Coins>=hintCoins)
        {
            foreach (Transform word in LineWords)
            {
                if (!word.GetComponent<LineWord>().AnswerChecked)
                {
                    word.GetComponent<LineWord>().Hint(hintCoins);
                    break;
                }
            }
        }
    }

    public void Shuffle()
    {
        texts.Clear();
        letters.Clear();

        foreach (var letter in LettersUsedInLevel)
        {
            letters.Add(letter);
            texts.Add(letter.GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        int total_Iterations = letters.Count;
        for (int i = 0; i < total_Iterations; i++)
        {
            int random = Random.Range(0, letters.Count);
            texts[i].transform.SetParent(letters[random]);
            letters.RemoveAt(random);
        }

        foreach (var text in texts)
        {
            float delay = 0.1f; // Adjust the delay between each iteration (e.g., 0.5 seconds)
            int index = System.Array.IndexOf(texts.ToArray(), text); // Get the index of the current iteration

            // Use DOVirtual.DelayedCall to delay each iteration
            DOVirtual.DelayedCall(index * delay, () =>
            {
                // Smooth movement to the new position
                text.transform
                    .DOLocalMove(Vector2.zero, 0.5f) // Move to localPosition (0, 0) over 0.5 seconds
                    .SetEase(Ease.InOutQuad); // Use an easing function for smooth animation
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
        SnowEffect.gameObject.SetActive(false);
        Dust.gameObject.SetActive(false);
        CompletedWords = 0;
        CurrentLevelCircle.gameObject.SetActive(false);
        levelComplete_Screen.gameObject.SetActive(true);
        LevelComplete_Level_No.text = PlayerPrefs.GetInt("SelectedLevel", 1).ToString();

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < stars.Length; i++)
        {
            int currentIndex = i; // Capture the current index in a local variable
            sequence.AppendCallback(() =>
            {
                stars[currentIndex].GetChild(0).gameObject.SetActive(true);
            });
            sequence.AppendInterval(0.35f);
        }
        PlaySound(LevelComplete);
    }

    public void NextButton()
    {
       SceneManager.LoadScene(0);
    }

    #endregion

    #region SOUNDS

    public void PlaySound(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    #endregion

    #endregion

}
