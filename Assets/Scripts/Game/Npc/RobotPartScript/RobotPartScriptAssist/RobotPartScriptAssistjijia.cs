using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 爆气
public class RobotPartScriptAssistjijia : RobotPartScriptAssist
{
    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        base.BaseEvent_OnUpdate();
       
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }
}