using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameDriver : MonoBehaviour
{
    [SerializeField] private float _inputDelay = .1f;
    private bool _isCoolingInput = false;
    [SerializeField] private Transform _uiContainer;
    [SerializeField] private GameObject _targetLostFeedbackPrefab;
    [SerializeField] private RectTransform _minigameRectTransform;
    [SerializeField] private TimedClickMiniGame _minigame;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private FishDespawner _despawner;
    [SerializeField] private KunaiThrower _kunaiThrower;
    [SerializeField] private KunaiManager _kunaiManager;
    [SerializeField] private bool _isInBulletTime = false;
    [SerializeField] private Animator _arrowAnimator;
    private List<GameObject> _inRangeFishList;
    private List<GameObject> _markedFishList;
    private List<GameObject> _targetableFish = new();
    [SerializeField] private GameObject _currentlyTargetedFish;
    private int _kunaiCommitted = 0;
    //[SerializeField] private List<GameObject> _markedFish = new();

    [Header("UnityEvents")]
    public UnityEvent OnEnterBulletTimeAction; 
    public UnityEvent OnExitBulletTimeAction;
    public UnityEvent<bool,GameObject> OnKunaiThrown;
    public UnityEvent OnHit;
    public UnityEvent OnMiss;
    public UnityEvent OnAllKunaiCommitted;
    

    private bool _isMinigameRunning = false;

    private bool _actionInput = false;
    private bool _nextInput = false;
    private bool _prevInput = false;
    private bool _backInput = false;



    //monobehaviours
    private void Awake()
    {
        _arrowAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    private void Update()
    {
        ListenForInput();
        ControlBulletTime();

        if (_isInBulletTime)
        {
           

            if (_currentlyTargetedFish == null)
            {
                UpdateTargetableFish();
                TargetFirstFish();
            }
            
            if (_currentlyTargetedFish != null)
            {
                if (_nextInput && !_isCoolingInput)
                {
                    CooldownInput();
                    ChangeFishTarget(1);
                }
                    
                if (_prevInput && !_isCoolingInput)
                {
                    CooldownInput();
                    ChangeFishTarget(-1);
                }

                DriveMinigameOnCurrentFish();
            }
            if (_arrowAnimator.isActiveAndEnabled)
            {
                if (_targetableFish.Count > 1 && !_arrowAnimator.GetBool("isOptionsAvailable"))
                    _arrowAnimator.SetBool("isOptionsAvailable", true);
                else if (_targetableFish.Count <= 1 && _arrowAnimator.GetBool("isOptionsAvailable"))
                    _arrowAnimator.SetBool("isOptionsAvailable", false);
            }
            
        }
    }



    //internals
    private void UpdateTargetableFish()
    {
        //Debug.Log("Detecting Fish...");
        if (_inRangeFishList == null)
            _inRangeFishList = _despawner.GetInRangeFish();
        if (_markedFishList == null)
            _markedFishList = _kunaiThrower.GetMarkedFish();

        _targetableFish.Clear();
        if (_inRangeFishList.Count > 0)
        {
            for (int i = 0; i < _inRangeFishList.Count; i++)
            {
                if (!_markedFishList.Contains(_inRangeFishList[i]))
                    _targetableFish.Add(_inRangeFishList[i]);
            }
        }

        _kunaiCommitted = _markedFishList.Count;

        
    }

    private void TargetFirstFish()
    {
        if (_targetableFish.Count > 0)
            _currentlyTargetedFish = _targetableFish[0];
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
        ThrowKunaiOnActionPress();

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
        if (_targetableFish.Count > 1)
        {
            int currentFishindex = _targetableFish.IndexOf(_currentlyTargetedFish);

            //if we're at the first index (and moving backwards), then go to the end of the list
            if ( currentFishindex == 0 && direction == -1)
                _currentlyTargetedFish = _targetableFish[_targetableFish.Count - 1];

            //else if were at the last index (and moving forwards), then go to the start of the list
            else if (currentFishindex == _targetableFish.Count - 1 && direction == 1)
                _currentlyTargetedFish = _targetableFish[0];

            else
                _currentlyTargetedFish = _targetableFish[currentFishindex + direction];

            _minigame.ResetCycler();
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

    private void ThrowKunaiOnActionPress()
    {
        if (_actionInput! && !_isCoolingInput)
        {
            CooldownInput();

            _minigame.FreezeCycler();
            bool fishHit = _minigame.IsRunnerOnSweetSpot();
            OnKunaiThrown?.Invoke(fishHit, _currentlyTargetedFish);

            if (fishHit)
                OnHit?.Invoke();
            else OnMiss?.Invoke();

            _kunaiCommitted++;
            if (_kunaiCommitted == _kunaiManager.KunaiCount())
                OnAllKunaiCommitted?.Invoke();
        }
    }
    private void CooldownInput()
    {
        _isCoolingInput = true;
        Invoke(nameof(ReadyInput), _inputDelay);

    }
    private void ReadyInput() { _isCoolingInput = false; }

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
        _currentlyTargetedFish = null;
    }
    public void RespondToFishOutOfRange(GameObject fish)
    {
        UpdateTargetableFish();

        if (fish == _currentlyTargetedFish)
        {
            Vector3 screenPosition = _mainCam.WorldToScreenPoint(fish.transform.position);
            GameObject targetLostEffect = Instantiate(_targetLostFeedbackPrefab, _uiContainer);
            targetLostEffect.GetComponent<RectTransform>().position = screenPosition;

            _currentlyTargetedFish = null;

            if (_isMinigameRunning)
            {
                _isMinigameRunning = false;
                _minigame.CloseCycler();
            }
        }
    }

    public void RespondToFishInRange(GameObject fish)
    {
        UpdateTargetableFish();
    }

    public void RespondToFishMarked(GameObject fish)
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




    public void LogThrow(bool result)
    {
        if (result)
            Debug.Log("Kunai HIT!!!");
        else Debug.Log("MISSED!");
    }





}
