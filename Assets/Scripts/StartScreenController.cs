using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> _tutorialScreens = new List<GameObject>();
    [SerializeField] private Animator _backgroundAnimator;
    [SerializeField] private Animator _titleAnimator;
    [SerializeField] private Animator _creditsAnimator;

    [SerializeField] private List<GameObject> _introUi = new();
    [SerializeField] private List<GameObject> _inGameUi = new();
    bool _gameStarted = false;
    [SerializeField] private float _inputDelay = .5f;
    [SerializeField] private float _gameStartDelay = 3f;
    private bool _isInputReady = true;
    private int _currentScreen = 0;

    private bool _onActionPressed = false;
    private bool _onBackPressed = false;
    private bool _leftPressed = false;
    private bool _rightPressed = false;
    private bool _firstPressDetected = false;


    public UnityEvent OnFirstPress;
    public UnityEvent OnInputPressed;
    public UnityEvent OnGameStarted;
    





    private void Start()
    {
        foreach (GameObject thing in _inGameUi)
            thing.SetActive(false);

        //make sure the first screen is always showing on start
        _tutorialScreens[_currentScreen].SetActive(true);
    }

    private void Update()
    {
        if (!_gameStarted)
        {
            ListenForInput();
            ControlScreens();
        }
    }


    private void ListenForInput()
    {
        _onActionPressed = Input.GetKeyDown(KeyCode.Space);
        _onBackPressed = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape);
        _leftPressed = Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow);
        _rightPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow);

        if (!_firstPressDetected)
        {
            if (_onActionPressed || _onBackPressed || _leftPressed || _rightPressed)
            {
                _firstPressDetected = true;
                OnFirstPress?.Invoke(); //for ambience to start
            }
        }
    }
    private void CooldownInput()
    {
        _isInputReady = false;
        Invoke(nameof(ReadyInput), _inputDelay);
    }
    private void ReadyInput()
    {
        _isInputReady = true;
    }


    private void ControlScreens()
    {
        if (_rightPressed || _onActionPressed)
        {
            
            if (_currentScreen == _tutorialScreens.Count -1)
            {
                
                CooldownInput();
                OnInputPressed?.Invoke();

                //we're at the end of the tutorials.
                //hide the last screen
                _tutorialScreens[_currentScreen].SetActive(false);

                //Start the game
                StartGame();
            }
            else
            {
                
                CooldownInput();
                OnInputPressed?.Invoke();

                //goto next screen
                _tutorialScreens[_currentScreen].SetActive(false);
                _currentScreen++;
                _tutorialScreens[_currentScreen].SetActive(true);
            }
        }
        else if (_leftPressed || _onBackPressed)
        {
            
            if (_currentScreen > 0)
            {
                //goto prev screen
                CooldownInput();
                OnInputPressed?.Invoke();

                _tutorialScreens[_currentScreen].SetActive(false);
                _currentScreen--;
                _tutorialScreens[_currentScreen].SetActive(true);
            }
        }
    }

    private void StartGame()
    {
        _backgroundAnimator.SetBool("isGameStarted", true);
        _titleAnimator.SetBool("isGameStarted",true);
        _creditsAnimator.SetBool("isGameStarted",true);

        foreach (GameObject thing in _introUi)
            thing.SetActive(false);

        //wait for the anim to play
        Invoke(nameof(TriggerStartEvent), _gameStartDelay);
    }
    private void TriggerStartEvent()
    {
        foreach (GameObject thing in _inGameUi)
            thing.SetActive(true);
        OnGameStarted?.Invoke();
    }
}
