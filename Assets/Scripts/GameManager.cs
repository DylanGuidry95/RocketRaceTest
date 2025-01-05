using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using StateMachine.Parameters;
using StateMachine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{        
    private List<TeamObject> _teams;
    private TeamObject _activeTeam;    
    private QuestionObject _activeQuestion;
    
    private StateMachine.GGD_StateMachine _gameFlow;
    private MainMenu _mainMenuState;
    private TeamSetup _teamSetupState;
    private GamePrep _gamePrepState;
    private ActiveQuestion _activeQuestionState;
    private ReviewQuestion _reveiewQuestionState;
    private RoundEnd _roundEndState;
    private GameOver _gameOverState;
    private OutOfTimeState _outOfTimeState;

    private QuestionType _activeCategory;

    [Header("Game Config")]
    public int NumberOfRounds;
    private int _currentRound;
    public float RoundTimeLimit;
    public int PointsForCorrectAnswer;
    public int PointsForIncorrectAnswer;
    private float _activeRoundTimer;

    [Header("Scene Refs")]
    public Button GameSetUpButton;
    public Button StartGameButton;
    public Button ReturnHomButton;
    public Button ReadyQuestionButton;
    public Button ExitGameButton;
    public Button ResetGameButton;

    public GameObject MainMenu;
    public GameObject GameSetUp;
    public GameObject GamePrep;
    public GameObject MainGame;
    public GameObject ReviewScreen;
    public GameObject GameOverScreen;

    public QuestionUIBehaviour QuestionUI;

    public AudioSource BackgroundAudio;

    private Coroutine _audioRoutine;

    public static System.Random GlobalRandomSeed;

    private GameSetUpUIBehaviour _setUpBeahiourRef => GameSetUp.GetComponent<GameSetUpUIBehaviour>();
    private QuestionBuilder _questionBuilder => FindObjectOfType<QuestionBuilder>();
    private RoundOverUIBehaviour _roundBreak => FindObjectOfType<RoundOverUIBehaviour>();
    private ReviewScreenUIBehaviour _reviewScreen => ReviewScreen.GetComponent<ReviewScreenUIBehaviour>();
    private GameOverUIBehaviour _gameOverScreen => GameOverScreen.GetComponent<GameOverUIBehaviour>();
    private string Feedback => (_activeTeam.IsBoosted) ? "CORRECT" : "INCORRECT";

    private void DisplayQuestion()
    {        
        _activeQuestion = _questionBuilder.LoadQuestion(_activeCategory, 3);
        QuestionUI.InitializeQuestion(_activeQuestion, _activeTeam);
    }

    private void SubmitAnswer(AnswerObject answer)
    {        
        _activeTeam.CurrentScore += _activeQuestion.CorrectAnswer == answer ? PointsForCorrectAnswer : PointsForIncorrectAnswer;
        _activeTeam.IsBoosted = _activeQuestion.CorrectAnswer == answer;
        _gameFlow.SetTrigger("QuestionAnswered");
    }

    private void GetNextTeam()
    {
        if (_activeTeam == _teams.Last())
        {
            _activeTeam = _teams.First();
            _gameFlow.SetTrigger("IsRoundComplete");
            _currentRound++;            
        }
        else
        {
            _activeTeam = _teams[_teams.IndexOf(_activeTeam) + 1];
            _gameFlow.SetTrigger("NextQuestion");
        }
    }

    private void InitializeGameFlow()
    {
        _gameFlow = new GGD_StateMachine();
        
        FloatParameter _currentRoundTimeRemaing;
        TriggerParameter _gameComplete;
        TriggerParameter _roundComplete;
        TriggerParameter _menuTrigger; //Trigger to go back to main menu
        TriggerParameter _setUpGameTrigger; //Trigger to go to game set up        
        TriggerParameter _startGameTrigger; //Trigger to start the game
        TriggerParameter _readyNextQuestionTrigger; //Trigger to get next question
        TriggerParameter _questionAnsweredTrigger; //Tigger for question answered
        TriggerParameter _reviewCompleteTrigger;

        //Define Conditions
        var menuCondition = new Condition(_menuTrigger = new TriggerParameter("ToMenu"), ConditionOperations.equals_to, true);

        var setUpCondition = new Condition(_setUpGameTrigger = new TriggerParameter("ToGameSetUp"), ConditionOperations.equals_to, true);
        var startGameCondition = new Condition(_startGameTrigger = new TriggerParameter("ToGameStart"), ConditionOperations.equals_to, true);

        var timerCondition = new Condition(_currentRoundTimeRemaing = new FloatParameter("timer", RoundTimeLimit), ConditionOperations.less_than_or_equals_to, 0);
        var roundComplete = new Condition(_roundComplete = new TriggerParameter("IsRoundComplete"), ConditionOperations.equals_to, true);
        var roundLimitReachedCondition = new Condition(_gameComplete = new TriggerParameter("GameOver"), ConditionOperations.equals_to, true);        
        var readyNextQuestionCondition = new Condition(_readyNextQuestionTrigger = new TriggerParameter("NextQuestion"), ConditionOperations.equals_to, true);
        var questionAnsweredCondition = new Condition(_questionAnsweredTrigger = new TriggerParameter("QuestionAnswered"), ConditionOperations.equals_to, true);
        var reviewCompleteCondition = new Condition(_reviewCompleteTrigger = new TriggerParameter("ReviewComplete"), ConditionOperations.equals_to, true);

        //Define Transitions
        _gameFlow.DefineTransition(_mainMenuState = new MainMenu(), _teamSetupState = new TeamSetup(), new[] { setUpCondition });
        _gameFlow.DefineTransition(_teamSetupState, _mainMenuState, new[] { menuCondition });

        _gameFlow.DefineTransition(_teamSetupState, _gamePrepState = new GamePrep(), new[] { startGameCondition });
        _gameFlow.DefineTransition(_gamePrepState, _activeQuestionState = new ActiveQuestion(), new[] { readyNextQuestionCondition });

        _gameFlow.DefineTransition(_activeQuestionState , _outOfTimeState = new OutOfTimeState(), new[] { timerCondition });
        _gameFlow.DefineTransition(_activeQuestionState, _reveiewQuestionState = new ReviewQuestion(), new[] { questionAnsweredCondition });

        _gameFlow.DefineTransition(_outOfTimeState, _roundEndState = new RoundEnd(), new[] { roundComplete, reviewCompleteCondition }, 1);
        _gameFlow.DefineTransition(_outOfTimeState, _activeQuestionState, new[] { readyNextQuestionCondition, reviewCompleteCondition });

        _gameFlow.DefineTransition(_reveiewQuestionState, _roundEndState = new RoundEnd(), new[] { roundComplete, reviewCompleteCondition }, 1);
        _gameFlow.DefineTransition(_reveiewQuestionState, _activeQuestionState, new[] { readyNextQuestionCondition, reviewCompleteCondition });

        _gameFlow.DefineTransition(_roundEndState, _gameOverState = new GameOver(), new[] { roundLimitReachedCondition }, 1);
        _gameFlow.DefineTransition(_roundEndState, _activeQuestionState, new[] { readyNextQuestionCondition });

        _gameFlow.DefineTransition(_gameOverState, _mainMenuState, new[] { menuCondition });

        //Add Parameters
        _gameFlow.AddParameter(_gameComplete);
        _gameFlow.AddParameter(_roundComplete);
        _gameFlow.AddParameter(_currentRoundTimeRemaing);
        _gameFlow.AddParameter(_menuTrigger);
        _gameFlow.AddParameter(_setUpGameTrigger);
        _gameFlow.AddParameter(_startGameTrigger);
        _gameFlow.AddParameter(_readyNextQuestionTrigger);
        _gameFlow.AddParameter(_questionAnsweredTrigger);
        _gameFlow.AddParameter(_reviewCompleteTrigger);
    }

    public void Awake()
    {
        GlobalRandomSeed = new System.Random(UnityEngine.Random.Range(0,999999999));        
        _teams = new List<TeamObject>();

        InitializeGameFlow();

        _mainMenuState.OnStateEnter = () =>
        {
            MainMenu.SetActive(true);
            _teams = new List<TeamObject>();            
        };
        _mainMenuState.OnStateExit = () => MainMenu.SetActive(false);

        _teamSetupState.OnStateEnter = () => GameSetUp.SetActive(true);
        _teamSetupState.OnStateExit = () =>
        {
            GameSetUp.SetActive(false);
            _activeCategory = _setUpBeahiourRef.GetQuestionCategory();
            _setUpBeahiourRef.ClearScreen();
        };

        _gamePrepState.OnStateEnter = () =>
        {
            GamePrep.SetActive(true);
            _roundBreak.BuildDisplay(_teams.ToArray(), PointsForCorrectAnswer * NumberOfRounds);
        };

        _gamePrepState.OnStateExit = () =>
        {
            GamePrep.SetActive(false);            
            var shuffleTeams = _teams.OrderBy(item => GlobalRandomSeed.Next()).ToList();
            _teams = shuffleTeams;
            _activeTeam = _teams[0];            
        };

        _activeQuestionState.OnStateEnter = () =>
        {
            BackgroundAudio.Pause();
            Debug.Log("Active Team " + _activeTeam.TeamName);
            _gameFlow.SetParameter("timer", RoundTimeLimit);
            _activeRoundTimer = RoundTimeLimit;
            DisplayQuestion();
            MainGame.SetActive(true);
            _audioRoutine = StartCoroutine(_questionBuilder.AudioPlay(2, RoundTimeLimit / _activeQuestion.CorrectAnswer.Audio.length));
        };
        _activeQuestionState.OnStateUpdate = () => { _gameFlow.SetParameter("timer", _activeRoundTimer -= Time.deltaTime); };
        _activeQuestionState.OnStateExit = () =>
        {
            BackgroundAudio.Play();
            MainGame.SetActive(false);
            QuestionUI.ClearScreen();
            StopCoroutine(_audioRoutine);
            _audioRoutine = null;
            _questionBuilder.AnswerAudioSource.Stop();
        };

        _reveiewQuestionState.OnStateEnter = () =>
        {
            ReviewScreen.gameObject.SetActive(true);
            _reviewScreen.UpdateFeedback(Feedback, _activeTeam);
        };
        _reveiewQuestionState.OnStateExit = () =>
        {
            ReviewScreen.gameObject.SetActive(false);            
        };

        _outOfTimeState.OnStateEnter = () =>
        {
            ReviewScreen.gameObject.SetActive(true);
            _reviewScreen.UpdateFeedback("OUT OF TIME", _activeTeam);
        };
        _outOfTimeState.OnStateExit = () =>
        {
            ReviewScreen.gameObject.SetActive(false);
            
        };

        _roundEndState.OnStateEnter = () =>
        {
            GamePrep.SetActive(true);
            _roundBreak.UpdateDisplays();
        };
        _roundEndState.OnStateExit = () =>
        {
            GamePrep.SetActive(false);            
        };

        _gameOverState.OnStateEnter = () =>
        {
            GameOverScreen.SetActive(true);
            _gameOverScreen.DisplayStandings(_teams.ToArray(), PointsForCorrectAnswer * NumberOfRounds);
        };
        _gameOverState.OnStateExit= () => GameOverScreen.SetActive(false);

        GameSetUpButton.onClick.AddListener(() => _gameFlow.SetTrigger("ToGameSetUp"));

        ReturnHomButton.onClick.AddListener(() => _gameFlow.SetTrigger("ToMenu"));

        StartGameButton.onClick.AddListener(() => {
            _gameFlow.SetTrigger("ToGameStart");
            _teams = _setUpBeahiourRef.GetAllTeams();
        });

        ReadyQuestionButton.onClick.AddListener(() => {
            if (_currentRound >= NumberOfRounds)
            {
                _gameFlow.SetTrigger("GameOver");
                return;
            }
            _gameFlow.SetTrigger("NextQuestion");
        });        
        

        AnswerUIBehaviour.QuestionAnswered.AddListener(SubmitAnswer);
        _reviewScreen.ProceedButton.onClick.AddListener(() => 
        {
            _gameFlow.SetTrigger("ReviewComplete");
            GetNextTeam();
        });

        ExitGameButton.onClick.AddListener(() => Application.Quit());
        ExitGameButton.gameObject.SetActive(Application.platform == RuntimePlatform.WindowsPlayer);


        ResetGameButton.onClick.AddListener(() => _gameFlow.SetTrigger("ToMenu"));
    }

    private void Start()
    {
        _gameFlow.StartMachine(_mainMenuState);
    }

    private void Update()
    {
        _gameFlow.Update();        
    }
}
