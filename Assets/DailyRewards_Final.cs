using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DailyRewards_Final : MonoBehaviour
{
    private int dayValue;

    // Start is called before the first frame update
    void Start()
    {
        dayValue = PlayerPrefs.GetInt("DayValue");
        CheckDailyRewards();
        SetDailyRewardButtonInteraction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    System.DateTime NextRewardTime, FirstRewardTime;
    public void CheckDailyRewards()
    {
        //PlayerPrefs.GetString("Day", DateTime.Now.ToString());
        //PlayerPrefs.SetString("Day", DateTime.Now.ToString());
        //Show Daily Rewards Screen
       

        if (DateLoader.Date == null && dayValue < 7)
        {
            //First Time run
            if (PlayerPrefs.GetString("Day") == "")
            {
                //dailyRewardsScreen.SetActive(true);
                Debug.Log("You got a reward today");

            }
            else
            {
                string s = PlayerPrefs.GetString("Day");
                NextRewardTime = Convert.ToDateTime(s);

                if (NextRewardTime.Subtract(DateTime.Now).Hours <= 0)
                {
                    //dailyRewardsScreen.SetActive(true);
                    Debug.Log("You got a reward today");
                }
                else
                {


                    Debug.LogError("You already claimed reward");
                }
            }

            DateLoader.Date = PlayerPrefs.GetString("Day");
        }
        else
        {
            
            print("Condition checking");
        }


        //Show Spin wheel if wheels > 0
    }
    public List<Button> rewardButtons = new List<Button>();
    public void RewardClaimButton()
    {
        FirstRewardTime = DateTime.Now;
        NextRewardTime = FirstRewardTime.AddDays(1);
        PlayerPrefs.SetString("Day", NextRewardTime.ToString());
        dayValue++;
        PlayerPrefs.SetInt("DayValue", dayValue);

        if(dayValue >= 7)
        {
            dayValue = 0;
            PlayerPrefs.SetInt("DayValue", dayValue);
        }
        
    }
    void SetDailyRewardButtonInteraction()
    {
        foreach (Button b in rewardButtons)
        {
            b.interactable = false;
        }
        rewardButtons[dayValue].interactable = true;
    }
}

public static class DateLoader
{
    public static string Date = null;
    public static int volume = 1;
}

