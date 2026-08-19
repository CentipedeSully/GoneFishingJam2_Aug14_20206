using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackFishSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _fishFeedbackprefab;
    [SerializeField] private RectTransform _bucketDropPosition;
    [SerializeField] private FishBasket _basket;
    [SerializeField] private float _multiSpawnDelay = .15f;
    [SerializeField] bool _cmdAddFishToBasket = false;
    [SerializeField] private int _amount = 3;
    private GameObject _fish;
    private FeedbackFishEffect _feedbackEffect;
    [SerializeField] private RectTransform _doubleFishSpawn1;
    [SerializeField] private RectTransform _doubleFishSpawn2;
    [SerializeField] private RectTransform _TripleFishSpawn1;
    [SerializeField] private RectTransform _TripleFishSpawn2;
    [SerializeField] private RectTransform _TripleFishSpawn3;



    public UnityEvent OnFishEnteredBasket;


    private void Update()
    {
        ListenToDebugCommands();
    }









    private void SpawnFeedbackFish(RectTransform spawnPoint)
    {
        _fish = Instantiate(_fishFeedbackprefab, spawnPoint,false);
        _feedbackEffect = _fish.GetComponent<FeedbackFishEffect>();
        _feedbackEffect.SetBucketDropPosition(_bucketDropPosition);
        _feedbackEffect.SetFeedbackSpawner(this);
    }


    private IEnumerator SpawnFishOverTime(int amount)
    {
        switch (amount)
        {
            case 1:
                SpawnFeedbackFish(GetComponent<RectTransform>());
                yield return null;
                break;

            case 2:
                SpawnFeedbackFish(_doubleFishSpawn1);
                yield return new WaitForSecondsRealtime(_multiSpawnDelay);
                SpawnFeedbackFish(_doubleFishSpawn2);
                break;

            case 3:
                SpawnFeedbackFish(_TripleFishSpawn1);
                yield return new WaitForSecondsRealtime(_multiSpawnDelay);
                SpawnFeedbackFish(_TripleFishSpawn2);
                yield return new WaitForSecondsRealtime(_multiSpawnDelay);
                SpawnFeedbackFish(_TripleFishSpawn3);
                break;

            default:
                int amountSpawned = 0;
                while (amountSpawned < amount)
                {
                    SpawnFeedbackFish(GetComponent<RectTransform>());
                    amountSpawned++;
                    yield return new WaitForSecondsRealtime(_multiSpawnDelay);
                }
                break;

        }
    }

    //externals
    public void AddFishToBasket(int amount)
    {
        StartCoroutine(SpawnFishOverTime(amount));
    }

    public void PerformBucketReaction()
    {
        OnFishEnteredBasket?.Invoke();
        
    }



    //debug
    private void ListenToDebugCommands()
    {
        if (_cmdAddFishToBasket)
        {
            _cmdAddFishToBasket = false;
            AddFishToBasket(_amount);
        }
    }
}
