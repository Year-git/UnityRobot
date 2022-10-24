using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[SerializeField]
public class CollInfo
{
    public float deltap = 0;
    public float cos = 0;
}

public enum CollisionType
{
    Weap,
    CarBody
}


public class LinshiYT : MonoBehaviour
{
    //撞击信息
    public List<CollInfo> InfList = new List<CollInfo>();

    //收集撞击信息的时间
    public float collectTime = 0.1f;

    //当前收集撞击信息时间
    float curTime = 0f;

    //当前伤害检测有效时间
    float validTime = 0;

    //下次伤害有效时间
    public float fixedValidTime = 1f;

    //是否撞击状态
    bool isCollision = false;

    //要碰撞的tag
    //public string Tag = "Weap";

    public CollisionType CType = CollisionType.Weap;

    // Start is called before the first frame update
    void Start()
    {
        if(HitEffect)
            HitEffect.Stop();
        //Debug.Log("Rotate: " + Quaternion.Euler(180, 0, 0));
    } 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0,0,1)*500000);
        }
    }


    private void FixedUpdate()
    {
        if (isCollision && validTime <= 0)
        {
            curTime += Time.fixedDeltaTime;
            if (curTime >= collectTime)
            {
                curTime = 0f;
                isCollision = false;
                validTime = fixedValidTime;
                if (InfList.Count == 0) return;
                ComDamage();
            }
        }

        if (!(validTime > 0)) return;
        validTime -= Time.fixedDeltaTime;
        if (InfList.Count > 0)
        {
            InfList.Clear();
        }
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    private void ComDamage()
    {
        float dmg = 0;
        float index = 0;
        float cos = 0;
        foreach (var t in InfList)
        {
            dmg += t.deltap;
            //cos += t.cos;
            index++;
        }
        cos = InfList[0].cos;
        InfList.Clear();
        //Debug.Log(cos / index);
        var speed = dmg / index;
        var li = speed * cos;
        Debug.Log("角度" + cos);
        Debug.Log("原值"  + " :" + Mathf.Abs(speed));
        Debug.Log("乘角度值" + " :" + Mathf.Abs(li));
    }

    public Vector3 Normal;
    public ParticleSystem HitEffect;
    private void OnCollisionEnter(Collision collision)
    {
        isCollision = true;
        switch (collision.collider.tag)
        {
            case "Weap":
                if (CType != CollisionType.Weap)
                {
                    WeapLogic(collision);
                }
                break;
            case "CarBody":
                if (CType != CollisionType.CarBody)
                {
                    CarBodyLogic(collision);
                }
                break;
        }
    }

    public Vector3 relativeVelocity;
    private void WeapLogic(Collision collision)
    {
        Normal = collision.GetContact(0).normal;
        relativeVelocity = collision.relativeVelocity;
        if (CType == CollisionType.CarBody)
        {
            var sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
            var speedSqrt = Mathf.Sqrt(sqrMagnitude);
            //var collGo = collision.gameObject.GetComponent<LinshiYT>();
            var orNormal = Normal*-1;
            var orNormalZ = new Vector3(Mathf.Abs(orNormal.x), Mathf.Abs(orNormal.y), Mathf.Abs(orNormal.z));
            var speedNormal = collision.relativeVelocity.normalized;
            var xq = orNormalZ.x * speedNormal.x + orNormalZ.y * speedNormal.y + orNormalZ.z * speedNormal.z;
            var sq = Mathf.Sqrt(orNormalZ.sqrMagnitude) * Mathf.Sqrt(speedNormal.sqrMagnitude);
            var cos = xq / sq;


            var relaSpeed = speedSqrt * cos;
            Debug.Log(Mathf.Abs(relaSpeed));
            Debug.DrawLine(collision.GetContact(0).point, -Normal * 1000, Color.red, 3f);
            if (HitEffect)
                HitEffect.Play();
        }
    }

    private void CarBodyLogic(Collision collision) 
    {
        Normal = collision.GetContact(0).normal;
        relativeVelocity = collision.relativeVelocity;
    }

    private void OnCollisionStay(Collision collision)
    {

    }



}
