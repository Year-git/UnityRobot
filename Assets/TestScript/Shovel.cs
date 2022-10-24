using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shovel : MonoBehaviour
{
    public float maxMass = 8000f;
    public float forceEnter = 1.2f;
    public float forceStay = 4f;
    public ForceMode forceModeEnter = ForceMode.Impulse;
    public ForceMode forceModeStay = ForceMode.Force;

    void Start()
    {
        
    }

    void Update()
    {
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
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {
            float mass = rigidbody.mass > maxMass? maxMass:rigidbody.mass;
            Vector3 fv = (-normal + Vector3.up).normalized * mass * forceEnter;
            rigidbody.AddForceAtPosition(fv, point, forceModeEnter);
            // Debug.DrawLine(contact.point, fv.normalized * 1000, Color.red, 3f);
            // Debug.DrawLine(contact.point, -contact.normal * 1000, Color.white, 3f);
            // Debug.DrawLine(contact.point, Vector3.up * 1000, Color.blue, 3f);
            // float angle = Mathf.Acos(Vector3.Dot(fv.normalized, Vector3.up)) * Mathf.Rad2Deg;
            // Debug.Log("angle = " + angle);
        }
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
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {
            float mass = rigidbody.mass > maxMass? maxMass:rigidbody.mass;
            Vector3 fv = (-normal + Vector3.up).normalized * mass * forceStay;
            // Debug.DrawLine(contact.point, fv.normalized * 1000, Color.red, 3f);
            // Debug.DrawLine(contact.point, -contact.normal * 1000, Color.white, 3f);
            // Debug.DrawLine(contact.point, Vector3.up * 1000, Color.blue, 3f);
            // float angle = Mathf.Acos(Vector3.Dot(fv.normalized, Vector3.up)) * Mathf.Rad2Deg;
            // Debug.Log("angle = " + angle);
        }
    }

}
