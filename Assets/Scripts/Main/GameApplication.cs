using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class GameApplication
{
    public static bool isGameRun{get; private set;} = false;
    public static void Start()
    {
        isGameRun = true;
        // 禁止Unity物理引擎的自动刷新
        Physics.autoSimulation = false;

        BehaviacManager.Init();
        Network.Start();
        FGUIManager.Start();
        AudioManager.Start();
        AudioSDKManager.Instance.Start();
        LuaGEvent.Start();
        StatusManager.Instance.Start();
        MapManager.Instance.Start();
        MyPlayer.Start();
    }

    public static void Update()
    {
        if(!isGameRun) return;
        Network.Update();
        Looper.TryUpdate();
        GEvent.DispatchEvent(GacEvent.Update);
        GEvent.DispatchEvent(GacEvent.LuaUpdate);
        LuaGEvent.DispatchEventToLua(GacEvent.LuaUpdate, Time.deltaTime);
        CommWriteLog.WriteLogUpdate();
    }

    public static void LateUpdate()
    {
        if(!isGameRun) return;
        GEvent.DispatchEvent(GacEvent.LateUpdate);
        GEvent.DispatchEvent(GacEvent.LuaLateUpdate);
    }

    public static void FixedUpdate()
    {
        if(!isGameRun) return;
        GEvent.DispatchEvent(GacEvent.FixedUpdate);
        GEvent.DispatchEvent(GacEvent.LuaFixedUpdate);
    }

    public static void OnDestroy()
    {
        if(!isGameRun) return;
        MapManager.Instance.OnDestroy();
        MyPlayer.OnDestroy();
        LuaGEvent.OnDestroy();
        Network.OnDestroy();
    }

    public static void OnApplicationQuit()
    {
        isGameRun = false;
        CommWriteLog.Clear();
        Application.Quit();
    }
}