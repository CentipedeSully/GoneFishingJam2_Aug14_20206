using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class HitShake : MonoBehaviour
{
    [SerializeField] private float _duration = .25f;
    [SerializeField] private float _magnitude = .25f;
    [SerializeField] private float _frequency = .02f;
    [SerializeField] private bool _isShaking = false;
    [SerializeField] private float _destoryDelay = 1f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _shakeRectTransform = false;
    [SerializeField] private float _rectTransformMagMultiplier = 10;
    private RectTransform _rectTransform;
    private float _currentTime = 0;
    private float _freqencyCounter = 0;
    private Vector3 _positionBeforeShake;
    private Vector3 _randomizedOffset;
    private float _xRandomized;
    private float _yRandomized;
    public UnityEvent OnShakeStarted;
    public UnityEvent OnShakeEnded;


    private void Awake()
    {
        if (_shakeRectTransform)
            _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_isShaking)
            Shake();
    }


    private void Shake()
    {
        _currentTime += Time.deltaTime;
        _freqencyCounter += Time.deltaTime;

        if (_freqencyCounter >= _frequency)
        {
            if (_shakeRectTransform)
            {
                float magnitude = _rectTransformMagMultiplier * _magnitude;
                _xRandomized = Random.Range(-magnitude, magnitude);
                _yRandomized = Random.Range(-magnitude, magnitude);
            }
            else
            {
                _xRandomized = Random.Range(-_magnitude, _magnitude);
                _yRandomized = Random.Range(-_magnitude, _magnitude);
            }
                

            _randomizedOffset = new Vector3(_xRandomized, _yRandomized, 0);

            if (_shakeRectTransform)
                _rectTransform.position = _positionBeforeShake + _randomizedOffset;
            else
                transform.position = _positionBeforeShake + _randomizedOffset;

            _freqencyCounter = 0;
        }

        if (_currentTime >= _duration)
            EndShake();
        
    }

    [ContextMenu("TriggerShake")]
    public void TriggerShake()
    {
        if (_isShaking)
            EndShake();

        if (_animator != null)
        {
            Sprite currentSprite = _spriteRenderer.sprite;
            _animator.SetBool("IsCaught",true);
            _spriteRenderer.sprite = currentSprite;
        }

        _isShaking = true;

        if (_shakeRectTransform)
            _positionBeforeShake = _rectTransform.position;
        else
            _positionBeforeShake = transform.position;

        OnShakeStarted?.Invoke();
    }

    public void EndShake()
    {
        _isShaking = false;
        _currentTime = 0;
        _freqencyCounter = 0;

        if (_shakeRectTransform)
            _rectTransform.position = _positionBeforeShake;
        else
            transform.position = _positionBeforeShake;

        OnShakeEnded?.Invoke();

    }

    public void DestroySelfAfterDelay()
    {
        Invoke(nameof(DestroySelf), _destoryDelay);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
