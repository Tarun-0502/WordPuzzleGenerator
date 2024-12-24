using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtraWordCell : MonoBehaviour
{
    public string letter;
    public TextMeshProUGUI Text;
    public bool showText;

    public void FlyText(Transform flyPosition)
    {
        if (showText)
        {
            ChangeColor(Game.Instance.colorCode);
            transform.SetParent(Game.Instance.TextPreview.transform);
            transform.position = Vector2.zero;
            transform.localScale = Vector3.one;

            transform.SetParent(flyPosition);
            Text.text = letter;
            transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);
            DOVirtual.DelayedCall(0.6f, () =>
            {
                Text.text = "";
                transform.localScale = Vector3.zero;
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
