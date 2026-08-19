using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishBasket : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> _spriteAmountFeedbacks = new();
    [Header("Settings")]
    [SerializeField] private List<int> _spriteThresholds = new();
    private int _currentThreshold = 0;
    [SerializeField] private int _salmonCaught = 0;
    private Animator _animator;




    [Header("UnityEvents")]
    public UnityEvent OnSpriteChanged;



    private void Awake()
    { 
        for (int i = 0; i < _spriteAmountFeedbacks.Count; i++)
        {
            if (i == 0)
                _spriteAmountFeedbacks[i].SetActive(true);
            else _spriteAmountFeedbacks[i].SetActive(false);
        }

        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }




    public void AddSalmonToBasket(int amount)
    {
        _salmonCaught += amount;

        //end now if we've reached the last visual feedback sprite
        if (_currentThreshold >= _spriteThresholds.Count)
            return;

        while (_salmonCaught > _spriteThresholds[_currentThreshold] && _currentThreshold + 1 < _spriteThresholds.Count)
        {
            _spriteAmountFeedbacks[_currentThreshold].SetActive(false);
            _currentThreshold++;
            _spriteAmountFeedbacks[_currentThreshold].SetActive(true);
        }
    }
    public void AnimateFishAdded()
    {
        _animator.SetTrigger("OnFishAdded");
    }
    public int GetSalmonCaught() { return _salmonCaught; }
}
