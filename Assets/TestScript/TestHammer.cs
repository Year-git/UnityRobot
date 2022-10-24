using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHammer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (curTime > 0)
        {
            curTime -= Time.fixedDeltaTime;
        }
    }

    public float curTime = 0;
    public float FixedTime = 1f;

    public float Force = 50000;
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.transform.GetComponent<Rigidbody>() && curTime <= 0)
        {
            var rg = col.gameObject.transform.GetComponent<Rigidbody>();
            Vector3 Apoint = Vector3.zero;
            Vector3 ANormal = Vector3.zero;

            foreach (var point in col.contacts)
            {
                Apoint += point.point;
                ANormal += (point.normal* -1);
            }

            Apoint /= col.contacts.Length;
            ANormal /= col.contacts.Length;

            rg.AddForceAtPosition(ANormal * Force, Apoint,ForceMode.Impulse);
            //rg.AddRelativeForce(ANormal* Force,ForceMode.VelocityChange);
            //Debug.DrawLine(Apoint, ANormal * 1000, Color.red, 3f);
            curTime = FixedTime;
        }
    }



}
