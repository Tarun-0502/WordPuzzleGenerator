using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{

    private TextMeshProUGUI changeText;

    public void changeText_(string text)
    {
        // Get the TextMeshProUGUI component from the first child of this transform
        changeText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (changeText != null)
        {
            // Set the font asset if one is assigned
            if (Game.Instance.NewFontAsset != null)
            {
                changeText.font = Game.Instance.NewFontAsset;
            }

            // Set the new text
            changeText.text = text;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found on child.");
        }
    }

    public void ChangeColor(string hexColorCode)
    {
        // Check if the renderer and material are valid
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
