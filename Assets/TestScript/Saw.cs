using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public ParticleSystem _ef;
    void Start()
    {
        
    }

    void Update()
    {

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
        _ef.Stop();
        _ef.transform.position = point;
        _ef.Play();
        //Debug.DrawLine(point, -normal * 1000, Color.blue, 3f);
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {

        }
    }
}
