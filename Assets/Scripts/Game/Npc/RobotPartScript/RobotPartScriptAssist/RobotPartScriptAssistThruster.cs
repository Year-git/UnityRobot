using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 爆气
public class RobotPartScriptAssistThruster : RobotPartScriptAssist
{
    [Header("爆气参数")]
    [SerializeField] [HeaderAttribute("爆气的瞬时力")] float force = 20000;
    [SerializeField] [HeaderAttribute("技能CD")] float CD = 1;
    [SerializeField] GameObject _juqis;
    [SerializeField] GameObject _baoqis;
    private float curCD = 0; //当前技能CD
    private BaseNpc _myNpc;
    private Rigidbody _rigidbody;

    protected override void BaseEvent_OnStart()
    {
    }

    public bool IsCD() { return curCD > 0; }
    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _myNpc = myElement.GetMyNpc();
        _rigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();

        if (_juqis != null)
        {
            _juqis.SetActive(true);
        }

        if (_baoqis != null)
        {
            _baoqis.SetActive(false);
        }
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        base.BaseEvent_OnUpdate();

        //CD计时
        if (curCD > 0)
        {
            curCD -= Time.deltaTime;
            if (curCD <= 0)
            {
                curCD = 0;
                if (_juqis != null)
                {
                    _juqis.SetActive(true);
                }
                if (_baoqis != null)
                {
                    _baoqis.SetActive(false);
                }
            }
        }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }

        if ((!_rigidbody && IsCD()) || _myNpc.isDead) return false;

        // 播放技能音效
        this.PlayPartSound(1, transform);

        curCD = CD;
        // 播放特效
        if (_juqis != null)
        {
            _juqis.SetActive(false);
        }

        if (_baoqis != null)
        {
            _baoqis.SetActive(true);
        }

        // 施力点 配件位置
        // Vector3 pos = new Vector3(transform.position.x, _rigidbody.transform.position.y, transform.position.z);
        // 施力点 机体位置
        Vector3 pos = _rigidbody.transform.position;
        _rigidbody.AddForceAtPosition(transform.forward * force, pos, ForceMode.Impulse);
        // _myNpc.attrEntity.ModfiyPercentValue(AttributeType.Speed,150);
        return true;
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }
    protected override void BaseEvent_OnNpcDead()
    {
        if (_juqis != null)
        {
            _juqis.SetActive(false);
        }

        if (_baoqis != null)
        {
            _baoqis.SetActive(false);
        }
    }
}