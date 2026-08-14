using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public enum FishType
{
    Unset,
    Salmon,
    Swordfish,
    Wyvern,

}
public class FishBehaviour : MonoBehaviour
{
    [SerializeField] public FishType _fishtype = FishType.Salmon;
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _gravity = 1f;
    [SerializeField] private float _terminalDrop = 5;
    [SerializeField] private float _minRotationRate = -5;
    [SerializeField] private float _maxRotationRate = 5;
    private float _rotationRate;
    private Vector3 _rotation;


    private void Awake()
    {
        _rotationRate = Random.Range(_minRotationRate,_maxRotationRate);
    }

    private void Update()
    {
        ApplyGravity();
        Move();
        Rotate();
    }



    private void ApplyGravity()
    {
        if (_speed > -Mathf.Abs(_terminalDrop))
            _speed -= _gravity * Time.deltaTime;
    }
    private void Move()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + _speed * Time.deltaTime, transform.position.z);
    }

    private void Rotate()
    {
        _rotation = transform.rotation.eulerAngles + new Vector3(0,0,_rotationRate) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_rotation);
    }


    public void SetSpeed(float speed)
    {
        _speed = speed;
    }




}
