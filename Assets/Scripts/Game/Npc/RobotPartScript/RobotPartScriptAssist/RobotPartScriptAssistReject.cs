using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 排斥环
public class RobotPartScriptAssistReject : RobotPartScriptAssist
{
    [Header("排斥环参数")]
    [SerializeField] [HeaderAttribute("技能CD")] float _maxCD = 3;
    [SerializeField] [HeaderAttribute("技能持续时间")] float _continueTime = 1;
    [SerializeField] [HeaderAttribute("技能半径")] float _radius = 6;
    [SerializeField] [HeaderAttribute("技能推力系数")] float _force = 2;
    [SerializeField] [HeaderAttribute("技能特效 1")] ParticleSystem[] _effices;
    [SerializeField] [HeaderAttribute("技能特效 2")] GameObject _effice;
    [SerializeField] [HeaderAttribute("技能位置")] GameObject _efficePos;

    private Rigidbody _npcRigidbody; // 机体刚体
    private  BaseNpc _npc;
    private float _curCD = 0; // 当前技能CD
    private float _curTime = 0; // 当前技能持续时间计时

    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = _npc.gameObject.GetComponent<Rigidbody>();
        
        _efficePos.transform.position = _npc.gameObject.transform.position;
        _efficePos.transform.eulerAngles = _npc.gameObject.transform.eulerAngles;
        foreach (ParticleSystem item in _effices)
        {
            item.Stop();
        }
        _efficePos.SetActive(false);
        _effice.SetActive(false);
        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (_curCD > 0 || myElement.isDead) return false;
        _curCD = _maxCD;
        _curTime = _continueTime;
        if(!_efficePos.activeSelf){
            _efficePos.SetActive(true);
        }
        foreach (ParticleSystem item in _effices)
        {
            item.Play();
        }
        _efficePos.transform.position = _npc.gameObject.transform.position;
        _efficePos.transform.eulerAngles = _npc.gameObject.transform.eulerAngles;
        _effice.SetActive(true);
        this.PlayPartSound(1, transform);
        return true;
    }

    protected override void BaseEvent_OnUpdate()
    {
        if (_curCD > 0)
        {
            _curCD -= Time.deltaTime;
            if (_curCD <= 0)
            {
                _curCD = 0;
            }
        }
        if (_curTime > 0)
        {
            _curTime -= Time.deltaTime;
            if (_curTime <= 0)
            {
                _curTime = 0;
                _effice.SetActive(false);
            }
        }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if (_curTime == 0 || myElement.isDead) return;
        //定义爆炸位置为炸弹位置
        Vector3 explosionPos = _npc.gameObject.transform.position;
        //这个方法用来反回球型半径之内（包括半径）的所有碰撞体collider[]
        Collider[] colliders = Physics.OverlapSphere(explosionPos, _radius);

        List<Collider> colliderList = new List<Collider>();
        colliderList.CopyTo(colliders);

        Dictionary<int, Rigidbody> rigList = new Dictionary<int, Rigidbody>();
        List<RobotPartScriptBase> PartList = new List<RobotPartScriptBase>();

        //遍历返回的碰撞体，如果是刚体，则给刚体添加力
        foreach (Collider hit in colliders)
        {
            Rigidbody rig = hit.attachedRigidbody;
            RobotPartScriptBase robotPartScript = hit.gameObject.
                GetComponentInParent<RobotPartScriptBase>();
            if (robotPartScript != null && robotPartScript.gameObject.layer != gameObject.layer)
                PartList.Add(robotPartScript);
            if (rig != null)
            {
                bool bIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(this.gameObject.layer, rig.gameObject.layer);
                if (!bIgnoreLayerCollision)
                {
                    if (!rigList.ContainsKey(rig.GetInstanceID()))
                    {
                        rigList.Add(rig.GetInstanceID(), rig);
                    }
                }
            }
        }
        foreach (var keyVal in rigList)
        {
            Rigidbody rig = keyVal.Value;
            Vector3 forceDis = rig.gameObject.transform.position - new Vector3(explosionPos.x, rig.gameObject.transform.position.y, explosionPos.z);
            float force = rig.mass * 20 * _force;
            rig.AddForce(forceDis.normalized * force, ForceMode.Force);
            Debug.DrawLine(rig.gameObject.transform.position, forceDis.normalized * 1000);
        }
        // foreach (var keyVal in rigList)
        // {
        //     Rigidbody rig = keyVal.Value;
        //     rig.AddExplosionForce(rig.mass * 20 * _force, explosionPos, _radius, 0.30f, ForceMode.Impulse);
        // }
    }
    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }
}