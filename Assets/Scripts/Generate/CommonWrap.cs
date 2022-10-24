﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CommonWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("Common");
		L.RegFunction("GetOSType", GetOSType);
		L.RegFunction("GetMacAddress", GetMacAddress);
		L.RegFunction("CreateXML", CreateXML);
		L.RegFunction("AmendPrecision", AmendPrecision);
		L.RegFunction("JObject2JArray", JObject2JArray);
		L.RegFunction("CopyObjectList", CopyObjectList);
		L.RegFunction("GetEquipmentID", GetEquipmentID);
		L.RegFunction("getMemory", getMemory);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetOSType(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Common.GetOSType();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMacAddress(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Common.GetMacAddress();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateXML(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			System.Xml.XmlDocument o = Common.CreateXML();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AmendPrecision(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				UnityEngine.Rigidbody arg1 = (UnityEngine.Rigidbody)ToLua.CheckObject<UnityEngine.Rigidbody>(L, 2);
				Common.AmendPrecision(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				UnityEngine.Rigidbody arg1 = (UnityEngine.Rigidbody)ToLua.CheckObject<UnityEngine.Rigidbody>(L, 2);
				UnityEngine.WheelCollider[] arg2 = ToLua.CheckObjectArray<UnityEngine.WheelCollider>(L, 3);
				Common.AmendPrecision(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Common.AmendPrecision");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int JObject2JArray(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Newtonsoft.Json.Linq.JObject arg0 = (Newtonsoft.Json.Linq.JObject)ToLua.CheckObject<Newtonsoft.Json.Linq.JObject>(L, 1);
			Newtonsoft.Json.Linq.JArray o = Common.JObject2JArray(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CopyObjectList(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			System.Collections.Generic.List<object> arg0 = (System.Collections.Generic.List<object>)ToLua.CheckObject(L, 1, typeof(System.Collections.Generic.List<object>));
			System.Collections.Generic.List<object> o = Common.CopyObjectList(arg0);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetEquipmentID(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Common.GetEquipmentID();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getMemory(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			object arg0 = ToLua.ToVarObject(L, 1);
			string o = Common.getMemory(arg0);
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
