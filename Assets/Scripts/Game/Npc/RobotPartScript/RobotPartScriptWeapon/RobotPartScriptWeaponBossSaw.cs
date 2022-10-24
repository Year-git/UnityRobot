using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPartScriptWeaponBossSaw : RobotPartScriptWeaponSaw
{

    [Header("Boss技能参数")]
    [SerializeField] [HeaderAttribute("开始角度")] float _startAngle = -30;
    [SerializeField] [HeaderAttribute("结束角度")] float _endAngle = 30;
    [SerializeField] [HeaderAttribute("平分数量")] float _num = 6;
    [SerializeField] [HeaderAttribute("发射时间间隔")] float _fixedShootTime = 0.5f;
    [SerializeField] [HeaderAttribute("发射炮弹数量")] float _sawNum = 5;
    private float _curShootTime = 0f; //当前发射时间
    private float _curNum = 0; // 当前发射炮弹数量
    public override bool DoSkill()
    {
        if (myElement.isDead) return false;
        _curNum = _sawNum;
        return true;
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
        if (_curShootTime > 0)
        {
            _curShootTime -= Time.fixedDeltaTime;
            if (_curShootTime < 0)
            {
                _curShootTime = 0;
            }
        }
        if (_curNum > 0 && _curShootTime == 0)
        {
            _curShootTime = _fixedShootTime;
            _curNum--;

            for (int i = 0; i < _num; i++)
            {
                float angle = 0;
                // 计算当前飞行的角度
                if (_num == 1)
                {
                    angle = 0;
                }
                else if (_startAngle > 0)
                {
                    angle = (Mathf.Abs(_endAngle) - Mathf.Abs(_startAngle)) / _num * i;
                }
                else
                {
                    angle = (Mathf.Abs(_startAngle) + Mathf.Abs(_endAngle)) / _num * i;
                }

                Vector3 flyAngle = new Vector3(0, _npc.gameObject.transform.eulerAngles.y + _startAngle + angle, 0);
                Vector3 pos = new Vector3(_posObj.gameObject.transform.position.x, _posObj.transform.position.y + _offsetHeight, _posObj.gameObject.transform.position.z);
                MapManager.Instance.baseMap.objectPoolController.Assign("Assets/Res/Prefabs/Component/Weapon/Bullet_Saw_Red.prefab", delegate (GameObject obj)
                {
                    var rt = obj.GetComponent<RobotPartScriptWeaponBulletSaw>();
                    Rigidbody rid = obj.GetComponent<Rigidbody>();
                    obj.transform.position = pos;
                    obj.transform.eulerAngles = flyAngle;
                    obj.transform.localScale = _posObj.transform.localScale;
                    rid.velocity = _npc.gameObject.GetComponent<Rigidbody>().velocity;
                    rt.SetAttribute(
                        _offsetHeight,
                        _posx,
                        _posz,
                        _coefficient,
                        _pow,
                        _moveForce,
                        _forqueForce,
                        _dragFalg,
                        _deleteTime,
                        _thrustForce,
                        _maxDamageTime,
                        _stopTime,
                        _scaleValue);
                    rt.SetOwner(_npc, this);
                // _posObj.transform.gameObject.SetActive(false);
                // _weaponCollider.SetActive(false);
                // 播放巨炮发射音效
                this.PlayPartSound(2, obj.transform.position);
                });
            }
        }
    }

}
