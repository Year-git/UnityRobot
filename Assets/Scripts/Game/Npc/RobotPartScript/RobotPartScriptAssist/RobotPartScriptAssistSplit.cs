using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 分裂
public class RobotPartScriptAssistSplit : RobotPartScriptAssist
{
    [Header("分裂参数")]
    [SerializeField] [HeaderAttribute("分裂npcName")] int _npcName;
    private BaseNpc _myNpc;
    private Rigidbody _rigidbody;
    private List<int> _levelNpcList;
    public static bool _bOnes = false;
    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _myNpc = myElement.GetMyNpc();
        _rigidbody = _myNpc.gameObject.GetComponent<Rigidbody>();

        _levelNpcList = MapManager.Instance.baseMap.levelNpcManager.GetLevelNpcList(_npcName.ToString());

        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnUpdate()
    {
        base.BaseEvent_OnUpdate();
        if (Input.GetKeyDown(KeyCode.R))
        {
            DoSkill();
        }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead || _bOnes) return false;
        _bOnes = true;
        for (int i = 0; i < _levelNpcList.Count; i++)
        {
            BaseNpc npc = MapManager.Instance.baseMap.levelNpcManager.GetNpc(_levelNpcList[i]);
            float x = (i == 0) ? 3 : -3;
            Vector3 pos = new Vector3(_myNpc.gameObject.transform.position.x + x, _myNpc.gameObject.transform.position.y + 1, _myNpc.gameObject.transform.position.z);
            npc.gameObject.transform.position = pos;
            npc.gameObject.transform.rotation = Quaternion.Euler(0, _myNpc.gameObject.transform.position.y, 0);
            Rigidbody rig = npc.gameObject.GetComponent<Rigidbody>();
            rig.angularVelocity = Vector3.zero;
            rig.velocity = Vector3.zero;
        }
        MapManager.Instance.baseMap.levelNpcManager.SetLevelNpcEnableByName(_npcName.ToString(), true);
        _myNpc.SetEnableState(false);

        // 出场特效
        for (int i = 0; i < _levelNpcList.Count; i++)
        {
            BaseNpc npc = MapManager.Instance.baseMap.levelNpcManager.GetNpc(_levelNpcList[i]);
            MapManager.Instance.baseMap.effectManager.SceneEffectAdd(18, npc.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        }
        MapManager.Instance.baseMap.effectManager.SceneEffectAdd(18, _myNpc.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        return true;
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        //this.PlayPartSound(0, collision.GetContact(0).point);
    }
    protected override void BaseEvent_OnNpcDead()
    {
        
    }
}