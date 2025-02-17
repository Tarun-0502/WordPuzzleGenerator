using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Cell : MonoBehaviour
{


    #region REFERENCES

    #region Hide_In_Inspector

    [HideInInspector]
    [SerializeField] private Transform coinHead;

    [HideInInspector]
    [SerializeField] private Transform hint;

    [HideInInspector]
    [SerializeField] private Transform hintParent;

    #endregion

    public string letter;

    [SerializeField] private TextMeshProUGUI Text;

    public bool showText;

    public int RowId;
    public int ColumnId;

    [SerializeField] private ParticleSystem Effect;
    [SerializeField] private ParticleSystem hintEffect;

    //[SerializeField] private Transform coin;
    [SerializeField] private Sprite Opacity, filled;

    public bool isOccupied;
    public bool isAssigned;

    public Transform Star;

    [SerializeField] string DefaultColorCode;

    #endregion


    #region METHODS

    private void Start()
    {
        transform.GetComponent<Image>().sprite = Opacity;
    }

    public bool IsFilled()
    {
        return !string.IsNullOrEmpty(letter);
    }

    public void AssignCharacter(char character)
    {
        if (!isOccupied)
        {
            if (Game.Instance.NewFontAsset != null)
            {
                Text.font = Game.Instance.NewFontAsset;
            }

            coinHead = Game.Instance.CoinPosition;
            hint = Game.Instance.Power_up.hintPosition;
            hintParent = Game.Instance.Power_up.hintParent;
            letter = character.ToString();
            isOccupied = true;
            Text.text = "";
            showText =false;
            Text.transform.localScale = Vector3.zero;
            //coin.localScale = Vector3.zero;
            hint.localScale = Vector3.zero;
            ChangeColor(Game.Instance.colorCode);
        }
    }

    public void Reset()
    {
        isOccupied = false;
        isAssigned = false;
        Text.text = string.Empty;
        showText = true;
        Text.transform.localScale = Vector3.one;
    }

    public void FlyText()
    {
        if (!showText)
        {
            if (Game.Instance.DailyChallenges && Star.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("STAR");
                Game.Instance.CollectedBadges++;
            }
            Star.gameObject.SetActive(false);
            Text.transform.SetParent(Game.Instance.TextPreview.transform);
            Text.transform.position = Vector2.zero;
            Text.transform.localScale = Vector3.zero;

            Text.transform.SetParent(this.transform);
            Text.text=letter;
            Text.transform.DOLocalMove(Vector3.zero, 0.25f);
            Text.transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
            {
                //coin.transform.DOScale(Vector3.one, 0.25f);
                //DOVirtual.DelayedCall(0.2f,() =>
                //{
                //    coin.transform.SetParent(coinHead);
                //    Game.Instance.PlaySound(Game.Instance.CoinCollect);
                //    coin.transform.DOLocalMove(Vector3.zero, 0.25f);
                //});
            });
            Effect.Play();
            showText = true;
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
                transform.GetComponent<Image>().sprite = filled;
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

    public void Animate()
    {
        Text.transform.DOShakePosition(duration: 1f, strength: new Vector3(5f, 0f, 0f), vibrato: 5, randomness: 10, snapping: false, fadeOut: true);
    }

    public void Hint(Transform Parent, Transform position)
    {
        hint = position;
        hintParent = Parent;

       if (showText)
       {
            //PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins")-hintCoins);
            Text.text = letter;
            hint.SetParent(this.transform);

            hint.transform.DOScale(Vector3.one, 0.25f);

            hint.transform.DOLocalMove(new Vector3(1.75f,1.75f,0),0.25f).OnComplete(() =>
            {
                hintEffect.Play();
                Text.transform.DOLocalMove(Vector3.zero, 0.1f);
                Text.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
                {
                    hint.transform.localScale = Vector3.zero;
                    hint.SetParent(hintParent);
                    hint.transform.localPosition = Vector3.zero;
                });
            });
       }
    }

    public void SpotLight(bool Reveal=false)
    {
        if (!Reveal)
        {
            if (!showText)
            {
                Text.text = letter;
                Text.transform.DOScale(Vector3.one, 0.1f);
                ChangeColor(Game.Instance.colorCode);
                DOVirtual.DelayedCall(1.25f, () =>
                {
                    Text.text = "";
                    Text.transform.DOScale(Vector3.zero, 0.1f);
                    ChangeColor(DefaultColorCode);
                });
            }
        }
        else
        {
            if (!showText)
            {
                Text.text = letter;
                Text.transform.DOScale(Vector3.one, 0.1f);
                ChangeColor(Game.Instance.colorCode);
                showText = true;
            }
        }
    }

    #endregion


}
