using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjoinColloder : MonoBehaviour
{
    public GameObject _effect;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {

    }

    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {

    }

    // 碰撞持结束
    void OnCollisionExit(Collision collision)
    {
        _effect.SetActive(false);
    }
}
