using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class QuestionBuilder : MonoBehaviour
{
    public AnswerBankScriptable[] AnswerBanks;
    public Dictionary<QuestionType, List<AnswerObject>> DisplayedAnswers;
    public AudioSource AnswerAudioSource;

    public QuestionObject LoadQuestion(QuestionType questionToLoad, int numDistractors)
    {
        if(DisplayedAnswers == null)
            DisplayedAnswers = new Dictionary<QuestionType, List<AnswerObject>>();
        if (!DisplayedAnswers.ContainsKey(questionToLoad))
            DisplayedAnswers.Add(questionToLoad, new List<AnswerObject>());
        var answerBank = AnswerBanks.FirstOrDefault(item => item.QuestionTypeLink == questionToLoad);
        if (answerBank == null)
        {
            throw new System.Exception($"No answer bank found for question type {questionToLoad}");
        }
        var possibleCorrectAnswers = answerBank.AnswerObjectsBank.Where(item => !DisplayedAnswers[questionToLoad].Contains(item)).ToList();
        var correctAnswerSelection = possibleCorrectAnswers[UnityEngine.Random.Range(0, possibleCorrectAnswers.Count)];
        DisplayedAnswers[questionToLoad].Add(correctAnswerSelection);
        var filteredAnswers = answerBank.AnswerObjectsBank.Where(item => item != correctAnswerSelection).ToList();
        var distractors = filteredAnswers.OrderBy(ans => GameManager.GlobalRandomSeed.Next()).Take(numDistractors);

        var builtQuestion = new QuestionObject() { CorrectAnswer = correctAnswerSelection, Distractors = distractors.ToArray(), QuestionPrompt = answerBank.QuestionPrompt };
        AnswerAudioSource.clip = correctAnswerSelection.Audio;
        if (builtQuestion.PassedValidation)
            return builtQuestion;
        else
            throw new Exception("Question failed to build properly");
    }

    public IEnumerator AudioPlay(float delay, float repetitions)
    {
        for (int i = 0; i < repetitions; i++)
        {
            AnswerAudioSource.Play();
            while (AnswerAudioSource.isPlaying)
            {
                yield return null;
            }
            yield return new WaitForSeconds(delay);            
        }
    }
}