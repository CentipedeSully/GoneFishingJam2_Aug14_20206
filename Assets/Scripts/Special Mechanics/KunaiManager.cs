using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KunaiManager : MonoBehaviour
{
    [SerializeField] private int _maxKunai = 3;
    [SerializeField] private int _kunaiAvailable = 3;
    [SerializeField] private GameObject _kunaiGraphic;
    [SerializeField] private RectTransform _kunaiContainer;
    [SerializeField] private float _kunaiRegenTime = .75f;
    [SerializeField] private float _regenCooldown = 2f;
    [SerializeField] private float _currentKunaiRegen = 0;
    private int _latestKunai = 2;
    private bool _regenReady = true;




    private void Update()
    {
        if (_regenReady)
            TickKunaiRegen();
    }


    private void TickKunaiRegen()
    {
        if (_kunaiAvailable < _maxKunai)
        {
            _currentKunaiRegen += Time.deltaTime;
            if (_currentKunaiRegen >= _kunaiRegenTime)
            {
                _currentKunaiRegen = 0;
                _kunaiAvailable++;
                _latestKunai++;
                _kunaiContainer.GetChild(_latestKunai).gameObject.SetActive(true);
            }
        }
    }

    private void ReadyRegen()
    {
        _regenReady = true;
        _currentKunaiRegen = 0;
    }

    public void DecrementKunai()
    {
        if (_kunaiAvailable > 0)
        {
            CancelInvoke();
            _kunaiContainer.GetChild(_latestKunai).gameObject.SetActive(false);
            

            _kunaiAvailable--;
            _latestKunai--;
            _regenReady = false;

            Invoke(nameof(ReadyRegen), _regenCooldown);
        }
        
    }

    public int KunaiCount() {  return _kunaiAvailable; }


}
