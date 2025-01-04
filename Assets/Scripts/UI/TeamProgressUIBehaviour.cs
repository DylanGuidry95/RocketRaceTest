using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamProgressUIBehaviour : MonoBehaviour
{
    public Slider ProgressIndicator;
    public TextMeshProUGUI TeamLabel;
    private TeamObject _linkedTeam;

    public void InitializeDisplay(TeamObject data, int maxProgress)
    {
        _linkedTeam = data;
        TeamLabel.text = data.TeamName;
        ProgressIndicator.maxValue = maxProgress;
        ProgressIndicator.minValue = 0;
    }

    public void UpdateDisplay()
    {
        ProgressIndicator.value = _linkedTeam.CurrentScore;
    }
}
