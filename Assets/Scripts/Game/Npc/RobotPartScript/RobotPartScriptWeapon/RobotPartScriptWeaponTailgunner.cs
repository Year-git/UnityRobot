using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponTailgunner : RobotPartScriptWeapon
{
    [Header("追踪炮参数")]
    [SerializeField] [HeaderAttribute("枪口")] Transform _gun;
    [SerializeField] [HeaderAttribute("发射时间间隔")] float _fixedShootTime = 1;
    [SerializeField] [HeaderAttribute("发射炮弹数量")] float _num = 5;
    [SerializeField] [HeaderAttribute("发射炮弹初速度")] float _speed = 3;
    [SerializeField] [HeaderAttribute("是否随机")] bool _bRandom = true;
    [SerializeField] [HeaderAttribute("最小发射炮弹初速度(不随机用该速度)")] float _minSpeed = 20;
    [SerializeField] [HeaderAttribute("最大发射炮弹初速度")] float _maxSpeed = 30;
    [SerializeField] [HeaderAttribute("角度随机范围")] float _randomAngle = 15;
    [SerializeField] [HeaderAttribute("发炮特效")] ParticleSystem[] _effects;
    [SerializeField] [HeaderAttribute("炮弹颜色")] bool _bColor = false;

    [Header("追踪炮弹参数")]
    [SerializeField] [HeaderAttribute("开启无限追踪")] bool _bInfinite = false;
    [SerializeField] [HeaderAttribute("追踪速度")] float _tailgunnerSpeed = 10;
    [SerializeField] [HeaderAttribute("开始跟踪的时间")] float _tailgunnerTime = 5;
    [SerializeField] [HeaderAttribute("播放爆炸红灯时间")] float _playAniTime = 13;
    [SerializeField] [HeaderAttribute("爆炸时间")] float _explosionTime = 15;
    [SerializeField] [HeaderAttribute("触发爆炸半径")] float _tirgger = 10;
    [SerializeField] [HeaderAttribute("爆炸的半径")] float _radius = 15;
    [SerializeField] [HeaderAttribute("最小受损比例")] float _minRatio = 0.8f;
    [SerializeField] [HeaderAttribute("视野范围")] float _view = 30;

    private float _curShootTime = 0; // 当前发射时间
    private float _curNum = 0; // 当前发射炮弹数量
    public BaseNpc _npc { get; private set; }
    private Rigidbody _npcRigidbody;

    protected override void BaseEvent_OnStart()
    {

    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        _npc = myElement.GetMyNpc();
        _npcRigidbody = _npc.gameObject.GetComponent<Rigidbody>();
        // _curShootTime = _fixedShootTime;
        foreach (ParticleSystem item in _effects)
        {
            item.Stop();
        }
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if (_npc.isDead) return;
        LoopShoot();
        if (Input.GetKey(KeyCode.R))
        {
            this.DoSkill();
        }
        float speed = _minSpeed;
        Transform gun = _gun;
        if (_bRandom)
        {
            // Transform[] t = _gun.GetComponentsInChildren<Transform>();
            // Transform gun = t[UnityEngine.Random.Range(0, t.Length)];
            speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);
            gun.localRotation = Quaternion.Euler(UnityEngine.Random.Range(-15, -15 - _randomAngle), UnityEngine.Random.Range(-_randomAngle, _randomAngle), 0);
        }
        if (_curNum > 0 && _curShootTime == 0)
        {
            _curShootTime = _fixedShootTime;
            _curNum--;

            foreach (ParticleSystem item in _effects)
            {
                item.Play();
            }
            // 播放巨炮发射音效
            this.PlayPartSound(1, gun);
            string bulletName = _bColor ? "Assets/Res/Prefabs/Component/Weapon/Bullet_Tailgunner_Red.prefab" : "Assets/Res/Prefabs/Component/Weapon/Bullet_Tailgunner_black.prefab";
            MapManager.Instance.baseMap.objectPoolController.Assign(bulletName, delegate (GameObject obj)
            {
                // GameObject obj = UnityEngine.Object.Instantiate<GameObject>(gobj);
                var rt = obj.GetComponent<RobotPartScriptWeaponBoomTailgunner>();
                rt.SetOwner(_npc, this, _tailgunnerSpeed, _explosionTime, _tailgunnerTime, _radius, _view, _playAniTime, _bInfinite, _tirgger, _minRatio);
                obj.transform.position = gun.position;
                obj.transform.eulerAngles = _npc.gameObject.transform.eulerAngles;
                Rigidbody rid = obj.GetComponent<Rigidbody>();
                rid.velocity = _npc.gameObject.GetComponent<Rigidbody>().velocity;
                rid.AddForce(gun.forward * speed, ForceMode.VelocityChange);
            });
        }
    }
    /// <summary>
    /// 自动发射炮弹
    /// </summary>
    public void LoopShoot()
    {
        if (_curShootTime > 0)
        {
            _curShootTime -= Time.fixedDeltaTime;
            if (_curShootTime < 0)
            {
                _curShootTime = 0;
            }
        }
    }

    /// <summary>
    /// 生成炮弹
    /// </summary>

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }
        _curNum = _num;
        return true;
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        // 播放巨炮碰撞音效
        this.PlayPartSound(0, pCollision.GetContact(0).point);
    }

    public void CannonBoomHit(Vector3 pHitPosition)
    {
        // 播放炮弹击中音效
        this.PlayPartSound(2, pHitPosition);
    }
}