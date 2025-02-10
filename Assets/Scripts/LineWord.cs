using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineWord : MonoBehaviour
{

    #region REFERENCES

    public string Answer;
    public List<Transform> Cells;
    public bool AnswerChecked;
    public bool isVertical;
    public bool isHorizontal;
    public bool isConnected;

    [SerializeField] int Filled_Cells_Count;

    #endregion


    #region METHODS

    public void CheckAnswer(string CurrentText)
    {
        if (!AnswerChecked)
        {
            if(Answer==CurrentText) 
            {
                foreach (Transform t in Cells)
                {
                    float delay = 0.1f; // Adjust the delay between each iteration (e.g., 0.5 seconds)
                    int index = System.Array.IndexOf(Cells.ToArray(), t); // Get the index of the current iteration

                    // Use DOVirtual.DelayedCall to delay each iteration
                    DOVirtual.DelayedCall(index * delay, () =>
                    {
                        var cell = t.GetComponent<Cell>();
                        cell.FlyText();
                        cell.ChangeColor(Game.Instance.colorCode);
                    });
                }
                AnswerChecked = true;
                Game.Instance.PlaySound(Game.Instance.WordComplete);
                Game.Instance.CompletedWords--;
                if (Game.Instance.CompletedWords==0)
                {
                    DOVirtual.DelayedCall(1.75f, () =>
                    {
                        Game.Instance.LevelCompleted();
                    });
                }
                Game.Instance.CurrentWord = "";
            }
            else
            {
                Game.Instance.TextPreview.transform.DOShakePosition(duration: 1f, strength: new Vector3(5f, 0f, 0f), vibrato: 5, randomness: 10, snapping: false, fadeOut: true);
                Game.Instance.CurrentWord = "";
            }
        }
        else
        {
            if (Answer==CurrentText)
            {
                foreach (Transform t in Cells)
                {
                    t.GetComponent<Cell>().Animate();
                }
            }
            Game.Instance.CurrentWord = "";
        }
    }

    public void Hint(Transform Parent , Transform position)
    {
        if (!AnswerChecked)
        {
            foreach (Transform t in Cells)
            {
                float delay = 0.1f; // Adjust the delay between each iteration (e.g., 0.5 seconds)
                int index = System.Array.IndexOf(Cells.ToArray(), t); // Get the index of the current iteration

                // Use DOVirtual.DelayedCall to delay each iteration
                if (!t.GetComponent<Cell>().showText)
                {
                    DOVirtual.DelayedCall(index * delay, () =>
                    {
                        var cell = t.GetComponent<Cell>();
                        cell.showText = true;
                        cell.Hint(Parent,position);
                        cell.ChangeColor(Game.Instance.colorCode);
                    });
                    break;
                }
            }
        }
    }

    public void CheckAllCellsFilled()
    {
        // Ensure the filled cell count is accurate
        Filled_Cells_Count = 0;

        foreach (Transform t in Cells)
        {
            if (t.GetComponent<Cell>().showText)
            {
                Filled_Cells_Count++;
            }
        }

        // Mark the answer as checked if all cells are filled
        if (Filled_Cells_Count == Cells.Count)
        {
            AnswerChecked = true;
            Game.Instance.CompletedWords--;

            // Trigger level completion if all words are completed
            if (Game.Instance.CompletedWords == 0)
            {
                DOVirtual.DelayedCall(0.75f, () =>
                {
                    Game.Instance.LevelCompleted();
                });
            }
        }
    }

    public void Reset()
    {
        // Clear the list of cells associated with this line word
        Cells.Clear();


        // Reset other fields if necessary
        Answer = string.Empty;
        isVertical = false;
        isHorizontal = false;

        // Deactivate the GameObject, as it is no longer used
        gameObject.SetActive(false);
    }

    #endregion

}
