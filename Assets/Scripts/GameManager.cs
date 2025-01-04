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

    private QuestionType _activeCategory;

    [Header("Game Config")]
    public int NumberOfRounds;
    public float RoundTimeLimit;
    private float _activeRoundTimer;

    [Header("Scene Refs")]
    public Button GameSetUpButton;
    public Button StartGameButton;
    public Button ReturnHomButton;
    public Button ReadyQuestionButton;

    public GameObject MainMenu;
    public GameObject GameSetUp;
    public GameObject GamePrep;
    public GameObject MainGame;

    public QuestionUIBehaviour QuestionUI;
    
    private GameSetUpUIBehaviour _setUpBeahiourRef => GameSetUp.GetComponent<GameSetUpUIBehaviour>();
    private QuestionBuilder _questionBuilder => FindObjectOfType<QuestionBuilder>();

    private void DisplayQuestion()
    {        
        _activeQuestion = _questionBuilder.LoadQuestion(_activeCategory, 3);
        QuestionUI.InitializeQuestion(_activeQuestion);
    }

    private void SubmitAnswer(AnswerObject answer)
    {        
        _activeTeam.IsBoosted = _activeQuestion.CorrectAnswer == answer;
        _gameFlow.SetTrigger("QuestionAnswered");
    }

    private void InitializeGameFlow()
    {
        _gameFlow = new GGD_StateMachine();

        IntParameter _currentRound;
        IntParameter _currentTeam;
        FloatParameter _currentRoundTimeRemaing;
        TriggerParameter _menuTrigger; //Trigger to go back to main menu
        TriggerParameter _setUpGameTrigger; //Trigger to go to game set up        
        TriggerParameter _startGameTrigger; //Trigger to start the game
        TriggerParameter _readyNextQuestionTrigger; //Trigger to get next question
        TriggerParameter _questionAnsweredTrigger; //Tigger for question answered

        //Define Conditions
        var menuCondition = new Condition(_menuTrigger = new TriggerParameter("ToMenu"), ConditionOperations.equals_to, true);

        var setUpCondition = new Condition(_setUpGameTrigger = new TriggerParameter("ToGameSetUp"), ConditionOperations.equals_to, true);
        var startGameCondition = new Condition(_startGameTrigger = new TriggerParameter("ToGameStart"), ConditionOperations.equals_to, true);

        var timerCondition = new Condition(_currentRoundTimeRemaing = new FloatParameter("timer", RoundTimeLimit), ConditionOperations.less_than_or_equals_to, 0);
        var roundComplete = new Condition(_currentTeam = new IntParameter("TeamId", 0), ConditionOperations.equals_to, _teams.Count - 1);
        var roundLimitReachedCondition = new Condition(_currentRound = new IntParameter("round", 0), ConditionOperations.greater_than_or_equals_to, _teams.Count);        
        var readyNextQuestionCondition = new Condition(_readyNextQuestionTrigger = new TriggerParameter("NextQuestion"), ConditionOperations.equals_to, true);
        var questionAnsweredCondition = new Condition(_questionAnsweredTrigger = new TriggerParameter("QuestionAnswered"), ConditionOperations.equals_to, true);

        //Define Transitions
        _gameFlow.DefineTransition(_mainMenuState = new MainMenu(), _teamSetupState = new TeamSetup(), new[] { setUpCondition });
        _gameFlow.DefineTransition(_teamSetupState, _mainMenuState, new[] { menuCondition });

        _gameFlow.DefineTransition(_teamSetupState, _gamePrepState = new GamePrep(), new[] { startGameCondition });
        _gameFlow.DefineTransition(_gamePrepState, _activeQuestionState = new ActiveQuestion(), new[] { readyNextQuestionCondition });

        _gameFlow.DefineTransition(_activeQuestionState , _reveiewQuestionState = new ReviewQuestion(), new[] { timerCondition });
        _gameFlow.DefineTransition(_activeQuestionState, _reveiewQuestionState = new ReviewQuestion(), new[] { questionAnsweredCondition }, 1);
        _gameFlow.DefineTransition(_reveiewQuestionState, _roundEndState = new RoundEnd(), new[] { roundComplete });
        _gameFlow.DefineTransition(_reveiewQuestionState, _activeQuestionState, new[] { readyNextQuestionCondition });

        _gameFlow.DefineTransition(_roundEndState, _gameOverState = new GameOver(), new[] { roundLimitReachedCondition }, 1);
        _gameFlow.DefineTransition(_roundEndState, _activeQuestionState, new[] { readyNextQuestionCondition });        

        //Add Parameters
        _gameFlow.AddParameter(_currentRound);
        _gameFlow.AddParameter(_currentTeam);
        _gameFlow.AddParameter(_currentRoundTimeRemaing);
        _gameFlow.AddParameter(_menuTrigger);
        _gameFlow.AddParameter(_setUpGameTrigger);
        _gameFlow.AddParameter(_startGameTrigger);
        _gameFlow.AddParameter(_readyNextQuestionTrigger);
        _gameFlow.AddParameter(_questionAnsweredTrigger);
    }

    public void Awake()
    {
        _teams = new List<TeamObject>();

        InitializeGameFlow();

        _mainMenuState.OnStateEnter = () => MainMenu.SetActive(true);
        _mainMenuState.OnStateExit = () => MainMenu.SetActive(false);

        _teamSetupState.OnStateEnter = () => GameSetUp.SetActive(true);
        _teamSetupState.OnStateExit = () =>
        {
            GameSetUp.SetActive(false);
            _activeCategory = _setUpBeahiourRef.GetQuestionCategory();
        };

        _gamePrepState.OnStateEnter = () => GamePrep.SetActive(true);
        _gamePrepState.OnStateExit = () =>
        {
            GamePrep.SetActive(false);
            var sysRandom = new System.Random();
            var shuffleTeams = _teams.OrderBy(item => sysRandom.Next()).ToList();
            _teams = shuffleTeams;
            _activeTeam = _teams[0];            
        };

        _activeQuestionState.OnStateEnter = () =>
        {
            Debug.Log("Active Team " + _activeTeam.TeamName);
            _gameFlow.SetParameter("timer", RoundTimeLimit);
            _activeRoundTimer = RoundTimeLimit;
            DisplayQuestion();
            MainGame.SetActive(true);
        };
        _activeQuestionState.OnStateUpdate = () => { _gameFlow.SetParameter("timer", _activeRoundTimer -= Time.deltaTime); };

        GameSetUpButton.onClick.AddListener(() => _gameFlow.SetTrigger("ToGameSetUp"));

        ReturnHomButton.onClick.AddListener(() => _gameFlow.SetTrigger("ToMenu"));

        StartGameButton.onClick.AddListener(() => {
            _gameFlow.SetTrigger("ToGameStart");
            _teams = _setUpBeahiourRef.GetAllTeams();
        });

        ReadyQuestionButton.onClick.AddListener(() => {
            _gameFlow.SetTrigger("NextQuestion");
        });
        

        AnswerUIBehaviour.QuestionAnswered.AddListener(SubmitAnswer);
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
