using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

public abstract class RobotPartScriptBody : RobotPartScriptBase
{
    public ParticleSystem _hurtEffect; // 受击特效
    private Rigidbody _rigidbody;
    
    private Animation[] _bodyAnimation;

    // 保存车体上所有子物体的容器，用作查询
    public Dictionary<string, Transform> _bodyAllChildContainer { get; private set; } = new Dictionary<string, Transform>();

    public bool IsHaveBodyChildTransform(string sChildName)
    {
        if (this._bodyAllChildContainer.ContainsKey(sChildName))
        {
            return true;
        }
        return false;
    }

    public Transform GetBodyChildTransform(string sChildName)
    {
        if (!this.IsHaveBodyChildTransform(sChildName))
        {
            return null;
        }
        return this._bodyAllChildContainer[sChildName];
    }

    protected override void BaseEvent_OnStart()
    {
        if (_hurtEffect)
        {
            _hurtEffect.Stop();

        }
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        // 初始化车体上模型所有挂点容器
        Transform[] allTransform = GetComponentsInChildren<Transform>();
        foreach (var pTransform in allTransform)
        {
            if (!_bodyAllChildContainer.ContainsKey(pTransform.name))
            {
                _bodyAllChildContainer.Add(pTransform.name, pTransform);
            }
        }

        int nNpcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        var pHoleInfo = pNpc.GetRobotPartHoleContainer();
        _rigidbody = pNpc.gameObject.GetComponent<Rigidbody>();

        JArray listHoleId = RobotPart.GetRobotPartBodyHoleList(this.myElement.myRobotPart.cfgId);
        if (listHoleId.Count > 0)
        {
            CommAsyncCounter pCounter = new CommAsyncCounter(listHoleId.Count, delegate ()
                {
                    base.BaseEvent_OnInstall(fLoaded);
                }
            );

            foreach (int nHoleId in listHoleId)
            {
                pNpc.RobotrPartHoleCreate(pHoleInfo.Count, nHoleId, delegate ()
                {
                    pCounter.Increase();
                });
            }
        }
        else
        {
            base.BaseEvent_OnInstall(fLoaded);
        }
        
        // Debug.Log("  ========   " + this.gameObject.GetComponent<Collider>().bounds.size.ToString("F6")); 
    }

    protected override void BaseEvent_OnUnInstall()
    {
        // 移除车体配件添加的槽位
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(myElement.myRobotPart.npcInstId);
        for (int i = pNpc.GetRobotPartHoleContainer().Count - 1; i > 0; i--)
        {
            pNpc.RobotrPartHoleRemove(i);
        }
    }

    //跳跃 demo版复位方法。。
    public override bool DoSkill()
    {
        base.DoSkill();
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        if (!_rigidbody || npc.isDead) return false;
        _rigidbody.velocity += transform.up * 15;
        return true;
    }

    protected override void BaseEvent_OnPartElementDead()
    {
        // 播放死亡爆炸音效
        this.PlayPartSound(1, transform.position);
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);

        if (_hurtEffect && !_hurtEffect.isPlaying)
        {
            _hurtEffect.Play();
        }
    }
}
