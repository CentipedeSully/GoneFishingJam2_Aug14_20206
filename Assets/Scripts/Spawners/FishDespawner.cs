using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishDespawner : MonoBehaviour
{
    [SerializeField] private Transform _inactiveFishContainer;
    [SerializeField] private List<GameObject> _spawnedFish = new();
    [SerializeField] private List<GameObject> _despawnFish = new();

    [SerializeField] private Transform _waterEscapePoint;
    [SerializeField] private Transform _leftTargetingLimit;
    [SerializeField] private Transform _rightTargetingLimit;
    [SerializeField] private Transform _upperTargetingLimit;
    [SerializeField] private Transform _lowestTargetingLimit;


    [SerializeField] private List<GameObject> _inRangeFish = new();
    [SerializeField] private List<GameObject> _outOfRangeFish = new();

    IEnumerator _fishDespawner;

    [Header("UnityEvents")]
    public UnityEvent<GameObject> OnFishDespawned;
    public UnityEvent<GameObject> OnRangeEntered;
    public UnityEvent<GameObject> OnRangeExited;

    


    private void Update()
    {
        foreach (GameObject fish in _spawnedFish)
        {
            UpdateFishRange(fish);

            //remove the fish if it escapes into the river
            if (fish.transform.position.y <= _waterEscapePoint.position.y)
            {
                _despawnFish.Add(fish);
            }
        }

        DespawnFish();
    }

    private IEnumerator DespawnFishAtEOF()
    {
        yield return new WaitForEndOfFrame();

        for (int i = _despawnFish.Count -1; i >= 0; i--)
        {
            GameObject fish = _despawnFish[i];

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
            _spawnedFish.Remove(fish);
            _despawnFish.Remove(fish);

            OnFishDespawned?.Invoke(fish);
        }

        _fishDespawner = null;
    }

    private void DespawnFish()
    {
        if (_fishDespawner == null)
        {
            _fishDespawner = DespawnFishAtEOF();
            
            StartCoroutine(DespawnFishAtEOF());
        }
    }


    private void UpdateFishRange(GameObject fish)
    {
        //update fish as out of range if it's out of range
        if (fish.transform.position.x < _leftTargetingLimit.position.x ||
            fish.transform.position.x > _rightTargetingLimit.position.x ||
            fish.transform.position.y > _upperTargetingLimit.position.y ||
            fish.transform.position.y < _lowestTargetingLimit.position.y)
        {
            
            if (_inRangeFish.Contains(fish))
            {
                _inRangeFish.Remove(fish);
                _outOfRangeFish.Add(fish);
                OnRangeExited?.Invoke(fish);
            }
        }

        //update fish as in range in re-entering range
        else
        {
            
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
 
    public void RespondToFishHit(GameObject fish)
    {
        if (!_despawnFish.Contains(fish))
            _despawnFish.Add(fish);

        DespawnFish();
    }

}
