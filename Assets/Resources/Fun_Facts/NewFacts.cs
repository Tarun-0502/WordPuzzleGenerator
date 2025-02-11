using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Facts_New")]

public class NewFacts : ScriptableObject
{
    public string CityName;
    public List<string> Facts;
}
