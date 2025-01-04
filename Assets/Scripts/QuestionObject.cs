using System.Linq;
using UnityEngine;

public class QuestionObject
{
    public string QuestionPrompt;
    public AnswerObject CorrectAnswer;
    public AnswerObject[] Distractors;
    public bool PassedValidation => !Distractors.Contains(CorrectAnswer);
}