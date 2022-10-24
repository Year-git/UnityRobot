using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 盾
public class RobotPartScriptAssistShield : RobotPartScriptAssist
{
    [Header("盾参数")]
    [SerializeField] [HeaderAttribute("减伤百分比(常规)")] float _mitigation = 0.5f;
    [SerializeField] [HeaderAttribute("减伤百分比(技能)")] float _skillMitigation = 1f;
    [SerializeField] [HeaderAttribute("技能持续时间")] float _skillTime = 5;
    [SerializeField] [HeaderAttribute("普通盾模型")] GameObject _dunModel;
    [SerializeField] [HeaderAttribute("技能盾模型")] GameObject _skillModel;
    [SerializeField] [HeaderAttribute("技能盾模型偏移")] Vector3 _skillModelV3 = Vector3.zero;
    [SerializeField] [HeaderAttribute("盾破碎特效")] ParticleSystem _effect;
    private BaseNpc _myNpc;
    private Rigidbody _rigidbody;
    private float _curSkillTime = 0;
    private float _curmitigation = 0.5f;

    protected override void BaseEvent_OnStart()
    {
        _curmitigation = _mitigation;
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _myNpc = myElement.GetMyNpc();
        _rigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();
        _skillModel.transform.localPosition = Vector3.zero + _skillModelV3;
        _skillModel.SetActive(false);
        _dunModel.SetActive(true);
        _effect.Stop();

        // gameObject.layer |= LayerMask.NameToLayer("Ground");
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        base.BaseEvent_OnUpdate();
        if (Input.GetKey(KeyCode.T))
        {
            DoSkill();
        }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();

        if (_curSkillTime > 0)
        {
            _curSkillTime = (_curSkillTime - Time.fixedDeltaTime) < 0 ? 0 : (_curSkillTime - Time.fixedDeltaTime);
            if (_curSkillTime == 0)
            {
                _curmitigation = _mitigation;
                _skillModel.SetActive(false);
                _dunModel.SetActive(true);
            }
        }
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead || _curSkillTime > 0) return false;
        _curmitigation = _skillMitigation;
        _curSkillTime = _skillTime;
        _skillModel.SetActive(true);
        _dunModel.SetActive(false);
        return true;
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        // this.PlayPartSound(0, collision.GetContact(0).point);
    }
    protected override void BaseEvent_OnPartElementDead()
    {
        _effect.Play();
    }

    protected override int BaseEvent_OnPartElementDamageInputFilter(int nDamage)
    {
        return (int)(nDamage * (1 - _curmitigation));
    }
}