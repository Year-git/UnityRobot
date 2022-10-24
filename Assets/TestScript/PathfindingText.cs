using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingText : MonoBehaviour
{
    [SerializeField] WheelCollider wheel;
    void Start()
    {
        
    }

    void Update()
    {
        Ray ray;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.blue, 3f);
                
            }
        }
    }
}
