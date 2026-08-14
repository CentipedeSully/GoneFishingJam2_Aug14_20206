using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishDespawner : MonoBehaviour
{
    [SerializeField] private Transform _inactiveFishContainer;
    [SerializeField] private List<GameObject> _spawnedFish = new();
    [SerializeField] private Transform _waterEscapePoint;
    public UnityEvent<GameObject> OnFishDespawned;


    private void Update()
    {
        foreach (GameObject fish in _spawnedFish)
        {
            if (fish.transform.position.y <= _waterEscapePoint.position.y)
            {
                fish.SetActive(false);
                fish.transform.SetParent(_inactiveFishContainer);
                OnFishDespawned?.Invoke(fish);
            }
        }
    }


    public void TrackActiveFishPosition(GameObject fish)
    {
        _spawnedFish.Add(fish);
    }
}
