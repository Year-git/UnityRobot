using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class Claw : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody _body;
    /// <summary>
    ///  爪子的质量
    /// </summary>
    float gameMass;
    public GameObject _parent;
    private Vector3 pos;
    private Vector3 direction;
    Rigidbody _clawedbody;
    private GameObject clawed;
    float speed = 100f;
    bool _bparent = false;
    float dis = 0;
    void Start()
    {
        _body = gameObject.GetComponent<Rigidbody>();
        pos = gameObject.transform.position;
        gameMass = _body.mass;
        direction = Vector3.forward;
        dis = Vector3.Distance(pos, _parent.transform.position);
        GameObject _ed=GameObject.Find("Cylinder (7)");
        //_ed.GetComponent<Rigidbody>().mass = 8;

    }

    // Update is called once per frame
    void Update()
    {
        this.Move();
    }
    private void Move()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //Socket
            //_body.AddForce(new Vector3(0,0,speed),ForceMode.Force);
            //gameObject.transform.Translate(direction*Time.deltaTime*10,Space.World);        
            // float y=Input.GetAxis("Vertical");
            // float x = Input.GetAxis("Horizontal");
            Vector3 dir = new Vector3(0, 0, speed);
            Vector3 _pos = gameObject.transform.position;
            if (_bparent)/// 质量大于车体 父物体移动 
            {
                _body.MovePosition(gameObject.transform.position);
                clawed.GetComponent<Rigidbody>().MovePosition(clawed.transform.position + dir * 0.1f);
                if (clawed.transform.position.z >= _pos.z)
                {
                    clawed.GetComponent<Rigidbody>().MovePosition(clawed.transform.position);
                }
            }
            else
            {
                _body.MovePosition(gameObject.transform.position + dir * 0.1f);
                if (clawed)
                {
                    clawed.GetComponent<Rigidbody>().MovePosition(clawed.transform.position + dir * 0.1f);
                    if (_pos.z <= pos.z)
                    {
                        clawed.GetComponent<Rigidbody>().MovePosition(clawed.transform.position);
                    }
                }
            }            
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (clawed)
        {
            return;
        }
        Rigidbody _bodys = collision.gameObject.GetComponent<Rigidbody>();
        if (!_bodys)
        {
            return;
        }
        float collisionMass = _bodys.mass;
        if (gameMass > collisionMass)  //拉回去
        {
            clawed = collision.gameObject;
            speed = -200f;
        }
        else
        { // 带过去
            clawed = _parent.gameObject;
            _bparent = true;
        }
    }
}