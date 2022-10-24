using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 氮气
public class RobotPartScriptAssistNitrogen : RobotPartScriptAssist
{
    [Header("氮气参数")]
    [SerializeField] [HeaderAttribute("加速百分比")] float _speedUpValue = 0.5f;
    [SerializeField] [HeaderAttribute("技能CD")] float _CD = 1;
    [SerializeField] [HeaderAttribute("氮气持续时间")] float _duration = 8;
    [SerializeField] GameObject[] _baoqis;
    private float _curCD = 0; //当前技能CD
    private float _curDuration = 0; //当前持续时间
    private Rigidbody _rigidbody;
    private BaseNpc _myNpc;

    protected override void BaseEvent_OnStart()
    {
    }

    public bool isCD { get { return _curCD > 0; } }
    public bool isSkillDuration { get { return _curDuration > 0; } }
    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _myNpc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _rigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();

        foreach (GameObject item in _baoqis)
        {
            item.SetActive(false);
        }
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        base.BaseEvent_OnUpdate();


    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
        //CD计时
        if (isCD)
        {
            _curCD -= Time.fixedDeltaTime;
            if (_curCD <= 0)
            {
                _curCD = 0;
            }
        }
        if (isSkillDuration)
        {
            _curDuration -= Time.fixedDeltaTime;
            if (_curDuration <= 0)
            {
                _curDuration = 0;

                // _myNpc.attrEntity.ModfiyPercentValue(AttributeType.Speed, _speedUpValue);
                RobotPart pPart = _myNpc.GetRobotMovePart();

                foreach (RobotPartElement item in pPart.myElementList)
                {
                    RobotPartScriptMove move = (RobotPartScriptMove)item.myScript;
                    move._speedValue -= _speedUpValue;
                }

                foreach (GameObject item in _baoqis)
                {
                    item.SetActive(false);
                }
            }
        }
        if (Input.GetKey(KeyCode.B))
        {
            DoSkill();
        }
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead || !_rigidbody || isCD || isSkillDuration) return false;

        // 播放技能音效
        this.PlayPartSound(1, transform);

        _curCD = _CD;
        _curDuration = _duration;
        // 加速
        // _myNpc.attrEntity.ModfiyPercentValue(AttributeType.Speed, _speedUpValue);
        RobotPart pPart = _myNpc.GetRobotMovePart();

        foreach (RobotPartElement item in pPart.myElementList)
        {
            RobotPartScriptMove move = (RobotPartScriptMove)item.myScript;
            move._speedValue += _speedUpValue;
        }

        foreach (GameObject item in _baoqis)
        {
            item.SetActive(true);
        }
        return true;
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }
    protected override void BaseEvent_OnNpcDead()
    {
        foreach (GameObject item in _baoqis)
        {
            item.SetActive(false);
        }
    }
}