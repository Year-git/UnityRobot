using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 滋悬浮
public class RobotPartScriptAssistMaglev : RobotPartScriptAssist
{
    [SerializeField] float _maxCD = 1; // 技能CD
    [SerializeField] float _maxHeight = 10; // 最高悬浮高度
    [SerializeField] float _posx = 3; //从中心点往左右偏移距离
    [SerializeField] float _posz = 6; //从中心点往前后偏移距离
    [SerializeField] float _coefficient = 1; // 浮空系数
    [SerializeField] float _pow = 1; // 浮空曲线
    [SerializeField] float _dragFalg = 0.25f; // 空气阻力

    private Rigidbody _npcRigidbody; // 机体刚体
    private float _curHeight = 0; // 当前悬浮高度
    private float _curCD = 0; // 当前技能CD
    private float _t = 0; // 当前技能持续时间计时
    private List<Vector3> _pos = new List<Vector3>(); // 记录辅助位置

    protected override void BaseEvent_OnStart()
    {
        
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        _npcRigidbody = npc.gameObject.GetComponent<Rigidbody>();

        _pos.Add(transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz)));
        base.BaseEvent_OnInstall(fLoaded);
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }
        _curHeight = transform.TransformPoint(_npcRigidbody.centerOfMass).y + _maxHeight;
        _curCD = _maxCD;
        _t = 4;

        // 播放碰撞音效
        // _playSoundScriptList[0].Play(collision.GetContact(0).point);
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
        if (_t > 0)
        {
            _t -= Time.deltaTime;
            if (_t <= 0)
            {
                _t = 0;
            }
        }
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        // 技能效果
        if (_curCD != 0 || myElement.isDead) return;

        _pos[0] = transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz));
        _pos[1] = transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x - _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z - _posz));
        _pos[2] = transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x + _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z - _posz));
        _pos[3] = transform.TransformPoint(new Vector3(_npcRigidbody.centerOfMass.x - _posx, _npcRigidbody.centerOfMass.y, _npcRigidbody.centerOfMass.z + _posz));

        float g = Physics.gravity.y * -1;
        float G = _npcRigidbody.mass * g; 
        float useForce = G / 4;
        float mySpace = _curHeight - transform.TransformPoint(_npcRigidbody.centerOfMass).y;
        _npcRigidbody.AddForce(new Vector3(0, -_npcRigidbody.velocity.y * _dragFalg, 0), ForceMode.Acceleration);
        foreach (Vector3 p in _pos)
        {
            float space = _curHeight - p.y;
            float addCoe = Mathf.Pow(Mathf.Abs(space) * _coefficient, _pow) * (space >= 0 ? 1 : -1);
            float addForce = useForce + addCoe;
            // Debug.DrawLine(p, p + Vector3.up * addForce, Color.yellow, 0.1f);
            _npcRigidbody.AddForceAtPosition(Vector3.up * addForce, p, ForceMode.Force);
        }
    }
}