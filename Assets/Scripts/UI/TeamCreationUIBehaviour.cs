using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TeamCreationUIBehaviour : MonoBehaviour
{
    public Button ChangeColorButton;
    public TMP_InputField NameField;
    public RawImage RocketImage;

    public UnityEvent<TeamCreationUIBehaviour> RequestChangeColorEvent;

    public void Awake()
    {
        ChangeColorButton.onClick.AddListener(() => RequestChangeColorEvent.Invoke(this));
    }

    public void SetNewColor(Color color)
    {
        RocketImage.color = color;
    }

    public TeamObject ExtractTeam()
    {
        return new TeamObject() { TeamColor = RocketImage.color, TeamName = NameField.text };
    }
}
