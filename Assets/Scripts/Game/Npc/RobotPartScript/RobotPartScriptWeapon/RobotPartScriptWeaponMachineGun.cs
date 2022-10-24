using UnityEngine;
using System.Collections;
using System;

public class RobotPartScriptWeaponMachineGun : RobotPartScriptWeapon
{
    private BaseNpc _npc;
    public BaseNpc Npc { get { return _npc; } }
    //枪口
    public Transform[] Gun;
    public GameObject[] GunEffect;
    //[SerializeField] private int BulletType = 0;
    [SerializeField] [HeaderAttribute("子弹数量")] private int BulletNumber = 15;
    [SerializeField] [HeaderAttribute("子弹时间")] private float BulletTime = 5;
    [SerializeField] [HeaderAttribute("散射最大角度X")] [Range(-15f, 15)] private float XMaxAngle = 10f;
    [SerializeField] [HeaderAttribute("散射最小角度X")] [Range(-15f, 15)] private float XMinAngle = -10f;
    [SerializeField] [HeaderAttribute("散射最大角度Y")] [Range(-15f, 15)] private float YMaxAngle = 10f;
    [SerializeField] [HeaderAttribute("散射最小角度Y")] [Range(-15f, 15)] private float YMinAngle = -10f;
    [SerializeField] [HeaderAttribute("力度")] private float MaxForce = 5000f;

    /// <summary>
    /// 发射间隔时间
    /// </summary>
    private float FixedShootTime = 0.2f;

    /// <summary>
    /// 当前子弹剩余数量
    /// </summary>
    private int CurBulletNum = 0;

    /// <summary>
    /// 当前发射时间
    /// </summary>
    private float CurShootTime = 0f;

    private int GunIndex = 0;

    protected override void BaseEvent_OnStart()
    {

    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _npc = MapManager.Instance.baseMap.GetNpc(npcInstId);

        FixedShootTime = BulletTime / BulletNumber;
        CurBulletNum = 0;
        foreach (var Ge in GunEffect)
        {
            Ge.SetActive(false);
        }
        base.BaseEvent_OnInstall(fLoaded);
    }

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        //临时代码-----------------------
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    if (CurBulletNum > 0) return;
        //    FixedShootTime = BulletTime / BulletNumber;
        //    CurBulletNum = BulletNumber;
        //}
        //------------------------------
        if (_npc.isDead) return;
        if (CurBulletNum > 0)
        {
            foreach (var Ge in GunEffect)
            {
                if (Ge.activeInHierarchy == false)
                    Ge.SetActive(true);
            }
        }
        else
        {
            foreach (var Ge in GunEffect)
            {
                if (Ge.activeInHierarchy == true)
                    Ge.SetActive(false);
            }
            return;
        }
        LoopShoot();
    }

    /// <summary>
    /// 自动发射炮弹
    /// </summary>
    public void LoopShoot()
    {
        if (CurShootTime > 0)
        {
            CurShootTime -= Time.fixedDeltaTime;
        }
        else
        {
            CurBulletNum -= 1;
            GunIndex += 1;
            if (GunIndex == 2)
            {
                GunIndex = 0;
            }
            Bullet();
            CurShootTime = FixedShootTime;
        }
    }


    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }
        if (CurBulletNum > 0) return false;
        FixedShootTime = BulletTime / BulletNumber;
        CurBulletNum = BulletNumber;
        this.PlayPartSound(1, transform);
        return true;
    }

    void Bullet()
    {
        MapManager.Instance.baseMap.objectPoolController.Assign(GetAssetsName(out int effectid), delegate (GameObject obj)
        {
            obj.layer = _npc.gameObject.layer;
            obj.transform.position = Gun[GunIndex].transform.position;
            obj.transform.eulerAngles = _npc.gameObject.transform.eulerAngles;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(true);
            var rb = obj.GetComponent<RobotPartScriptWeaponBullet>();
            float x = UnityEngine.Random.Range(XMinAngle, XMaxAngle);
            float y = UnityEngine.Random.Range(YMinAngle, YMaxAngle);
            rb.setOwner(this, this.gameObject.layer, effectid);
            Vector3 force = _npc.gameObject.transform.TransformDirection(Vector3.forward) * MaxForce;
            Vector3 velocity = _npc.gameObject.GetComponent<Rigidbody>().velocity + new Vector3(x, y, 0);
            rb.SetRigidBody(force, velocity);
        });
    }


    string GetAssetsName(out int eid)
    {
        string name = "";
        eid = 0;
        switch (Npc.type)
        {
            case NpcType.PlayerNpc:
                name = "Assets/Res/Prefabs/Component/Weapon/Bullet_Gun_Blue.prefab";
                eid = 17;
                break;
            case NpcType.MonsterNpc:
                name = "Assets/Res/Prefabs/Component/Weapon/Bullet_Gun_Red.prefab";
                eid = 16;
                break;
        }
        return name;
    }


    protected override void BaseEvent_OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        // 播放巨炮碰撞音效
        this.PlayPartSound(0, pCollision.GetContact(0).point);
    }

    public void CannonBoomHit(Vector3 pHitPosition)
    {
        // 播放炮弹击中音效
        // this.PlayPartSound(1, pHitPosition);
    }


}
