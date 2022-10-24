using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyrostatMove : MonoBehaviour
{
    [SerializeField] float _moveForce = 1000; // 移动推力
    [SerializeField] float _straightForce = 1000; // 辅助直力
    [SerializeField] float _limitAngle = 10; // 陀螺极限角度
    [SerializeField] float _steerSpeed = 0.1f; // 旋转速率
    [SerializeField] float _moveDetection = 3f; // 离地检测距离
    [SerializeField] GameObject _movePos; // 移动推点
    [SerializeField] GameObject _centerOfMass;
    [SerializeField] GameObject _chassis; // 陀螺脚尖
    [SerializeField] GameObject _body; // 机体
    [SerializeField] GameObject[] _effects; // 特效组
    private Rigidbody _rb;
    private HingeJoint _hingeJoint;
    private Vector3 _forward;
    private Ray _ray;
    private RaycastHit _raycastHit;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _hingeJoint = _body.GetComponent<HingeJoint>();
        _forward = transform.forward;
        if (_rb != null && _centerOfMass != null)
        {
            _rb.centerOfMass = _centerOfMass.transform.localPosition;
        }

        // 设置最大扭矩速度上限
        _rb.maxAngularVelocity = 100000;
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

        // 控制稳定性
        float x = ChangeAngle(transform.eulerAngles.x);
        float z = ChangeAngle(transform.eulerAngles.z);
        if (Mathf.Abs(x) > _limitAngle || Mathf.Abs(z) > _limitAngle)
        {
            x = Mathf.Pow((Mathf.Abs(x) - _limitAngle) / _straightForce, 2) * (x > 0 ? -1 : 1);
            z = Mathf.Pow((Mathf.Abs(z) - _limitAngle) / _straightForce, 2) * (z > 0 ? -1 : 1);
            _rb.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
        }
        if (_chassis)
        {
            _chassis.transform.Rotate(Vector3.up, 450 * Time.deltaTime, Space.Self);
        }

        // 升天
        if (Input.GetKey(KeyCode.R))
        {
            _rb.AddForce(Vector3.up * _moveForce, ForceMode.Force);
        }

        // 机体旋转
        if (Input.GetKey(KeyCode.F))
        {
            _hingeJoint.useMotor = true;
            for (int i = 0; i < _effects.Length; i++)
            {
                if (!_effects[i].activeSelf)
                {
                    _effects[i].SetActive(true);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            _hingeJoint.useMotor = false;
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].SetActive(false);
            }
        }

        float angle = Vector3.Angle(_forward, _rb.velocity);
        Vector3 c = Vector3.Cross(_forward, _rb.velocity.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        Quaternion q = Quaternion.Euler(_body.transform.rotation.x, angle, _body.transform.rotation.z);
        // 朝向移动方向
        if (!_hingeJoint.useMotor && (new Vector2(_rb.velocity.x, _rb.velocity.z)).magnitude > 1 && Quaternion.Angle(_body.transform.rotation, q) > 0.1f)
        {
            _body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, q, _steerSpeed);
            if (Quaternion.Angle(_body.transform.rotation, q) < 1)
            {
                _body.transform.rotation = q;
            }
        }

        // 浮空检测
        _ray.origin = _centerOfMass.transform.position;
        _ray.direction = -_centerOfMass.transform.up;
        if (Physics.Raycast(_ray, out _raycastHit, _moveDetection))
        {

            Vector3 addForcePos = _movePos.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                _rb.AddForceAtPosition(Vector3.forward * _moveForce, addForcePos, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _rb.AddForceAtPosition(Vector3.forward * -_moveForce, addForcePos, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _rb.AddForceAtPosition(Vector3.right * -_moveForce, addForcePos, ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _rb.AddForceAtPosition(Vector3.right * _moveForce, addForcePos, ForceMode.Force);
            }
            Debug.DrawLine(_centerOfMass.transform.position, transform.TransformPoint(-_centerOfMass.transform.up * 1), Color.yellow, 0.1f);
        }
    }
    // 碰撞开始
    void OnCollisionEnter(Collision collision)
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
        // Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);
        Rigidbody rigidbody = collision.rigidbody;
    }
}
