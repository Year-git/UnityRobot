using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeseNiuJiJia : MonoBehaviour
{
    public float _moveForce;
    public float _upForce;
    public float _maxSpeed = 20;
    public Animation _animation;
    public Rigidbody _rigidbody;
    void Start()
    {
    }

    void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude > 2)
        {
            _animation.Play("Body_niujijia_walk");

            // 转向操作
            Vector3 _lookTargetPoint = gameObject.transform.position + (_rigidbody.velocity * 100);
            Vector3 dis = _lookTargetPoint - gameObject.transform.position;
            float angle = Vector3.Angle(Vector3.forward, dis);
            Vector3 c = Vector3.Cross(Vector3.forward, dis.normalized);
            if (c.y < 0)
            {
                angle = 360 - angle;
            }
            float myY = gameObject.transform.eulerAngles.y;
            float tagAngles = angle - myY;
            if (Mathf.Abs(tagAngles) > 180)
            {
                tagAngles = (tagAngles > 0) ? (tagAngles - 360) : (tagAngles + 360);
            }
            float n = tagAngles > 0 ? 1 : -1;
            _rigidbody.maxAngularVelocity = 10;
            _rigidbody.AddRelativeTorque(0, Mathf.Pow(Mathf.Abs(tagAngles), 2) * 10 * n, 0, ForceMode.Acceleration);
            if (tagAngles < 0.1f)
            {
                _rigidbody.angularVelocity = new Vector3(_rigidbody.angularVelocity.x, 0, _rigidbody.angularVelocity.z);
            }
        }
        else
        {
            _animation.Play("Take 001");
        }


        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(Vector3.forward * _moveForce, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(Vector3.forward * -_moveForce, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddForce(Vector3.right * -_moveForce, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddForce(Vector3.right * _moveForce, ForceMode.Acceleration);
        }

        // 限制最大速度
        if (_rigidbody.velocity.magnitude > _maxSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
        }
    }
}
