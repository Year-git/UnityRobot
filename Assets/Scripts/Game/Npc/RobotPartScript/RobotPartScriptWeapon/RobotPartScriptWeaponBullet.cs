using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPartScriptWeaponBullet : MonoBehaviour
{
    /// <summary>
    /// 是否开启回收
    /// </summary>
    private bool isOpen = false;
    /// <summary>
    /// 开启回收时长
    /// </summary>
    private float OpenTime = 2f;

    private float curOpenTime = 0f;

    private RobotPartScriptWeaponMachineGun owner;

    private Rigidbody tRigidbody;

    private int Layer;
    private int effecttype;

    public TrailRenderer taril;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        tRigidbody = this.gameObject.AddComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (tRigidbody != null) Destroy(tRigidbody);
    }


    public void setOwner(RobotPartScriptWeaponMachineGun ow,int ly,int etype)
    {
        owner = ow;
        Layer = ly;
        effecttype = etype;
    }

    public void SetRigidBody(Vector3 force,Vector3 velocity)
    {
        tRigidbody.useGravity = false;
        tRigidbody.freezeRotation = true;
        tRigidbody.mass = 10;
        tRigidbody.velocity = velocity;
        tRigidbody.AddForce(force, ForceMode.Acceleration);
    }
    
    public void FixedUpdate()
    {
        if (isOpen == true && this.gameObject.activeInHierarchy == true)
        {
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.eulerAngles = Vector3.zero;
            taril.Clear();
            isOpen = false;
            curOpenTime = 0;
            MapManager.Instance.baseMap.objectPoolController.Recover(this.gameObject);
        }
        else if(isOpen == false && this.gameObject.activeInHierarchy == true)
        {
            if (curOpenTime >= OpenTime)
            {
                isOpen = true;
            }
            else
            {
                curOpenTime += Time.fixedDeltaTime;
            }
        }
    }


    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "MainCamera")
        {
            if (collision.gameObject.layer == Layer)
                return;
            MapManager.Instance.baseMap.effectManager.SceneEffectAdd(effecttype,collision.GetContact(0).point,
                Quaternion.Euler(-90,0,0));
            isOpen = true;
            RobotPartScriptBase robotPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
            if (robotPartScript == null || owner.Npc == null)
            {
                return;
            }

            RobotPartElement targetElement = robotPartScript.myElement;
            if (targetElement.isDead)
            {
                return;
            }
            int attack = owner.Npc.GetNpcAttr(AttributeType.Attack); // 10;
            owner.Npc.DamageOutput(owner.myElement, targetElement, attack);
        }
    }


}
