using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSetUpUIBehaviour : MonoBehaviour
{
    public TMP_Dropdown CategorySelection;
    public TMP_Dropdown NumberOfTeams;
    public TMP_Dropdown NumRounds;
    public TMP_Dropdown RoundDuration;

    public GameObject ColorSelectUIRef;
    private ColorUIBehaviour ColorUIBehaviour => ColorSelectUIRef.GetComponentInChildren<ColorUIBehaviour>();

    private List<TeamCreationUIBehaviour> _activeTeams;
    public TeamCreationUIBehaviour TeamCreatorPrefab;
    public LayoutGroup TeamDisplay;

    private TeamCreationUIBehaviour _activeTeamColorChange;    


    public void Awake()
    {
        _activeTeams = new List<TeamCreationUIBehaviour>();        
        CategorySelection.options.Clear();
        for(int i = 0; i < Enum.GetValues(typeof(QuestionType)).Length; i++) 
        {
            CategorySelection.options.Add(new TMP_Dropdown.OptionData(((QuestionType)i).ToString()));
        }

        CategorySelection.value = 0;

        NumberOfTeams.options.Clear();
        for(int i = 1; i <= 5;  i++) 
        {
            NumberOfTeams.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        NumberOfTeams.onValueChanged.AddListener(UpdateTeamsView);
        NumberOfTeams.value = 0;        

        ColorUIBehaviour.OnColorSelected.AddListener(SetTeamColor);
        ColorSelectUIRef.SetActive(false);

        RoundDuration.options.Clear();
        for(int i = 10; i <= 120; i+=10)
        {
            RoundDuration.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        NumRounds.options.Clear();
        for (int i = 1; i <= 10; i++)
        {
            NumRounds.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
    }

    public void Start()
    {
        UpdateTeamsView(0);
    }

    public void UpdateTeamsView(int selectionIndex)
    {
        var numTeams = selectionIndex + 1;
        if(numTeams < _activeTeams.Count)
        {
            var lastTeam = _activeTeams[numTeams - 1];
            for (int i = _activeTeams.Count - 1; i >= numTeams; i--)
            {
                Destroy(_activeTeams[i].gameObject);
                _activeTeams.RemoveAt(i);
            }
        }
        else
        {
            for(int i = _activeTeams.Count; i < numTeams; i++) 
            {
                var newTeam = Instantiate(TeamCreatorPrefab, TeamDisplay.transform) as TeamCreationUIBehaviour;
                newTeam.SetNewColor(ColorUIBehaviour.DefaultColor);
                _activeTeams.Add(newTeam);
                newTeam.RequestChangeColorEvent.AddListener((arg) =>
                {
                    _activeTeamColorChange = arg;
                    ColorSelectUIRef.SetActive(true);
                });
            }
        }
    }

    public void SetTeamColor(Color color)
    {
        ColorSelectUIRef.SetActive(false);
        _activeTeamColorChange.SetNewColor(color);
    }

    public List<TeamObject> GetAllTeams()
    {
        var retValue = new List<TeamObject>();
        foreach(var teamCreationObject in _activeTeams)
        {
            retValue.Add(teamCreationObject.ExtractTeam());
        }
        return retValue;
    }

    public QuestionType GetQuestionCategory()
    {
        return (QuestionType)Enum.Parse(typeof(QuestionType), CategorySelection.options[CategorySelection.value].text);
    }

    public int GetNumRounds()
    {
        return int.Parse(NumRounds.options[NumRounds.value].text);
    }

    public int GetRoundDuration()
    {
        return int.Parse(RoundDuration.options[RoundDuration.value].text);
    }

    public void ClearScreen()
    {
        foreach(var item in _activeTeams)
        {
            Destroy(item.gameObject);
        }
        _activeTeams.Clear();
        NumberOfTeams.value = 0;
        CategorySelection.value = 0;
    }
}
