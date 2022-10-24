using UnityEngine;
using System;
using System.Collections.Generic;
//弹簧拳
public class RobotPartScriptWeaponSpring : RobotPartScriptWeapon
{
    [SerializeField] [HeaderAttribute("拳套")] GameObject _springFist;
    [SerializeField] [HeaderAttribute("弹簧")] GameObject _springPole;
    [SerializeField] [HeaderAttribute("给拳头的力")] float _fistForce = 500;
    [SerializeField] [HeaderAttribute("给自己的反作用力的系数")] float _myCoefficient = 3;
    [SerializeField] [HeaderAttribute("给对方的力的系数")] float _otherCoefficient = 3;
    [SerializeField] [HeaderAttribute("给对方的向上力的系数")] float _otherCoefficientY = 3;
    [SerializeField] [HeaderAttribute("拳头z轴最大距离限制")] float _fistMaxZ = 20;
    [SerializeField] [HeaderAttribute("拳头z轴最小距离限制")] float _fistMinZ = 0.3f;
    [SerializeField] [HeaderAttribute("技能给拳头的力")] float _skillForce = 1500;
    [SerializeField] [HeaderAttribute("拳头技能CD")] float _maxskillCD = 2;
    [SerializeField] [HeaderAttribute("拳头技能自动攻击间隔时间")] float _maxskillTime = 0.2f;
    [SerializeField] [HeaderAttribute("拳头自动攻击间隔时间")] float _maxautoTime = 1;
    [SerializeField] [HeaderAttribute("是否是左拳")] bool _bLeft = false;
    [SerializeField] [HeaderAttribute("攻击特效")] GameObject _effect;

    private ConfigurableJoint _Joint; // 拳头关节
    private Rigidbody _npcRigidbody; // 机体刚体
    private Rigidbody _rigidbody; // 拳头刚体
    private float _startDis = 1; // 弹簧与拳头的开始距离
    private float _skillCD = 0; // 拳头技能CD计时
    private float _autoTime = 0; // 拳头自动攻击间隔计时
    private bool _bDelay = true; //是否是延迟出拳
    private bool _bDamage = true; //是否是延迟出拳
    private Vector3 _springStartPos; //拳头初始位置
    private BaseNpc _npc;
    protected override void BaseEvent_OnStart()
    {
        _rigidbody = _springFist.GetComponent<Rigidbody>();
        _Joint = _springFist.GetComponent<ConfigurableJoint>();
        _startDis = _springFist.transform.localPosition.z - _springPole.transform.localPosition.z;
        _springStartPos = _springFist.transform.localPosition;
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _npc = myElement.GetMyNpc();
        _npcRigidbody = _npc.gameObject.GetComponent<Rigidbody>();
        _Joint.connectedBody = _npcRigidbody;

        // 左拳慢一拍出拳
        _autoTime = _bLeft ? _maxautoTime / 2 : 0;

        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }

