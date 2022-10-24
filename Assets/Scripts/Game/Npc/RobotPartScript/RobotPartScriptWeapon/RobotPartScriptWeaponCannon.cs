using System;
using UnityEngine;

public class RobotPartScriptWeaponCannon : RobotPartScriptWeapon
{
    [Header("炮参数")]
    [SerializeField] [HeaderAttribute("枪口")] Transform _gun;
    [SerializeField] [HeaderAttribute("发射时间间隔")] float _fixedShootTime = 1;
    [SerializeField] [HeaderAttribute("发射炮弹数量")] float _num = 5;
    [SerializeField] [HeaderAttribute("是否随机")] bool _bRandom = true;
    [SerializeField] [HeaderAttribute("最小发射炮弹初速度(不随机用该速度)")] float _minSpeed = 20;
    [SerializeField] [HeaderAttribute("最大发射炮弹初速度")] float _maxSpeed = 30;
    [SerializeField] [HeaderAttribute("角度随机范围")] float _randomAngle = 15;

    [Header("炮弹参数")]
    [SerializeField] [HeaderAttribute("爆炸半径")] public float Radius = 15f;
    [SerializeField] [HeaderAttribute("距离系数")] public float distance = 15f;
    [SerializeField] [HeaderAttribute("爆炸力系数")] public float Force = 40f;
    [SerializeField] [HeaderAttribute("技能是否释放")] float _heightForce = 10f;
    [SerializeField] [HeaderAttribute("炮弹的高度系数")] float _forceRatio = 0.8f;
    [Rename("爆炸向上力")] [SerializeField] private float UpForce = 40f;
    [Rename("最小衰减系数")] [SerializeField] private float minRatio = 0.1f;

    private float _curShootTime = 0f; //当前发射时间
    private float _curNum = 0; // 当前发射炮弹数量
    private BaseNpc _npc;
    public BaseNpc Npc
    {
        get { return _npc; }
    }

    protected override void BaseEvent_OnStart()
    {

    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        // if (Input.GetKey(KeyCode.R))
        // {
        //     DoSkill();
        // }
        if (_npc.isDead) return;
        LoopShoot();

        if (_curNum > 0 && _curShootTime == 0)
        {
            _curShootTime = _fixedShootTime;
            _curNum--;
            float speed = _minSpeed;
            Transform gun = _gun;
            if (_bRandom)
            {
                speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);
                gun.localRotation = Quaternion.Euler(
                    UnityEngine.Random.Range(-15, -15 - _randomAngle), UnityEngine.Random.Range(-_randomAngle, _randomAngle), 0);
            }
            // 播放巨炮发射音效
            this.PlayPartSound(2, gun);

            MapManager.Instance.baseMap.objectPoolController.Assign(GetAssetsName(), delegate (GameObject obj)
            {
                obj.layer = _npc.gameObject.layer;
                obj.transform.position = gun.position;
                obj.transform.eulerAngles = _npc.gameObject.transform.eulerAngles;
                var rt = obj.GetComponent<RobotPartScriptWeaponBoom>();
                rt.SetParmer(this, Radius, Force);
                rt.upForce = UpForce;
                rt.minRatio = minRatio;
                rt.SetRigidBody(gun.forward * speed);
            });
        }
    }

    string GetAssetsName()
    {
        string name = "";
        switch (Npc.type)
        {
            case NpcType.PlayerNpc:
                name = "Assets/Res/Prefabs/Component/Weapon/Bullet_Cannon.prefab";
                break;
            case NpcType.MonsterNpc:
                name = "Assets/Res/Prefabs/Component/Weapon/Bullet_Cannon_Monster.prefab";
                break;
        }
        return name;
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

    /// <summary>
    /// 计算发射力(高度一体)
    /// </summary>
    /// <returns></returns>
    Vector3 CalcForce()
    {
        Vector3 f = transform.TransformDirection(Vector3.forward);
        f = f + new Vector3(0, _forceRatio, 0);
        f = f * distance * 100;
        return f;
    }

    /// <summary>   
    /// 单独计算发射力的高度
    /// </summary>
    /// <returns></returns>
    Vector3 CalcFoceHeight()
    {
        Vector3 f = transform.TransformDirection(Vector3.forward);
        f = f + new Vector3(0, 0.8f, 0);
        float Yf = _forceRatio * _heightForce * 100;
        Vector3 zx = f * distance * 100;
        zx += new Vector3(0, Yf, 0);
        return Vector3.zero;
    }


    protected override void BaseEvent_OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        // 播放巨炮碰撞音效
        this.PlayPartSound(0, pCollision.GetContact(0).point);
    }

    public void CannonBoomHit(Vector3 pHitPosition)
    {
        // 播放炮弹击中音效
        this.PlayPartSound(1, pHitPosition);
    }
}