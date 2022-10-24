using LuaInterface;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class LuaGEvent
{
    private static LuaFunction _luaDisplayCSharpEvent;

    public static void Start()
    {
        _luaDisplayCSharpEvent = LuaClient.GetMainState().GetFunction("DisplayCSharpEvent");
        if (_luaDisplayCSharpEvent == null)
        {
            Debug.LogError("DisplayCSharpEvent not found！");
            return;
        }
    }

    public static void DispatchEventToLua(string msgFun, params object[] args)
    {
        _luaDisplayCSharpEvent.BeginPCall();
        _luaDisplayCSharpEvent.Push(msgFun);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == null ||
                args[i].GetType() == typeof(int) ||
                args[i].GetType() == typeof(short) ||
                args[i].GetType() == typeof(long) ||
                args[i].GetType() == typeof(float) ||
                args[i].GetType() == typeof(double) ||
                args[i].GetType() == typeof(bool) ||
                args[i].GetType() == typeof(string) ||
                args[i].GetType() == typeof(JArray) ||
                args[i].GetType() == typeof(JObject))
            {
                if (args[i].GetType() == typeof(JArray) || args[i].GetType() == typeof(JObject))
                {
                    args[i] = "_t" + JsonConvert.SerializeObject(args[i]);
                }
                _luaDisplayCSharpEvent.Push(args[i]);
            }
            else
            {
                Debug.LogError("存在不支持的参数类型！");
                return;
            }
        }
        _luaDisplayCSharpEvent.PCall();
        _luaDisplayCSharpEvent.EndPCall();
    }

    public static void OnDestroy()
    {
        if (_luaDisplayCSharpEvent != null)
        {
            _luaDisplayCSharpEvent.Dispose();
            _luaDisplayCSharpEvent = null;
        }
    }
}