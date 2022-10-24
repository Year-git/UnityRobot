using UnityEngine;
using System;
using System.Collections.Generic;

public class RobotPartScriptMoveGyrostat : RobotPartScriptMove
{
    [SerializeField] float _moveForce = 1000; // 移动推力
    [SerializeField] float _straightForce = 1000; // 辅助直力
    [SerializeField] float _limitAngle = 10; // 陀螺极限角度
    [SerializeField] float _steerSpeed = 0.1f; // 旋转速率
    [SerializeField] float _moveDetection = 3f; // 离地检测距离
    [SerializeField] GameObject _movePos; // 移动推点
    [SerializeField] GameObject _centerOfMass;
    [SerializeField] GameObject _body; // 机体
    [SerializeField] GameObject[] _effects; // 特效组
    private Rigidbody _npcRigidbody;
    private HingeJoint _hingeJoint;
    private Vector3 _forward;
    private Ray _ray;
    private RaycastHit _raycastHit;

    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {

        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = npc.gameObject.GetComponent<Rigidbody>();

        _hingeJoint = _body.GetComponent<HingeJoint>();
        _forward = transform.forward;
        if (_npcRigidbody != null && _centerOfMass != null)
        {
            _npcRigidbody.centerOfMass = _centerOfMass.transform.localPosition;

            // 设置最大扭矩速度上限
            _npcRigidbody.maxAngularVelocity = 100000;
        }

        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {

    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        // 控制稳定性
        float x = ChangeAngle(transform.eulerAngles.x);
        float z = ChangeAngle(transform.eulerAngles.z);
        if (Mathf.Abs(x) > _limitAngle || Mathf.Abs(z) > _limitAngle)
        {
            x = Mathf.Pow((Mathf.Abs(x) - _limitAngle) / _straightForce, 2) * (x > 0 ? -1 : 1);
            z = Mathf.Pow((Mathf.Abs(z) - _limitAngle) / _straightForce, 2) * (z > 0 ? -1 : 1);
            _npcRigidbody.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
        }
        transform.Rotate(Vector3.up, 450 * Time.deltaTime, Space.Self);

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

        float angle = Vector3.Angle(_forward, _npcRigidbody.velocity);
        Vector3 c = Vector3.Cross(_forward, _npcRigidbody.velocity.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        Quaternion q = Quaternion.Euler(_body.transform.rotation.x, angle, _body.transform.rotation.z);
        // 朝向移动方向
        if (!_hingeJoint.useMotor && (new Vector2(_npcRigidbody.velocity.x, _npcRigidbody.velocity.z)).magnitude > 1 && Quaternion.Angle(_body.transform.rotation, q) > 0.1f)
        {
            _body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, q, _steerSpeed);
            if (Quaternion.Angle(_body.transform.rotation, q) < 1)
            {
                _body.transform.rotation = q;
            }
        }
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
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

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }

    public override void StartSteerAngle(int angle)
    {
        base.StartSteerAngle(angle);
        // 浮空检测
        _ray.origin = _centerOfMass.transform.position;
        _ray.direction = -_centerOfMass.transform.up;
        if (Physics.Raycast(_ray, out _raycastHit, _moveDetection))
        {
            Vector3 v1 = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
            Vector3 addForcePos = _movePos.transform.position;
            _npcRigidbody.AddForceAtPosition(v1 * _moveForce, addForcePos, ForceMode.Force);
            // Debug.DrawLine(_centerOfMass.transform.position, transform.TransformPoint(-_centerOfMass.transform.up * 1), Color.yellow, 0.1f);
        }
    }
}
