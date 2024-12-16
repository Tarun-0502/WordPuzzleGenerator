using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

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
            Text.transform.DOScale(new Vector3(5,5,5), 0.25f);
            DOVirtual.DelayedCall(0.3f, () => 
            {
                Text.text = "";
            });
        }
    }
}
