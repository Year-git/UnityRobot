using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponNiuJiJiaSpring : RobotPartScriptWeapon
{
    public Collider weaponCollider;

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            base.BaseEvent_OnPartElementCollisionEnter(myCollider, collision, pTargetScript);
            
            if (myCollider == weaponCollider)
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

                pMyNpc.DamageOutput(this.myElement, pTargetPartScript.myElement, 10);
            }
        }
    }
}
