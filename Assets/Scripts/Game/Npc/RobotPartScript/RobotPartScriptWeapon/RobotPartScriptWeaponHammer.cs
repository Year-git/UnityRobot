using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponHammer : RobotPartScriptWeapon
{
    [Rename("力度")] public float Force = 50000;
    [Rename("爆炸半径")] public float _Radius = 10;
    [Rename("爆炸力系数")] public float Explosionforce = 100f;
    [Rename("向上力")][SerializeField] private float _upForce = 25;
    [Rename("作用力时间间隔")] [SerializeField] private float _FTime = 0.2f;
    [Space(7)] [Rename("转动轴承")] public GameObject Bearing;
    private float CollisionTime = 0;
    [Rename("技能伤害间隔时间")][SerializeField] private float CollisionFixedTime = 1.5f;
    [Rename("技能特效物体")]public GameObject SkillObj;
    [Rename("爆炸点物体")]public Transform ExplosionPoint;//爆炸点
    [Rename("技能动画")] public Animator SkillAnimator;


    private HingeJoint _Joint;
    private Rigidbody _rigidbody;
    private int Layer;
    private bool release; //技能是否释放
    private float skillTime = 0f;
    private BaseNpc _Npc;
    private Dictionary<int, float> _enemyTime = new Dictionary<int, float>();
    private List<int> _desKey = new List<int>();
    private List<int> _forKey = new List<int>();

    protected override void BaseEvent_OnStart()
    {
    }

    protected override void BaseEvent_OnInstall(Action fLoaded)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        _Npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Rigidbody npcrid = _Npc.gameObject.GetComponent<Rigidbody>();
        SetCommected(npcrid);
        Layer = _Npc.gameObject.layer;
        CollisionTime = CollisionFixedTime;
        base.BaseEvent_OnInstall(fLoaded);
    }

    void SetCommected(Rigidbody rb)
    {
        _rigidbody = Bearing.GetComponent<Rigidbody>();
        _Joint = Bearing.GetComponent<HingeJoint>();
        _Joint.connectedBody = rb;
    }  

    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        if (CollisionTime > 0)
        {
            CollisionTime -= Time.fixedDeltaTime;
        }

        if(_enemyTime!=null)
        {
            if(_enemyTime.Count > 0)
            {
                _forKey.Clear();
                foreach (var v in _enemyTime)
                {
                    _forKey.Add(v.Key);
                }

                for (int i = 0; i < _forKey.Count; i++)
                {
                    if (_enemyTime[_forKey[i]] <= 0)
                    {
                        _desKey.Add(_forKey[i]);
                    }
                    else
                    {
                        _enemyTime[_forKey[i]] -= Time.fixedDeltaTime;
                    }
                }

                for (int i = 0; i < _desKey.Count; i++)
                {
                    if(_enemyTime.ContainsKey(_desKey[i]))
                    {
                        _enemyTime.Remove(_desKey[i]);
                    }
                }
                _desKey.Clear();
            }
        }
    }

    public override bool DoSkill()
    {
        base.DoSkill();
        if (myElement.isDead)
        {
            return false;
        }
        Layer = _Npc.gameObject.layer;

        // 播放技能音效
        this.PlayPartSound(2, transform);

        SkillObj.SetActive(true);
        skillTime = SkillAnimator.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(DelaySkillEfffect());
        return true;
    }
    
    IEnumerator DelaySkillEfffect()
    {
        if (skillTime < 0.1f)
            skillTime = 0.2f;
        yield return new WaitForSeconds(0.5f);
        SkillInstance();
    }

    protected override void BaseEvent_OnPartElementCollisionEnter(Collider myCollider, Collision collision, RobotPartScriptBase pTargetScript)
    {
        if (pTargetScript != null)
        {
            if (myCollider.gameObject.name == "HammerBox")
            {
                if (collision.gameObject.layer == Layer) return;
                RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
                if (pTargetPartScript == null)
                {
                    AddForce(collision);
                }
                else
                {
                    RobotPartElement targetElement = pTargetPartScript.myElement;
                    if (targetElement.isDead)
                    {
                        return;
                    }

                    for (int i = 0; i < _enemyTime.Count; i++)
                    {
                        if (_enemyTime.ContainsKey(targetElement.GetMyNpc().InstId))
                        {
                            return;
                        }
                        else
                        {
                            _enemyTime.Add(targetElement.GetMyNpc().InstId, _FTime);
                            AddForce(collision);
                        }
                    }

                    if (_enemyTime.Count == 0)
                    {
                        _enemyTime.Add(targetElement.GetMyNpc().InstId, _FTime);
                        AddForce(collision);
                    }
                    
                }


                if (CollisionTime <= 0)
                {

                    //Debug.DrawLine(Apoint, ANormal * 1000, Color.red, 10f);
                    //添加特效
                    Vector3 pVector3 = myCollider.transform.rotation.eulerAngles;
                    pVector3.x = pVector3.x - 90f;

                    MapManager.Instance.baseMap.effectManager.SceneEffectAdd(5,
                        myCollider.gameObject.transform.position, Quaternion.Euler(pVector3));

                    DamageOut(myCollider, collision);

                    // 播放击中音效
                    this.PlayPartSound(1, collision.GetContact(0).point);
                }
            }
        }

        // 播放碰撞音效
        this.PlayPartSound(0, collision.GetContact(0).point);
    }

    /// <summary>
    /// 伤害输出
    /// </summary>
    /// <param name="myCollider"></param>
    /// <param name="collision"></param>
    private void DamageOut(Collider myCollider, Collision collision)
    {
        RobotPartScriptBase pTargetPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pTargetPartScript == null)
        {
            return;
        }
        RobotPartType eMyType = this.myElement.myRobotPart.partType;
        if (eMyType != RobotPartType.Weapon)
        {
            return;
        }

        RobotPartElement targetElement = pTargetPartScript.myElement;
        if (targetElement.isDead)
        {
            return;
        }

        BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
        if (pMyNpc == null)
        {
            return;
        }

        float nSquaredLength = collision.relativeVelocity.sqrMagnitude;
        float nLength = collision.relativeVelocity.magnitude;

        float nSpeed = 2;
        if (nSpeed > collision.relativeVelocity.magnitude)
        {
            return;
        }
        float Rspeed = Mathf.Abs((float)CalcDamage(collision));
        int attack = _Npc.GetNpcAttr(AttributeType.Attack);
        float calcattack = attack * 4;//(Rspeed / 33);
        pMyNpc.DamageOutput(this.myElement, targetElement, (int)calcattack);
        CollisionTime = CollisionFixedTime;
    }

    /// <summary>
    /// 添加额外力
    /// </summary>
    /// <param name="collision"></param>
    private void AddForce(Collision collision)
    {
        var rg = collision.rigidbody;
        if (rg)
        {
            Vector3 Apoint = Vector3.zero;
            Vector3 ANormal = Vector3.zero;

            foreach (var point in collision.contacts)
            {
                Apoint += point.point;
                ANormal += (point.normal * -1);
            }

            Apoint /= collision.contacts.Length;
            ANormal /= collision.contacts.Length;
            float f = rg.mass * Force;
            rg.AddForceAtPosition(this.gameObject.transform.forward * f, Apoint, ForceMode.Impulse);
            float uf = rg.mass * _upForce;
            rg.AddForceAtPosition(new Vector3(0,1,0) * uf, Apoint , ForceMode.Impulse);
        }
    }


    private void SkillInstance()
    {
        SkillObj.SetActive(false);
        SkillObj.transform.GetChild(0).eulerAngles = Vector3.zero;

        Collider[] colliders = Physics.OverlapSphere(ExplosionPoint.position, _Radius);
        List<Collider> colliderList = new List<Collider>();
        colliderList.CopyTo(colliders);
        Dictionary<int, Rigidbody> rigList = new Dictionary<int, Rigidbody>();
        List<RobotPartScriptBase> PartList = new List<RobotPartScriptBase>();

        //遍历返回的碰撞体，如果是刚体，则给刚体添加力
        foreach (Collider hit in colliders)
        {
            Rigidbody rig = hit.attachedRigidbody;
            if (rig != null)
            {
                bool bIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(this.gameObject.layer, rig.gameObject.layer);
                RobotPartScriptBase robotPartScript = hit.gameObject.GetComponentInParent<RobotPartScriptBase>();
                if (robotPartScript != null && !bIgnoreLayerCollision)
                {
                    PartList.Add(robotPartScript);
                }
                if (!bIgnoreLayerCollision)
                {
                    if (!rigList.ContainsKey(rig.GetInstanceID()))
                    {
                        rigList.Add(rig.GetInstanceID(), rig);
                    }
                }
            }
        }

        foreach (var keyVal in rigList)
        {
            Rigidbody rig = keyVal.Value;
            float a = Vector3.Distance(rig.gameObject.transform.position, ExplosionPoint.position);
            float b = 1 - (a / _Radius);
            float c = rig.mass * b;
            float d = c * Explosionforce;
            float L = Explosionforce * rig.mass;
            rig.AddExplosionForce(L, ExplosionPoint.position, _Radius, 0.30f, ForceMode.Impulse);
        }
        SkillDamage(PartList, ExplosionPoint.position);
    }


    float CalcDamage(Collision collision)
    {
        Vector3 ANormal = Vector3.zero;
        foreach (var point in collision.contacts)
        {
            ANormal += (point.normal * -1);
        }
        ANormal /= collision.contacts.Length;
        var sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
        var speedSqrt = Mathf.Sqrt(sqrMagnitude);
        var orNormalZ = new Vector3(Mathf.Abs(ANormal.x), Mathf.Abs(ANormal.y), Mathf.Abs(ANormal.z));
        var speedNormal = collision.relativeVelocity.normalized;
        var xq = orNormalZ.x * speedNormal.x + orNormalZ.y * speedNormal.y + orNormalZ.z * speedNormal.z;
        var sq = Mathf.Sqrt(orNormalZ.sqrMagnitude) * Mathf.Sqrt(speedNormal.sqrMagnitude);
        var cos = xq / sq;
        float relaSpeed = speedSqrt * cos;
        return relaSpeed;
    }

    private void SkillDamage(List<RobotPartScriptBase> PartList, Vector3 explosionPos)
    {
        BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
        if (pMyNpc == null)
        {
            return;
        }

        if (PartList.Count == 0)
        {
            return;
        }

        float a = Vector3.Distance(PartList[0].transform.position, explosionPos);
        float b = 1 - (a / _Radius);
        foreach (RobotPartScriptBase pt in PartList)
        {
            if (!pt.myElement.isDead)
            {
                float c = pMyNpc.GetNpcAttr(AttributeType.Attack) * b;
                pMyNpc.DamageOutput(this.myElement, pt.myElement, (int)c);
            }
        }
    }

    protected override void BaseEvent_OnPartElementDead()
    {
        // 不删除 模型
        // Destroy(_Joint);
        // Destroy(_rigidbody);
        base.BaseEvent_OnPartElementDead();
    }

}