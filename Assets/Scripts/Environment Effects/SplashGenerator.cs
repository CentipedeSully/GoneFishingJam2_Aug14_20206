using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SplashGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _splashPrefab;
    [SerializeField] private Vector3 _downSplashOffset;

    public UnityEvent OnUpSplash;
    public UnityEvent OnDownSplash;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Splash detected");
        
        FishBehaviour fishBehavior = collision.GetComponent<FishBehaviour>();

        if (fishBehavior != null)
        {
            if (fishBehavior.GetSpeed() > .1f)
            {
                Instantiate(_splashPrefab, collision.transform.position, Quaternion.identity);
                OnUpSplash?.Invoke();
            }
            else
            {
                Instantiate(_splashPrefab, collision.transform.position + _downSplashOffset, Quaternion.identity);
                OnDownSplash?.Invoke();
            }
        }
    }


}
