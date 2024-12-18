using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratePattern : MonoBehaviour
{


    #region SINGLETON REFERENCE

    public static GeneratePattern Instance;

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

    #region Hide_In_Inspector

    [HideInInspector]
    [SerializeField] List<string> gameLevels;

    [HideInInspector]
    [SerializeField] List<LineWord> TotalLineWords = new List<LineWord>();

    [HideInInspector]
    [SerializeField] Transform lineWordsParent;

    [HideInInspector]
    public Cell cell_;

    [HideInInspector]
    public int index_;

    [HideInInspector]
    public List<Cell> cells_;

    [HideInInspector]
    [SerializeField] Transform levelSelectionScreen;

    #endregion


    [SerializeField] List<string> words = new List<string>();
    [SerializeField] Transform GridLayout;
    [SerializeField] List<Row> Allrows = new List<Row>();
    [SerializeField] internal List<string> AssignedWords = new List<string>();
    [SerializeField] List<LineWord> CurrentUsedLineWords = new List<LineWord>();

    [SerializeField] GameObject lineWord_Prefab;

    [SerializeField] List<string> WordsNotAssigned = new List<string>(); 

    [SerializeField] int CurrentLevel;

    [SerializeField] Transform levelsParent;

    private const int CellsPerRow = 9; // Define a constant for cells per row
    private const int MaxColumnId = 8; // Define a constant for maximum column index

    #endregion


    #region METHODS

    void Start()
    {
        if (Game.Instance.levelSelectionScreen != null)
        {
            levelSelectionScreen = Game.Instance.levelSelectionScreen.GetComponent<Transform>();
        }
        getAllCellIntoRows();
        lineWordsParent = GridLayout.parent;
       
        //SetLevels();
        //levelSelectionScreen.gameObject.SetActive(true);
    }

    #region LevelSelectionScreen

    public void SetLevels()
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

    [SerializeField] GameObject themeSelection;

    public void Chooselevel(int selectedLevel)
    {
        themeSelection.SetActive(false);

        List<char> characters = new List<char>();

        CurrentLevel = selectedLevel;
        Levels.Instance.LoadLevelData("Levels","Level-"+selectedLevel);
        gameLevels = Levels.Instance.levelWords;

        if (gameLevels == null || gameLevels.Count == 0)
        {
            Debug.LogError($"GameLevels not found or empty for level: {selectedLevel}");
            return;
        }

        words.Clear();
        characters.Clear();
        foreach (var word in gameLevels)
        {
            words.Add(word.ToUpper());
        }
        string highestword = FindHighestWord();
        //Debug.Log(highestword);
        for (int i=0;i<highestword.Length;i++)
        {
            characters.Add(highestword[i]);
        }
        foreach (var word in words)
        {
            for (int i = 0;i < word.Length; i++)
            {
                if (!characters.Contains(word[i]))
                {
                    characters.Add(word[i]);
                }
            }
        }

        Game.Instance.CurrentLevelButton(selectedLevel, characters);
        
        InstantiateLineWords();

        switch (FindHighestWord().Length)
        {
            case 3:
                Debug.Log(FindHighestWord().Length + "3" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(-190, -70, 0);
                break;

            case 4:
                Debug.Log(FindHighestWord().Length + "4" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(0, 0, 0);
                break;

            case 5:
                Debug.Log(FindHighestWord().Length + "5" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(-50, 10, 0);
                break;

            case 6:
                Debug.Log(FindHighestWord().Length + "6" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(-25, -50, 0);
                break;

            case 7:
                Debug.Log(FindHighestWord().Length + "7" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(-90, 100, 0);
                break;

            default:
                Debug.Log(FindHighestWord().Length + "default" + FindHighestWord());
                GridLayout.transform.localPosition = new Vector3(0, 0, 0);
                break;
        }

        WordToPlace(FindHighestWord(), true); // To place the first word

        levelSelectionScreen.gameObject.SetActive(false);
    }

    #endregion

    #region Puzzle Grid Generator

    void getAllCellIntoRows()
    {
        Allrows.Clear();

        int totalChildren = GridLayout.childCount;
        int numberOfRows = Mathf.CeilToInt((float)totalChildren / CellsPerRow);

        for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
        {
            Row newRow = new Row();

            for (int j = 0; j < CellsPerRow; j++)
            {
                int cellIndex = rowIndex * CellsPerRow + j;

                if (cellIndex < totalChildren)
                {
                    Transform currentCell = GridLayout.GetChild(cellIndex);
                    Cell cellComponent = currentCell.GetComponent<Cell>();
                    cellComponent.ColumnId = j;
                    cellComponent.RowId = rowIndex;
                    cellComponent.Reset();
                    newRow.RowCells.Add(currentCell);
                    cellComponent.gameObject.SetActive(false);
                }
            }
            Allrows.Add(newRow);
        }
    }

    void InstantiateLineWords()
    {
        int count = words.Count - TotalLineWords.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject lineWord = Instantiate(lineWord_Prefab, Vector3.zero, Quaternion.identity, lineWordsParent);
            TotalLineWords.Add(lineWord.GetComponent<LineWord>());
            lineWord.SetActive(false);
        }
    }

    string FindHighestWord()
    {
        string longestWord = string.Empty;
        foreach (string word in words)
        {
            if (!AssignedWords.Contains(word) && word.Length > longestWord.Length)
            {
                longestWord = word;
            }
        }
        return longestWord;
    }

    void WordToPlace(string word, bool isFirstWord = false)
    {
        if (string.IsNullOrEmpty(word)) return;

        int row = 0;
        int middleCell = 0;

        if (isFirstWord)
        {
            AssignedWords.Clear();
            int direction = 0; // 0 for horizontal, 1 for vertical
            //direction = Random.Range(0, 2);
            row = (Allrows.Count / 2);
            //row = 0;
            middleCell = (Allrows[row].RowCells.Count / 2 + 1);

            if (direction == 0) // Horizontal
            {
                middleCell = Mathf.Max(0, middleCell - word.Length / 2);
                cell_ = Allrows[row].RowCells[middleCell].GetComponent<Cell>();
                var result = CheckColumnWise(cell_, 0,FindHighestWord());
                cells_ = result.Item1;
                AssignNextWord(word, cells_, result.Item2);
            }
            else // Vertical
            {
                row = Mathf.Max(0, row - word.Length / 2);
                cell_ = Allrows[row].RowCells[middleCell].GetComponent<Cell>();
                var result = CheckRowWise(cell_, 0, FindHighestWord());
                cells_ = result.Item1;
                AssignNextWord(word, cells_, result.Item2);
            }
        }
    }

    void AssignNextWord(string word, List<Cell> cells, int direction)
    {
        if (cells == null || cells.Count != word.Length)
        {
            Debug.LogWarning(FindHighestWord()+" "+" Not Assigned!!! ");
            //return;

            if (AssignedWords.Count < words.Count)
            {
                WordsNotAssigned.Add(FindHighestWord());
                string NxtWord = "";
                foreach (string nxtWord in words)
                {
                    if (!string.IsNullOrEmpty(nxtWord) && !AssignedWords.Contains(nxtWord) && !WordsNotAssigned.Contains(nxtWord) && nxtWord.Length > NxtWord.Length)
                    {
                        NxtWord = nxtWord;
                    }
                }
                if (!string.IsNullOrEmpty(NxtWord))
                {
                    Debug.Log("NEXT WORD TO ASSIGN " + " " + NxtWord);

                    CheckPossibility(NxtWord);
                    Assign(NxtWord, cells_, index_);
                }
            }
            else
            {
                return;
            }
        }

        else if (AssignedWords.Count < words.Count && cells != null && cells.Count == word.Length)
        {
            Assign(word, cells, direction);
        }

        else
        {
            return;
        }
    }

    void Assign(string word,List<Cell> cells, int direction)
    {
        LineWord lineWord = TotalLineWords[CurrentUsedLineWords.Count];
        lineWord.Answer = word;
        int indexCount = 0;

        if (cells != null)
        {
            foreach (var cell in cells)
            {
                if (!cell.isOccupied)
                {
                    cell.gameObject.SetActive(true);
                    //cell.transform.SetParent(ActiveCellsParent);
                    cell.AssignCharacter(word[indexCount]);
                    indexCount++;
                }
                else
                {
                    cell.isAssigned = true;
                    indexCount++;
                }
                lineWord.Cells.Add(cell.transform);
            }
        }

        if (indexCount == word.Length)
        {
            lineWord.gameObject.SetActive(true);
            lineWord.isVertical = direction == 1;
            lineWord.isHorizontal = direction == 0;
            CurrentUsedLineWords.Add(lineWord);
            Game.Instance.CompletedWords = CurrentUsedLineWords.Count;
            Game.Instance.LineWords.Add(lineWord.transform);
            AssignedWords.Add(word);
            //Debug.LogWarning("AssignedWords " + " :: " + word);

            cell_ = null;
            index_ = -1;
            cells_.Clear();
            if (WordsNotAssigned.Contains(word))
            {
                WordsNotAssigned.Remove(word);
            }

            CheckPossibility(FindHighestWord());
            AssignNextWord(FindHighestWord(), cells_, index_);

        }
    }

    (List<Cell>, int) CheckPossibility(string word)
    {
        //Helper function to check a single word for possible placement of target word
        (List<Cell>, int) CheckInSingleWord(LineWord currentWord)
        {
            //Iterate through each character in the target word
            foreach (char targetChar in word)
            {
                //Check each cell in the current word's cells
                foreach (var cellTransform in currentWord.Cells)
                {
                    Cell cell = cellTransform.GetComponent<Cell>();

                    //Check if the cell's letter matches the target character and cell is not assigned
                    if (cell.letter == targetChar.ToString() && !cell.isAssigned  )
                    {
                        
                        //Determine direction to check based on orientation of the current word
                        var result = currentWord.isVertical ?
                            CheckColumnWise(cell, word.IndexOf(targetChar),word) :
                            CheckRowWise(cell, word.IndexOf(targetChar), word);

                        cells_ = result.Item1;
                        index_ = result.Item2;

                        //Check if cells found are sufficient to place the word
                        if (cells_ != null && cells_.Count == word.Length)
                        {
                            currentWord.isConnected = true;
                            return (cells_, index_);
                        }
                    }
                }
            }
            return (null, -1);
        }

        // Step 1: Check all unconnected words in reverse order
        for (int i = 0; i <= CurrentUsedLineWords.Count - 1; i++)
        {
            LineWord currentWord = CurrentUsedLineWords[i];
            if (!currentWord.isConnected)
            {
                var result = CheckInSingleWord(currentWord);
                if (result.Item1 != null) return result;
            }
        }

        //Step 2: Check all connected words if no match is found
        for (int i = 0; i <= CurrentUsedLineWords.Count - 1; i++)
        {
            LineWord currentWord = CurrentUsedLineWords[i];
            if (currentWord.isConnected)
            {
                var result = CheckInSingleWord(currentWord);
                if (result.Item1 != null) return result;
            }
        }

        //Step 3: If no match found in any words, return null and - 1
        return (null, -1);
    }

    bool isValidInColumnWise(Cell cell1, Cell cell2)
    {
        bool isCell1Valid = false;
        bool isCell2Valid = false;

        // Validate cell1
        if (cell1.ColumnId == 0)
        {
            isCell1Valid = true; // First column, no left neighbor to check
        }
        else if (cell1.ColumnId > 0 && cell1.ColumnId < 8)
        {
            var leftCell = Allrows[cell1.RowId].RowCells[cell1.ColumnId - 1].GetComponent<Cell>();
            if (!leftCell.isOccupied || string.IsNullOrEmpty(leftCell.letter))
            {
                isCell1Valid = true;
            }
        }

        // Validate cell2
        if (cell2.ColumnId == 8)
        {
            isCell2Valid = true; // Last column, no right neighbor to check
        }
        else if (cell2.ColumnId >= 0 && cell2.ColumnId < 8)
        {
            var rightCell = Allrows[cell2.RowId].RowCells[cell2.ColumnId + 1].GetComponent<Cell>();
            if (!rightCell.isOccupied || string.IsNullOrEmpty(rightCell.letter))
            {
                isCell2Valid = true;
            }
        }

        return isCell1Valid && isCell2Valid;
    }

    bool isValidInRowWise(Cell cell1, Cell cell2)
    {
        bool isCell1Valid = false;
        bool isCell2Valid = false;

        // Validate cell1
        if (cell1.RowId == 0)
        {
            isCell1Valid = true; // No row above to check
        }
        else if (cell1.RowId > 0 && cell1.RowId < 8)
        {
            var aboveCell = Allrows[cell1.RowId - 1].RowCells[cell1.ColumnId].GetComponent<Cell>();
            if (!aboveCell.isOccupied || string.IsNullOrEmpty(aboveCell.letter))
            {
                isCell1Valid = true;
            }
        }

        // Validate cell2
        if (cell2.RowId == 8)
        {
            isCell2Valid = true; // No row below to check
        }
        else if (cell2.RowId >= 0 && cell2.RowId < 8)
        {
            var belowCell = Allrows[cell2.RowId + 1].RowCells[cell2.ColumnId].GetComponent<Cell>();
            if (!belowCell.isOccupied || string.IsNullOrEmpty(belowCell.letter))
            {
                isCell2Valid = true;
            }
        }

        return isCell1Valid && isCell2Valid;
    }

    private (List<Cell>, int) CheckRowWise(Cell identifiedCell, int index,string word)
    {
        List<Cell> cells = new List<Cell>();
        int rowId = identifiedCell.RowId;
        int columnId = identifiedCell.ColumnId;
        string wordToAssign = word;

        for (int i = index - 1; i >= 0; i--)
        {
            rowId--;
            if (rowId >= 0 && rowId < Allrows.Count)
            {
                Cell cell = Allrows[rowId].RowCells[columnId].GetComponent<Cell>();
                if ((cell.letter == wordToAssign[i].ToString() || !cell.isOccupied))
                {
                    if (!cell.isOccupied && CheckSideCells(cell, true))
                    {
                        cells.Insert(0, cell);
                    }
                    else
                    {
                        if (cell.letter == wordToAssign[i].ToString())
                        {
                           cells.Insert(0, cell);
                        }
                    }
                }
            }
        }

        rowId = identifiedCell.RowId;

        for (int j = index; j < wordToAssign.Length; j++)
        {
            if (rowId >= 0 && rowId < Allrows.Count)
            {
                Cell cell = Allrows[rowId].RowCells[columnId].GetComponent<Cell>();
                if ((cell.letter == wordToAssign[j].ToString() || !cell.isOccupied) )
                {
                    if (!cell.isOccupied && CheckSideCells(cell, true))
                    {
                        cells.Add(cell);
                    }
                    else
                    {
                        if (cell.letter == wordToAssign[j].ToString())
                        {
                            cells.Add(cell);
                        }
                    }
                }
            }
            rowId++;
        }

        if (cells.Count == wordToAssign.Length)
        {
            Cell cell1 = cells[0];
            Cell cell2 = cells[cells.Count - 1];

            if (isValidInRowWise(cell1, cell2))
            {
                //Debug.LogWarning("CALLED COLUMNWISE" + " " + wordToAssign);

                return (cells, 1);
            }
        }

        return (null, 1);
    }

    private (List<Cell>, int) CheckColumnWise(Cell identifiedCell, int index, string word)
    {
        List<Cell> cells = new List<Cell>();
        int rowId = identifiedCell.RowId;
        int columnId = identifiedCell.ColumnId;
        string wordToAssign = word;

        for (int i = index - 1; i >= 0; i--)
        {
            columnId--;
            if (columnId >= 0 && columnId < CellsPerRow)
            {
                Cell cell = Allrows[rowId].RowCells[columnId].GetComponent<Cell>();
                if ((cell.letter == wordToAssign[i].ToString() || !cell.isOccupied))
                {
                    if (!cell.isOccupied && CheckSideCells(cell, false))
                    {
                        cells.Insert(0, cell);
                    }
                    else
                    {
                        if (cell.letter == wordToAssign[i].ToString())
                        {
                            cells.Insert(0, cell);
                        }
                    }
                }
            }
        }

        columnId = identifiedCell.ColumnId;

        for (int j = index; j < wordToAssign.Length; j++)
        {
            if (columnId >= 0 && columnId < Allrows[rowId].RowCells.Count)
            {
                Cell cell = Allrows[rowId].RowCells[columnId].GetComponent<Cell>();
                if (!cell.isOccupied && CheckSideCells(cell, false))
                    {
                        cells.Add(cell);
                    }
                    else
                    {
                        if (cell.letter == wordToAssign[j].ToString())
                        {
                            cells.Add(cell);
                        }
                    }
            }
            columnId++;
        }

        if (cells.Count == wordToAssign.Length)
        {
            Cell cell1 = cells[0];
            Cell cell2 = cells[cells.Count - 1];

            if (isValidInColumnWise(cell1, cell2))
            {
                //Debug.LogWarning("CALLED COLUMNWISE" + " " + wordToAssign);

                return (cells, 0);
            }
        }

        return (null, 0);

    }

    private bool CheckSideCells(Cell cell, bool rowWise)
    {
        int rowId = cell.RowId;
        int columnId = cell.ColumnId;

        // Return false if the cell is already assigned
        if (cell.isAssigned) return false;

        // Validate grid bounds dynamically
        int maxRows = Allrows.Count;
        int maxColumns = Allrows[0].RowCells.Count;

        if (!rowWise) // Check for row-wise (vertical direction)
        {
            bool above = rowId > 0 && !Allrows[rowId - 1].RowCells[columnId].GetComponent<Cell>().isOccupied;
            bool below = rowId < maxRows - 1 && !Allrows[rowId + 1].RowCells[columnId].GetComponent<Cell>().isOccupied;

            if (rowId == 0) return below; // Only check below if it's the first row
            if (rowId == maxRows - 1) return above; // Only check above if it's the last row

            return above && below; // Check both if it's neither the first nor the last row
        }
        else // Check for column-wise (horizontal direction)
        {
            bool left = columnId > 0 && !Allrows[rowId].RowCells[columnId - 1].GetComponent<Cell>().isOccupied;
            bool right = columnId < maxColumns - 1 && !Allrows[rowId].RowCells[columnId + 1].GetComponent<Cell>().isOccupied;

            if (columnId == 0) return right; // Only check right if it's the first column
            if (columnId == maxColumns - 1) return left; // Only check left if it's the last column

            return left && right; // Check both if it's neither the first nor the last column
        }
    }

    private bool checkAdjacentCells(int rowId,int colId)
    {
        bool assign = true;

        if (rowId>0)
        {
            Cell TopCell = Allrows[rowId-1].RowCells[colId].GetComponent<Cell>();
            if (!TopCell.isAssigned)
            {
                assign = true;
            }
            else
            {
                return false;
            }
        }
        if (colId>0)
        {
            Cell leftCell = Allrows[rowId].RowCells[colId-1].GetComponent<Cell>();
            if (!leftCell.isAssigned)
            {
                assign = true;
            }
            else
            {
                return false;
            }
        }
        if (rowId<8)
        {
            Cell bottomCell = Allrows[rowId+1].RowCells[colId].GetComponent<Cell>();
            if (!bottomCell.isAssigned)
            {
                assign = true;
            }
            else
            {
                return false;
            }
        }
        if (colId < 8)
        {
            Cell rightCell = Allrows[rowId].RowCells[colId+1].GetComponent<Cell>();
            if (!rightCell.isAssigned)
            {
                assign = true;
            }
            else
            {
                return false;
            }
        }

        return assign;
    }

    public void DeActivateAllCells()
    {
        foreach (var row in Allrows)
        {
            foreach (var cellTransform in row.RowCells)
            {
                Cell cell = cellTransform.GetComponent<Cell>();
                cell.gameObject.SetActive(false);
                cell.Reset();
            }
        }
        CurrentUsedLineWords.Clear();
        words.Clear();
        foreach (LineWord word in TotalLineWords)
        {
           word.Reset();
        }
    }

    #endregion

    #endregion


}