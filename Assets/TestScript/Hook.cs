using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public GameObject _hook; // 钩子磁铁
    public GameObject _limb; // 钩子手臂
    public float _force; // 力
    private float startDis = 0; //钩子磁铁与钩子手臂的开始距离
    private Rigidbody _hookRigidbody;
    void Start()
    {
        startDis = Mathf.Abs(_hook.transform.localPosition.z - _limb.transform.localPosition.z);
        _hookRigidbody = _hook.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 弹簧长度自适应
        float curDis = Mathf.Abs(transform.localPosition.z - _limb.transform.localPosition.z);
        Vector3 scale = new Vector3(_limb.transform.localScale.x, _limb.transform.localScale.y, curDis / startDis);
        _limb.transform.localScale = scale;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _hookRigidbody.AddForce(_hook.transform.forward * _force, ForceMode.Force);
        }
    }
}
