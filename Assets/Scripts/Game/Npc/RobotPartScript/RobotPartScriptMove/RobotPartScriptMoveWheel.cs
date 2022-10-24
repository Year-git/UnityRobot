using UnityEngine;
using System;
using System.Collections.Generic;
public class RobotPartScriptMoveWheel : RobotPartScriptMove
{

    [Header("Wheels")]
    [SerializeField] [HeaderAttribute("轮子集合")] List<WheelCollider> _allWheels;
    [SerializeField] [HeaderAttribute("前轮子")] List<WheelCollider> _frontWheels;
    [SerializeField] [HeaderAttribute("后轮子")] List<WheelCollider> _backWheels;
    [SerializeField] [HeaderAttribute("轮子马力曲线")] AnimationCurve _motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));
    [SerializeField] [HeaderAttribute("最大马力")] float _maxTorque = 10000;
    [SerializeField] [HeaderAttribute("轮子刹车力度")] float _brakeForce = 1500.0f;
    [SerializeField] [HeaderAttribute("轮子拐弯最大角度")] float _steerAngle = 30.0f;
    [SerializeField] [HeaderAttribute("轮子拐弯速率")] float _steerSpeed = 0.2f;
    [SerializeField] [HeaderAttribute("未损坏的尘土特效")] List<GameObject> _dustEffects;
    [SerializeField] [HeaderAttribute("损坏的火花特效")] List<GameObject> _deatEffects;
    [SerializeField] [HeaderAttribute("未损坏的模型")] List<GameObject> _dustModels;
    [SerializeField] [HeaderAttribute("损坏的模型")] List<GameObject> _deatModels;
    [SerializeField] [HeaderAttribute("损坏的轮子半径")] float _damageRadius = 0.3f;
    [SerializeField] [HeaderAttribute("损坏的前摩擦")] float _damageForwardFriction = 0.6f;
    [SerializeField] [HeaderAttribute("损坏的侧摩擦")] float _damageSidewaysFriction = 0.8f;
    [SerializeField] [HeaderAttribute("不要跟随转向角(用于坦克)")] bool cancelSteerAngle = false;
    [SerializeField] [HeaderAttribute("")] Vector3 localRotOffset;
    [SerializeField] [HeaderAttribute("y轴")] float yOffset = 0f;
    [SerializeField] [HeaderAttribute("看的方向")] GameObject lookat;

    private BaseNpc _myNpc; // 当前NPC
    private Rigidbody _npcRigidbody; // 机体刚体
    private List<GameObject> _curEffects; // 当前轮子特效
    public float _rigidbodySpeed = 0.0f; // 机体的速度
    private float _startRadius = 0.6f;// 初始的轮子半径
    private float _startForwardFriction = 2; // 初始的前摩擦
    private float _startSidewaysFriction = 3; // 初始的侧摩擦
    private float _traiTime = 0.0f; // 胎印记录时间
    private float _tangerAngle = 0;
    private float _lookAngle = 0;
    private Keyframe[] _keyframes;

    protected override void BaseEvent_OnStart()
    {

    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _myNpc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();
        _keyframes = _motorTorque.keys;
        _npcRigidbody.angularDrag = 1;
        _npcRigidbody.drag = 0.5f;
        _curEffects = _dustEffects;
       

        // 初始化轮子属性
        foreach (WheelCollider wheelCollider in _allWheels)
        {
            wheelCollider.motorTorque = 0;
            wheelCollider.brakeTorque = 0;
            _startRadius = wheelCollider.radius;
            _startForwardFriction = wheelCollider.forwardFriction.stiffness;
            _startSidewaysFriction = wheelCollider.sidewaysFriction.stiffness;
        }


        foreach (GameObject item in _curEffects)
        {
            item.SetActive(false);
        }

        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        if (_myNpc.isDead)
        {
            for (int i = 0; i < _allWheels.Count; i++)
            {
                _allWheels[i].steerAngle = 0;
                _allWheels[i].motorTorque = 0;
                _allWheels[i].brakeTorque = 0;

                _deatEffects[i].SetActive(false);
                _dustEffects[i].SetActive(false);
            }
            return;
        }
        else if (Mathf.Abs(_rigidbodySpeed) > 10)
        {
            for (int i = 0; i < _allWheels.Count; i++)
            {
                if (_allWheels[i].isGrounded && _curEffects.Count > i)
                {
                    _curEffects[i].SetActive(true);
                }
                else
                {
                    foreach (GameObject item in _curEffects)
                    {
                        item.SetActive(false);
                    }
                }
            }
        }
        for (int i = 0; i < _allWheels.Count; i++)
        {
            WheelCollider wheelCollider = _allWheels[i];
            GameObject dustModel = _dustModels[i];
            GameObject deatModel = _deatModels[i];
            Vector3 pos = new Vector3(0, 0, 0);
            Quaternion quat = new Quaternion();
            wheelCollider.GetWorldPose(out pos, out quat);

            dustModel.transform.rotation = quat;
            deatModel.transform.rotation = quat;
            if (cancelSteerAngle)
            {
                dustModel.transform.rotation = dustModel.transform.parent.rotation;
                deatModel.transform.rotation = deatModel.transform.parent.rotation;
            }

            dustModel.transform.localRotation *= Quaternion.Euler(localRotOffset);
            deatModel.transform.localRotation *= Quaternion.Euler(localRotOffset);
            pos.y -= yOffset;
            dustModel.transform.position = pos;
            WheelHit wheelHit;
            wheelCollider.GetGroundHit(out wheelHit);
        }

    }
    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        if (_myNpc.isDead)
        {
            foreach (WheelCollider wheelCollider in _allWheels)
            {
                wheelCollider.steerAngle = 0;
                wheelCollider.motorTorque = 0;
                wheelCollider.brakeTorque = 0;
            }
            return;
        };

        base.BaseEvent_OnFmSynLogicUpdate();

        float speedValue = _myNpc.GetNpcAttr(AttributeType.Speed);
        // _motorTorque = new AnimationCurve(
        //     new Keyframe(0, _keyframes[0].value * _speedValue ),
        //     new Keyframe(_keyframes[1].time * _speedValue, _keyframes[1].value * _speedValue ),
        //     new Keyframe(_keyframes[2].time * _speedValue, 0 ));
        // _motorTorque = new AnimationCurve(
        //             new Keyframe(0, _maxTorque * _speedValue),
        //             new Keyframe(speedValue * 0.9f * _speedValue * 5, _maxTorque * 0.9f * _speedValue),
        //             new Keyframe(speedValue * _speedValue * 5, 0));


        // 更新机体速度
        Vector3 velocitys = lookat.transform.InverseTransformDirection(_npcRigidbody.velocity);
        _rigidbodySpeed = velocitys.z * 3.6f;
        _rigidbodySpeed = _rigidbodySpeed < 0 ? 0 : _rigidbodySpeed;

        // 下面是 控制车的朝向
        float MyY = _myNpc.gameObject.transform.eulerAngles.y;

        // 当前轮子需要的的朝向
        float tagAngles = CalcRelativeAngle(MyY, _tangerAngle);
        lookat.transform.localEulerAngles = new Vector3(0, tagAngles, 0);

        // 一个是让车朝向相机 一个是朝向移动方向
        float tagAngle = CalcRelativeAngle(MyY, _lookAngle);

        int isFu = Mathf.Abs(tagAngles) > 90 ? -1 : 1;
        // 旋转力
        float absSpeed = _rigidbodySpeed > 10 ? _rigidbodySpeed - 10 : 0;

        // 计算转向需要的力
        float torqueAngle = Mathf.Abs(tagAngle) > 45 ? 45 * (tagAngle > 0 ? 1 : -1) : tagAngle;
        float angleFlag = torqueAngle * 500 + (1500 * (Mathf.Min(absSpeed, 50) / 50));

        if (Mathf.Abs(angleFlag) > 10000)
        {
            angleFlag = 10000 * (angleFlag > 0 ? 1 : -1);
        }

        float stiffness = 0.5f + (0.5f * (Mathf.Min(absSpeed, 50) / 50));
        foreach (WheelCollider wheelCollider in _allWheels)
        {
            float tagMotorTorque = wheelCollider.motorTorque;
            // 设置轮胎朝向
            wheelCollider.steerAngle = tagAngles;

            // 修改轮子测摩擦力
            if (Mathf.Abs(wheelCollider.sidewaysFriction.stiffness - stiffness) > 0.01)
            {
                WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
                sidewaysFriction.asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip;
                sidewaysFriction.asymptoteValue = wheelCollider.sidewaysFriction.asymptoteValue;
                sidewaysFriction.extremumSlip = wheelCollider.sidewaysFriction.extremumSlip;
                sidewaysFriction.extremumValue = wheelCollider.sidewaysFriction.extremumValue;
                sidewaysFriction.stiffness = stiffness;
                wheelCollider.sidewaysFriction = sidewaysFriction;
            }

            // 角度够了 并且没有操作 踩刹车
            if (Mathf.Abs(tagAngle) < 3 && tagMotorTorque == 0)
            {
                wheelCollider.brakeTorque = _brakeForce;
                continue;
            }
            wheelCollider.brakeTorque = 0;

            float judgePos = wheelCollider.gameObject.transform.localPosition.x;
            if (Mathf.Abs(tagAngles) > 45 && Mathf.Abs(tagAngles) <= 135)
            {
                judgePos = wheelCollider.gameObject.transform.localPosition.z;
                isFu = tagAngles > 0 ? -1 : 1;
            }
            if (judgePos > 0)
            {
                tagMotorTorque -= (angleFlag * isFu);
            }
            else
            {
                tagMotorTorque += (angleFlag * isFu);
            }
            //tagMotorTorque = tagMotorTorque < 0 ? 0 : tagMotorTorque;
            wheelCollider.motorTorque = tagMotorTorque;
        }
    }

    protected override void BaseEvent_OnPartElementTreatmentInput()
    {
        _curEffects = _dustEffects;
        foreach (WheelCollider wheelCollider in _allWheels)
        {
            wheelCollider.radius = _startRadius;
            // 恢复为初始前摩擦
            WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
            forwardFriction.asymptoteSlip = wheelCollider.forwardFriction.asymptoteSlip;
            forwardFriction.asymptoteValue = wheelCollider.forwardFriction.asymptoteValue;
            forwardFriction.extremumSlip = wheelCollider.forwardFriction.extremumSlip;
            forwardFriction.extremumValue = wheelCollider.forwardFriction.extremumValue;
            forwardFriction.stiffness = _startForwardFriction;
            wheelCollider.forwardFriction = forwardFriction;

            // 恢复为初始侧摩擦
            WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
            sidewaysFriction.asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip;
            sidewaysFriction.asymptoteValue = wheelCollider.sidewaysFriction.asymptoteValue;
            sidewaysFriction.extremumSlip = wheelCollider.sidewaysFriction.extremumSlip;
            sidewaysFriction.extremumValue = wheelCollider.sidewaysFriction.extremumValue;
            sidewaysFriction.stiffness = _startSidewaysFriction;
            wheelCollider.sidewaysFriction = sidewaysFriction;
        }

    }

    protected override void BaseEvent_OnPartElementDead()
    {
        if (_myNpc.isDead)
        {
            foreach (WheelCollider wheelCollider in _allWheels)
            {
                wheelCollider.steerAngle = 0;
                wheelCollider.motorTorque = 0;
                wheelCollider.brakeTorque = 0;
            }
            return;
        };
        _curEffects = _deatEffects;
        for (int i = 0; i < _allWheels.Count; i++)
        {
            _deatEffects[i].SetActive(false);
            _dustEffects[i].SetActive(false);
            _allWheels[i].radius = _damageRadius;
            // 损坏后的前摩擦
            WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
            forwardFriction.asymptoteSlip = _allWheels[i].forwardFriction.asymptoteSlip;
            forwardFriction.asymptoteValue = _allWheels[i].forwardFriction.asymptoteValue;
            forwardFriction.extremumSlip = _allWheels[i].forwardFriction.extremumSlip;
            forwardFriction.extremumValue = _allWheels[i].forwardFriction.extremumValue;
            forwardFriction.stiffness = _damageForwardFriction;
            _allWheels[i].forwardFriction = forwardFriction;

            // 损坏后的侧摩擦
            WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
            sidewaysFriction.asymptoteSlip = _allWheels[i].sidewaysFriction.asymptoteSlip;
            sidewaysFriction.asymptoteValue = _allWheels[i].sidewaysFriction.asymptoteValue;
            sidewaysFriction.extremumSlip = _allWheels[i].sidewaysFriction.extremumSlip;
            sidewaysFriction.extremumValue = _allWheels[i].sidewaysFriction.extremumValue;
            sidewaysFriction.stiffness = _damageSidewaysFriction;
            _allWheels[i].sidewaysFriction = sidewaysFriction;
        }

    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }

    public override void StartSteerAngle(int angle)
    {
        base.StartSteerAngle(angle);
        // 我的角度
        float MyY = _myNpc.gameObject.transform.eulerAngles.y;

        foreach (WheelCollider wheelCollider in _allWheels)
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = _motorTorque.Evaluate(_rigidbodySpeed);
        }
        float tagY = _lookAngle + angle;
        if (tagY > 360)
        {
            tagY -= 360;
        }
        _tangerAngle = tagY;
    }

   

    public override void DownSpace()
    {
        base.DownSpace();
        foreach (WheelCollider wheelCollider in _allWheels)
        {
            wheelCollider.motorTorque = 0;
        }
    }

    public override void SlowDown()
    {
        foreach (WheelCollider wheelCollider in _allWheels)
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = 0;
        }
    }
    public override void LookAtAngle(float lookAngle)
    {
        _lookAngle = (float)lookAngle;
    }
}
