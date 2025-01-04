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
    private QuestionBuilder _questionBuilder;
    private QuestionObject _activeQuestion;
    

    private StateMachine.GGD_StateMachine _gameFlow;
    private MainMenu _mainMenuState;
    private TeamSetup _teamSetupState;    
    private ActiveQuestion _activeQuestionState;
    private ReviewQuestion _reveiewQuestionState;
    private RoundEnd _roundEndState;
    private GameOver _gameOverState;

    [Header("Game Config")]
    public int NumberOfRounds;
    public int RoundTimeLimit;

    [Header("Scene Refs")]
    public Button GameSetUpButton;
    public Button StartGameButton;
    public Button ReturnHomButton;

    public GameObject MainMenu;
    public GameObject GameSetUp;
    public GameObject MainGame;

    private GameSetUpUIBehaviour _setUpBeahiourRef => GameSetUp.GetComponent<GameSetUpUIBehaviour>();


    public void InitializeGame()
    {        
        _questionBuilder = FindObjectOfType<QuestionBuilder>();
        if (_questionBuilder != null)
            throw new System.Exception("No Question Loader in scene");        
        _gameFlow.SetParameter("round", 0);
    }

    public void DisplayQuestion()
    {
        var questionType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(QuestionType)).Length);
        _activeQuestion = _questionBuilder.LoadQuestion((QuestionType)questionType, 3);
    }    

    public void SubmitAnswer(AudioClip clip)
    {        
        _activeTeam.IsBoosted = _activeQuestion.CorrectAnswer == clip;
        var currentTeamIndex = _teams.IndexOf(_activeTeam);
        if (currentTeamIndex == _teams.Count - 1)
            _gameFlow.SetParameter("TeamId", 0);
        else
            _gameFlow.SetParameter("TeamId", currentTeamIndex++);        
    }

    public void Awake()
    {
        _teams = new List<TeamObject>();
        _gameFlow = new GGD_StateMachine();

        IntParameter _currentRound;
        IntParameter _currentTeam;
        FloatParameter _currentRoundTimeRemaing;
        TriggerParameter _menuTrigger;
        TriggerParameter _setUpGameTrigger;
        TriggerParameter _startGameTrigger;
        TriggerParameter _readyNextQuestionTrigger;

        var menuCondition = new Condition(_menuTrigger = new TriggerParameter("ToMenu"), ConditionOperations.equals_to, true);

        var setUpCondition = new Condition(_setUpGameTrigger = new TriggerParameter("ToGameSetUp"), ConditionOperations.equals_to, true);
        var startGameCondition = new Condition(_startGameTrigger = new TriggerParameter("ToGameStart"), ConditionOperations.equals_to, true);

        var timerCondition = new Condition(_currentRoundTimeRemaing = new FloatParameter("timer", RoundTimeLimit), ConditionOperations.less_than_or_equals_to, 0);
        var roundComplete = new Condition(_currentTeam = new IntParameter("TeamId", 0), ConditionOperations.equals_to, _teams.Count - 1);
        var roundLimitReachedCondition = new Condition(_currentRound = new IntParameter("round", 0), ConditionOperations.greater_than_or_equals_to, _teams.Count);
        var readNextQuestionCondition = new Condition(_readyNextQuestionTrigger = new TriggerParameter("NextQuestion"), ConditionOperations.equals_to, true);

        _gameFlow.DefineTransition(_mainMenuState = new MainMenu(), _teamSetupState = new TeamSetup(), new[] { setUpCondition });
        _gameFlow.DefineTransition(_teamSetupState, _mainMenuState, new[] { menuCondition });

        _gameFlow.DefineTransition(_teamSetupState, _activeQuestionState = new ActiveQuestion(), new[] { startGameCondition });

        _gameFlow.DefineTransition(_activeQuestionState, _reveiewQuestionState = new ReviewQuestion(), new[] { timerCondition });
        _gameFlow.DefineTransition(_reveiewQuestionState, _roundEndState = new RoundEnd(), new[] { roundComplete });
        _gameFlow.DefineTransition(_reveiewQuestionState, _activeQuestionState, new[] { readNextQuestionCondition });

        _gameFlow.DefineTransition(_roundEndState, _gameOverState = new GameOver(), new[] { roundLimitReachedCondition }, 1);
        _gameFlow.DefineTransition(_roundEndState, _activeQuestionState, new[] { readNextQuestionCondition });

        GameSetUpButton.onClick.AddListener(() => _setUpGameTrigger.Trigger());
        ReturnHomButton.onClick.AddListener(() => _menuTrigger.Trigger());
        StartGameButton.onClick.AddListener(() => {
            _readyNextQuestionTrigger.Trigger();
            _teams = _setUpBeahiourRef.GetAllTeams();
            GameSetUp.SetActive(false);
            MainGame.SetActive(true);
        });

        //Add Parameters
        _gameFlow.AddParameter(_currentRound);
        _gameFlow.AddParameter(_currentTeam);
        _gameFlow.AddParameter(_currentRoundTimeRemaing);
        _gameFlow.AddParameter(_menuTrigger);
        _gameFlow.AddParameter(_setUpGameTrigger);
        _gameFlow.AddParameter(_startGameTrigger);
        _gameFlow.AddParameter(_readyNextQuestionTrigger);

        _mainMenuState.OnStateEnter = () => MainMenu.SetActive(true);
        _mainMenuState.OnStateExit = () => MainMenu.SetActive(false);

        _teamSetupState.OnStateEnter = () => GameSetUp.SetActive(true);
        _teamSetupState.OnStateExit = () => GameSetUp.SetActive(false);

        _gameFlow.StartMachine(_mainMenuState);
    }

    private void Update()
    {
        _gameFlow.Update();
    }
}
