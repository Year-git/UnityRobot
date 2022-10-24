using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public GameObject springPole; //弹簧杆
    float startDis = 0; //弹簧与头的开始距离
    public Rigidbody carRig;
    Rigidbody _rigidbody;
    float skillTime = 0f;
    public float fistForce = 500;
    public float force = 10000;
    [SerializeField] float maxAutoTime = 1f;
    private float autoTime = 0f;
    
    [SerializeField] bool bLeft = false; //是否是左拳
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        startDis = Mathf.Abs(transform.localPosition.z - springPole.transform.localPosition.z);
        if(bLeft){
            autoTime = maxAutoTime * 0.5f;
        }
    }
    void Update()
    {
        if (skillTime > 0)
        {
            skillTime -= Time.deltaTime;
            if (skillTime < 0)
            {
                skillTime = 0;
            }
        }
        if (autoTime > 0)
        {
            autoTime -= Time.deltaTime;
            if (autoTime < 0)
            {
                autoTime = 0;
            }
        }

        if (autoTime == 0)
        {
            autoTime = maxAutoTime;
            if (_rigidbody)
            {
                _rigidbody.AddForce(transform.forward * fistForce, ForceMode.Impulse);
            }
        }

        // Debug.Log(skillTime);
        float curDis = Mathf.Abs(transform.localPosition.z - springPole.transform.localPosition.z);
        // 弹簧长度自适应
        Vector3 scale = new Vector3(springPole.transform.localScale.x, springPole.transform.localScale.y, curDis / startDis);
        springPole.transform.localScale = scale;
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.C))
        {
            skillTime = 0.5f;
            if (_rigidbody)
            {
                _rigidbody.AddForce(transform.forward * 100, ForceMode.Impulse);
            }
        }
    }

    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        List<ContactPoint> Contacts = new List<ContactPoint>();
        int ContactCount = collision.GetContacts(Contacts);
        foreach (ContactPoint item in Contacts)
        {
            point += item.point;
            normal += item.normal;
        }
        point /= ContactCount;
        normal /= ContactCount;
        //Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);
        Rigidbody otherRig = collision.rigidbody;

        SpringAddForce(otherRig, normal, point);
    }

    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        List<ContactPoint> Contacts = new List<ContactPoint>();
        int ContactCount = collision.GetContacts(Contacts);
        foreach (ContactPoint item in Contacts)
        {
            point += item.point;
            normal += item.normal;
        }
        point /= ContactCount;
        normal /= ContactCount;
        //Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);
        Rigidbody otherRig = collision.rigidbody;

        SpringAddForce(otherRig, normal, point);
    }

    private void SpringAddForce(Rigidbody otherRig, Vector3 normal, Vector3 point)
    {
        if (skillTime == 0) return;
        skillTime = 0f;
        float carMass = carRig ? carRig.mass : 0;
        float otherMass = otherRig ? otherRig.mass : 0;
        if (carRig)
        {
            carRig.AddForceAtPosition(normal * carMass * 5f, point, ForceMode.Impulse);
        }
        if (otherRig)
        {
            otherRig.AddForceAtPosition(-normal * otherMass * 8f, point, ForceMode.Impulse);
        }
    }
}
