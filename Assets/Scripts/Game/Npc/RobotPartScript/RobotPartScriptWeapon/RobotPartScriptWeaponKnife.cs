using System;
using UnityEngine;

public class RobotPartScriptWeaponKnife : RobotPartScriptWeapon
{
    private float _curTime;
    private BaseNpc _npc;
    private bool _release;
    public Collider WeaponCollider;

    #region 父类中要重写的方法
    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if (_release)
        {
            if (_curTime < 7)
            {
                _curTime += Time.fixedDeltaTime;
            }
            else
            {
                _release = false;
                _curTime = 0;
                transform.localScale = Vector3.one;
            }
        }
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision,
        RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            base.BaseEvent_OnPartElementCollisionEnter(myCollider, collision, pTargetScript);

            if (myCollider == WeaponCollider)
                EffectDamage(collision);
            // 播放击中音效
            //_playSoundScriptList[1].Play(collision.GetContact(0).point);
        }

        // 播放碰撞音效
        //_playSoundScriptList[0].Play(collision.GetContact(0).point);
    }
    #endregion

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead) return false;

        // 播放技能音效
        //_playSoundScriptList[2].Play(transform);
        _release = true;
        transform.localScale = new Vector3(1, 1, 3);
        return true;
    }

    private void EffectDamage(Collision collision)
    {
        RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pTargetPartScript == null) return;

        RobotPartElement targetElement = pTargetPartScript.myElement;
        if (targetElement.isDead) return;

        BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
        if (pMyNpc == null)
        {
            return;
        }

        MapManager.Instance.baseMap.effectManager.SceneEffectAdd(2, collision.GetContact(0).point,
            Quaternion.identity);

        float nSpeed = 2;
        if (nSpeed > collision.relativeVelocity.magnitude) return;
        float Rspeed = Mathf.Abs((float) CalcDamage(collision));
        if (Rspeed <= 3) return;

        var attack = _npc.GetNpcAttr(AttributeType.Attack);
        float calcattack = attack * 5;
        if (_release) calcattack *= 1.5f;
        pMyNpc.DamageOutput(this.myElement, targetElement, (int)calcattack);
    }

    private float CalcDamage(Collision collision)
    {
        Vector3 ANormal = Vector3.zero;
        foreach (var point in collision.contacts) ANormal += point.normal * -1;
        ANormal /= collision.contacts.Length;
        float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
        float speedSqrt = Mathf.Sqrt(sqrMagnitude);
        Vector3 orNormalZ = new Vector3(Mathf.Abs(ANormal.x), Mathf.Abs(ANormal.y), Mathf.Abs(ANormal.z));
        Vector3 speedNormal = collision.relativeVelocity.normalized;
        float xq = orNormalZ.x * speedNormal.x + orNormalZ.y * speedNormal.y + orNormalZ.z * speedNormal.z;
        float sq = Mathf.Sqrt(orNormalZ.sqrMagnitude) * Mathf.Sqrt(speedNormal.sqrMagnitude);
        float cos = xq / sq;
        float relaSpeed = speedSqrt * cos;
        return relaSpeed;
    }


}