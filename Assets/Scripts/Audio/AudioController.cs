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

    /*
    [Header("Salmon Hit (OneShot) Audio Settings")]
    [SerializeField] private AudioClip _HitClip;
    [SerializeField] private float _salmonPitchBaseline = 1;
    [SerializeField] private float _salmonPitchRange = .2f;
    [SerializeField] private float _salmonBaseVolume = .5f;
    [SerializeField] private bool _cmdTriggerSalmonHitEffect;
    */


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
    }










}
