using UnityEngine;
using System;
using System.Collections.Generic;
//磁悬浮移动部件
public class RobotPartScriptMoveMaglev : RobotPartScriptMove
{
    [Header("磁悬浮参数")]
    [SerializeField] [HeaderAttribute("最高悬浮高度")] float _maxHeight = 5;
    [SerializeField] [HeaderAttribute("从中心点往左右偏移距离")] float _posx = 2;
    [SerializeField] [HeaderAttribute("从中心点往前后偏移距离")] float _posz = 2;
    [SerializeField] [HeaderAttribute("浮空系数")] float _coefficient = 80;
    [SerializeField] [HeaderAttribute("浮空曲线")] float _pow = 2;
    [SerializeField] [HeaderAttribute("空气阻力")] float _dragFalg = 0.25f;
    [SerializeField] [HeaderAttribute("速度系数")] float _speed = 10f;
    [SerializeField] [HeaderAttribute("旋转扭矩力系数")] float _angleForce = 0.1f;
    [SerializeField] [HeaderAttribute("旋转扭矩力上限")] float _limitForce = 200;
    [SerializeField] [HeaderAttribute("检测偏移")] float _detectionOffset = 0.6f;
    [SerializeField] [HeaderAttribute("喷气零件")] GameObject[] _maglevs;
    [SerializeField] [HeaderAttribute("喷气特效")] GameObject[] _effects;

    private Rigidbody _npcRigidbody; // 机体刚体
    public float _rigidbodySpeed; // 机体速度
    private List<Vector3> _pos = new List<Vector3>(); // 记录辅助位置
    private BaseNpc _myNpc; // 当前NPC
    // private PlaySoundScript[] _playSoundScriptList;
    // 当前目标朝向
    private float _lookAngle = 0;
    protected override void BaseEvent_OnStart()
    {
        // _playSoundScriptList = GetComponents<PlaySoundScript>();
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _myNpc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();
        _npcRigidbody.angularDrag = 15;
        //_npcRigidbody.drag = 1;
        _npcRigidbody.maxAngularVelocity = 100000;
        _pos.Add(_myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(_myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(_myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(_myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));

        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        Vector3 velocitys = _myNpc.gameObject.transform.InverseTransformDirection(_npcRigidbody.velocity);
        _rigidbodySpeed = Mathf.Abs(_npcRigidbody.velocity.magnitude * 3.6f);
        _rigidbodySpeed = _rigidbodySpeed < 0 ? 0 : _rigidbodySpeed;
        // Vector3 dis = new Vector3(velocitys.z, 0, -velocitys.x);
        // foreach (GameObject item in _maglevs)
        // {
        //     //_myNpc.gameObject.transform.forward;
        //     // if(a < 30){
        //     // Quaternion next = item.transform.localRotation;
        //     // item.transform.Rotate(dis.normalized * 3);
        //     // float a = Vector3.Angle(item.transform.up, Vector3.up);
        //     // if (a > 30)
        //     // {
        //     //     item.transform.localRotation = next;
        //     // }
        //     float x = Mathf.Min(Mathf.Abs(velocitys.x), 1) * velocitys.x > 0 ? 1 : -1;
        //     float z = Mathf.Min(Mathf.Abs(velocitys.z), 1) * velocitys.z > 0 ? 1 : -1;
        //     Vector3 lookPos = new Vector3(x, item.transform.position.y - 3, z);
        //     item.transform.LookAt(lookPos);
        //     // }else{
        //     // }
        // }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
        // if (myElement.isDead) return;
        _pos[0] = _myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y + _detectionOffset, _npcRigidbody.centerOfMass.z + _posz));
        _pos[1] = _myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x - _posx, _npcRigidbody.centerOfMass.y + _detectionOffset, _npcRigidbody.centerOfMass.z - _posz));
        _pos[2] = _myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y + _detectionOffset, _npcRigidbody.centerOfMass.z - _posz));
        _pos[3] = _myNpc.gameObject.transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x - _posx, _npcRigidbody.centerOfMass.y + _detectionOffset, _npcRigidbody.centerOfMass.z + _posz));

        // 帮助平稳
        float x = ChangeAngle(transform.eulerAngles.x);
        float z = ChangeAngle(transform.eulerAngles.z);
        if (Mathf.Abs(x) > 5 || Mathf.Abs(z) > 5)
        {
            x = Mathf.Pow((Mathf.Abs(x) - 5) / 5, 2) * (x > 0 ? -1 : 1);
            z = Mathf.Pow((Mathf.Abs(z) - 5) / 5, 2) * (z > 0 ? -1 : 1);
            _npcRigidbody.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
        }

        // 转向操作
        float angle = (float)_lookAngle;
        float myY = _myNpc.gameObject.transform.eulerAngles.y;
        float tagAngles = CalcRelativeAngle(myY, angle);
        float n = tagAngles > 0 ? 1 : -1;
        _npcRigidbody.AddRelativeTorque(0, Mathf.Min(Mathf.Pow(Mathf.Abs(tagAngles), 2) * _angleForce, _limitForce) * n, 0, ForceMode.Acceleration);
        
        // 浮空
        float g = Physics.gravity.y * -1;
        float G = _npcRigidbody.mass * g;
        float useForce = G / 4;
        _npcRigidbody.AddForce(new Vector3(-_npcRigidbody.velocity.x * _dragFalg, -_npcRigidbody.velocity.y * _dragFalg, -_npcRigidbody.velocity.z * _dragFalg), ForceMode.Acceleration);
        foreach (Vector3 p in _pos)
        {
            Ray _ray = new Ray();
            RaycastHit _raycastHit;
            // 浮空检测
            _ray.origin = p;
            _ray.direction = -Vector3.up;
            int layersMask = 0;
            layersMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Obstacle"));
            if (Physics.Raycast(_ray, out _raycastHit, 100, layersMask))
            {
                float space = Mathf.Max(_maxHeight - _raycastHit.distance, -3);
                float addCoe = Mathf.Pow(Mathf.Abs(space) * _coefficient, _pow) * (space >= 0 ? 1 : -1);
                float addForce = useForce + addCoe;
                Vector3 end = new Vector3(p.x, p.y + -_maxHeight, p.z);
                Debug.DrawLine(p, end, Color.yellow, 0.1f);
                _npcRigidbody.AddForceAtPosition(Vector3.up * addForce, p, ForceMode.Force);
            }
        }
    }

    protected override void BaseEvent_OnNpcDead()
    {
        foreach (GameObject item in _effects)
        {
            item.SetActive(false);
        }
    }
    public override void StartSteerAngle(int angle)
    {
        base.StartSteerAngle(angle);
        Vector3 cameraForward = Quaternion.Euler(0, (float)_lookAngle, 0) * Vector3.forward;

        if((_myNpc.myBehaviacController.aiName == null || _myNpc.myBehaviacController.aiName == "") && _myNpc.type == NpcType.PlayerNpc){
            cameraForward = Vector3.forward;
        }
        // Vector3 cameraForward = _myNpc.type == NpcType.PlayerNpc ? Vector3.forward : Quaternion.Euler(0, (float)_lookAngle, 0) * Vector3.forward;
        Vector3 dis = Quaternion.Euler(0, angle, 0) * cameraForward;

        float speedValue = _myNpc.GetNpcAttr(AttributeType.Speed);
        _npcRigidbody.AddForce(dis.normalized * speedValue / 3.6f * _speedValue * _speed, ForceMode.Acceleration);
    }

    public override void LookAtAngle(float lookAngle)
    {
        _lookAngle = lookAngle;
    }

}
