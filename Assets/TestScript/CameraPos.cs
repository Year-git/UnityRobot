using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VehicleBehaviour;

public class CameraPos : MonoBehaviour
{
    [SerializeField] public Text _textSpeed;
    [SerializeField] public Text _textTime;
    private float lastTime;
    private float curTime;
    public Rigidbody _rigidbody;

    void Start()
    {
        lastTime = Time.time;
    }

    void Update()
    {
        curTime = Time.time;
        StringBuilder t = new StringBuilder();
        t.Append("计时:   ");
        t.Append(((int)(curTime)).ToString());
        t.Append(" s");
        _textTime.text = t.ToString();
        StringBuilder sb = new StringBuilder();
        sb.Append("当前速度:");
        sb.Append((Math.Round(_rigidbody.velocity.magnitude / 3.6f, 1)).ToString());
        sb.Append(" m/s  ");
        sb.Append((Math.Round(_rigidbody.velocity.magnitude)).ToString());
        sb.Append(" kph");
        _textSpeed.text = sb.ToString();


        //射线检测 点击获取寻路点
        Ray ray;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 v = new Vector3(UnityEngine.Random.Range(-10, 10),UnityEngine.Random.Range(-10, 10),UnityEngine.Random.Range(-10, 10));
                float f = UnityEngine.Random.Range(5, 10);
                hit.rigidbody.AddForceAtPosition(v.normalized * hit.rigidbody.mass * f, hit.point, ForceMode.Impulse);
                Debug.DrawLine(hit.point, v.normalized * 500);
            }
        }
    }
}
