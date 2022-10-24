using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAIController
{
    public BaseNpc _npcBase { get; private set; }
    public bool _bOpen; //是否开启AI控制器
    private Rigidbody _rigidbody;
    private List<Vector3> _targetPointList = new List<Vector3>(); // 目标点列表
    private List<RobotPartElement> _listElement; // 移动
    private Ray ray;
    private RaycastHit hit;

    public RobotAIController(BaseNpc npcBase, bool bOpen = false)
    {
        _npcBase = npcBase;
        _bOpen = bOpen;
        if (_npcBase != null)
        {
            _rigidbody = _npcBase.gameObject.GetComponent<Rigidbody>();
            _listElement = _npcBase.GetRobotPartElement(RobotPartType.Move);
        }
    }

    // AI控制器刷新帧
    public void AIControllerUpdate()
    {
        if (_npcBase == null || !_bOpen) return;

        // 方便测试 定点
        if (Input.GetMouseButtonDown(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                AddTargetPoint(hit.point);
            }
        }

        // 前往目标点
        if (_targetPointList.Count > 0)
        {
            Vector3 myPoint = _npcBase.gameObject.transform.position;
            Vector3 targetPoint = _targetPointList[0];

            Vector3 myForwardVector = _npcBase.gameObject.transform.forward;
            Vector3 myRightVector = _npcBase.gameObject.transform.right;
            Vector3 targetVector = targetPoint - myPoint;

            Vector3 c = Vector3.Cross(myForwardVector, targetVector);
            // 通过反正弦函数获取向量 a、b 夹角（默认为弧度）
            float radians = Mathf.Asin(Vector3.Distance(Vector3.zero, Vector3.Cross(myForwardVector.normalized, targetVector.normalized)));
            float angle = radians * Mathf.Rad2Deg;
            
            Debug.DrawLine(myPoint, targetPoint, Color.yellow, 0.05f);
            Debug.DrawLine(myPoint, myForwardVector * 1000, Color.red, 0.05f);

            Debug.Log("angle = " + angle + "   c.y = " + c.y);

            if ((myPoint - targetPoint).magnitude <= 0.1f)
            {
                _targetPointList.RemoveAt(0);
                foreach (RobotPartElement element in _listElement)
                {
                    RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
                    moveElement.DownSpace();
                }
            }
            else
            {
                foreach (RobotPartElement element in _listElement)
                {
                    RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
                    // moveElement.FindAdvance(myPoint, targetPoint);
                }
            }
            // if (c.y > 2)
            // {
            //     Debug.Log("右拐？");
            //     foreach (RobotPartElement element in _listElement)
            //     {
            //         RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
            //         moveElement._joInput["Space"] = 0;
            //         moveElement._joInput["W"] = GetSpeed() < 5 ? 1 : 0;
            //         moveElement._joInput["D"] = 1;
            //         moveElement._joInput["A"] = 0;
            //     }
            // }
            // else if (c.y < -2)
            // {
            //     Debug.Log("左拐？");
            //     foreach (RobotPartElement element in _listElement)
            //     {
            //         RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
            //         moveElement._joInput["Space"] = 0;
            //         moveElement._joInput["W"] = GetSpeed() < 5 ? 1 : 0;
            //         moveElement._joInput["A"] = 1;
            //         moveElement._joInput["D"] = 0;
            //     }
            // }
            // else
            // {
            //     Debug.Log("直行？");
            //     foreach (RobotPartElement element in _listElement)
            //     {
            //         RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
            //         moveElement._joInput["Space"] = 0;
            //         moveElement._joInput["W"] = 1;
            //         int randomNum = Random.Range(0, 2);
            //         moveElement._joInput["A"] = Vector3.Cross(myRightVector, targetVector).y > 0 && randomNum == 0 ? 1 : 0;
            //         moveElement._joInput["D"] = Vector3.Cross(myRightVector, targetVector).y > 0 && randomNum == 1 ? 1 : 0;
            //     }
            // }

            // if ((myPoint - targetPoint).magnitude <= 5)
            // {
            //     Debug.Log("到达？");
            //     foreach (RobotPartElement element in _listElement)
            //     {
            //         RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
            //         moveElement._joInput["W"] = 0;
            //         moveElement._joInput["A"] = 0;
            //         moveElement._joInput["D"] = 0;
            //         moveElement._joInput["Space"] = 1;
            //     }
            //     _targetPointList.Clear();
            // }
        }

        if (Mathf.Abs(GetSpeed()) < 0.1f)
        {
            foreach (RobotPartElement element in _listElement)
            {
                RobotPartScriptMove moveElement = (RobotPartScriptMove)element.myScript;
                moveElement._joInput["Space"] = 0;
            }
        }
    }

    // 添加一个目标点
    public void AddTargetPoint(Vector3 point)
    {
        // _targetPointList.Clear();
        _targetPointList.Add(point);
    }

    public float GetSpeed()
    {
        float speed = Mathf.Floor(_npcBase.gameObject.transform.InverseTransformDirection(_rigidbody.velocity).z);
        return speed;
    }
}
