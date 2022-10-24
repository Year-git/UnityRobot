using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class RobotPartScriptMove : RobotPartScriptBase
{
    public List<Vector3> _targetPointList = new List<Vector3>(); // 目标点列表
    public Vector3 _lookTargetPoint;
    public float _speedValue = 1; // 速度倍数
    private float _arriveDis = 2; // 到达的距离
    private bool _arrive = false; // 是否到达
    private int _angle = -1; // -1不执行接口
    private int _targetSteerAngle = 0;
    
    // TODO 临时按键
    public JObject _joInput = new JObject
    {
        ["W"] = 0,
        ["A"] = 0,
        ["S"] = 0,
        ["D"] = 0,
        ["Space"] = 0,
        ["SteerAngle"] = -1,
    };
    
    /// <summary>
    /// 帧同步逻辑帧数据更新调用通知
    /// </summary>
    /// <param name="jInfo"></param>
    protected override void BaseEvent_OnFmSynLogicDataUpdate(Newtonsoft.Json.Linq.JObject jInfo)
    {
        _joInput["W"] = jInfo["W"] != null ? jInfo["W"] : _joInput["W"];
        _joInput["A"] = jInfo["A"] != null ? jInfo["A"] : _joInput["A"];
        _joInput["S"] = jInfo["S"] != null ? jInfo["S"] : _joInput["S"];
        _joInput["D"] = jInfo["D"] != null ? jInfo["D"] : _joInput["D"];
        _joInput["Space"] = jInfo["Space"] != null ? jInfo["Space"] : _joInput["Space"];
        _joInput["SteerAngle"] = jInfo["SteerAngle"] != null ? jInfo["SteerAngle"] : _joInput["SteerAngle"];
    }

    /// <summary>
    /// 帧同步逻辑帧更新调用通知
    /// </summary>
    protected override void BaseEvent_OnFmSynLogicUpdate()
    {
        base.BaseEvent_OnFmSynLogicUpdate();
        // 死亡不接收操作
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        if (npc.isDead) return;
        if ((int)_joInput["Space"] == 1)
        {
            DownSpace();
        }
        if (Input.GetKey(KeyCode.B))
        {
            MoveToAngle(0);
        }
        int SteerAngle = (int)_joInput["SteerAngle"];
        if (SteerAngle == -1)
        {
            // 没有操控摇杆 就看看是否操控键盘了 
            int joInputAngle = 0;
            int flag = (int)_joInput["W"] + (int)_joInput["A"] + (int)_joInput["S"] + (int)_joInput["D"];
            if ((int)_joInput["A"] == 1)
            {
                joInputAngle += 270;
            }
            if ((int)_joInput["S"] == 1)
            {
                joInputAngle += 180;
            }
            if ((int)_joInput["D"] == 1)
            {
                joInputAngle += 90;
            }
            if ((int)_joInput["W"] == 1)
            {
                joInputAngle += 0;
                if (joInputAngle > 180)
                {
                    joInputAngle += 360;
                }
            }

            if (flag > 0)
            {
                _targetPointList.Clear();
                joInputAngle = joInputAngle / flag;
                StartSteerAngle((int)(joInputAngle + Camera.main.transform.eulerAngles.y));
            }
            else
            {
                if(_targetPointList.Count <= 0){
                    DownSpace();
                }
            }
        }
        else
        {
            _targetPointList.Clear();
            StartSteerAngle((int)(SteerAngle + Camera.main.transform.eulerAngles.y));
        }
        //射线检测 点击获取寻路点
        // Ray ray;
        // RaycastHit hit;
        // if (Input.GetMouseButtonDown(0))
        // {
        //     // 主相机屏幕点转换为射线
        //     ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     // 射线碰到了物体
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //          AddTargetPoint(hit.point);
        //     }
        // }

        //向该角度前进
        if(_angle != -1){
            StartSteerAngle(_angle);
        }

        // 寻路
        if (_targetPointList.Count > 0)
        {
            Vector3 myPoint = npc.gameObject.transform.position;
            Vector3 targetPoint = _targetPointList[0];
            // Debug.DrawLine(myPoint, targetPoint, Color.yellow, 0.05f);
            // Debug.DrawLine(myPoint, npc.gameObject.transform.forward * 1000, Color.red, 0.05f);
            if ((myPoint - targetPoint).magnitude <= _arriveDis)
            {
                _targetPointList.RemoveAt(0);
                _arrive = _targetPointList.Count == 0;
                DownSpace();
            }
            else
            {
                LookAtPos(targetPoint);
                FindAdvance(targetPoint);
            }
        }
        else if(npc.type == NpcType.PlayerNpc)
        {
            PlayerNpc my = npc as PlayerNpc;
            if (my.myBehaviacController.aiName == null || my.myBehaviacController.aiName == "")
            {
                if((my.TargetMonster == null) || (MyPlayer.cameraType == 0) || my.isPlayerMoving){
                    //LookAtAngle(Camera.main.transform.eulerAngles.y);
                    LookAtAngle(_targetSteerAngle);
                }else{
                    LookAtPos(my.TargetMonster.gameObject.transform.position);
                }
            }
        }
    }

    // 添加一个目标点
    public bool AddTargetPoint(Vector3 point)
    {
        _targetPointList.Clear();
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Vector3[] points = RobotFindPath.FindPath(npc.gameObject.transform.position, point);
        _arrive = points == null;
        bool bCanReach = false;
        if(points != null)
        {
            foreach (Vector3 pos in points)
            {
                if((npc.gameObject.transform.position - pos).magnitude > _arriveDis){
                    _targetPointList.Add(pos);
                    bCanReach = true;
                }
            }
        }
        return bCanReach;
    }

    // 按键操作
    public virtual void StartSteerAngle(int angle) 
    {
        _targetSteerAngle = angle;
        MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId).SetNpcIsMove();
    }
    
    // 停止
    public virtual void DownSpace() 
    { 
        _targetPointList.Clear();

        MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId).SetNpcIsStand();
    }

    // 减速
    public virtual void SlowDown() { }

    // 寻路
    public virtual void FindAdvance(Vector3 targetPoint)
    {
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Vector3 dis = targetPoint - npc.gameObject.transform.position;
        float angle = Vector3.Angle(npc.gameObject.transform.forward, dis);
        Vector3 c = Vector3.Cross(npc.gameObject.transform.forward, dis.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        StartSteerAngle((int)angle);
    }
     public virtual void MoveToAngle(int angle)
    {
        _angle = angle;
    }
    // 面向(点)
    public virtual void LookAtPos(Vector3 targetPoint)
    {
        _lookTargetPoint = targetPoint;
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Vector3 dis = _lookTargetPoint - npc.gameObject.transform.position;
        float angle = Vector3.Angle(Vector3.forward, dis);
        Vector3 c = Vector3.Cross(Vector3.forward, dis.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        LookAtAngle(angle);
    }

    // 面向(角度)
    public virtual void LookAtAngle(float lookAngle) { }

    // 是否到达目标点
    public virtual bool IsArriveTarget()
    {
        return _arrive;
    }

    // 是否转向完成
    public bool IsLookAt()
    {   
        int npcInstId = myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Vector3 dis = (Vector3)_lookTargetPoint - npc.gameObject.transform.position;
        float angle = Vector3.Angle(npc.gameObject.transform.forward, dis);
        return Mathf.Abs(angle) <= 3f;
    }
}
