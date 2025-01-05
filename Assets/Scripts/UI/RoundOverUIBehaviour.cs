using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundOverUIBehaviour : MonoBehaviour
{
    public TeamProgressUIBehaviour _progressPrefabRef;
    public LayoutGroup ProgressGroup;

    private List<TeamProgressUIBehaviour> _progressDisplays;
    public Button ContinueButton;    

    public void BuildDisplay(TeamObject[] teams, int maxPoints)
    { 
        _progressDisplays = new List<TeamProgressUIBehaviour>();
        foreach (var team in teams) 
        {
            var newDisplay = Instantiate(_progressPrefabRef, ProgressGroup.transform);
            newDisplay.InitializeDisplay(team, maxPoints);
            _progressDisplays.Add(newDisplay);
        }
    }

    public void UpdateDisplays()
    {
        foreach(var team in _progressDisplays) 
        {
            team.UpdateDisplay();
        }
    }

    private void Update()
    {
        ContinueButton.interactable = true;
        foreach (var team in _progressDisplays) 
        {
            if(!team.DoneUpdate)
                ContinueButton.interactable = false;
        }                
    }
}
