using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct FishSpeeds
{
    public FishType _type;
    public float _minSpeed;
    public float _maxSpeed;

}

public class FishThrower : MonoBehaviour
{
    [SerializeField] private List<FishSpeeds> _speedSettings = new();
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();
    [SerializeField] private Transform _spawnPointsContainer;
    private List<Transform> _spawnPoints = new();
    [SerializeField] private Transform _activeFishContainer;
    [SerializeField] private Transform _deactivatedFishContainer;

    

    [SerializeField] private float _spawnRateBase = 1f;
    [SerializeField] private float _spawnRateVariance = .5f;
    [SerializeField] private bool _spawnFish = false;
    [SerializeField] private Transform _riverStart;
    [SerializeField] private Transform _riverEnd;
    private float _nextSpawnThreshold;
    private float _currentTime;

    private Scroll _fishScroll;
    private Transform _selectedSpawnPoint;
    private FishBehaviour _spawnedFishBehaviour;
    private GameObject _spawnedFishObject;
    private float _randomizedSpeed;
    
    
    public UnityEvent<GameObject> OnFishSpawned;
    



    private void Awake()
    {
        for (int i = 0; i < _spawnPointsContainer.childCount; i++) 
            _spawnPoints.Add(_spawnPointsContainer.GetChild(i));
    }

    private void Update()
    {
        if (_spawnFish)
            TickTime();
    }




    private void TickTime()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= _nextSpawnThreshold)
        {
            _currentTime = _currentTime - _nextSpawnThreshold;
            _nextSpawnThreshold = UnityEngine.Random.Range(_spawnRateBase - _spawnRateVariance, _spawnRateBase + _spawnRateVariance);
            SpawnFish();
        }
    }

    private void SpawnFish()
    {
        if (_deactivatedFishContainer.childCount == 0)
            _spawnedFishObject = Instantiate(_prefabs[UnityEngine.Random.Range(0, _prefabs.Count)],_activeFishContainer);
        else
        {
            _spawnedFishObject = _deactivatedFishContainer.GetChild(0).gameObject;
            _spawnedFishObject.SetActive(true);
            _spawnedFishObject.transform.SetParent(_activeFishContainer);
            _spawnedFishObject.transform.rotation = Quaternion.identity;
        }

        //set the fish's spawn point
        _selectedSpawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
        _spawnedFishObject.transform.position = _selectedSpawnPoint.position;

        //make sure the fish is initialized properly
        _spawnedFishBehaviour = _spawnedFishObject.GetComponent<FishBehaviour>();

        

        //randomize the fish's horizontal speed
        _fishScroll = _spawnedFishObject.GetComponent<Scroll>();
        _fishScroll.SetRiverStartPoint(_riverStart);
        _fishScroll.SetRiverEndPoint(_riverEnd);
        

        //randomize the fish's vertical jump
        foreach (FishSpeeds speedSetting in _speedSettings)
        {
            if (speedSetting._type == _spawnedFishBehaviour._fishtype)
            {
                _randomizedSpeed = UnityEngine.Random.Range(speedSetting._minSpeed, speedSetting._maxSpeed);
                _fishScroll.SetSpeed(_randomizedSpeed);
                _spawnedFishBehaviour.SetSpeed(_randomizedSpeed);
            }
                
        }


        OnFishSpawned?.Invoke(_spawnedFishObject);
        

    }

    public void StartSpawningFish()
    {
        _spawnFish = true;
    }

    public void StopSpawningFish()
    {
        _spawnFish = false;
    }

    public void LogFishSpawn(GameObject fish)
    {
        Debug.Log($"Spawned a [{fish.GetComponent<FishBehaviour>()._fishtype}]!");
    }

}
