using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnswerBank", menuName = "Answers/New AnswerScriptable")]
public class AnswerBankScriptable : ScriptableObject
{
    public QuestionType QuestionTypeLink;
    public string QuestionPrompt;
    public List<AnswerObject> AnswerObjectsBank;    
}