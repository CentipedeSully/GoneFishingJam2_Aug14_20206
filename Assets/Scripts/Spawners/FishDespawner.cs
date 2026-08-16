using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishDespawner : MonoBehaviour
{
    [SerializeField] private Transform _inactiveFishContainer;
    [SerializeField] private List<GameObject> _spawnedFish = new();
    [SerializeField] private Transform _waterEscapePoint;
    [SerializeField] private Transform _leftTargetingLimit;
    [SerializeField] private Transform _rightTargetingLimit;
    [SerializeField] private Transform _upperTargetingLimit;
    [SerializeField] private Transform _lowestTargetingLimit;


    [SerializeField] private List<GameObject> _inRangeFish = new();
    [SerializeField] private List<GameObject> _outOfRangeFish = new();

    [Header("UnityEvents")]
    public UnityEvent<GameObject> OnFishDespawned;
    public UnityEvent<GameObject> OnRangeEntered;
    public UnityEvent<GameObject> OnRangeExited;

    


    private void Update()
    {
        foreach (GameObject fish in _spawnedFish)
        {
            UpdateFishRange(fish);

            if (fish.transform.position.y <= _waterEscapePoint.position.y)
            {
                //clear the fish from both range lists
                if (_inRangeFish.Contains(fish))
                {
                    _inRangeFish.Remove(fish);
                    OnRangeExited?.Invoke(fish);
                }
                else if (_outOfRangeFish.Contains(fish))
                {
                    _outOfRangeFish.Remove(fish);
                }

                //deactivate and pool the fish for later
                fish.SetActive(false);
                fish.transform.SetParent(_inactiveFishContainer);
                OnFishDespawned?.Invoke(fish);
            }
        }
    }




    private void UpdateFishRange(GameObject fish)
    {
        //update the fish's current targeting range
        if (fish.transform.position.x < _leftTargetingLimit.position.x ||
            fish.transform.position.x > _rightTargetingLimit.position.x ||
            fish.transform.position.y > _upperTargetingLimit.position.y ||
            fish.transform.position.y < _lowestTargetingLimit.position.y)
        {
            //update fish as out of range
            if (_inRangeFish.Contains(fish))
            {
                _inRangeFish.Remove(fish);
                _outOfRangeFish.Add(fish);
                OnRangeExited?.Invoke(fish);
            }
        }

        else
        {
            //add to the 'inRange' list if not out of range (& not already in the list)
            if (!_inRangeFish.Contains(fish))
            {
                _inRangeFish.Add(fish);
                _outOfRangeFish.Remove(fish);
                OnRangeEntered?.Invoke(fish);
            }
        }
    }




    public void TrackActiveFishPosition(GameObject fish)
    {
        _spawnedFish.Add(fish);
    }
    public List<GameObject> GetInRangeFish()
    {
        return _inRangeFish;

    }

}
