using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigCollosionScript : MonoBehaviour
{
    public GameObject _effect;
    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {
        if (_effect)
        {
            _effect.SetActive(true);
        }
        ContactPoint contactPoint = collision.GetContact(0);
        NpcScript pPart = contactPoint.thisCollider.gameObject.GetComponentInParent<NpcScript>();
        if (pPart == null)
        {
            //Debug.LogError("RigCollosionScript.cs->OnCollisionEnter->Get NpcScript Count Is Error!");
            return;
        }
        // 正确的情况下，只应获取到一个【配件零件下不应存在另一个配件零件】
        pPart.OnCollisionEnter(collision);
    }

    // 碰撞持续中
    public void OnCollisionStay(Collision collision)
    {
        if (_effect && !_effect.activeSelf)
        {
            _effect.SetActive(true);
        }
        ContactPoint contactPoint = collision.GetContact(0);
        NpcScript pPart = contactPoint.thisCollider.gameObject.GetComponentInParent<NpcScript>();
        if (pPart == null)
        {
            //Debug.LogError("RigCollosionScript.cs->OnCollisionStay->Get NpcScript Count Is Error!");
            return;
        }
        // 正确的情况下，只应获取到一个【配件零件下不应存在另一个配件零件】
        pPart.OnCollisionStay(collision);
    }

    // 碰撞持结束
    public void OnCollisionExit(Collision collision)
    {
        if (_effect)
        {
            _effect.SetActive(false);
        }
        NpcScript NpcScrip = this.gameObject.GetComponentInParent<NpcScript>();
        if (NpcScrip == null)
        {
            //Debug.LogError("RigCollosionScript.cs->OnCollisionExit->Get RobotPartScriptBase Count Is Error!");
            return;
        }

        // 正确的情况下，只应获取到一个【配件零件下不应存在另一个配件零件】
        NpcScrip.OnCollisionExit(collision);
    }
}
