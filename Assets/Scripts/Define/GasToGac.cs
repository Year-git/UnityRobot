using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using Framework;

/// <summary>
/// 服务器发送至客户端的消息需要在此注册
/// </summary>
public static class GasToGac
{
    /// <summary>
    /// 登陆成功
    /// </summary>
    public static void C_OnlineSucceed()
    {
    }

    /// <summary>
    /// 上线同步结束
    /// </summary>
    public static void CL_SyncPlayerInfoMsg(JArray data)
    {
        MyPlayer.SyncPlayerInfoMsg(data);
    }

    /// <summary>
    /// 同步玩家已通关的关卡Id
    /// </summary>
    /// <param name="nGameLevelId"></param>
    public static void C_PlayerPassGameLevel(int nGameLevelId)
    {
        MyPlayer.passGameLevelId = nGameLevelId;
    }

    /// <summary>
    /// 同步给客户端服务器时间
    /// </summary>
    public static void C_ServerTime(double nowSecondTime, double nowMilliTime)
    {
        GTime.SetServerTime(nowSecondTime, nowMilliTime);
    }

    /// <summary>
    /// 进入地图
    /// </summary>
    public static void C_EnterMapCfg(int nMapCfgID)
    {
        MapManager.Instance.LoadMap(nMapCfgID);
    }

    /// <summary>
    /// 进入关卡
    /// </summary>
    /// <param name="nGameLevel">关卡Id</param>
    /// <param name="tAllNpcInfo">关卡Npc信息</param>
    public static void C_EnterMap(int nGameLevel, JArray tAllNpcInfo)
    {
        MapManager.Instance.ServerToEnterGameLevel(nGameLevel, tAllNpcInfo);
    }

    /// <summary>
    /// 离开地图
    /// </summary>
    public static void C_LeaveMap()
    {
        MapManager.Instance.ServerToLeaveMap();
    }

    /// <summary>
    /// 上线同步结束
    /// </summary>
    public static void C_OnlineSyncEnd()
    {
        MapManager.Instance.LoadMap(Convert.ToInt32(SenceId.MainSence));
    }

    public static void C_SyncMapObjectIDMsg(object param1)
    {
        // MyPlayer.SyncMapObjectIDMsg(param1);
    }

    /// <summary>
    /// 服务端通知开始帧同步
    /// </summary>
    public static void C_StartFrameSynchron()
    {
        // 废弃服务器帧同步，由客户端接管处理
        // FrameSynchronManager.Instance.Start();
        // LocalFrameSynServer.SynFrameData();
        // MapManager.Instance.baseMap.MapLevelGameStart();
    }

    /// <summary>
    /// 接收服务端关键帧消息
    /// </summary>
    /// <param name="data"></param>
    /// <param name="seed"></param>
    public static void C_PlayerOperationMsg(object data, int seed)
    {
        // 废弃服务器帧同步，由客户端接管处理
        // FrameSynchronManager.Instance.SavePlayerOperation((JArray)data, seed);
    }

    public static void C_LogicFrame(int logicFrame)
    {
        // 废弃服务器帧同步，由客户端接管处理
        // FrameSynchronManager.Instance.LogicFrame(logicFrame);
    }

    public static void C_MapInitEnd()
    {

    }

    /// <summary>
    /// 玩家全部加载成功回掉
    /// </summary>
    public static void CL_ReqLoadSucceed()
    {
    }
}