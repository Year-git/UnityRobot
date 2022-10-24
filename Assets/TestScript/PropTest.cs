using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropTest : MonoBehaviour
{
    float a = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        a = a + 0.01f;
        transform.Rotate(0, 1, 0);
        
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Cos(a)/30, transform.position.z);
    }
}
