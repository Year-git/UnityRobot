using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponAwl : RobotPartScriptWeapon
{
    private BaseNpc _Npc;
    [SerializeField] private GameObject SkillObject;
    private bool release;
    private float curTime = 0f;
    [SerializeField][Rename("力")]private float Force = 500f;
    private float CollisionTime = 0;
    private float CollisionFixedTime = 1f;

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _Npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        SkillObject.SetActive(false);
        CollisionTime = 0f;
        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }

        // 播放技能音效
        this.PlayPartSound(2, transform);

        SkillObject.SetActive(true);
        release = true;
        return true;
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if(release)
        {
            if(curTime < 7)
            {
                curTime += Time.fixedDeltaTime;
            }
            else
            {
                release = false;
                curTime = 0;
                SkillObject.SetActive(false);
            }
        }
        if (CollisionTime > 0)
        {
            CollisionTime -= Time.fixedDeltaTime;
        }
    }


    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            base.BaseEvent_OnPartElementCollisionEnter(myCollider, collision, pTargetScript);
            
            if (myCollider.gameObject.name == "WeaponCollider")
            {
               
                RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
                if(pTargetPartScript == null)
                {
                    return;
                }

                BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
                if (pMyNpc == null)
                {
                    return;
                }
               

                float Rspeed = Mathf.Abs((float)CalcDamage(collision));
                RobotPartElement targetElement = pTargetPartScript.myElement;
                if (targetElement.isDead)
                {
                    return;
                }
                AddForce(collision);
                MapManager.Instance.baseMap.effectManager.SceneEffectAdd(2, collision.GetContact(0).point,
                    Quaternion.identity);
                
                float nSpeed = 2;
                if (nSpeed > collision.relativeVelocity.magnitude)
                {
                    return;
                }
                 
                if (Rspeed <= 3)
                {
                    return;
                }
                if (CollisionTime > 0)
                {
                    return;
                }
                CollisionTime = CollisionFixedTime;
                int attack = _Npc.GetNpcAttr(AttributeType.Attack);
                float calcattack = attack * 5;//(Rspeed / 33);
                if (release) calcattack *= 1.5f; 
                pMyNpc.DamageOutput(this.myElement, targetElement, (int)calcattack);
                // 播放击中音效
                this.PlayPartSound(1, collision.GetContact(0).point);
            }
        }

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }


    void AddForce(Collision collision)
    {
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

        Rigidbody otherRigidbody = collision.rigidbody;
        float otherMass = otherRigidbody ? otherRigidbody.mass : 0;
        if (otherRigidbody)
        {
            otherRigidbody.AddForceAtPosition(this.gameObject.transform.forward * Force, point, ForceMode.VelocityChange);
        }
    }

    float CalcDamage(Collision collision)
    {
        Vector3 ANormal = Vector3.zero;
        foreach (var point in collision.contacts)
        {
            ANormal += (point.normal * -1);
        }
        ANormal /= collision.contacts.Length;
        var sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
        var speedSqrt = Mathf.Sqrt(sqrMagnitude);
        var orNormalZ = new Vector3(Mathf.Abs(ANormal.x), Mathf.Abs(ANormal.y), Mathf.Abs(ANormal.z));
        var speedNormal = collision.relativeVelocity.normalized;
        var xq = orNormalZ.x * speedNormal.x + orNormalZ.y * speedNormal.y + orNormalZ.z * speedNormal.z;
        var sq = Mathf.Sqrt(orNormalZ.sqrMagnitude) * Mathf.Sqrt(speedNormal.sqrMagnitude);
        var cos = xq / sq;
        float relaSpeed = speedSqrt * cos;
        return relaSpeed;
    }


}
