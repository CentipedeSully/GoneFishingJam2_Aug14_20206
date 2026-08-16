using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToGameObject : MonoBehaviour
{
    private GameObject _target;



    private void Update()
    {
        if (_target != null)
            transform.position = _target.transform.position;
    }


    public void SetStickyTarget(GameObject newTarget)
    {
        _target = newTarget;
    }
}
