﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_GTimeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("GTime");
		L.RegFunction("SetServerTime", SetServerTime);
		L.RegVar("DateTimeOrigin", get_DateTimeOrigin, null);
		L.RegVar("MaxDateTime", get_MaxDateTime, null);
		L.RegVar("RealtimeSinceStartup", get_RealtimeSinceStartup, null);
		L.RegVar("DeltaTime", get_DeltaTime, null);
		L.RegVar("Time", get_Time, null);
		L.RegVar("RenderedFrameCount", get_RenderedFrameCount, null);
		L.RegVar("FrameCount", get_FrameCount, null);
		L.RegVar("TimeScale", get_TimeScale, set_TimeScale);
		L.RegVar("MaximumParticleDeltaTime", get_MaximumParticleDeltaTime, set_MaximumParticleDeltaTime);
		L.RegVar("SmoothDeltaTime", get_SmoothDeltaTime, null);
		L.RegVar("MaximumDeltaTime", get_MaximumDeltaTime, set_MaximumDeltaTime);
		L.RegVar("CaptureFramerate", get_CaptureFramerate, set_CaptureFramerate);
		L.RegVar("FixedDeltaTime", get_FixedDeltaTime, set_FixedDeltaTime);
		L.RegVar("UnscaledDeltaTime", get_UnscaledDeltaTime, null);
		L.RegVar("FixedUnscaledTime", get_FixedUnscaledTime, null);
		L.RegVar("UnscaledTime", get_UnscaledTime, null);
		L.RegVar("FixedTime", get_FixedTime, null);
		L.RegVar("TimeSinceLevelLoad", get_TimeSinceLevelLoad, null);
		L.RegVar("FixedUnscaledDeltaTime", get_FixedUnscaledDeltaTime, null);
		L.RegVar("InFixedTimeStep", get_InFixedTimeStep, null);
		L.RegVar("ServerTime", get_ServerTime, null);
		L.RegVar("ServerSeconds", get_ServerSeconds, null);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetServerTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 1);
			double arg1 = (double)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.SetServerTime(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DateTimeOrigin(IntPtr L)
	{
		try
		{
			ToLua.PushValue(L, Framework.GTime.DateTimeOrigin);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MaxDateTime(IntPtr L)
	{
		try
		{
			ToLua.PushValue(L, Framework.GTime.MaxDateTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RealtimeSinceStartup(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.RealtimeSinceStartup);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.DeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Time(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.Time);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RenderedFrameCount(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Framework.GTime.RenderedFrameCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FrameCount(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Framework.GTime.FrameCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TimeScale(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.TimeScale);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MaximumParticleDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.MaximumParticleDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SmoothDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.SmoothDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MaximumDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.MaximumDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CaptureFramerate(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Framework.GTime.CaptureFramerate);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FixedDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.FixedDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UnscaledDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.UnscaledDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FixedUnscaledTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.FixedUnscaledTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UnscaledTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.UnscaledTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FixedTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.FixedTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TimeSinceLevelLoad(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.TimeSinceLevelLoad);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FixedUnscaledDeltaTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Framework.GTime.FixedUnscaledDeltaTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_InFixedTimeStep(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Framework.GTime.InFixedTimeStep);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ServerTime(IntPtr L)
	{
		try
		{
			ToLua.PushValue(L, Framework.GTime.ServerTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ServerSeconds(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Framework.GTime.ServerSeconds);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_TimeScale(IntPtr L)
	{
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.TimeScale = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_MaximumParticleDeltaTime(IntPtr L)
	{
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.MaximumParticleDeltaTime = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_MaximumDeltaTime(IntPtr L)
	{
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.MaximumDeltaTime = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_CaptureFramerate(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.CaptureFramerate = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FixedDeltaTime(IntPtr L)
	{
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			Framework.GTime.FixedDeltaTime = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
