using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TeamProgressUIBehaviour : MonoBehaviour
{
    public Slider ProgressIndicator;
    public TextMeshProUGUI TeamLabel;
    private TeamObject _linkedTeam;

    public bool DoneUpdate;    

    public void InitializeDisplay(TeamObject data, int maxProgress)
    {
        _linkedTeam = data;
        TeamLabel.text = data.TeamName;
        ProgressIndicator.maxValue = maxProgress;
        ProgressIndicator.minValue = 0;
        DoneUpdate = true;
    }

    public void UpdateDisplay()
    {
        DoneUpdate = false;        
    }

    private void Update()
    {
        if (!DoneUpdate)
        {
            StartCoroutine(UpdateProgressOverTime(4f)); // Call the coroutine with a duration
            DoneUpdate = true;
        }
    }

    private IEnumerator UpdateProgressOverTime(float duration)
    {        
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            ProgressIndicator.value = Mathf.Lerp(ProgressIndicator.value, _linkedTeam.CurrentScore, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }

        // Ensure the value exactly matches the target at the end
        ProgressIndicator.value = _linkedTeam.CurrentScore;
    }
}
