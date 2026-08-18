using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float _delay = 2f;
    [SerializeField] private bool _triggerTimerOnStart = false;

    private void Start()
    {
        if (_triggerTimerOnStart)
            Invoke(nameof(DestroySelf), _delay);
    }





    private void DestroySelf()
    {
        Destroy(gameObject);
    }




    public void TriggerDelay()
    {
        CancelInvoke();
        Invoke(nameof(DestroySelf), _delay);
    }
}
