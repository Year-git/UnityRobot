using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)){
            this.GetComponent<Rigidbody>().AddTorque(Vector3.forward * 100,ForceMode.Acceleration);
        }
        if(Input.GetKey(KeyCode.S)){
            this.GetComponent<Rigidbody>().AddTorque(Vector3.forward * - 100,ForceMode.Acceleration);
        }
        if(Input.GetKey(KeyCode.A)){
            this.GetComponent<Rigidbody>().AddTorque(Vector3.left * 100,ForceMode.Acceleration);
        }
        if(Input.GetKey(KeyCode.D)){
            this.GetComponent<Rigidbody>().AddTorque(Vector3.right * 100,ForceMode.Acceleration);
        }
    }
}
