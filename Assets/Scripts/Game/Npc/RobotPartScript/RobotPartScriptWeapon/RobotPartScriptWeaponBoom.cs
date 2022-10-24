using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartScriptWeaponBoom : MonoBehaviour
{

    public float Foece = 10f;
    private float _Radius = 15; // 圆环的半径
    private float _loclScla = 1;
    private float _FixedR = 15f;
    private RobotPartScriptWeaponCannon thisPart;
    private Rigidbody _rigidbody;
    private bool _b = false;
    private int _effectId;
    public float upForce;
    public float minRatio;
    public TrailRenderer taril;
    private void Start()
    {
        _FixedR = 6.5f;
    }


    private void OnEnable()
    {
        _rigidbody = this.gameObject.AddComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (_rigidbody != null) Destroy(_rigidbody);
    }

    public void SetParmer(RobotPartScriptWeaponCannon rs,float radius, float force, float loclScla = 1)
    {
        thisPart = rs;
        _Radius = radius;
        _loclScla = loclScla;
        Foece = force;
    }


    public void SetRigidBody(Vector3 force)
    {
        _rigidbody.mass = 10;
        _rigidbody.drag = 0;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.centerOfMass = new Vector3(0, 0, 0.734f);
        _rigidbody.velocity = thisPart.Npc.gameObject.GetComponent<Rigidbody>().velocity;
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        if (_b || _rigidbody == null || thisPart.Npc.type == NpcType.PlayerNpc) return;
        _b = true;
        
        // 预警特效
        Vector3 velocity = _rigidbody.velocity;
        Vector3 pos = transform.position;
        int PointsCount = 50;
        float Interval = 0.1f;
        float Gravity = Physics.gravity.magnitude;
        List<Vector3> Points = new List<Vector3>();
        for (int i = 0; i < PointsCount; i++)
        {
            Points.Add(pos);
            if (Points.Count > 1)
            {
                Vector3 dis = Points[i] - Points[i - 1];
                Ray _ray = new Ray(Points[i - 1], dis.normalized);
                RaycastHit _raycastHit;
                int layersMask = 1 << LayerMask.NameToLayer("Ground");
                if (Physics.Raycast(_ray, out _raycastHit, dis.magnitude, layersMask))
                {

                    float sclaCicle = _Radius / _FixedR;
                    _effectId = MapManager.Instance.baseMap.effectManager.SceneEffectAdd(14, new Vector3(_raycastHit.point.x, _raycastHit.point.y + 0.1f, _raycastHit.point.z), Quaternion.Euler(0, 0, 0), sclaCicle);
                    break;
                }
            }
            velocity += Vector3.down * Gravity * Interval;
            pos += velocity * Interval;
        }
        // for (int i = 0; i < Points.Count - 1; i++)
        // {
        //     Debug.DrawLine(Points[i], Points[i + 1], Color.yellow, 100f);
        // }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (_effectId != -1)
        {
            MapManager.Instance.baseMap.effectManager.EffectDel(_effectId);
        }
        else
        {
            Debug.Log("_effectId = " + _effectId);
            if (thisPart == null)
            {
                Debug.Log("thisPart == null");
            }
        }
        if (collision.gameObject.tag != "MainCamera")
        {
            //if(MyPlayer)
            //定义爆炸位置为炸弹位置
            Vector3 explosionPos = transform.position;
            //这个方法用来反回球型半径之内（包括半径）的所有碰撞体collider[]
            Collider[] colliders = Physics.OverlapSphere(explosionPos, _Radius);

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
                if (robotPartScript != null && robotPartScript.gameObject.layer != thisPart.Npc.gameObject.layer)
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
                //float distance = Vector3.Distance(rig.gameObject.transform.position, explosionPos);
                //float ratio = 1 - (distance / _Radius);
                float massForce = keyVal.Value.mass * Foece;
                //if(ratio <= minRatio)
                //{
                //    ratio = minRatio;
                //}
                //float result = massForce * ratio;
                keyVal.Value.AddExplosionForce(massForce, explosionPos, _Radius, upForce, ForceMode.Impulse);
            }
            Damage(PartList, explosionPos);

            // 通知巨炮炮弹击中
            thisPart.CannonBoomHit(transform.position);
            //float sclaCicle = _Radius / _FixedR;
            // 特效播放
            MapManager.Instance.baseMap.effectManager.SceneEffectAdd(6, explosionPos, Quaternion.identity);
            Recycle();
        }
    }

    private void Recycle()
    {
        _Radius = 0;
        _loclScla = 0;
        Foece = 0;   
        _effectId = -1;
        _b = false;
        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.transform.eulerAngles = Vector3.zero;
        this.gameObject.transform.localScale = Vector3.one;
        taril.Clear();
        MapManager.Instance.baseMap.objectPoolController.Recover(this.gameObject);
    }

    private void Damage(List<RobotPartScriptBase> PartList, Vector3 explosionPos)
    {
        if (PartList.Count == 0)
        {
            return;
        }

        float distance = Vector3.Distance(PartList[0].transform.position, explosionPos);
        float ratio = 1 - (distance / _Radius);
        if (ratio <= minRatio)
        {
            ratio = minRatio;
        }
        foreach (RobotPartScriptBase pt in PartList)
        {
            if (!pt.myElement.isDead)
            {
                float dValue = thisPart.Npc.GetNpcAttr(AttributeType.Attack) * ratio;
                thisPart.Npc.DamageOutput(thisPart.myElement, pt.myElement, (int)dValue);
            }
        }
    }

    ///  渲染范围  关闭
    ///  如需要可以开启
    //public float m_Theta = 0.05f; // 值越低圆环越平滑
    //public Color m_Color = Color.green; // 线框颜色
    //void OnDrawGizmos()
    //{
    //    if (m_Theta < 0.0001f) m_Theta = 0.0001f;

    //    // 设置矩阵
    //    Matrix4x4 defaultMatrix = Gizmos.matrix;
    //    Gizmos.matrix = transform.localToWorldMatrix;

    //    // 设置颜色
    //    Color defaultColor = Gizmos.color;
    //    Gizmos.color = m_Color;

    //    // 绘制圆环
    //    Vector3 beginPoint = Vector3.zero;
    //    Vector3 firstPoint = Vector3.zero;
    //    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
    //    {
    //        float x = _Radius * Mathf.Cos(theta);
    //        float z = _Radius * Mathf.Sin(theta);
    //        Vector3 endPoint = new Vector3(x, 0, z);
    //        if (theta == 0)
    //        {
    //            firstPoint = endPoint;
    //        }
    //        else
    //        {
    //            Gizmos.DrawLine(beginPoint, endPoint);
    //        }
    //        beginPoint = endPoint;
    //    }

    //    // 绘制最后一条线段
    //    Gizmos.DrawLine(firstPoint, beginPoint);

    //    // 恢复默认颜色
    //    Gizmos.color = defaultColor;

    //    // 恢复默认矩阵
    //    Gizmos.matrix = defaultMatrix;
    //}
}