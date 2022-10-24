using System;
using System.Reflection;
using UnityEngine;
using Framework;

/// <summary>
/// 状态管理类、单例模式、记录游戏当前状态且唯一
/// </summary>
public class StatusManager : Singleton<StatusManager>
{
    private object status;
    private MethodInfo onEnter;
    private MethodInfo onExecute;
    private MethodInfo onLateExecute;
    private MethodInfo onFixedExecute;
    private MethodInfo onExit;

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public Type Type { get; private set; }

    public void Start()
    {
        GEvent.RegistEvent(GacEvent.Update, Update);
        GEvent.RegistEvent(GacEvent.LateUpdate, LateUpdate);
        GEvent.RegistEvent(GacEvent.FixedUpdate, FixedUpdate);
    }

    public void Update()
    {
        if (onExecute != null)
        {
            onExecute.Invoke(status, null);
        }
    }

    public void LateUpdate()
    {
        if (onLateExecute != null)
        {
            onLateExecute.Invoke(status, null);
        }
    }

    public void FixedUpdate()
    {
        if (onFixedExecute != null)
        {
            onFixedExecute.Invoke(status, null);
        }
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="type"></param>
    public void ChangeStatus(Type type)
    {
        if (Type == type)
        {
            Debug.LogWarning("游戏状态不允许重复切换！");
            return;
        }

        //退出状态
        ExitStatus();

        //创建新状态
        Type = type;
        status = Activator.CreateInstance(Type);

        if (status == null)
        {
            Debug.LogError("游戏状态类不存在，切换失败！");
            return;
        }

        onEnter = type.GetMethod("OnEnter");
        onExecute = type.GetMethod("OnExecute");
        onLateExecute = type.GetMethod("OnLateExecute");
        onFixedExecute = type.GetMethod("OnFixedExecute");
        onExit = type.GetMethod("OnExit");

        //进入状态
        if (onEnter != null)
        {
            onEnter.Invoke(status, null);
            LuaGEvent.DispatchEventToLua(GacEvent.StatusOnEnter, type.ToString());
        }
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    private void ExitStatus()
    {
        if (onExit != null)
        {
            onExit.Invoke(status, null);
            LuaGEvent.DispatchEventToLua(GacEvent.StatusOnExit, Type.ToString());
        }
        Type = null;
        status = null;
        onEnter = null;
        onExecute = null;
        onLateExecute = null;
        onFixedExecute = null;
        onExit = null;
    }
}