using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip[] BgMusic;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound",0);
        }
        switch (PlayerPrefs.GetInt("Sound"))
        {
            case 0:
                AudioSource.clip = BgMusic[0];
                PlayerPrefs.SetInt("Sound", 1);
                break;

            case 1:
                AudioSource.clip = BgMusic[1];
                PlayerPrefs.SetInt("Sound", 0);
                break;

            default:
                AudioSource.clip = BgMusic[0];
                PlayerPrefs.SetInt("Sound", 1);
                break;
        }

        AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
