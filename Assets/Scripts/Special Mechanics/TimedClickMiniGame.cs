using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimedClickMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject _cycler;
    [SerializeField] private Transform _playerRunner;
    [SerializeField] private Transform _minRunner;
    [SerializeField] private Transform _maxRunner;
    [SerializeField] private Image _fillAreaImage;
    [SerializeField] private Image _fillBackground;

    [SerializeField] private float _currentValue = 0;
    [SerializeField] private float _maxValue = 360;
    [SerializeField] private float _cycleRate = 1f;
    [SerializeField] private float _sweetSpotRange;
    [SerializeField] private bool _isCycling = false;
    [SerializeField] private float _sweetSpotMin;
    [SerializeField] private float _sweetSpotMax;

    [Header("Difficulty Progression")]
    [SerializeField] private float _lv1Range;
    [SerializeField] private float _lv2Range;
    [SerializeField] private float _lv3Range;
    [SerializeField] private float _lv4Range;
    [SerializeField] private float _lv5Range;
    [SerializeField] private int _currentLevel = 1;


    

    [Header("UnityEvents")]
    public UnityEvent<TimedClickMiniGame> OnMinigameStarted;
    public UnityEvent<bool> OnCycleStopped;
    public UnityEvent OnMinigameEnded;



    [Header("Debug")]
    [SerializeField] private bool _isDebugActive = false;
    [SerializeField] private bool _cmdCalculateSweetSpot = false;
    [SerializeField] private bool _cmdStartRunner = false;
    [SerializeField] private bool _cmdCloseRunner = false;
    [SerializeField] private bool _cmdFreezeRunner = false;
    [SerializeField] private bool _cmdResumeRunner = false;




    //monobehaviours
    private void Awake()
    {
        _sweetSpotRange = _lv1Range;
    }

    private void Update()
    {
        if (_isDebugActive)
            ListenForDebugCommands();

        if (_isCycling)
        {
            TickCycler();
        }
            
    }



    //internals
    private void TickCycler()
    {
        _currentValue += Time.unscaledDeltaTime * _cycleRate;

        
        if (_currentValue >= _maxValue)
            _currentValue -= _maxValue;

        _playerRunner.rotation = Quaternion.Euler(0, 0, -_currentValue);

    }


    private void CalculateSweetSpot()
    {
        float sweetSpot = Random.Range(0, _maxValue);

        _sweetSpotMin = sweetSpot - _sweetSpotRange;
        if (_sweetSpotMin < 0)
            _sweetSpotMin = _maxValue + _sweetSpotMin;

        _sweetSpotMax = sweetSpot + _sweetSpotRange;
        if (_sweetSpotMax > _maxValue)
            _sweetSpotMax = _sweetSpotMax - _maxValue;

        //rotate the runners to visually represent the target area
        _minRunner.rotation = Quaternion.Euler(0, 0, -_sweetSpotMin);
        _maxRunner.rotation = Quaternion.Euler(0, 0, -_sweetSpotMax);

        //also make sure the fill area reflects the target area
        _fillAreaImage.transform.rotation = Quaternion.Euler(0, 0, -sweetSpot + _sweetSpotRange);
        _fillBackground.transform.rotation = Quaternion.Euler(0, 0, -sweetSpot + _sweetSpotRange);
        _fillAreaImage.fillAmount = _sweetSpotRange * 2 / _maxValue;

    }



    //externals
    public void StartCycler()
    {
        _cycler.SetActive(true);
        _currentValue = 0;
        CalculateSweetSpot();
        _isCycling = true;
        OnMinigameStarted?.Invoke(this);
    }
    public void FreezeCycler()
    {
        _isCycling = false;
        OnCycleStopped?.Invoke(this);
    }
    public void CloseCycler()
    {
        _isCycling = false;
        _cycler.SetActive(false);
        OnMinigameEnded?.Invoke();
    }
    public void ResumeRunner()
    {
        _isCycling = true;
    }

    
    public void ResetCycler()
    {
        _currentValue = 0;
        CalculateSweetSpot();
    }

    public bool IsRunnerOnSweetSpot()
    {
        float distanceFromMinBar = Mathf.Abs(Mathf.DeltaAngle(_playerRunner.rotation.eulerAngles.z, _minRunner.rotation.eulerAngles.z));
        //Debug.Log($"Runner Distance from Minbar: {distanceFromMinBar}");
        float distanceFromMaxBar = Mathf.Abs(Mathf.DeltaAngle(_playerRunner.rotation.eulerAngles.z, _maxRunner.rotation.eulerAngles.z));
        //Debug.Log($"Runner Distance from Maxbar: {distanceFromMaxBar}");

        if (distanceFromMinBar <= _sweetSpotRange * 2 && distanceFromMaxBar <= _sweetSpotRange * 2)
            return true;
        return false;
    }

    public void RespondToTripleHit(int fishHit)
    {
        if (fishHit < 3)
            return;

        if (_currentLevel < 5)
        {
            switch (_currentLevel)
            {
                case 1:
                    _currentLevel++;
                    _sweetSpotRange = _lv2Range;
                    break;

                case 2:
                    _currentLevel++;
                    _sweetSpotRange = _lv3Range;
                    break;

                case 3:
                    _currentLevel++;
                    _sweetSpotRange = _lv4Range;
                    break;

                case 4:
                    _currentLevel++;
                    _sweetSpotRange = _lv5Range;
                    break;

                default:
                    break;
            }
        }
    }

    //Debug
    private void ListenForDebugCommands()
    {
        if (_cmdCalculateSweetSpot)
        {
            _cmdCalculateSweetSpot = false;
            CalculateSweetSpot();
        }

        if (_cmdStartRunner)
        {
            _cmdStartRunner = false;
            StartCycler();

        }
        if (_cmdFreezeRunner)
        {
            _cmdFreezeRunner = false;
            FreezeCycler();
        }
        if (_cmdResumeRunner)
        {
            _cmdResumeRunner = false;
            ResumeRunner();
        }
        if (_cmdCloseRunner)
        {
            _cmdCloseRunner = false;
            CloseCycler();
        }
    }
}
