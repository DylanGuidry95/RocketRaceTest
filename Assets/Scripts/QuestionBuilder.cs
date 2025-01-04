using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class QuestionBuilder : MonoBehaviour
{
    public AnswerBankScriptable[] AnswerBanks;
    public Dictionary<QuestionType, List<AudioClip>> DisplayedAnswers;

    public QuestionObject LoadQuestion(QuestionType questionToLoad, int numDistractors)
    {
        if (!DisplayedAnswers.ContainsKey(questionToLoad))
            DisplayedAnswers.Add(questionToLoad, new List<AudioClip>());
        var answerBank = AnswerBanks.FirstOrDefault(item => item.QuestionTypeLink == questionToLoad);
        if (answerBank == null)
        {
            throw new System.Exception($"No answer bank found for question type {questionToLoad}");
        }
        var systemRandom = new System.Random();
        var correctAnswerSelection = answerBank.AnswerBank.Where(item => !DisplayedAnswers[questionToLoad].Contains(item)).First();
        DisplayedAnswers[questionToLoad].Add(correctAnswerSelection);
        var filteredAnswers = answerBank.AnswerBank.Where(item => item != correctAnswerSelection).ToList();
        var distractors = filteredAnswers.OrderBy(ans => systemRandom.Next()).Take(numDistractors);

        var builtQuestion = new QuestionObject() { CorrectAnswer = correctAnswerSelection, Distractors = distractors.ToArray() };
        if (builtQuestion.PassedValidation)
            return builtQuestion;
        else
            throw new Exception("Question failed to build properly");
    }
}