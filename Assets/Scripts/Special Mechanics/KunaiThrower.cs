using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KunaiThrower : MonoBehaviour
{
    [SerializeField] private BulletTime _bulletTimeController;
    [SerializeField] private KunaiManager _kunaiManager;
    [SerializeField] private GameObject _throwPrefab;
    [SerializeField] private GameObject _hitPrefab;
    private Dictionary<GameObject, bool> _throwResults = new();
    private Dictionary<GameObject, GameObject> _fishWithIcons = new();
    private List<GameObject> _currentlyMarkedFish = new();

    [Header("UnityEvents")]
    public UnityEvent<GameObject> OnFishMarked;
    public UnityEvent<GameObject> OnFishLeftRange;
    public UnityEvent<GameObject> OnFishHit;
    public UnityEvent<GameObject> OnFishMissed;



    //monobehaviours





    //internals
    


    //externals
    public void RespondToKunaiThrown(bool hitStatus,GameObject fish)
    {
        _throwResults.Add(fish, hitStatus);
        GameObject icon = Instantiate(_throwPrefab);
        icon.GetComponent<StickToGameObject>().SetStickyTarget(fish);
        _fishWithIcons.Add(fish, icon);
        _currentlyMarkedFish.Add(fish);
        OnFishMarked?.Invoke(fish);
    }

    public void RespondToBulletTimeEnd()
    {
        foreach(KeyValuePair<GameObject,bool> entry in _throwResults)
        {
            _kunaiManager.DecrementKunai();

            if (entry.Value)
            {
                //spawn a hit effect at the fish's position
                GameObject newHiteEffect = Instantiate(_hitPrefab, entry.Key.transform.position, Quaternion.identity);

                //make sure the sprite of the catch matches the hit effect
                Sprite sprite = entry.Key.GetComponentInChildren<SpriteRenderer>().sprite;
                newHiteEffect.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

                //activate the hit effect, too
                newHiteEffect.GetComponent<HitShake>().TriggerShake();

                OnFishHit?.Invoke(entry.Key);
            }
            else OnFishMissed?.Invoke(entry.Key);
        }

        foreach(KeyValuePair<GameObject, GameObject> entry in _fishWithIcons)
        {
            Destroy(entry.Value);
        }


        

        _throwResults.Clear();
        _fishWithIcons.Clear();
        _currentlyMarkedFish.Clear();
    }
    
    public void RespondToFishOutOfRange(GameObject fish)
    {
        if (_throwResults.ContainsKey(fish))
        {
            _throwResults.Remove(fish);

            //destroy the icon marking the now-escaped fish
            Destroy(_fishWithIcons[fish]);
            _fishWithIcons.Remove(fish);
            
            _currentlyMarkedFish.Remove(fish);

            OnFishLeftRange?.Invoke(fish);
        }
    }

    public List<GameObject> GetMarkedFish() { return _currentlyMarkedFish; }

    public void RespondToAllKunaiCommitted()
    {
        _bulletTimeController.ExitBulletTime();
    }







}
