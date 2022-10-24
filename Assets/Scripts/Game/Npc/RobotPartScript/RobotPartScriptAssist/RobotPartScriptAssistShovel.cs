using UnityEngine;
using System;
using System.Collections.Generic;
public class RobotPartScriptAssistShovel : RobotPartScriptAssist
{
    [SerializeField] float maxMass = 8000f;
    [SerializeField] float forceEnter = 1.2f;
    [SerializeField] float forceStay = 4f;
    [SerializeField] ForceMode forceModeEnter = ForceMode.Impulse;
    [SerializeField] ForceMode forceModeStay = ForceMode.Force;
    [SerializeField] GameObject xuanzhuanzhou;
    private HingeJoint _Joint;
    private Rigidbody _rigidbody;

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _rigidbody = xuanzhuanzhou.GetComponent<Rigidbody>();
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Rigidbody npcrid = npc.gameObject.GetComponent<Rigidbody>();
        _Joint = xuanzhuanzhou.GetComponent<HingeJoint>();
        _Joint.connectedBody = npcrid;
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            ContactPoint contact = collision.GetContact(0);
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

            // 辅助线
            // Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);

            Rigidbody rigidbody = collision.rigidbody;
            if (rigidbody)
            {
                float mass = rigidbody.mass > maxMass ? maxMass : rigidbody.mass;
                Vector3 fv = (-normal + Vector3.up).normalized * mass * forceEnter;
                rigidbody.AddForceAtPosition(fv, point, forceModeEnter);

                // 辅助线 角度
                // Debug.DrawLine(contact.point, fv.normalized * 1000, Color.red, 3f);
                // Debug.DrawLine(contact.point, -contact.normal * 1000, Color.white, 3f);
                // Debug.DrawLine(contact.point, Vector3.up * 1000, Color.blue, 3f);
                // float angle = Mathf.Acos(Vector3.Dot(fv.normalized, Vector3.up)) * Mathf.Rad2Deg;
                // Debug.Log("angle = " + angle);
            }
        }
    }

    // 碰撞持续中
    protected override void BaseEvent_OnPartElementCollisionStay(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript == null)
        {
            return;
        }
        ContactPoint contact = collision.GetContact(0);
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

        // 辅助线
        // Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);

        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {
            float mass = rigidbody.mass > maxMass ? maxMass : rigidbody.mass;
            Vector3 fv = (-normal + Vector3.up).normalized * mass * forceStay;
            rigidbody.AddForceAtPosition(fv, point, forceModeStay);

            // 辅助线 角度
            // Debug.DrawLine(contact.point, fv.normalized * 1000, Color.red, 3f);
            // Debug.DrawLine(contact.point, -contact.normal * 1000, Color.white, 3f);
            // Debug.DrawLine(contact.point, Vector3.up * 1000, Color.blue, 3f);
            // float angle = Mathf.Acos(Vector3.Dot(fv.normalized, Vector3.up)) * Mathf.Rad2Deg;
            // Debug.Log("angle = " + angle);
        }
    }

    protected override void BaseEvent_OnPartElementDead()
    {
        // Destroy(_Joint);
        // Destroy(_rigidbody);
        base.BaseEvent_OnPartElementDead();
    }
}