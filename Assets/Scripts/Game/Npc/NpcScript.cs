using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    public BaseNpc myNpc;

    // 碰撞开始
    public void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
        RobotPartScriptBase pPart = contactPoint.thisCollider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pPart == null)
        {
            return;
        }
        pPart.myElement.OnPartElementCollisionEnter(contactPoint.thisCollider, collision, pPart);
    }

    // 碰撞持续中
    public void OnCollisionStay(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
        RobotPartScriptBase pPart = contactPoint.thisCollider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pPart == null)
        {
            return;
        }
        pPart.myElement.OnPartElementCollisionStay(contactPoint.thisCollider, collision, pPart);
    }

    // 碰撞结束【将Unity的碰撞结束添加到碰撞扩展器中】
    public void OnCollisionExit(Collision collision)
    {
        RobotPartScriptBase pPart = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pPart == null)
        {
            return;
        }
        MapManager.Instance.baseMap.collisionExtend.ExitExtendAdd(collision, pPart);
    }

    // 支持带有自身碰撞盒参数的碰撞结束【调用来自碰撞扩展器】
    public void OnCollisionExitExtend(Collider myCollider, RobotPartScriptBase myScript, Collision collision, RobotPartScriptBase targetScript)
    {
        // Dictionary<int, RobotPartScriptBase> UsePartsList = new Dictionary<int, RobotPartScriptBase>();
        // GameObject gobj = myCollider.gameObject;
        // RobotPartScriptBase pPart = gobj.GetComponentInParent<RobotPartScriptBase>();
        // if(pPart != null ){
        //     int instID = pPart.GetInstanceID();
        //     if (!UsePartsList.ContainsKey(instID))
        //     {
        //         UsePartsList[instID] = pPart;
        //     }
        //     foreach (var pair in UsePartsList)
        //     {
        //         pair.Value.myElement.OnPartElementCollisionExit(myCollider, collision);
        //     }
        // }
        myScript.myElement.OnPartElementCollisionExit(myCollider, collision, targetScript);
    }
}