        if (_skillCD > 0) return false;
        _skillCD = _maxskillCD;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _autoTime = _maxskillTime;
        if (_bDelay && _bLeft)
        {
            _autoTime = _maxskillTime / 2;
            _bDelay = false;
        }
        if (!_bLeft)
        {
            _rigidbody.AddForce(_springFist.transform.forward * _skillForce, ForceMode.Impulse);
            _effect.SetActive(true);

            // 播放攻击音效
            this.PlayPartSound(2, transform);
        }
        return true;
    }
    protected override void BaseEvent_OnUpdate()
    {
        if (myElement.isDead) return;
        // CD计时
        if (_skillCD > 0)
        {
            _skillCD -= Time.deltaTime;
            if (_skillCD < 0)
            {
                _skillCD = 0;
                _bDelay = true;
            }
        }

        if (_autoTime > 0)
        {
            _autoTime -= Time.deltaTime;
            if (_autoTime < 0)
            {
                _autoTime = 0;
                _bDamage = true;
                _effect.SetActive(false);
            }
        }

        float curDis = _springFist.transform.localPosition.z - _springPole.transform.localPosition.z;
        // 弹簧长度自适应
        Vector3 scale = new Vector3(_springPole.transform.localScale.x, _springPole.transform.localScale.y, curDis / _startDis);
        _springPole.transform.localScale = scale;
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if (myElement.isDead)
        {
            return;
        }

        if (_autoTime == 0 && (_npc.type == NpcType.PlayerNpc || _skillCD > 0))
        {
            float force = _fistForce;
            _autoTime = _maxautoTime;
            // 释放技能振幅
            if (_skillCD > 0)
            {
                force = _skillForce;
                _autoTime = _maxskillTime;
                if (_bDelay && _bLeft)
                {
                    _autoTime = _maxskillTime / 2;
                    _bDelay = false;
                }
            }
            if (_rigidbody)
            {
                _rigidbody.AddForce(_springFist.transform.forward * force, ForceMode.Impulse);
				
                _effect.SetActive(true);

                // 播放攻击音效
                this.PlayPartSound(2, transform);
            }
        }

        // 强制限制拳头的最大最小位置
        float curDis = _springFist.transform.localPosition.z - _springPole.transform.localPosition.z;
        if (curDis > _fistMaxZ)
        {
            _springFist.transform.localPosition = new Vector3(_springFist.transform.localPosition.x, _springFist.transform.localPosition.y, _springPole.transform.localPosition.z + 3);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        if (curDis < _fistMinZ)
        {
            _springFist.transform.localPosition = new Vector3(_springFist.transform.localPosition.x, _springFist.transform.localPosition.y, _springPole.transform.localPosition.z + 0.3f);
        }
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // if (pTargetScript != null)
        // {
            SpringAddForce(collision);
        // }

        this.PlayPartSound(0, collision.GetContact(0).point);
    }

    protected override void BaseEvent_SwitchDeadModel()
    {
        _effect.SetActive(false);
    }
    protected override void BaseEvent_OnPartElementDead()
    {
        _effect.SetActive(false);
    }

    protected override void BaseEvent_SwitchNormalModel()
    {
        _springFist.transform.localPosition = _springStartPos;
        _springPole.transform.localScale = new Vector3(1, 1, 1);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    protected override void BaseEvent_SwitchDamageModel()
    {
        _springFist.transform.localPosition = _springStartPos;
        _springPole.transform.localScale = new Vector3(1, 1, 1);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    // 弹簧施力
    private void SpringAddForce(Collision collision)
    {
        if (_autoTime == 0 || !_bDamage ) return;
        // _autoTime = 0f;
        _bDamage = false;
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

        normal = new Vector3(normal.x, 0, normal.y).normalized;
        // normal = (normal + _springFist.transform.forward).normalized;
        // Debug.DrawLine(point, normal * 1000, Color.blue, 3f);
        Rigidbody otherRigidbody = collision.rigidbody;
        float carMass = _npcRigidbody ? _npcRigidbody.mass : 0;
        float otherMass = otherRigidbody ? otherRigidbody.mass : 0;
        if (_npcRigidbody)
        {
            _npcRigidbody.AddForceAtPosition(-normal * carMass * _myCoefficient, point, ForceMode.Impulse);
			
        }
        if (otherRigidbody)
        {
            otherRigidbody.AddForceAtPosition(normal * _otherCoefficient, point, ForceMode.VelocityChange);
			// test 
			otherRigidbody.AddForceAtPosition(Vector3.up * _otherCoefficientY, point, ForceMode.VelocityChange);
        }

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        // 造成伤害
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

        pMyNpc.DamageOutput(this.myElement, targetElement, 1 * _npc.GetNpcAttr(AttributeType.Attack));
        Debug.Log("_npc.GetNpcAttr(AttributeType.Attack) = " + _npc.GetNpcAttr(AttributeType.Attack));
        // 播放击中音效
        this.PlayPartSound(1, collision.GetContact(0).point);
    }


}