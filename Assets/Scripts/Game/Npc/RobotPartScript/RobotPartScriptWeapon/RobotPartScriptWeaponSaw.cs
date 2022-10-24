using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPartScriptWeaponSaw : RobotPartScriptWeapon
{
    [Header("锯子参数")]
    [SerializeField] [HeaderAttribute("锯子碰撞体")] public GameObject _weaponCollider;
    [SerializeField] [HeaderAttribute("锯子本体位置标识")] public GameObject _posObj;
    [SerializeField] [HeaderAttribute("需旋转的模型")] GameObject _roatationObj;
    [SerializeField] [HeaderAttribute("模型旋转速度")] float _rotationSpeed = 20;
    [SerializeField] [HeaderAttribute("技能CD")] public float _skillCD = 3;
    [SerializeField] [HeaderAttribute("特效")] GameObject _effect;
    [SerializeField] [HeaderAttribute("关节")] FixedJoint _Joint;

    [Header("锯子子弹参数")]
    [SerializeField] [HeaderAttribute("悬浮高度偏移")] public float _offsetHeight = 0;
    [SerializeField] [HeaderAttribute("从中心点往左右偏移距离")] public float _posx = 0.5f;
    [SerializeField] [HeaderAttribute("从中心点往前后偏移距离")] public float _posz = 0.5f;
    [SerializeField] [HeaderAttribute("浮空系数")] public float _coefficient = 10;
    [SerializeField] [HeaderAttribute("浮空曲线")] public float _pow = 2;
    [SerializeField] [HeaderAttribute("移动加速度")] public float _moveForce = 10000;
    [SerializeField] [HeaderAttribute("旋转力")] public float _forqueForce = 10000;
    [SerializeField] [HeaderAttribute("模拟的空气阻力")] public float _dragFalg = 0.25f;
    [SerializeField] [HeaderAttribute("持续时间")] public float _deleteTime = 3;
    [SerializeField] [HeaderAttribute("推力系数")] public float _thrustForce = 3;
    [SerializeField] [HeaderAttribute("伤害检测间隔")] public float _maxDamageTime = 0.5f;
    [SerializeField] [HeaderAttribute("飞行停止时间")] public float _stopTime = 3;
    [SerializeField] [HeaderAttribute("技能模型放大倍数")] public float _scaleValue = 2;
    [SerializeField] [HeaderAttribute("是否从机甲中心发射")] public bool _bCentre = false;

    [Header("散射参数")]
    [SerializeField] [HeaderAttribute("开始角度")] float _startAngle = 0;
    [SerializeField] [HeaderAttribute("结束角度")] float _endAngle = 0;
    [SerializeField] [HeaderAttribute("平分数量")] float _num = 1;
    [SerializeField] [HeaderAttribute("发射时间间隔")] float _fixedShootTime = 0;
    [SerializeField] [HeaderAttribute("发射炮弹数量")] float _sawNum = 1;

    private float _curShootTime = 0f; //当前发射时间
    private float _curNum = 0; // 当前发射炮弹数量
    public BaseNpc _npc;
    private Rigidbody _npcRigidbody;
    private float _curDamageTime = 0;
    private float _curCD = 0;
    private Collider _collider;
    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _npc = myElement.GetMyNpc();
        _npcRigidbody = _npc.gameObject.GetComponent<Rigidbody>();

        _collider = _weaponCollider.gameObject.GetComponent<Collider>();
        _Joint.connectedBody = _npcRigidbody;

        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead) return false;
        _curNum = _sawNum;
        return true;
    }

    protected override void BaseEvent_OnUpdate()
    {
        _roatationObj.transform.Rotate(0, 0, _rotationSpeed);
    
    }
    protected override void BaseEvent_OnFmSynLogicUpdate()
    {

        if (myElement.isDead) return;
        // 技能CD计时
        if (_curCD > 0)
        {
            _curCD -= Time.deltaTime;
            if (_curCD < 0)
            {
                _curCD = 0;
                _posObj.transform.gameObject.SetActive(true);
                _weaponCollider.SetActive(true);
            }
        }
        if (!_weaponCollider.activeSelf)
        {
            _effect.SetActive(false);
        }
        if (_curShootTime > 0)
        {
            _curShootTime -= Time.fixedDeltaTime;
            if (_curShootTime < 0)
            {
                _curShootTime = 0;
            }
        }
        if (_curNum > 0 && _curShootTime == 0)
        {
            _curShootTime = _fixedShootTime;
            _curNum--;

            for (int i = 0; i < _num; i++)
            {
                float angle = 0;

                // 计算当前飞行的角度
                if (_num == 1)
                {
                    angle = 0;
                }
                else if (_startAngle > 0)
                {
                    angle = (Mathf.Abs(_endAngle) - Mathf.Abs(_startAngle)) / _num * i;
                }
                else
                {
                    angle = (Mathf.Abs(_startAngle) + Mathf.Abs(_endAngle)) / _num * i;
                }

                Vector3 flyAngle = _bCentre ? new Vector3(0, _npc.gameObject.transform.eulerAngles.y + _startAngle + angle, 0) : new Vector3(_posObj.transform.eulerAngles.x, _npc.gameObject.transform.eulerAngles.y, _posObj.transform.eulerAngles.z);
                Vector3 pos = new Vector3(_posObj.transform.position.x, _posObj.transform.position.y + _offsetHeight, _posObj.transform.position.z);
                string sawName = _npc.type == NpcType.PlayerNpc ? "Assets/Res/Prefabs/Component/Weapon/Bullet_Saw_White.prefab" : "Assets/Res/Prefabs/Component/Weapon/Bullet_Saw_Red.prefab";
                
                //生成锯子子弹
                MapManager.Instance.baseMap.objectPoolController.Assign(sawName, delegate (GameObject obj)
                {
                    var rt = obj.GetComponent<RobotPartScriptWeaponBulletSaw>();
                    Rigidbody rid = obj.GetComponent<Rigidbody>();
                    obj.transform.position = pos;
                    obj.transform.eulerAngles = flyAngle;
                    obj.transform.localScale = _posObj.transform.localScale;
                    rid.velocity = _npc.gameObject.GetComponent<Rigidbody>().velocity;
                    rt.SetAttribute(
                        _offsetHeight,
                        _posx,
                        _posz,
                        _coefficient,
                        _pow,
                        _moveForce,
                        _forqueForce,
                        _dragFalg,
                        _deleteTime,
                        _thrustForce,
                        _maxDamageTime,
                        _stopTime,
                        _scaleValue);
                    rt.SetOwner(_npc, this);

                    // 隐藏手中锯子
                    // _posObj.transform.gameObject.SetActive(false);
                    // _weaponCollider.SetActive(false);

                    // 播放巨炮发射音效
                    this.PlayPartSound(2, obj.transform.position);
                });
            }
        }
    }
    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (myElement.isDead) return;
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
        if (pTargetScript != null)
        {
            base.BaseEvent_OnPartElementCollisionStay(myCollider, collision, pTargetScript);
            if (myCollider == _collider)
            {
                RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
                if (pTargetPartScript == null)
                {
                    return;
                }

                BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
                if (pMyNpc == null)
                {
                    return;
                }

                RobotPartElement targetElement = pTargetPartScript.myElement;
                if (targetElement.isDead)
                {
                    return;
                }

                int attack = _npc.GetNpcAttr(AttributeType.Attack);
                pMyNpc.DamageOutput(this.myElement, targetElement, (int)attack);
            }
        }

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }
    protected override void BaseEvent_OnPartElementCollisionStay(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (myElement.isDead) return;
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

        if (pTargetScript != null)
        {
            base.BaseEvent_OnPartElementCollisionStay(myCollider, collision, pTargetScript);
            if (myCollider == _collider)
            {
                RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
                if (pTargetPartScript == null)
                {
                    return;
                }

                BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
                if (pMyNpc == null)
                {
                    return;
                }

                RobotPartElement targetElement = pTargetPartScript.myElement;
                if (targetElement.isDead)
                {
                    return;
                }
                _curDamageTime += Time.fixedDeltaTime;
                if (_curDamageTime >= _maxDamageTime)
                {
                    _curDamageTime = 0;
                    int attack = _npc.GetNpcAttr(AttributeType.Attack);
                    pMyNpc.DamageOutput(this.myElement, targetElement, (int)attack);
                }
            }
        }
    }

    protected override void BaseEvent_OnPartElementDead()
    {
        _weaponCollider.SetActive(false);
        _effect.SetActive(false);
    }

    protected override void BaseEvent_SwitchDamageModel()
    {
        _effect.SetActive(false);
    }

    protected override void BaseEvent_SwitchNormalModel()
    {
        _effect.SetActive(false);
    }
    protected override void BaseEvent_SwitchDeadModel()
    {
        _effect.SetActive(false);
    }
}
