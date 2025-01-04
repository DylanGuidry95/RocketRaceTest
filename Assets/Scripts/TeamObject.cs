using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamObject
{
    public string TeamName;
    public Color TeamColor;
    public List<string> Members;

    public int CurrentScore = 0;
    public bool IsBoosted;
}
