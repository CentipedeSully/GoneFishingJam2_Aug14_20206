using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BulletTime : MonoBehaviour
{
    [SerializeField] private Image _radialTimer;
    [SerializeField] private KunaiManager _kunaiManager;
    [SerializeField] private SpriteRenderer _bulletTimeGraphic;
    [SerializeField] private Image _hourglass;
    [SerializeField] private float _inactiveAlpha;
    [SerializeField] private float _activeAlpha;

    [Space(20)]
    [SerializeField] private float _transitionDuration = .5f;
    [SerializeField] private bool _isTransitioning = false;
    private float _currentTransitionTime = 0;
    [Space(20)]

    [SerializeField] private bool _isInBulletTime = false;
    [SerializeField] private float _bulletTimeScale = .2f;
    [SerializeField] private float _bulletTimeDuration = 5f;
    private float _currentBulletTime = 0;


    [Header("unityEvents")]
    public UnityEvent OnBulletTimeEntered;
    public UnityEvent OnBulletTimeExited;

    [Header("Debug")]
    [SerializeField] private bool _debugActive = false;
    [SerializeField] private bool _cmdToggleBulletTime = false;
    private float _transitionAlpha;

    float _remainingTime;


    private void Start()
    {
        _radialTimer.gameObject.SetActive(false);
        _hourglass.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_debugActive)
            ListenForDebugCommands();

        if (_isTransitioning)
            TickTransition();

        if (_isInBulletTime)
        {
            TickBulletTime();
        }

    }




    private void TickBulletTime()
    {
        _currentBulletTime += Time.unscaledDeltaTime;
        _remainingTime = _bulletTimeDuration - _currentBulletTime;
        _radialTimer.fillAmount = _remainingTime/_bulletTimeDuration;
        

        if (_currentBulletTime >= _bulletTimeDuration)
        {
            ExitBulletTime();
        }
    }

    private void TickTransition()
    {
        //enter bullet time if we aren't in bullet time
        if (!_isInBulletTime)
        {
            _currentTransitionTime += Time.unscaledDeltaTime;
            _transitionAlpha = Mathf.Lerp(_inactiveAlpha, _activeAlpha, _currentTransitionTime / _transitionDuration);
            _bulletTimeGraphic.color = new Color(_bulletTimeGraphic.color.r, _bulletTimeGraphic.color.g, _bulletTimeGraphic.color.b, _transitionAlpha); 
            Time.timeScale = Mathf.Lerp(1, _bulletTimeScale, _currentTransitionTime / _transitionDuration);
            
            if (_currentTransitionTime >= _transitionDuration)
            {
                _currentTransitionTime = 0;
                _isTransitioning = false;
                _isInBulletTime = true;
                OnBulletTimeEntered?.Invoke();
            }
        }

        //exit bullet time
        else
        {
            _currentTransitionTime += Time.unscaledDeltaTime;
            _transitionAlpha = Mathf.Lerp(_activeAlpha, _inactiveAlpha, _currentTransitionTime / _transitionDuration);
            _bulletTimeGraphic.color = new Color(_bulletTimeGraphic.color.r, _bulletTimeGraphic.color.g, _bulletTimeGraphic.color.b, _transitionAlpha);
            Time.timeScale = Mathf.Lerp(_bulletTimeScale, 1, _currentTransitionTime / _transitionDuration);

            if (_currentTransitionTime >= _transitionDuration)
            {
                _currentTransitionTime = 0;
                _isTransitioning = false;
                _isInBulletTime = false;

            }
        }
    }




    public void EnterBulletTime()
    {
        if (!_isTransitioning && !_isInBulletTime && _kunaiManager.KunaiCount() > 0)
        {
            _isTransitioning = true;
            _radialTimer.gameObject.SetActive(true);
            _hourglass.gameObject.SetActive(true);
            _radialTimer.fillAmount = 1;
            
        }
       
    }

    public void ExitBulletTime()
    {
        if (!_isTransitioning && _isInBulletTime)
        {
            _radialTimer.gameObject.SetActive(false);
            _hourglass.gameObject.SetActive(false);
            _currentBulletTime = 0;
            _isTransitioning = true;
            OnBulletTimeExited?.Invoke();
        }
    }

    public bool IsInBulletTime()
    {
        return _isInBulletTime;
    }

    public float GetBulletTimeDuration() { return _bulletTimeDuration; }


    private void ListenForDebugCommands()
    {
        if (_cmdToggleBulletTime)
        {
            _cmdToggleBulletTime = false;
            if (_isInBulletTime)
                ExitBulletTime();
            else EnterBulletTime();
        }
    }

}
