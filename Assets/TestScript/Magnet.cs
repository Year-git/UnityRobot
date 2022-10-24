using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float F = 50f;
    void Start()
    {
        
    }

    void FixedUpdate()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == transform.name || !other.attachedRigidbody) return;
        Vector3 otherPos = other.attachedRigidbody.position;
        Vector3 n = transform.position - otherPos;//* other.attachedRigidbody.mass
        other.attachedRigidbody.AddForce(n.normalized * F / n.magnitude, ForceMode.Force);
    }
}
