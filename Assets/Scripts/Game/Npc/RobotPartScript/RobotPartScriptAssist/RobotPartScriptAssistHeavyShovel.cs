using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//重铲
public class RobotPartScriptAssistHeavyShovel : RobotPartScriptAssist
{
    [SerializeField] [HeaderAttribute("铲子")] GameObject _shocel;
    [SerializeField] [HeaderAttribute("对象极限加力质量(对象超过该值。将不再加大铲力质量系数)")] float _maxMass = 8000f;
    [SerializeField] [HeaderAttribute("开始碰撞冲击力基础系数")] float _forceEnter = 1.2f;
    [SerializeField] [HeaderAttribute("开始碰撞使用的力类型")] ForceMode _forceModeEnter = ForceMode.Impulse;
    [SerializeField] [HeaderAttribute("技能CD")] float _maxSkillCD = 1f;
    [SerializeField] [HeaderAttribute("技能CD计时")] float _skillTime = 0f;
    [SerializeField] [HeaderAttribute("铲子上扬速度")] float _shovelVelocity = 10000;
    [SerializeField] [HeaderAttribute("铲子上扬达到的角度")] float _shovelUpAngle = -140;
    private HingeJoint _Joint;
    private Rigidbody _shocelRigidbody;
    private BaseNpc _npc;

    private Rigidbody _npcRigidbody; // 机体刚体
    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _shocelRigidbody = _shocel.GetComponent<Rigidbody>();
        _npc = myElement.GetMyNpc();
        _npcRigidbody = _npc.gameObject.GetComponent<Rigidbody>();
        _Joint = _shocel.GetComponent<HingeJoint>();
        _Joint.connectedBody = _npcRigidbody;
        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }

        if (_skillTime > 0) return false;

        // 播放技能音效
        this.PlayPartSound(1, transform);

        _skillTime = _maxSkillCD;
        JointMotor motor = _Joint.motor;
        motor.targetVelocity = -_shovelVelocity;
        _Joint.motor = motor;
        return true;
    }

    protected override void BaseEvent_OnUpdate()
    {
        // if (Input.GetKey(KeyCode.R))
        // {
        //     DoSkill();
        // }
        if (_skillTime > 0)
        {
            _skillTime -= Time.deltaTime;
            if (_skillTime <= 0)
            {
                _skillTime = 0;
                JointMotor motor = _Joint.motor;
                motor.targetVelocity = _shovelVelocity;
                _Joint.motor = motor;
            }
        }
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            if (_skillTime == 0) return;
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
                // float mass = rigidbody.mass > _maxMass ? _maxMass : rigidbody.mass;
                // Vector3 fv = (-normal + Vector3.up).normalized * mass * _forceEnter;
                Vector3 fv = (-normal + Vector3.up).normalized * _forceEnter;
                rigidbody.AddForceAtPosition(fv, point, _forceModeEnter);
            }
        }

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }

}