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
                AnimateKunaiRegen();
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
            AnimateKunaiAsThrown();

            _kunaiAvailable--;
            _regenReady = false;

            Invoke(nameof(ReadyRegen), _regenCooldown);
        }
        
    }

    public int KunaiCount() {  return _kunaiAvailable; }

    public void CommitKunai()
    {
        for (int i = _maxKunai - 1;i>=0; i--)
        {
            Animator kunaiAnimator = _kunaiContainer.GetChild(i).GetComponent<Animator>();
            if (!kunaiAnimator.GetBool("isCommitted"))
            {
                kunaiAnimator.SetBool("isCommitted", true);
                return;
            }
        }
    }
    public void UncommitKunai()
    {
        for (int i = _maxKunai - 1; i >= 0; i--)
        {
            Animator kunaiAnimator = _kunaiContainer.GetChild(i).GetComponent<Animator>();
            if (kunaiAnimator.GetBool("isCommitted"))
            {
                kunaiAnimator.SetBool("isCommitted", false);
                return;
            }
        }
    }
    public void UncommitAllKunai()
    {
        for (int i = _maxKunai - 1; i >= 0; i--)
        {
            Animator kunaiAnimator = _kunaiContainer.GetChild(i).GetComponent<Animator>();
            if (kunaiAnimator.GetBool("isCommitted"))
            {
                kunaiAnimator.SetBool("isCommitted", false);
            }
        }
    }

    public void AnimateKunaiAsThrown()
    {
        for (int i = _maxKunai - 1; i >= 0; i--)
        {
            Animator kunaiAnimator = _kunaiContainer.GetChild(i).GetComponent<Animator>();
            if (!kunaiAnimator.GetBool("isGone"))
            {
                
                kunaiAnimator.SetTrigger("onThrow");
                kunaiAnimator.SetBool("isGone", true);
                if (kunaiAnimator.GetBool("isCommitted"))
                    kunaiAnimator.SetBool("isCommitted", false);
                return;
            }
        }
    }

    public void AnimateKunaiRegen()
    {
        for (int i = _maxKunai - 1; i >= 0; i--)
        {
            Animator kunaiAnimator = _kunaiContainer.GetChild(i).GetComponent<Animator>();
            if (kunaiAnimator.GetBool("isGone"))
            {
                kunaiAnimator.SetBool("isGone", false);
                return;
            }
        }
    }


}
