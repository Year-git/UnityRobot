using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySaw : MonoBehaviour
{
    public ParticleSystem _ef;

    public bool _curDuration = false;
    public Rigidbody _rb;
    public float _maxHeight = 10; // 最高悬浮高度
    public float _posx = 3;
    public float _posz = 6;
    private float _curHeight = 0; // 当前悬浮高度
    public float coefficient = 1; // 浮空系数
    public float Pow = 1;// 浮空曲线
    public float _moveForce = 10000;
    public float _moveVelocity = 2;
    public float _forqueForce = 10000;

    public float dragFalg = 0.25f;
    private List<Vector3> _pos = new List<Vector3>();
    void Start()
    {
        _rb.maxAngularVelocity = 100000;
        _rb.AddForce(transform.forward * _moveVelocity,ForceMode.VelocityChange);
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
    }

    void Update()
    {

    }
/// <summary>
    /// 将角度 0~360 转换为 -180~180
    /// </summary>
    public float ChangeAngle(float oldAngle)
    {
        return (oldAngle - 180) > 0 ? (oldAngle - 360) : oldAngle;
    }
    void FixedUpdate()
    {
        _rb.AddTorque(0,_forqueForce,0);
        _rb.AddForce(transform.forward * _moveForce,ForceMode.Force);
        _pos[0] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));
        _pos[1] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
        _pos[2] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
        _pos[3] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));

        if (_curDuration)
        {
            float x = ChangeAngle(transform.eulerAngles.x);
            float z = ChangeAngle(transform.eulerAngles.z);
            if (Mathf.Abs(x) > 5 || Mathf.Abs(z) > 5)
            {
                x = Mathf.Pow((Mathf.Abs(x) - 5) / 5, 2) * (x > 0 ? -1 : 1);
                z = Mathf.Pow((Mathf.Abs(z) - 5) / 5, 2) * (z > 0 ? -1 : 1);
                _rb.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
            }

            float angle = Vector3.Angle(Vector3.forward, _rb.velocity);
            Vector3 c = Vector3.Cross(Vector3.forward, _rb.velocity.normalized);
            if (c.y < 0)
            {
                angle = 360 - angle;
            }
            Quaternion q = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
            if ((new Vector2(_rb.velocity.x, _rb.velocity.z)).magnitude > 1 && Quaternion.Angle(transform.rotation, q) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.1f);
                if (Quaternion.Angle(transform.rotation, q) < 1)
                {
                    transform.rotation = q;
                }
            }

            float g = Physics.gravity.y * -1;
            float G = _rb.mass * g;
            float useForce = G / 4;
            float mySpace = _curHeight - transform.TransformPoint(_rb.centerOfMass).y;
            _rb.AddForce(new Vector3(0, -_rb.velocity.y * dragFalg, 0), ForceMode.Acceleration);
            foreach (Vector3 p in _pos)
            {
                Ray _ray = new Ray();
                RaycastHit _raycastHit;
                // 浮空检测
                _ray.origin = p;
                _ray.direction = -Vector3.up;
                if (Physics.Raycast(_ray, out _raycastHit, 100))
                {
                    float space = _maxHeight - _raycastHit.distance;
                    float addCoe = Mathf.Pow(Mathf.Abs(space) * coefficient, Pow) * (space >= 0 ? 1 : -1);
                    float addForce = useForce + addCoe;
                    Vector3 end = new Vector3(p.x, p.y + -_maxHeight, p.z);
                    Debug.DrawLine(p, end, Color.yellow, 0.1f);
                    _rb.AddForceAtPosition(Vector3.up * addForce, p, ForceMode.Force);
                }
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                _rb.AddForce(Vector3.forward * _moveForce, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _rb.AddForce(Vector3.forward * -_moveForce, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _rb.AddForce(Vector3.right * -_moveForce, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _rb.AddForce(Vector3.right * _moveForce, ForceMode.Force);
            }
        }
        else
        {
            _curHeight = transform.TransformPoint(_rb.centerOfMass).y + _maxHeight;
        }
    }
    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        List<ContactPoint> Contacts = new List<ContactPoint>();
        int ContactCount = collision.GetContacts(Contacts);
        foreach (ContactPoint item in Contacts)
        {
            point += item.point;
            normal += item.normal;
        }
        point /= ContactCount;
        normal /= ContactCount;
        _ef.Stop();
        _ef.transform.position = point;
        _ef.Play();
        //Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {

        }
    }
    
}
