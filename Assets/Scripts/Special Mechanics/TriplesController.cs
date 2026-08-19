using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TriplesController : MonoBehaviour
{
    [SerializeField] private GameObject _triplesStarPrefab;
    [SerializeField] private GameObject _validation;
    [SerializeField] private float _tripleHitRewardDelay;
    private int _triplesCount;
    private int _starMax = 5;
    private int _starsDisplayed = 0;

    public UnityEvent OnTripleHit;
    public UnityEvent OnGameCompleted;


    [ContextMenu("Force Triple Hit")]
    public void DebugTrackTriple()
    {
        TrackTripleHit(3);
    }

    public void TrackTripleHit(int fishHit)
    {
        if (fishHit < 3)
            return;


        StartCoroutine(TripleHitPoster());
        
    }

    IEnumerator TripleHitPoster()
    {
        yield return new WaitForSecondsRealtime(_tripleHitRewardDelay);
        OnTripleHit?.Invoke();

        _triplesCount++;
        PinStarOntoScreen();
    }

    private void PinStarOntoScreen()
    {
        if (_starsDisplayed < _starMax)
        {
            Instantiate(_triplesStarPrefab, GetComponent<RectTransform>());
            _starsDisplayed++;

            if (_starsDisplayed == _starMax)
            {
                _validation.SetActive(true);
                OnGameCompleted?.Invoke();
            }
        }
    }

}
