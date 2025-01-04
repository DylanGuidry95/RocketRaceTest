using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReviewScreenUIBehaviour : MonoBehaviour
{
    public TextMeshProUGUI FeedbackLabel;
    public Button ProceedButton;
    public RawImage TeamDisplay;

    public void UpdateFeedback(string text, TeamObject _activeTeam)
    {
        TeamDisplay.color = _activeTeam.TeamColor;
        FeedbackLabel.text = text;
    }
}
