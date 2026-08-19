using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletTimeEyesController : MonoBehaviour
{
    [SerializeField] private HitShake _hitShake;
    [SerializeField] private List<Image> _eyes = new();
    [SerializeField] private Sprite _idle;
    [SerializeField] private Sprite _closed;
    [SerializeField] private Sprite _lastSecond;
    [SerializeField] private Sprite _twoSecondsLeft;
    [SerializeField] private Sprite _threeSecondsLeft;
    [SerializeField] private Sprite _fourSecondsLeft;

    [SerializeField] private bool _isInBulletTime = false;
    [SerializeField] private float _avgBlinkCooldown = 6f;
    [SerializeField] private float _blinkVariance = 2f;
    [SerializeField] private float _blinkClosedDuration = .2f;


    private void SetEyes(Sprite sprite)
    {
        foreach (Image image in _eyes)
        {
            image.sprite = sprite;
        }
    }

    public void Blink()
    {
        if (_isInBulletTime)
            return;

        CancelInvoke();
        SetEyes(_closed);


        Invoke(nameof(Unblink), _blinkClosedDuration);
    }

    private void Unblink()
    {
        SetEyes(_idle);

        float timeTillNextBlink = Random.Range(-_blinkVariance, _blinkVariance) + _avgBlinkCooldown;
        Invoke(nameof(Blink), timeTillNextBlink);
    }

    public void RespondToEnteringBulletTime()
    {
        _isInBulletTime = true;
        SetEyes(_fourSecondsLeft);
        _hitShake.TriggerShake();
    }

    public void RespondToExitingBulletTime()
    {
        _isInBulletTime = false;
        _hitShake.EndShake();
        Blink();
    }

}
