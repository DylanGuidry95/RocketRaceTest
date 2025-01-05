using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamGameOverUIBehaviour : MonoBehaviour
{
    public RawImage RocketImage;
    public TextMeshProUGUI TeamName;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Placing;

    public void BuildDisplay(int place, TeamObject data)
    {
        RocketImage.color = data.TeamColor;
        TeamName.text = data.TeamName;
        Score.text = data.CurrentScore.ToString();
        Placing.text = place.ToString();
    }
}
