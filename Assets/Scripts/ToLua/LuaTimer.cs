using LuaInterface;
using System;
using Framework;

public static class LuaTimer
{
    public static Timer CreateOnceTimer(float time, LuaFunction callBack, params object[] args)
    {
        if (callBack == null)
        {
            throw new Exception("Lua.CreateOnceTimer,  func is null");
        }
        return Timer.CreateTimer(time, false, delegate { CallLuaFunction(callBack, args); });
    }

    public static Timer CreateLoopTimer(float time, LuaFunction callBack, params object[] args)
    {
        if (callBack == null)
        {
            throw new Exception("Lua.CreateOnceTimer,  func is null");
        }
        return Timer.CreateTimer(time, true, delegate { CallLuaFunction(callBack, args); });
    }

    public static void StopTimer(Timer timer)
    {
        if (timer == null)
        {
            throw new Exception("Lua.StopTimer,  timer is null");
        }
        timer.Stop();
    }

    /// <summary>
    /// 回调lua
    /// </summary>
    /// <param name="luaFun"></param>
    private static void CallLuaFunction(LuaFunction callBack, params object[] args)
    {
        if (callBack != null)
        {
            callBack.BeginPCall();
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    callBack.Push(args[i]);
                }
            }
            callBack.PCall();
            callBack.EndPCall();
        }
    }
}
