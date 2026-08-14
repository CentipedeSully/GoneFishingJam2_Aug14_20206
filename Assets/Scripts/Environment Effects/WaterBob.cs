using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBob : MonoBehaviour
{
    [SerializeField] private List<Transform> _BobTargets = new List<Transform>();
    [SerializeField] private Transform _peakHeight;
    [SerializeField] private Transform _valleyHeight;
    [SerializeField] private float _bobTime;
    [Space(20)]
    [SerializeField] private Transform _startHeight;
    [SerializeField] private Transform _endHeight;
    private float _currentIterationTime = 0;
    private bool _reversed = false;

    private Vector3 _cachedPositionStart;
    private Vector3 _cachedPositionEnd;



    private void Update()
    {
        BobTargets();
    }



    private void BobTargets()
    {
        foreach (Transform target in _BobTargets)
            LerpBobPosition(target);
    }

    private void LerpBobPosition(Transform target)
    {
        _currentIterationTime += Time.deltaTime;
        
        
        if (!_reversed)
        {
            _cachedPositionStart = new Vector3(target.position.x, _startHeight.position.y, target.position.z);
            _cachedPositionEnd = new Vector3(target.position.x, _endHeight.position.y, target.position.z);
        }
        else
        {
            _cachedPositionStart = new Vector3(target.position.x, _endHeight.position.y, target.position.z);
            _cachedPositionEnd = new Vector3(target.position.x, _startHeight.position.y, target.position.z);
        }

        target.position = Vector3.Lerp(_cachedPositionStart, _cachedPositionEnd, _currentIterationTime / _bobTime);

        if (_currentIterationTime >= _bobTime)
        {
            _currentIterationTime = 0;
            _reversed = !_reversed;
        }

    }
}
