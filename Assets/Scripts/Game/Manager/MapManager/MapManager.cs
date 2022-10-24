using Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 地图管理类、单例模式
/// </summary>
public class MapManager : Singleton<MapManager>
{
    /// <summary>
    /// 当前地图id
    /// </summary>
    public int curMapCfgId { get; private set; }
    
    /// <summary>
    /// 地图注册类
    /// </summary>
    private Dictionary<int, Type> _mapClass = new Dictionary<int, Type>();

    /// <summary>
    /// 当前地图基类
    /// </summary>
    public BaseMap baseMap { get; private set; }

    /// <summary>
    /// 记录地图加载成功回调
    /// </summary>
    private Action _callBack;

    /// <summary>
    /// 当前地图是否加载完成
    /// </summary>
    public bool loadEnd 
    {
        get 
        {
            if (baseMap == null)
            {
                return false;
            }
            return baseMap.isLoadEnd;
        } 
    }
    /// <summary>
    /// 地图AB字典
    /// </summary>
    public Dictionary<string, AssetBundle> MapABAssets = new Dictionary<string, AssetBundle>();

    public void Start()
    {
        GEvent.RegistEvent(GacEvent.Update, Update);
        GEvent.RegistEvent(GacEvent.LateUpdate, LateUpdate);
        GEvent.RegistEvent(GacEvent.FixedUpdate, FixedUpdate);

        _mapClass.Add(-1, typeof(LoginMap));
        _mapClass.Add(0, typeof(MainMap));
        _mapClass.Add(1, typeof(BattleMap));
    }

    public void OnDestroy()
    {
        GEvent.RemoveEvent(GacEvent.Update, Update);
        GEvent.RemoveEvent(GacEvent.LateUpdate, LateUpdate);
        GEvent.RemoveEvent(GacEvent.FixedUpdate, FixedUpdate);
    }

    public void Update()
    {
        if (!loadEnd) return;
        baseMap.OnUpdate();
    }

    public void LateUpdate()
    {
        if (!loadEnd) return;
        baseMap.OnLateUpdate();
    }

    public void FixedUpdate()
    {
        if (!loadEnd)
        {
            if (baseMap != null)
            {
                baseMap.LoadProgressUpdate();
            }
            return;
        }
        baseMap.OnFixedUpdate();
    }

    private void OnDelete()
    {
        //if (!LoadEnd) return;
        if (baseMap != null)
        {
            FrameSynchronManager.Instance.OnDestroy();
            baseMap.OnExit();
            LuaGEvent.DispatchEventToLua(GacEvent.MapOnExit, curMapCfgId);
            baseMap = null;
        }
    }
    /// <summary>
    /// 加载地图
    /// </summary>
    /// <param name="nMapCfgID"></param>
    public void LoadMap(int nMapCfgID, int nGameLevelId = 0, JArray jNpcInfo = null, Action fCallBack = null)
    {
        RobotPartScriptAssistSplit._bOnes = false;
        if (nMapCfgID == curMapCfgId)
        {
            if(fCallBack!=null)
            {
                fCallBack();
            }    
            return;
        }
        _callBack = fCallBack;
        //先销毁
        OnDelete();
        //再创建
        curMapCfgId = nMapCfgID;
        //地图类型
        int playerType = ConfigManager.GetValue<int>("Scene_C", nMapCfgID, "playType");
        Type type = _mapClass[playerType];
        baseMap = Activator.CreateInstance(type, curMapCfgId) as BaseMap;

        // 缓存地图的关卡Id
        baseMap.gameLevelId = nGameLevelId;
        // 缓存地图的Npc信息
        baseMap.npcInfoContainer = jNpcInfo;

        //通知lua
        LuaGEvent.DispatchEventToLua(GacEvent.MapOnLoad, curMapCfgId);
        //加载地图
        baseMap.OnLoad(nMapCfgID, Loaded);
    }

    /// <summary>
    /// 加载完成
    /// </summary>
    public void Loaded()
    {
        if (!loadEnd) return;
        // LuaGEvent.DispatchEventToLua(GacEvent.MapOnEnter, curMapCfgId);
        if (_callBack != null)
        {
            _callBack();
        }
    }

    /// <summary>
    /// 进入地图
    /// </summary>
    // public void ServerToEnterMap(int nMapCfgID, JArray jAllNpcInfo)
    // {
    //     Instance.LoadMap(nMapCfgID);
    // }

    /// <summary>
    /// 进入关卡
    /// </summary>
    /// <param name="nGameLevel"></param>
    /// <param name="jAllNpcInfo"></param>
    public void ServerToEnterGameLevel(int nGameLevel, JArray jAllNpcInfo)
    {
        Instance.LoadMap(BaseMap.GetGameLevelSceneId(nGameLevel), nGameLevel, jAllNpcInfo);
    }

    /// <summary>
    /// 离开地图
    /// </summary>
    public void ServerToLeaveMap()
    {
        Instance.LoadMap(Convert.ToInt32(SenceId.MainSence));
    }

    /// <summary>
    /// 服务器行为通知接口
    /// </summary>
    /// <param name="data"></param>
    public void C_Mod(JArray jData)
    {
    }

    /// <summary>
    /// 获取当前地图加载进度值
    /// </summary>
    public int progressValue
    {
        get
        {
            return baseMap.loadProgressVal;
        }
    }

    /// <summary>
    /// 获取当前地图加载进度总值
    /// </summary>
    /// <value></value>
    public int progressTotal
    {
        get
        {
            return baseMap.loadProgressTotal;
        }
    }

    public void UpdateMapFrameSynLogic(JArray jInfo)
    {
        if(baseMap == null)
        {
            Debug.Log("BaseMap == null");
            return;
        }
        baseMap.UpdateFrameSynLogic(jInfo);
    }
    
    public void UpdateMapFrameSynRender(Fix64 nInterpolation)
    {
        if(baseMap == null)
        {
            return;
        }
        baseMap.UpdateFrameSynRender(nInterpolation);
    }
}