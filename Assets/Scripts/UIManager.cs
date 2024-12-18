using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform levelSelection;

    // Start is called before the first frame update
    void Start()
    {
        SetLevels(levelSelection);
    }

    public void SetLevels(Transform levelsParent)
    {
        //Transform levelsParent = levelSelectionScreen.GetChild(0);

        for (int i = 0; i < levelsParent.childCount; i++)
        {
            int currentLevel1 = i + 1;
            levelsParent.GetChild(i).gameObject.name = "Level" + currentLevel1;
            levelsParent.GetChild(i).transform.GetComponent<Button>().onClick.AddListener(() => Chooselevel(currentLevel1));
            levelsParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLevel1.ToString();
        }
    }

    void Chooselevel(int current)
    {
        PlayerPrefs.SetInt("SelectedLevel",current);
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
