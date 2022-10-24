using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponBoomTailgunner : MonoBehaviour
{
    public float _force = 20f; // 力
    private float _tailgunnerSpeed = 2f; // 追踪速度
    private float _explosionTime = 8f; // 爆炸时间
    private float _tailgunnerTime = 3f; // 跟踪时间
    private float _playAniTime = 13; // 播放爆炸动作时间
    private float _curTailgunnerTime = 0; // 跟踪时间
    private float _curPlayAniTime = 0; // 播放爆炸动作时间
    private float _curExplosionTime = 0; // 爆炸时间
    private float _tirgger = 10f; // 触发爆炸半径
    public float _radius = 15f; // 圆环的半径
    public float _minRatio = 0.8f; // 圆环的半径
    private float _view = 20f; // 视野范围
    private bool _bInfinite = false; // 是否开启追踪
    private RobotPartScriptBase _target; // 追踪目标

    // 是否爆炸
    private bool _bExplosion
    {
        get
        {
            return _curExplosionTime < 0;
        }
    }

    // 是否爆炸动作
    private bool _bPlayAni
    {
        get
        {
            float dis = _target == null ? 99999999 : (_target.transform.position - gameObject.transform.position).magnitude;
            if (_bInfinite)
            {
                return _target != null && dis < _tirgger;
            }
            _curPlayAniTime = dis < _tirgger ? 0 : _curPlayAniTime;
            return _curPlayAniTime < 0;
        }
    }
    private bool _bTailgunner { get { return _curTailgunnerTime < 0; } } // 是否开启跟踪

    public ParticleSystem _effect; // 快爆炸特效
    public GameObject _effectBoss; // boss预警特效
    public GameObject _collider; // 碰撞体
    public float _collideHeight = 1; // 碰撞体高度偏移
    public float _offsetHeight = 10; // 悬浮高度偏移
    public float _posx = 1; // 从中心点往左右偏移距离
    public float _posz = 1; // 从中心点往前后偏移距离
    public float _coefficient = 40; // 浮空系数
    public float _pow = 2; // 浮空曲线
    public float _dragFalg = 5; // 模拟的空气阻力
    public Animation _animation;
    private List<Vector3> _pos = new List<Vector3>();
    private float _curHeight = 0; // 当前悬浮高度
    private Rigidbody _rb;
    private BaseNpc Owner;
    private RobotPartScriptWeaponTailgunner thisPart;
    public List<Vector3> _targetPointList = new List<Vector3>(); // 目标点列表
    private bool bJump = false;
    private bool bSound = false;
    private bool bCollisionEnter = false;
    private void Start()
    {
        _rb.maxAngularVelocity = 100000;
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
    }


    private void Update()
    {
    }


    // 添加一个目标点
    public void AddTargetPoint(Vector3 point)
    {
        _targetPointList.Clear();
        Vector3[] points = RobotFindPath.FindPath(transform.position, point);
        if (points != null)
        {
            foreach (Vector3 pos in points)
            {
                if ((transform.position - pos).magnitude > 1)
                {
                    _targetPointList.Add(pos);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (!gameObject.activeInHierarchy) return;

        if (bCollisionEnter)
            _curTailgunnerTime -= Time.fixedDeltaTime;

        //追踪
        if (_bTailgunner)
        {
            _curPlayAniTime -= Time.fixedDeltaTime;

            _rb.drag = 1;
            _rb.angularDrag = 5;

            // 帮助平稳
            float x = thisPart.ChangeAngle(transform.eulerAngles.x);
            float z = thisPart.ChangeAngle(transform.eulerAngles.z);
            if (Mathf.Abs(x) > 5 || Mathf.Abs(z) > 5)
            {
                x = Mathf.Pow((Mathf.Abs(x) - 5) / 5, 2) * (x > 0 ? -1 : 1);
                z = Mathf.Pow((Mathf.Abs(z) - 5) / 5, 2) * (z > 0 ? -1 : 1);
                _rb.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
            }

            // 悬浮
            _pos[0] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));
            _pos[1] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
            _pos[2] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
            _pos[3] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));

            float g = Physics.gravity.y * -1;
            float G = _rb.mass * g;
            float useForce = G / 4;
            _rb.AddForce(new Vector3(0, -_rb.velocity.y * _dragFalg, 0), ForceMode.Acceleration);
            foreach (Vector3 p in _pos)
            {
                float space = _offsetHeight - p.y;
                float addCoe = Mathf.Pow(Mathf.Abs(space) * _coefficient, _pow) * (space >= 0 ? 1 : -1);
                float addForce = useForce + addCoe;
                Vector3 end = new Vector3(p.x, p.y + -_offsetHeight, p.z);
                // Debug.DrawLine(p, end, Color.yellow, 0.1f);
                _rb.AddForceAtPosition(Vector3.up * addForce, p, ForceMode.Force);
            }

            if (!_target)
            {
                if (!bJump)
                {
                    bJump = true;
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _rb.angularVelocity = Vector3.zero;
                    _rb.velocity = Vector3.zero;
                    PlayAnimation("Prop_zibaojijia_jump");
                }
                if (!_animation.IsPlaying("Prop_zibaojijia_jump"))
                {
                    PlayAnimation("Prop_zibaojijia_idle");
                }

                // 获取目标
                Vector3 explosionPos = transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, _view);

                List<Collider> colliderList = new List<Collider>();
                colliderList.CopyTo(colliders);

                List<RobotPartScriptBase> PartList = new List<RobotPartScriptBase>();

                foreach (Collider hit in colliders)
                {
                    RobotPartScriptBase robotPartScript = hit.gameObject.GetComponentInParent<RobotPartScriptBase>();
                    if (robotPartScript != null)
                    {
                        if (robotPartScript.gameObject.layer != Owner.gameObject.layer && robotPartScript.myElement.GetMyNpc().type != Owner.type)
                        {
                            _target = robotPartScript;
                        }
                    }
                }
            }
            else if (!_bExplosion && !_animation.IsPlaying("Prop_zibaojijia_jump"))
            {
                PlayAnimation("Prop_zibaojijia_run");

                Vector3 curPos = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
                AddTargetPoint(curPos);
                if (_targetPointList.Count > 0)
                {
                    Vector3 targetPoint = new Vector3(_targetPointList[0].x, transform.position.y, _targetPointList[0].z);
                    if ((transform.position - targetPoint).magnitude <= 1)
                    {
                        _targetPointList.RemoveAt(0);
                    }
                    else
                    {
                        // Debug.DrawLine(transform.position, targetPoint, Color.yellow, 0.05f);
                        // Debug.DrawLine(transform.position, transform.forward * 1000, Color.red, 0.05f);
                        // 转向操作
                        Vector3 pos = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
                        Vector3 dis = pos - transform.position;
                        float angle = Vector3.Angle(transform.forward, dis);
                        Vector3 c = Vector3.Cross(transform.forward, dis.normalized);
                        float n = c.y > 0 ? 1 : -1;
                        _rb.AddRelativeTorque(0, Mathf.Min(Mathf.Abs(angle) * 20, 30) * n, 0, ForceMode.Acceleration);
                        // 前进
                        _rb.AddForce(transform.forward * _tailgunnerSpeed / 3.6f, ForceMode.Acceleration);
                    }
                }

            }
        }

        if (_bPlayAni || _curExplosionTime != _explosionTime)
        {
            _curExplosionTime -= Time.fixedDeltaTime;

            _effect.Play();
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            if (!bSound)
            {
                bSound = true;
                thisPart.PlayPartSound(3, transform.position);
            }
            if (Owner.type == NpcType.MonsterNpc)
            {
                _effectBoss.SetActive(true);
            }
        }
        //爆炸
        if (_bExplosion)
        {
            Explosion();
        }
        if (_effectBoss.activeSelf)
        {
            _effectBoss.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetOwner(BaseNpc npc, RobotPartScriptWeaponTailgunner rs, float tailgunnerSpeed, float explosionTime, float tailgunnerTime, float radius, float view, float playAniTime, bool bInfinite, float tirgger, float minRatio)
    {
        Owner = npc;
        thisPart = rs;
        _tailgunnerSpeed = tailgunnerSpeed;
        _explosionTime = explosionTime;
        _tailgunnerTime = tailgunnerTime;
        _radius = radius;
        _view = view;
        _playAniTime = playAniTime;
        _bInfinite = bInfinite;
        _tirgger = tirgger;
        _minRatio = minRatio;
        _effectBoss.gameObject.transform.localScale = new Vector3(_radius / 7, _radius / 7, _radius / 7);
        foreach (var pChildTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            pChildTransform.gameObject.layer = Owner.gameObject.layer;
        }

        _rb = GetComponent<Rigidbody>();
        _curTailgunnerTime = _tailgunnerTime;
        _curPlayAniTime = _playAniTime;
        _curExplosionTime = _explosionTime;
        _effect.Stop();
        _effectBoss.SetActive(false);
        PlayAnimation("Prop_zibaojijia_qiu");
        _animation["Prop_zibaojijia_run"].speed = 2;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        thisPart.PlayPartSound(0, transform.position);
        bCollisionEnter = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    private void Damage(List<RobotPartScriptBase> PartList, Vector3 explosionPos)
    {
        if (PartList.Count == 0)
        {
            return;
        }

        float distance = Vector3.Distance(PartList[0].transform.position, explosionPos);
        float ratio = 1 - (distance / _radius);
        if (ratio <= _minRatio)
        {
            ratio = _minRatio;
        }
        foreach (RobotPartScriptBase pt in PartList)
        {
            if (!pt.myElement.isDead)
            {
                float c = thisPart._npc.GetNpcAttr(AttributeType.Attack) * ratio;
                Owner.DamageOutput(thisPart.myElement, pt.myElement, (int)c);
            }
        }
    }


    public float m_Theta = 0.05f; // 值越低圆环越平滑
    public Color m_Color = Color.green; // 线框颜色
    void OnDrawGizmos()
    {
        if (m_Theta < 0.0001f) m_Theta = 0.0001f;

        // 设置矩阵
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = m_Color;

        // 绘制圆环
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
        {
            float x = _radius * Mathf.Cos(theta);
            float z = _radius * Mathf.Sin(theta);
            Vector3 endPoint = new Vector3(x, 0, z);
            if (theta == 0)
            {
                firstPoint = endPoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endPoint);
            }
            beginPoint = endPoint;
        }

        // 绘制最后一条线段
        Gizmos.DrawLine(firstPoint, beginPoint);

        // 恢复默认颜色
        Gizmos.color = defaultColor;

        // 恢复默认矩阵
        Gizmos.matrix = defaultMatrix;

        _effectBoss.gameObject.transform.localScale = new Vector3(_radius / 7, _radius / 7, _radius / 7);
    }

    private void Explosion()
    {
        //定义爆炸位置为炸弹位置
        Vector3 explosionPos = transform.position;
        //这个方法用来反回球型半径之内（包括半径）的所有碰撞体collider[]
        Collider[] colliders = Physics.OverlapSphere(explosionPos, _radius);

        List<Collider> colliderList = new List<Collider>();
        colliderList.CopyTo(colliders);

        Dictionary<int, Rigidbody> rigList = new Dictionary<int, Rigidbody>();
        List<RobotPartScriptBase> PartList = new List<RobotPartScriptBase>();

        //遍历返回的碰撞体，如果是刚体，则给刚体添加力
        foreach (Collider hit in colliders)
        {
            Rigidbody rig = hit.attachedRigidbody;
            RobotPartScriptBase robotPartScript = hit.gameObject.
                GetComponentInParent<RobotPartScriptBase>();
            if (robotPartScript != null && robotPartScript.gameObject.layer != Owner.gameObject.layer)
                PartList.Add(robotPartScript);
            if (rig != null)
            {
                bool bIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(this.gameObject.layer, rig.gameObject.layer);
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
            float a = Vector3.Distance(rig.gameObject.transform.position, explosionPos);
            float b = 1 - (a / _radius);
            float c = rig.mass * b;
            float d = c * _force;
            rig.AddExplosionForce(d, explosionPos, _radius, 0.30f, ForceMode.Impulse);
        }
        Damage(PartList, explosionPos);

        // 通知巨炮炮弹击中
        thisPart.PlayPartSound(2, transform.position);
        // 特效播放
        MapManager.Instance.baseMap.effectManager.SceneEffectAdd(6, explosionPos, Quaternion.identity);
        BulletRecover();
    }

    private void PlayAnimation(string name)
    {
        if (!_animation.IsPlaying(name))
        {
            _animation.Play(name);
        }
    }

    // 回收还原
    private void BulletRecover()
    {
        bJump = false;
        bSound = false;
        bCollisionEnter = false;
        _target = null;
        _rb.drag = 0.01f;
        _rb.angularDrag = 0.05f;

        MapManager.Instance.baseMap.objectPoolController.Recover(gameObject);
        _targetPointList.Clear();
    }
}