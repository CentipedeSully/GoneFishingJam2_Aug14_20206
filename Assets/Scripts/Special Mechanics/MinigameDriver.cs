using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameDriver : MonoBehaviour
{
    [SerializeField] private RectTransform _minigameRectTransform;
    [SerializeField] private TimedClickMiniGame _minigame;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private FishDespawner _despawner;
    [SerializeField] private bool _isInBulletTime = false;
    private List<GameObject> _targetableFishList;
    [SerializeField] private GameObject _currentlyTargetedFish;
    //[SerializeField] private List<GameObject> _markedFish = new();

    [Header("UnityEvents")]
    public UnityEvent OnEnterBulletTimeAction; 
    public UnityEvent OnExitBulletTimeAction; 

    private bool _isMinigameRunning = false;

    private bool _actionInput = false;
    private bool _nextInput = false;
    private bool _prevInput = false;
    private bool _backInput = false;



    //monobehaviours
    private void Update()
    {
        ListenForInput();
        ControlBulletTime();

        if (_isInBulletTime)
        {
            if (_currentlyTargetedFish == null)
                DetectNewFish();
            
            if (_currentlyTargetedFish != null)
            {
                if (_nextInput)
                    ChangeFishTarget(1);
                if (_prevInput)
                    ChangeFishTarget(-1);

                DriveMinigameOnCurrentFish();
            }
        }
    }



    //internals
    private void DetectNewFish()
    {
        //Debug.Log("Detecting Fish...");
        if (_targetableFishList == null)
            _targetableFishList = _despawner.GetInRangeFish();

        if (_targetableFishList.Count > 0)
        {
            _currentlyTargetedFish = _targetableFishList[0];
        }
    }
    
    
    private void DriveMinigameOnCurrentFish()
    {
        //Debug.Log("Driving Minigame...");
        if (_isMinigameRunning == false)
        {
            _isMinigameRunning = true;
            _minigame.StartCycler();
        }

        _minigameRectTransform.position = _mainCam.WorldToScreenPoint(_currentlyTargetedFish.transform.position);


    }

    private void ListenForInput()
    {
        _actionInput = Input.GetKeyDown(KeyCode.Space);
        _nextInput= Input.GetKeyDown(KeyCode.E);
        _prevInput = Input.GetKeyDown(KeyCode.Q);
        _backInput = Input.GetKeyDown(KeyCode.Escape);
    }

    private void ChangeFishTarget(int direction)
    {
        if (_targetableFishList.Count > 1)
        {
            int currentFishindex = _targetableFishList.IndexOf(_currentlyTargetedFish);

            //if we're at the first index (and moving backwards), then go to the end of the list
            if ( currentFishindex == 0 && direction == -1)
                _currentlyTargetedFish = _targetableFishList[_targetableFishList.Count - 1];

            //else if were at the last index (and moving forwards), then go to the start of the list
            else if (currentFishindex == _targetableFishList.Count - 1 && direction == 1)
                _currentlyTargetedFish = _targetableFishList[0];

            else
                _currentlyTargetedFish = _targetableFishList[currentFishindex + direction];
        }
    }

    private void ControlBulletTime()
    {
        if (_actionInput && !_isInBulletTime)
        {
            OnEnterBulletTimeAction?.Invoke();
        }
        else if ( _backInput && _isInBulletTime)
        {
            OnExitBulletTimeAction?.Invoke();
        }
    }

    //externals
    public void RespondToBulletTimeEntered()
    {
        _isInBulletTime = true;
    }
    public void RespondToBulletTimeExited()
    {
        _isInBulletTime = false;

        if (_isMinigameRunning)
        {
            _isMinigameRunning = false;
            _minigame.CloseCycler();
        }
    }
    public void RespondToFishOutOfRange(GameObject fish)
    {
        if (fish == _currentlyTargetedFish)
        {
            _currentlyTargetedFish = null;

            if (_isMinigameRunning)
            {
                _isMinigameRunning = false;
                _minigame.CloseCycler();
            }
        }
    }








}
