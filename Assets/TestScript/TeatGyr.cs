using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeatGyr : MonoBehaviour
{
    public bool bStayPlane = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 endPos = new Vector3(transform.position.x,transform.position.y - 5,transform.position.z);
        // Debug.DrawLine(transform.position, endPos, Color.red, 1f);
        // bool grounded  = Physics.Linecast(transform.position, endPos);
        // if (grounded)
        // {
        //     Debug.LogError("发生了碰撞");   
  
        // }
        // else {
        //     Debug.LogError("碰撞结束");
        // }
    }

    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Debug.Log(collision.collider.name);
        bStayPlane = collision.collider.name == "Plane";
    }
}
