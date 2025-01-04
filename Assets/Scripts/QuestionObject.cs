using System.Linq;
using UnityEngine;

public class QuestionObject
{
    public AudioClip CorrectAnswer;
    public AudioClip[] Distractors;
    public bool PassedValidation => !Distractors.Contains(CorrectAnswer);
}