using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameDriver : MonoBehaviour
{
    [SerializeField] private RectTransform _minigameRectTransform;
    [SerializeField] private TimedClickMiniGame _minigame;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private FishDespawner _despawner;
    [SerializeField] private bool _isInBulletTime = false;
    private List<GameObject> _targetableFishList;
    [SerializeField] private GameObject _currentlyTargetedFish;
    //[SerializeField] private List<GameObject> _markedFish = new();

    private bool _isMinigameRunning = false;



    //monobehaviours
    private void Update()
    {
        if (_isInBulletTime)
        {
            if (_currentlyTargetedFish == null)
                DetectNewFish();
            
            if (_currentlyTargetedFish != null)
            {
                DriveMinigameOnCurrentFish();
            }
        }
    }



    //internals
    private void DetectNewFish()
    {
        Debug.Log("Detecting Fish...");
        if (_targetableFishList == null)
            _targetableFishList = _despawner.GetInRangeFish();

        if (_targetableFishList.Count > 0)
        {
            _currentlyTargetedFish = _targetableFishList[0];
        }
    }
    
    
    private void DriveMinigameOnCurrentFish()
    {
        Debug.Log("Driving Minigame...");
        if (_isMinigameRunning == false)
        {
            _isMinigameRunning = true;
            _minigame.StartCycler();
        }

        _minigameRectTransform.position = _mainCam.WorldToScreenPoint(_currentlyTargetedFish.transform.position);


    }


    //externals
    public void RespondToBulletTimeEntered()
    {
        _isInBulletTime = true;
    }
    public void RespondToBulletTimeExited()
    {
        _isInBulletTime = false;

        if (_isMinigameRunning)
        {
            _isMinigameRunning = false;
            _minigame.CloseCycler();
        }
    }
    public void RespondToFishOutOfRange(GameObject fish)
    {
        if (fish == _currentlyTargetedFish)
        {
            _currentlyTargetedFish = null;

            if (_isMinigameRunning)
            {
                _isMinigameRunning = false;
                _minigame.CloseCycler();
            }
        }
    }








}
