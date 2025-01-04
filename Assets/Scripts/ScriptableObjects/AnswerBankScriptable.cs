using UnityEngine;

[CreateAssetMenu(fileName = "AnswerBank", menuName = "Answers/New AnswerScriptable")]
public class AnswerBankScriptable : ScriptableObject
{
    public QuestionType QuestionTypeLink;
    public AudioClip[] AnswerBank;
}