using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float _moveForce;
    public float _speed;
    void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

        
        
    }

    void FixedUpdate(){
        _rigidbody.AddForce(Vector3.forward * _moveForce, ForceMode.Force);

        if(_rigidbody.velocity.magnitude > 3){
            _rigidbody.velocity = _rigidbody.velocity.normalized * 3;
        }

        
        _speed = _rigidbody.velocity.magnitude;
    }
}
