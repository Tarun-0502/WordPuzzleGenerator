using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] List<float> RotatePowers;
    public float RotatePower; // Power applied to rotate the wheel
    public float StopPower; // Deceleration rate

    private Rigidbody2D rbody;
    private int inRotate;
    private float t;

    [SerializeField] GameObject Rewards_Screen;
    [SerializeField] Image Rewards;
    [SerializeField] Sprite Coins_50, Coins_100, Coins_150, Gems, Power;
    [SerializeField] TextMeshProUGUI Count;

    [SerializeField] Transform coin_pos, coin_rot;
    [SerializeField] Transform gem_pos, gem_rot;

    [SerializeField] Transform Coins_Parent, Gems_Parent;

    [SerializeField] int SpinCount;

    [SerializeField] TextMeshProUGUI SpinText;

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        LastSpin();
        SpinCount=PlayerPrefs.GetInt("SpinCount");
    }

    private void Update()
    {
        // Apply gradual stopping force
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        // Detect when the wheel stops and trigger the reward
        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += Time.deltaTime;
            if (t >= 0.5f) // Delay before calculating the reward
            {
                GetReward();
                inRotate = 0;
                t = 0;
            }
        }

        SpinText.text = SpinCount.ToString();

    }

    // Method to start spinning the wheel
    public void Spin()
    {
        if (inRotate == 0 && SpinCount > 0)
        {
            PlayerPrefs.SetInt("lastSpin", DateTime.Now.DayOfYear);
            PlayerPrefs.Save();

            // Pick a random rotation power from the predefined list
            RotatePower = RotatePowers[UnityEngine.Random.Range(0, RotatePowers.Count)];

            // Apply the selected power
            rbody.AddTorque(RotatePower);

            inRotate = 1;
            SpinCount--;
            PlayerPrefs.SetInt("SpinCount", SpinCount);
            PlayerPrefs.Save();
        }
    }

    void GetReward()
    {
        float rot = transform.eulerAngles.z;
        float targetAngle = 0;
        int score = 1;

        if (rot > 0 + 22 && rot <= 45 + 22) { targetAngle = 45; score = 2; }
        else if (rot > 45 + 22 && rot <= 90 + 22) { targetAngle = 90; score = 3; }
        else if (rot > 90 + 22 && rot <= 135 + 22) { targetAngle = 135; score = 4; }
        else if (rot > 135 + 22 && rot <= 180 + 22) { targetAngle = 180; score = 5; }
        else if (rot > 180 + 22 && rot <= 225 + 22) { targetAngle = 225; score = 6; }
        else if (rot > 225 + 22 && rot <= 270 + 22) { targetAngle = 270; score = 7; }
        else if (rot > 270 + 22 && rot <= 315 + 22) { targetAngle = 315; score = 8; }
        else if (rot > 315 + 22 && rot <= 360 + 22) { targetAngle = 0; score = 1; }

        // Smoothly rotate the wheel to align with the winning segment
        StartCoroutine(SmoothRotate(targetAngle, 0.5f)); // Rotates over 0.5 seconds
        Win(score);
    }

    // Coroutine to smoothly rotate the wheel to the exact reward angle
    IEnumerator SmoothRotate(float targetAngle, float duration)
    {
        float startAngle = transform.eulerAngles.z;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAngle = Mathf.LerpAngle(startAngle, targetAngle, elapsed / duration);
            transform.eulerAngles = new Vector3(0, 0, newAngle);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle); // Ensure exact target rotation
    }

    // Display the winning result
    void Win(int Score)
    {
        print("DAY " + Score);

        switch (Score)
        {
            case 1:
                ShowRewardsScreen("50", Coins_50, true);
                break;

            case 2:
                GetSpin("+1 Spin", Power);
                break;

            case 3:
                ShowRewardsScreen("100", Coins_100, true);
                break;

            case 4:
                ShowRewardsScreen("10", Gems);
                break;

            case 5:
                ShowRewardsScreen("150", Coins_150, true);
                break;

            case 6:
                GetSpin("+1 Spin", Power);
                break;

            case 7:
                ShowRewardsScreen("5", Gems);
                break;
            case 8:
                ShowRewardsScreen("200", Coins_150, true);
                break;
        }
    }

    void GetSpin(string text, Sprite _2D)
    {
        SpinCount++;
        PlayerPrefs.SetInt("SpinCount", SpinCount);
        Rewards_Screen.SetActive(true);
        Count.text = text;
        Rewards.sprite = _2D;
        Rewards.SetNativeSize();
        Rewards.transform.DOScale(Vector3.one, 0.35f);
        DOVirtual.DelayedCall(0.35f, () =>
        {
            Rewards.transform.DOScale(Vector3.one, 0.35f).OnComplete(() =>
            {
                Rewards_Screen.SetActive(false);
            });
        });
    }

    void ShowRewardsScreen(string text, Sprite _2D, bool Coins = false)
    {
        Rewards_Screen.SetActive(true);
        Count.text = text;
        Rewards.sprite = _2D;
        Rewards.SetNativeSize();
        Rewards.transform.DOScale(Vector3.one, 0.25f);
        if (Coins)
        {
            Move_Coins(Coins_Parent);
            int num = int.Parse(text);
            //Debug.LogError(int.Parse(text));
            UIManager.Instance.AddCoins(num);
        }
        else
        {
            Move_Gems(Gems_Parent);
            int num = int.Parse(text);
            //Debug.LogError(int.Parse(text));
            UIManager.Instance.AddGems(num);
        }
    }

    void Move_Coins(Transform parent)
    {
        float delayIncrement = 0.1f; // Adjust for spacing
        float duration = 0.35f;

        int count = parent.childCount;
        Sequence hintSequence = DOTween.Sequence(); // Create a DOTween sequence

        for (int i = 0; i < count; i++)
        {
            Transform coin = parent.GetChild(0);
            coin.SetParent(coin_rot);
            coin.localScale = Vector3.zero;

            hintSequence.AppendCallback(() =>
            {
                // Scale animation
                coin.DOScale(Vector3.one, duration);
            });

            hintSequence.AppendInterval(delayIncrement);

            hintSequence.AppendCallback(() =>
            {
                // Move animation
                coin.DOMove(coin_pos.position, duration).OnComplete(() =>
                {
                    coin.localScale = Vector3.zero;
                    coin.SetParent(parent);
                    coin.SetAsFirstSibling();
                    coin.localPosition = Vector3.zero;
                });
            });

            hintSequence.AppendInterval(delayIncrement);
        }

        // After all coins are moved, hide rewards screen
        hintSequence.AppendCallback(() =>
        {
            Rewards.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                Rewards_Screen.gameObject.SetActive(false);
            });
        });

        hintSequence.Play(); // Start the sequence
    }

    void Move_Gems(Transform parent)
    {
        float delayIncrement = 0.1f; // Adjust for spacing
        float duration = 0.35f;

        int count = parent.childCount;
        Sequence hintSequence = DOTween.Sequence(); // Create a DOTween sequence

        for (int i = 0; i < count; i++)
        {
            Transform gem = parent.GetChild(0);
            gem.SetParent(gem_rot);
            gem.localScale = Vector3.zero;

            hintSequence.AppendCallback(() =>
            {
                // Scale animation
                gem.DOScale(Vector3.one, duration);
            });

            hintSequence.AppendInterval(delayIncrement);

            hintSequence.AppendCallback(() =>
            {
                // Move animation
                gem.DOLocalMove(gem_pos.localPosition, duration).OnComplete(() =>
                {
                    gem.localScale = Vector3.zero;
                    gem.SetParent(parent);
                    gem.SetAsFirstSibling();
                    gem.localPosition = Vector3.zero;
                });
            });

            hintSequence.AppendInterval(delayIncrement);
        }

        // After all gems are moved, hide rewards screen
        hintSequence.AppendCallback(() =>
        {
            Rewards.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                Rewards_Screen.gameObject.SetActive(false);
            });
        });

        hintSequence.Play(); // Start the sequence
    }

    void LastSpin()
    {
        if (PlayerPrefs.GetInt("lastSpin") != DateTime.Now.DayOfYear)
        {
            SpinCount++;
            PlayerPrefs.SetInt("SpinCount", SpinCount);
        }
    }

}