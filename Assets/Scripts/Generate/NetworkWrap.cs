﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class NetworkWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("Network");
		L.RegFunction("Start", Start);
		L.RegFunction("Update", Update);
		L.RegFunction("OnDestroy", OnDestroy);
		L.RegFunction("Connect", Connect);
		L.RegFunction("LuaSend", LuaSend);
		L.RegFunction("CSharpSend", CSharpSend);
		L.RegFunction("Closed", Closed);
		L.RegVar("IsConnect", get_IsConnect, null);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Start(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Network.Start();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Network.Update();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Network.OnDestroy();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Connect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			string arg0 = ToLua.CheckString(L, 1);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			LuaFunction arg2 = ToLua.CheckLuaFunction(L, 3);
			LuaFunction arg3 = ToLua.CheckLuaFunction(L, 4);
			Network.Connect(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaSend(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
			object[] arg0 = ToLua.ToParamsObject(L, 1, count);
			Network.LuaSend(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CSharpSend(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
			object[] arg0 = ToLua.ToParamsObject(L, 1, count);
			Network.CSharpSend(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Closed(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Network.Closed();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsConnect(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Network.IsConnect);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

