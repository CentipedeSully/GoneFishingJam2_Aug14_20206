using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _ambienceSource;
    [SerializeField] private float _ambienceVolume;
    [SerializeField] private float _ambienceFadeInDuration = 2;
    private float _currentAmbienceFadeTime = 0;
    private bool _isFadingInAmbience = false;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private float _musicVolume;
    [SerializeField] private AudioSource _oneShotSource;

    [Header("Inspector Tools Settings")]
    [SerializeField] private bool _enableTestCommands = false;

    [Header("Bullet Time Settings")]
    [SerializeField] private AudioSource _bulletTimeSource;
    [SerializeField] private float _bulletTimeVolume;
    [SerializeField] private float _reducedAmbienceVol;
    [SerializeField] private float _reducedMusicVol;
    [SerializeField] private float _fadeDuration;
    private float _currentFadeTime;
    private bool _isBulletTimePlaying = false;
    private bool _isFading = false;
    [SerializeField] private AudioClip _bulletTimePulseClip;
    [SerializeField] private float _firstPulsePitch;
    [SerializeField] private float _secondPulsePitch;
    [SerializeField] private float _pulseVolume;
    [SerializeField] private float _pulseDelay = .3f;
    [SerializeField] private float _pulseFrequency= .5f;
    private IEnumerator _paceMaker;


    [Header("Throw Audio (OneShot) Settings")]
    [SerializeField] private AudioClip _throwClip;
    [SerializeField] private float _throwPitchBaseline = 1;
    [SerializeField] private float _throwPitchRange;
    [SerializeField] private float _throwBaseVolume = .5f;
    [SerializeField] private float _multiThrowDelay = .1f;
    [SerializeField] private bool _cmdTriggerThrowEffect;
    [SerializeField] private int _throwsToMake = 1;

    [Header("Kunai Regen (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _kunaiRegenClip;
    [SerializeField] private float _kunaiPitchBaseline = 1;
    [SerializeField] private float _kunaiPitchRange = .2f;
    [SerializeField] private float _kunaiBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerKunaiRegenEffect;


    [Header("Salmon Hit (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _salmonHitClip;
    [SerializeField] private float _salmonPitchBaseline = 1;
    [SerializeField] private float _salmonPitchRange = .2f;
    [SerializeField] private float _salmonBaseVolume = .5f;
    [SerializeField] private float _multiSalmonHitDelay = .1f;
    [SerializeField] private float _salmonPlayWaitDelay = .15f;
    [SerializeField] private bool _cmdTriggerSalmonHitEffect;
    [SerializeField] private int _hitsToPerform = 1;
    private float _currentSalmonPlayDelay = 0;
    private bool _isDelayingSalmonPlay = false;
    private int _delayedPlayTimes;


    [Header("UpSplash (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _upsplashClip;
    [SerializeField] private float _upsplashPitchBaseline = 1;
    [SerializeField] private float _upsplashPitchRange = .2f;
    [SerializeField] private float _upsplashBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerUpsplashEffect;

    [Header("DownSplash (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _downsplashClip;
    [SerializeField] private float _downsplashPitchBaseline = 1;
    [SerializeField] private float _downsplashPitchRange = .2f;
    [SerializeField] private float _downsplashBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerDownsplashEffect;


    [Header("Fish Deposit (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _depositClip;
    [SerializeField] private float _depositPitchBaseline = 1;
    [SerializeField] private float _depositPitchRange = .2f;
    [SerializeField] private float _depositBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerDepositEffect;

    [Header("TripleHit (OneShot) Audio Setting")]
    [SerializeField] private AudioClip _tripleJingleClip;
    [SerializeField] private float _triplePitchBaseline = 1;
    [SerializeField] private float _tripleBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerTripleEffect;

    [Header("GameEnd (OneShot) Audio Setting")]
    [SerializeField] private AudioClip _completionClip;
    [SerializeField] private AudioClip _completionClip2;
    [SerializeField] private AudioSource _completionSource;
    [SerializeField] private float _betweenDelay = .56f;
    [SerializeField] private float _completionPitchBaseline = 1;
    [SerializeField] private float _completionBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerCompletionEffect;

    [Header("Ui Click (OneShot) Audio Setting")]
    [SerializeField] private AudioClip _uiClickClip;
    [SerializeField] private float _uiClickPitchBaseline = 1;
    [SerializeField] private float _uiClickBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerUiClickEffect;

    
    private enum MultiShotClips
    {
        Throw,
        SalmonHit
    }


    //monobehaviors

    private void Update()
    {
        if (_enableTestCommands)
            ListenForDebugCommands();

        if (_isFadingInAmbience)
            FadeInAmbience();

        if (_isDelayingSalmonPlay)
            TickSalmonWaitDelay();

        if (_isFading)
            TickBulletTimeTransition();

        
    }


    //internals
    private void FadeInAmbience()
    {
        _currentAmbienceFadeTime += Time.deltaTime;
        _ambienceSource.volume = _ambienceVolume * (_currentAmbienceFadeTime / _ambienceFadeInDuration);

        if (_currentAmbienceFadeTime >= _ambienceFadeInDuration)
        {
            _currentAmbienceFadeTime = 0;
            _isFadingInAmbience = false;
        }
    }
    private void TickSalmonWaitDelay()
    {
        _currentSalmonPlayDelay += Time.unscaledDeltaTime;
        if (_currentSalmonPlayDelay >= _salmonPlayWaitDelay)
        {
            _currentSalmonPlayDelay = 0;
            _isDelayingSalmonPlay = false;
            PlaySalmonHitAudio(_delayedPlayTimes);
            _delayedPlayTimes = 0;
        }
    }
    private IEnumerator PlayMultiShotAudio(int timesToPlay, MultiShotClips clip)
    {
        int playsTriggered = 0;

        while (playsTriggered < timesToPlay)
        {
            switch (clip)
            {
                case MultiShotClips.Throw:
                    PlayThrowAudio();
                    yield return new WaitForSecondsRealtime(_multiThrowDelay);
                    break;

                case MultiShotClips.SalmonHit:
                    PlaySalmonHitAudio();
                    yield return new WaitForSecondsRealtime(_multiSalmonHitDelay);
                    break;
            }

            playsTriggered++;
        }
    }

    private float GetRandomizedPitch(float baseline,float range)
    {
        return Random.Range(-range, range) + baseline;
    }

    private void TickBulletTimeTransition()
    {
        
        //fade into bullet time
        if (!_isBulletTimePlaying)
        {
            _currentFadeTime += Time.unscaledDeltaTime;
            float progression = _currentFadeTime / _fadeDuration;
            _bulletTimeSource.volume = Mathf.Lerp(0, _bulletTimeVolume, progression);
            _musicSource.volume = Mathf.Lerp(_musicVolume, _reducedMusicVol, progression);
            _ambienceSource.volume = Mathf.Lerp(_ambienceVolume, _reducedAmbienceVol, progression);

            if (_currentFadeTime >= _fadeDuration)
            {
                _currentFadeTime = 0;
                _isBulletTimePlaying = true;
                _isFading = false;

            }
        }

        //fade out of bullet time
        else
        {
            _currentFadeTime += Time.unscaledDeltaTime;
            float progression = _currentFadeTime / _fadeDuration;
            _bulletTimeSource.volume = Mathf.Lerp(_bulletTimeVolume, 0, progression);
            _musicSource.volume = Mathf.Lerp(_reducedMusicVol, _musicVolume, progression);
            _ambienceSource.volume = Mathf.Lerp(_reducedAmbienceVol, _ambienceVolume, progression);

            if (_currentFadeTime >= _fadeDuration)
            {
                _currentFadeTime = 0;
                _isBulletTimePlaying = false;
                _isFading = false;

                //end the bullet time ambience
                if (_paceMaker != null)
                {
                    StopCoroutine(_paceMaker);
                    _paceMaker = null;
                }

            }
        }

    }

    private IEnumerator TickPulses()
    {
        while (true)
        {
            FirstPulse();
            yield return new WaitForSecondsRealtime(_pulseDelay);
            SecondPulse();
            yield return new WaitForSecondsRealtime(_pulseFrequency);
        }

    }

    private void FirstPulse()
    {
        _oneShotSource.volume = _pulseVolume;
        _oneShotSource.pitch = _firstPulsePitch;
        _oneShotSource.PlayOneShot(_bulletTimePulseClip);
    }
    private void SecondPulse()
    {
        _oneShotSource.volume = _pulseVolume;
        _oneShotSource.pitch = _secondPulsePitch;
        _oneShotSource.PlayOneShot(_bulletTimePulseClip);
    }

  



    //externals
    public void RespondToEnterBulletTime()
    {
        //ensure all utiliites are reset
        _isFading = true;
        _currentFadeTime = 0;
        _isBulletTimePlaying = false;

        if (_paceMaker != null)
            StopCoroutine(_paceMaker);

        _paceMaker = TickPulses();
        StartCoroutine(_paceMaker);
    }
    public void RespondToExitBulletTime()
    {
        //ensure all utiliites are reset
        _isFading = true;
        _currentFadeTime = 0;
        _isBulletTimePlaying = true;

        
    }

    public void PlayThrowAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_throwPitchBaseline,_throwPitchRange);
        _oneShotSource.volume = _throwBaseVolume;
        _oneShotSource.PlayOneShot(_throwClip);
    }

    public void PlayThrowAudio(int times)
    {
        StartCoroutine(PlayMultiShotAudio(times,MultiShotClips.Throw));
    }

    public void PlayKunaiRegenAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_kunaiPitchBaseline, _kunaiPitchRange);
        _oneShotSource.volume = _kunaiBaseVolume;
        _oneShotSource.PlayOneShot(_kunaiRegenClip);
    }

    public void PlaySalmonHitAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_salmonPitchBaseline, _salmonPitchRange);
        _oneShotSource.volume = _salmonBaseVolume;
        _oneShotSource.PlayOneShot(_salmonHitClip);
    }

    public void PlaySalmonHitAudio(int times)
    {
        StartCoroutine(PlayMultiShotAudio(times, MultiShotClips.SalmonHit));
    }

    public void PlayDelayedSalmonHitAudio(int times)
    {
        _isDelayingSalmonPlay = true;
        _delayedPlayTimes = times;
    }

    public void PlayUpsplashAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_upsplashPitchBaseline, _upsplashPitchRange);
        _oneShotSource.volume = _upsplashBaseVolume;
        _oneShotSource.PlayOneShot(_upsplashClip);
    }

    public void PlayDownsplashAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_downsplashPitchBaseline, _downsplashPitchRange);
        _oneShotSource.volume = _downsplashBaseVolume;
        _oneShotSource.PlayOneShot(_downsplashClip);
    }

    public void PlayDepositAudio()
    {
        _oneShotSource.pitch = GetRandomizedPitch(_depositPitchBaseline, _depositPitchRange);
        _oneShotSource.volume = _depositBaseVolume;
        _oneShotSource.PlayOneShot(_depositClip);
    }

    public void PlayTripleHitAudio()
    {
        _oneShotSource.pitch = _triplePitchBaseline;
        _oneShotSource.volume = _tripleBaseVolume;
        _oneShotSource.PlayOneShot(_tripleJingleClip);
    }

    public void PlayCompletionAudio()
    {
        _completionSource.pitch = _completionPitchBaseline;
        _completionSource.volume = _completionBaseVolume;
        _completionSource.Play();
    }

    public void PlayAmbience()
    {
        _ambienceSource.volume = 0;
        _ambienceSource.Play();
        _isFadingInAmbience = true;
    }

    public void PlayMusic()
    {
        _musicSource.Play();
        _musicSource.volume = _musicVolume;
    }

    public void PlayUiClickAudio()
    {
        _oneShotSource.pitch = _uiClickPitchBaseline;
        _oneShotSource.volume = _uiClickBaseVolume;
        _oneShotSource.PlayOneShot(_uiClickClip);
    }


    //Debug
    private void ListenForDebugCommands()
    {
        if (_cmdTriggerThrowEffect)
        {
            _cmdTriggerThrowEffect = false;
            PlayThrowAudio(_throwsToMake);
        }

        if (_cmdTriggerKunaiRegenEffect)
        {
            _cmdTriggerKunaiRegenEffect = false;
            PlayKunaiRegenAudio();
        }

        if (_cmdTriggerSalmonHitEffect)
        {
            _cmdTriggerSalmonHitEffect = false;
            PlaySalmonHitAudio(_hitsToPerform);
        }

        if (_cmdTriggerUpsplashEffect)
        {
            _cmdTriggerUpsplashEffect = false;
            PlayUpsplashAudio();
        }

        if (_cmdTriggerDownsplashEffect)
        {
            _cmdTriggerDownsplashEffect = false;
            PlayDownsplashAudio();
        }

        if (_cmdTriggerDepositEffect)
        {
            _cmdTriggerDepositEffect = false;
            PlayDepositAudio();
        }

        if (_cmdTriggerTripleEffect)
        {
            _cmdTriggerTripleEffect = false;
            PlayTripleHitAudio();
        }

        if (_cmdTriggerCompletionEffect)
        {
            _cmdTriggerCompletionEffect = false;
            PlayCompletionAudio();
        }

        if (_cmdTriggerUiClickEffect)
        {
            _cmdTriggerUiClickEffect = false;
            PlayUiClickAudio();
        }

    }










}
