using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    public bool _bGround = false;
    void Start()
    {

    }

    void Update()
    {

    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _bGround = true;
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if (_bGround) return;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _bGround = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        _bGround = false;
    }
}
