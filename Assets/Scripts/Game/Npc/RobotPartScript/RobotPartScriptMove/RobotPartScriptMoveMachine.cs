using UnityEngine;
using System;
using System.Collections.Generic;
//磁悬浮移动部件
public class RobotPartScriptMoveMachine : RobotPartScriptMove
{
    [Header("磁悬浮参数")]
    [SerializeField] [HeaderAttribute("最高悬浮高度")] float _maxHeight = 5;
    [SerializeField] [HeaderAttribute("速度系数")] float _speed = 10f;
    [SerializeField] [HeaderAttribute("限制最大速度")] float _maxSpeed = 30;
    [SerializeField] [HeaderAttribute("旋转扭矩力系数")] float _angleForce = 0.1f;
    [SerializeField] [HeaderAttribute("旋转扭矩力上限")] float _limitForce = 200;
    [SerializeField] [HeaderAttribute("检测偏移")] float _detectionOffset = 0.6f;
    [SerializeField] [HeaderAttribute("喷气零件")] GameObject[] _maglevs;
    [SerializeField] [HeaderAttribute("喷气特效")] GameObject[] _effects;
    [SerializeField] [HeaderAttribute("动画")] Animation _animation;
    [SerializeField] [HeaderAttribute("移动平滑度")] float _lerp = 0.1f;
    [SerializeField] [HeaderAttribute("检测")] OnTrigger _onTrigger;
    [SerializeField] [HeaderAttribute("检测")] OnTrigger _onTriggerTurn;
    [SerializeField] [HeaderAttribute("翻身力度系数")] float _turnF = 10;


    private Rigidbody _npcRigidbody; // 机体刚体
    public float _rigidbodySpeed; // 机体速度
    public bool _bGround; // 碰撞到的地面
    private BaseNpc _myNpc; // 当前NPC
    private float _lookAngle = 0;// 当前目标朝向
    private float _turnTime = 0;// 翻身时间
    private bool _bturn = false;// 是否翻身


    private bool _bMove
    {
        get
        {
            return _onTrigger._bGround;
        }
    }

    protected override void BaseEvent_OnStart()
    {

    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _myNpc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();
        _npcRigidbody.angularDrag = 0.05f;
        _npcRigidbody.drag = 0.01f;
        _npcRigidbody.maxAngularVelocity = _limitForce;
        _animation["Test_qiujijia_run"].speed = 2;

        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        Vector3 velocitys = _myNpc.gameObject.transform.InverseTransformDirection(_npcRigidbody.velocity);
        _rigidbodySpeed = _npcRigidbody.velocity.magnitude;
        Vector3 pos = new Vector3(_myNpc.gameObject.transform.position.x, _myNpc.gameObject.transform.position.y + 0.5f, _myNpc.gameObject.transform.position.z);
        // Debug.DrawLine(pos, new Vector3(pos.x, pos.y - 3, pos.z));
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
        // if (myElement.isDead) return;
        float jia = Vector3.Angle(Vector3.up, _myNpc.gameObject.transform.up);
        
        _myNpc._bAttrack = _bMove;
        Debug.DrawLine(_myNpc.gameObject.transform.position, _myNpc.gameObject.transform.forward * 1000);
        if (_bMove)
        {
            // 转向操作
            float angle = _lookAngle;
            float myY = _myNpc.gameObject.transform.eulerAngles.y;
            float tagAngles = CalcRelativeAngle(myY, angle);

            float n = tagAngles > 0 ? 1 : -1;
            _npcRigidbody.maxAngularVelocity = _limitForce;
            _npcRigidbody.AddRelativeTorque(0, Mathf.Pow(Mathf.Abs(tagAngles), 2) * _angleForce * n, 0, ForceMode.Acceleration);
            // _npcRigidbody.AddRelativeTorque(0, Mathf.Abs(tagAngles) * _angleForce * n, 0, ForceMode.Acceleration);
            if (tagAngles < 0.1f)
            {
                _npcRigidbody.angularVelocity = new Vector3(_npcRigidbody.angularVelocity.x, 0, _npcRigidbody.angularVelocity.z);
            }
        }

        if (_rigidbodySpeed > 2 || _npcRigidbody.angularVelocity.magnitude > 1)
        {
            PlayAnimation("Test_qiujijia_run");

            if (_bMove && jia < 3)
            {
                float a = Vector3.Angle(_myNpc.gameObject.transform.forward, _npcRigidbody.velocity);
                Vector3 c = Vector3.Cross(_myNpc.gameObject.transform.forward, _npcRigidbody.velocity.normalized);
                if (c.y < 0)
                {
                    a = 360 - a;
                }
                Quaternion q = Quaternion.Euler(transform.localRotation.x, a, transform.localRotation.z);
                // 朝向移动方向
                if ((new Vector2(_npcRigidbody.velocity.x, _npcRigidbody.velocity.z)).magnitude > 1 && Quaternion.Angle(transform.localRotation, q) > 0.1f)
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, q, 0.2f);
                    if (Quaternion.Angle(transform.localRotation, q) < 1)
                    {
                        transform.localRotation = q;
                    }
                }
            };
        }
        else
        {
            PlayAnimation("Test_qiujijia_idle");
        }

        //限制最大速度
        if (_npcRigidbody.velocity.magnitude > _maxSpeed)
        {
            _npcRigidbody.velocity = _npcRigidbody.velocity.normalized * _maxSpeed;
        }

        //翻身
        _turnTime = _onTriggerTurn._bGround ? _turnTime + Time.fixedDeltaTime : 0;

        if (_turnTime > 2)
        {
            // 帮助平稳
            float x = ChangeAngle(_myNpc.gameObject.transform.eulerAngles.x);
            float z = ChangeAngle(_myNpc.gameObject.transform.eulerAngles.z);
            x = Mathf.Pow((Mathf.Abs(x) - 5) / _turnF, 2) * (x > 0 ? -1 : 1);
            z = Mathf.Pow((Mathf.Abs(z) - 5) / _turnF, 2) * (z > 0 ? -1 : 1);
            // x = Mathf.Abs(x) / _turnF * (x > 0 ? -1 : 1);
            // z = Mathf.Abs(z) / _turnF * (z > 0 ? -1 : 1);
            _npcRigidbody.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
            Debug.Log("x == " + x + "    z == " + z);
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
        if (!_bMove) return;
        base.StartSteerAngle(angle);
        Vector3 cameraForward = Quaternion.Euler(0, (float)_lookAngle, 0) * Vector3.forward;

        if ((_myNpc.myBehaviacController.aiName == null || _myNpc.myBehaviacController.aiName == "") && _myNpc.type == NpcType.PlayerNpc)
        {
            cameraForward = Vector3.forward;
        }
        Vector3 dis = Quaternion.Euler(0, angle, 0) * cameraForward;

        float speedValue = _myNpc.GetNpcAttr(AttributeType.Speed) / 3.6f * _speedValue * _speed;
        _npcRigidbody.AddForce(dis.normalized * speedValue, ForceMode.Acceleration);
    }

    public override void LookAtAngle(float lookAngle)
    {
        _lookAngle = lookAngle;
    }
    private void PlayAnimation(string name)
    {
        if (!_animation.IsPlaying(name))
        {
            _animation.Play(name);
        }
    }
}
