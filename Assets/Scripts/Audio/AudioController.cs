using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _ambienceSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _oneShotSource;

    [Header("Inspector Tools Settings")]
    [SerializeField] private bool _enableTestCommands = false;

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


    private enum MultiShotClips
    {
        Throw,
        SalmonHit
    }


    //monobehaviors
    private void Start()
    {
        _ambienceSource.Play();
        _musicSource.Play();
    }

    private void Update()
    {
        if (_enableTestCommands)
            ListenForDebugCommands();

        if (_isDelayingSalmonPlay)
            TickSalmonWaitDelay();

    }


    //internals
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



    //externals
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
    }










}
