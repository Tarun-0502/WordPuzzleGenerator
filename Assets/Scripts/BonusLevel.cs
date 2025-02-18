using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ThemePuzzle
{
    public Sprite MainImage;
    public Sprite _1;
    public Sprite _2;
    public Sprite _3;
    public Sprite _4;
}

public class BonusLevel : MonoBehaviour
{
    #region SINGLETON

    public static BonusLevel Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region THEME_PUZZLE

    [SerializeField] ThemePuzzle Paris;
    [SerializeField] ThemePuzzle Egypt;
    [SerializeField] ThemePuzzle London;
    [SerializeField] ThemePuzzle Tokyo;
    [SerializeField] ThemePuzzle SanFrancisco;
    [SerializeField] ThemePuzzle LosAngeles;
    [SerializeField] ThemePuzzle Toronto;
    [SerializeField] ThemePuzzle Dubai;
    [SerializeField] ThemePuzzle NewYork;
    [SerializeField] ThemePuzzle Berlin;
    [SerializeField] ThemePuzzle Barcelona;
    [SerializeField] ThemePuzzle Bangkok;
    [SerializeField] ThemePuzzle MexicoCity;
    [SerializeField] ThemePuzzle KualaLumpur;
    [SerializeField] ThemePuzzle Shanghai;
    [SerializeField] ThemePuzzle Rome;
    [SerializeField] ThemePuzzle Mumbai;
    [SerializeField] ThemePuzzle SaoPaulo;
    [SerializeField] ThemePuzzle Istanbul;
    [SerializeField] ThemePuzzle CapeTown;

    #endregion

    #region REFERENCES

    public List<string> levelWords = new List<string>(); // List to store the words
    public TextAsset levelText;

    public ThemePuzzle Preview;

    [SerializeField] Image MainImage, _1, _2, _3, _4;

    #endregion

    public void BounusLevelCompleted(int bonusLevel)
    {
        int themeIndex = ((bonusLevel - 1) / 4) % 20; // Change theme every 4 levels
        int spriteIndex = (bonusLevel - 1) % 4 + 1;    // Select sprite for current level

        ThemePuzzle selectedTheme = null;

        switch (themeIndex)
        {
            case 0: selectedTheme = Paris; Preview.MainImage = Paris.MainImage; break;
            case 1: selectedTheme = Egypt; Preview.MainImage = Egypt.MainImage; break;
            case 2: selectedTheme = London; Preview.MainImage = London.MainImage; break;
            case 3: selectedTheme = Tokyo; Preview.MainImage = Tokyo.MainImage; break;
            case 4: selectedTheme = SanFrancisco; Preview.MainImage = SanFrancisco.MainImage; break;
            case 5: selectedTheme = LosAngeles; Preview.MainImage = LosAngeles.MainImage; break;
            case 6: selectedTheme = Toronto; Preview.MainImage = Toronto.MainImage; break;
            case 7: selectedTheme = Dubai; Preview.MainImage = Dubai.MainImage; break;
            case 8: selectedTheme = NewYork; Preview.MainImage = NewYork.MainImage; break;
            case 9: selectedTheme = Berlin; Preview.MainImage = Berlin.MainImage; break;
            case 10: selectedTheme = Barcelona; Preview.MainImage = Barcelona.MainImage; break;
            case 11: selectedTheme = Bangkok; Preview.MainImage = Bangkok.MainImage; break;
            case 12: selectedTheme = MexicoCity; Preview.MainImage = MexicoCity.MainImage; break;
            case 13: selectedTheme = KualaLumpur; Preview.MainImage = KualaLumpur.MainImage; break;
            case 14: selectedTheme = Shanghai; Preview.MainImage = Shanghai.MainImage; break;
            case 15: selectedTheme = Rome; Preview.MainImage = Rome.MainImage; break;
            case 16: selectedTheme = Mumbai; Preview.MainImage = Mumbai.MainImage; break;
            case 17: selectedTheme = SaoPaulo; Preview.MainImage = SaoPaulo.MainImage; break;
            case 18: selectedTheme = Istanbul; Preview.MainImage = Istanbul.MainImage; break;
            case 19: selectedTheme = CapeTown; Preview.MainImage = CapeTown.MainImage; break;
        }

        Sprite selectedSprite = null;
        switch (spriteIndex)
        {
            case 1: selectedSprite = selectedTheme._1; break;
            case 2: selectedSprite = selectedTheme._2; break;
            case 3: selectedSprite = selectedTheme._3; break;
            case 4: selectedSprite = selectedTheme._4; break;
        }

        Preview._1 = selectedSprite;
        // Perform additional logic after bonus level completion
        MainMenu(spriteIndex);
    }

    private void MainMenu(int spriteIndex)
    {
        MainImage.sprite= Preview.MainImage;
        MainImage.gameObject.SetActive(true);
        // Logic to load main menu
        switch (spriteIndex)
        {
            case 1:
                _1.sprite = Preview._1;
                _1.gameObject.SetActive(true);
                break;

            case 2:
                _2.sprite = Preview._2;
                _2.gameObject.SetActive(true);
                break;

            case 3:
                _3.sprite = Preview._3;
                _3.gameObject.SetActive(true);
                break;

            case 4:
                _4.sprite = Preview._4;
                _4.gameObject.SetActive(true);
                break;
        }
    }

    public void LoadLevelData( string levelName)
    {
        // Load the text file as a TextAsset
        TextAsset textAsset = levelText;

        if (textAsset != null)
        {
            // Get the text content
            string fileContent = textAsset.text;

            // Split the file into lines
            string[] lines = fileContent.Split('\n');

            // Find the level and get its words
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals(levelName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < lines.Length) // Ensure there is a next line
                    {
                        // Split the next line into words, remove extra spaces, and store them in the list
                        string wordsLine = lines[i + 1].Trim();
                        levelWords = new List<string>(wordsLine.Split(',')
                                                  .Select(word => word.Trim()) // Remove extra spaces
                                                  .Where(word => !string.IsNullOrEmpty(word)) // Remove empty words
                        );

                        Debug.Log($"Loaded words for {levelName}: {string.Join(", ", levelWords)}");
                        return;
                    }
                }
            }


            Debug.LogWarning($"Level '{levelName}' not found or has no words!");
        }
       
    }

    

}
