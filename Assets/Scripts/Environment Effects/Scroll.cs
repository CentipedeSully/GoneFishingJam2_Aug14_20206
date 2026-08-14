using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Scroll : MonoBehaviour
{
    [SerializeField] private Transform _scrollTarget;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _speed;




    private void Awake()
    {
        _scrollTarget = transform;
    }

    private void Update()
    {
        ScrollTarget(_scrollTarget);

    }



    private void ScrollTarget(Transform target)
    {
        target.position = new Vector3(target.position.x + _speed * Time.deltaTime, target.position.y, target.position.z);

        if (target.position.x >= _endPoint.position.x)
            target.position = new Vector3(_startPoint.position.x, target.position.y, target.position.z);
    }



    public void SetRiverStartPoint(Transform start)
    {
        _startPoint = start;
    }
    public void SetRiverEndPoint(Transform end)
    {
        _endPoint = end;
    }
    public void SetSpeed(float speed) { _speed = speed; }
}
