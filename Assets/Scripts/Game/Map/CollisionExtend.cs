using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExtend
{
    private Collision _firstCollision;
    private RobotPartScriptBase _firstTargetScript;
    public void ExitExtendAdd(Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (_firstCollision == null)
        {
            _firstCollision = collision;
            _firstTargetScript = pTargetScript;
        }
        else
        {
            NpcScript pFirstNpcScript = collision.gameObject.GetComponent<NpcScript>();
            NpcScript pSecondNpcScript = _firstCollision.gameObject.GetComponent<NpcScript>();
            if (pFirstNpcScript != null && pSecondNpcScript != null)
            {
                pFirstNpcScript.OnCollisionExitExtend(collision.collider, _firstTargetScript, _firstCollision, pTargetScript);
                pSecondNpcScript.OnCollisionExitExtend(_firstCollision.collider, pTargetScript, collision, _firstTargetScript);
            }
            _firstCollision = null;
            _firstTargetScript = null;
        }
    }
}
