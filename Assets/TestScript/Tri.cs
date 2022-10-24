using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tri : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("OnTriggerEnter = " + collider.gameObject.name);
    }
    void OnTriggerStay(Collider collider)
    {
        Debug.Log("OnTriggerStay = " + collider.gameObject.name);
    }
    void OnTriggerExit(Collider collider)
    {
        Debug.Log("OnTriggerExit = " + collider.gameObject.name);
    }
}
