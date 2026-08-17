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
    private float _currentTime = 0;
    private float _freqencyCounter = 0;
    private Vector3 _positionBeforeShake;
    private Vector3 _randomizedOffset;
    private float _xRandomized;
    private float _yRandomized;
    public UnityEvent OnShakeStarted;
    public UnityEvent OnShakeEnded;


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
            _xRandomized = Random.Range(-_magnitude, _magnitude);
            _yRandomized = Random.Range(-_magnitude, _magnitude);

            _randomizedOffset = new Vector3(_xRandomized, _yRandomized, 0);
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
        _positionBeforeShake = transform.position;
        OnShakeStarted?.Invoke();
    }

    public void EndShake()
    {
        _isShaking = false;
        _currentTime = 0;
        _freqencyCounter = 0;
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
