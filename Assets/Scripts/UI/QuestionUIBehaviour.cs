using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.Linq;

public class QuestionUIBehaviour : MonoBehaviour
{
    public TextMeshProUGUI QuestionHeader;
    public AnswerUIBehaviour AnswerPrefabRef;
    public LayoutGroup AnswerLayoutGroup;

    private List<AnswerUIBehaviour> _activeAnswers;

    public void InitializeQuestion(QuestionObject question, TeamObject data)
    {
        QuestionHeader.richText = true;
        QuestionHeader.text = $"<color=#{ColorUtility.ToHtmlStringRGB(data.TeamColor)}>{data.TeamName}</color> {question.QuestionPrompt}";
        var answers = new List<AnswerObject>() { question.CorrectAnswer };
        answers.AddRange(question.Distractors);
        
        var shuffledAnswers = answers.OrderBy(item => GameManager.GlobalRandomSeed.Next()).ToList();

        if (_activeAnswers == null)
            _activeAnswers = new List<AnswerUIBehaviour>();

        foreach(var ans in shuffledAnswers) 
        {
            var newAnsObject = Instantiate(AnswerPrefabRef, AnswerLayoutGroup.transform);
            newAnsObject.InitializeAnswer(ans);
            _activeAnswers.Add(newAnsObject);
        }
    }

    public void ClearScreen()
    {
        foreach(var ans in _activeAnswers)
        {
            Destroy(ans.gameObject);
        }
        _activeAnswers.Clear();
    }
}
