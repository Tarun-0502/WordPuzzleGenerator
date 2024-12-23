using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public string letter;
    public TextMeshProUGUI Text;
    public bool showText;

    public void FlyText(Transform flyPosition)
    {
        if (showText)
        {
            Text.transform.SetParent(Game.Instance.TextPreview.transform);
            Text.transform.position = Vector2.zero;
            Text.transform.localScale = Vector3.zero;

            Text.transform.SetParent(flyPosition);
            Text.text = letter;
            Text.transform.DOLocalMove(Vector3.zero, 0.25f);
            //Text.transform.DOScale(new Vector3(5, 5, 5), 0.25f);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                Text.text = "";
            });
        }
    }

    public void ChangeColor(string hexColorCode)
    {
        if (transform.GetComponent<Image>() != null)
        {
            // Convert hex color to Unity's Color
            if (ColorUtility.TryParseHtmlString(hexColorCode, out Color newColor))
            {
                // Apply the new color to the material
                transform.GetComponent<Image>().color = newColor;
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

}
