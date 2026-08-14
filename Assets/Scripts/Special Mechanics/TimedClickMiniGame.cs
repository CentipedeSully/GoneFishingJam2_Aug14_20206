using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedClickMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject _cycler;
    [SerializeField] private Transform _playerRunner;
    [SerializeField] private Transform _minRunner;
    [SerializeField] private Transform _maxRunner;

    [SerializeField] private float _currentValue = 0;
    [SerializeField] private float _maxValue = 360;
    [SerializeField] private float _cycleRate = 1f;
    [SerializeField] private float _sweetSpotRange;
    [SerializeField] private bool _isCycling = false;
    [SerializeField] private float _sweetSpotMin;
    [SerializeField] private float _sweetSpotMax;

    [Space(20)]
    [SerializeField] private bool _isDebugActive = false;
    [SerializeField] private bool _cmdCalculateSweetSpot = false;
    [SerializeField] private bool _cmdStartRunner = false;
    [SerializeField] private bool _cmdCloseRunner = false;
    [SerializeField] private bool _cmdFreezeRunner = false;
    [SerializeField] private bool _cmdResumeRunner = false;





    private void Update()
    {
        if (_isDebugActive)
            ListenForDebugCommands();

        if (_isCycling)
            TickCycler();
    }


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

        _minRunner.rotation = Quaternion.Euler(0, 0, -_sweetSpotMin);
        _maxRunner.rotation = Quaternion.Euler(0, 0, -_sweetSpotMax);
    }

    public void StartCycler()
    {
        _cycler.SetActive(true);
        _currentValue = 0;
        CalculateSweetSpot();
        _isCycling = true;
    }

    public void FreezeCycler()
    {
        _isCycling = false;

    }

    public void CloseCycler()
    {
        _isCycling = false;
        _cycler.SetActive(false);
    }

    public void ResumeRunner()
    {
        _isCycling = true;
    }

    


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
