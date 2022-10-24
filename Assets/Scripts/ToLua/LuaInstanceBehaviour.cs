using UnityEngine;
using LuaInterface;
using System.Collections.Generic;

/// <summary>
/// 适用于View下面的实例化对象
/// </summary>
public class LuaInstanceBehaviour : MonoBehaviour
{
    public LuaTable LuaClass { get; private set; }
    public List<GameObject> uiList = new List<GameObject>();

    private LuaFunction _luaAwake;
    private LuaFunction _luaStart;
    private LuaFunction _luaOnEnable;
    private LuaFunction _luaOnDisable;
    private LuaFunction _luaOnDestroy;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialization(LuaTable luaClass, LuaTable parameters)
    {
        LuaClass = luaClass;

        //将部分引用指向lua
        LuaClass["gameObject"] = gameObject;
        LuaClass["transform"] = transform;
        for (int i = 0; i < uiList.Count; i++)
        {
            GameObject go = uiList[i];
            if (LuaClass[go.name] != null)
            {
                Debug.LogError(gameObject.name + "有重名的变量，Lua映射创建失败！");
                return;
            }
            LuaClass[go.name] = uiList[i];
        }

        //缓存lua脚本固定回调函数
        _luaAwake = LuaClass.GetLuaFunction("Awake");
        _luaStart = LuaClass.GetLuaFunction("Start");
        _luaOnEnable = LuaClass.GetLuaFunction("Enable");
        _luaOnDisable = LuaClass.GetLuaFunction("Disable");
        _luaOnDestroy = LuaClass.GetLuaFunction("Destroy");

        //模拟Awake
        CallLuaFunction(_luaAwake, parameters);
        DestroyLuaFunction(_luaAwake);
		
		//模拟Enable
        CallLuaFunction(_luaOnEnable);
        DestroyLuaFunction(_luaOnEnable);
    }

    public void Start()
    {
        CallLuaFunction(_luaStart);
        DestroyLuaFunction(_luaStart);
    }

    public void OnEnable()
    {
        CallLuaFunction(_luaOnEnable);
    }

    public void OnDisable()
    {
        CallLuaFunction(_luaOnDisable);
    }

    public void OnDestroy()
    {
        CallLuaFunction(_luaOnDestroy);
        DestroyLuaFunction(_luaOnEnable);
        DestroyLuaFunction(_luaOnDisable);
        DestroyLuaFunction(_luaOnDestroy);
        if (LuaClass != null)
        {
            LuaClass.Dispose();
            LuaClass = null;
        }
    }

    /// <summary>
    /// 调用lua函数
    /// </summary>
    /// <param name="luaFun"></param>
    private void CallLuaFunction(LuaFunction luaFun, LuaTable parameters = null)
    {
        if (luaFun != null)
        {
            luaFun.BeginPCall();
            luaFun.Push(LuaClass);
            if ((luaFun == _luaAwake || luaFun == _luaOnEnable) && parameters != null)
            {
                object[] ret = parameters.ToArray();
                for (int i = 0; i < ret.Length; i++)
                {
                    luaFun.Push(ret[i]);
                }
            }
            luaFun.PCall();
            luaFun.EndPCall();
        }
    }

    /// <summary>
    /// 释放lua函数
    /// </summary>
    /// <param name="luaFun"></param>
    private void DestroyLuaFunction(LuaFunction luaFun)
    {
        if (luaFun != null)
        {
            luaFun.Dispose();
            luaFun = null;
        }
    }
}