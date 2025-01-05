using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIBehaviour : MonoBehaviour
{
    public TeamProgressUIBehaviour TeamGameOverDisplayPrefab;
    public LayoutGroup StandingsLayout;
    public TextMeshProUGUI WinnerText;

    private List<TeamProgressUIBehaviour> _instances;

    public void DisplayStandings(TeamObject[] teams, int maxScore)
    {
        _instances = new List<TeamProgressUIBehaviour>();
        var sortedList = teams.ToList();
        sortedList.Sort((lhs, rhs) => lhs.CurrentScore.CompareTo(rhs.CurrentScore));
        foreach (var team in sortedList)
        {
            var newTeamDisplay = Instantiate(TeamGameOverDisplayPrefab, StandingsLayout.transform);
            var currentStanding = sortedList.IndexOf(team) + 1;
            newTeamDisplay.InitializeDisplay(team, maxScore);
            newTeamDisplay.ProgressIndicator.value = team.CurrentScore;
            _instances.Add(newTeamDisplay);
        }
        WinnerText.text = $"Congrats {sortedList.First().TeamName} you won!";
    }

    public void ClearScreen()
    {
        foreach(var team in _instances)
        {
            Destroy(team.gameObject);
        }
        _instances = new List<TeamProgressUIBehaviour>();
    }
}
