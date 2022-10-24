using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

public static class ConfigManager
{
    /// <summary>
    /// 配置表数据
    /// </summary>
    private static Dictionary<string, LuaTable> _configs = new Dictionary<string, LuaTable>();

    /// <summary>
    /// 缓存Lua端发送来的配置表
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="configTable"></param>
    public static void SetConfig(string configName, LuaTable configTable)
    {
        if (_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " Repeated loading！");
            return;
        }
        if (configTable == null)
        {
            Debug.LogError(configName + " configTable is null！");
            return;
        }
        _configs.Add(configName, configTable);
    }

    /// <summary>
    /// 获取表一维的长度
    /// </summary>
    /// <param name="configName"></param>
    /// <returns></returns>
    public static int GetLenght(string configName)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " is not found");
            return -1;
        }
        LuaTable config = _configs[configName];
        return config.Length;
    }

    /// <summary>
    /// 获取表二维的长度
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int GetLenght(string configName, int key)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " is not found");
            return -1;
        }
        LuaTable config = _configs[configName];
        LuaTable row = config.RawGetIndex<LuaTable>(key);
        if (row == null)
        {
            return -1;
        }
        return row.Length;
    }

    /// <summary>
    /// 根据表名、值key获取对应的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configName"></param>
    /// <param name="valueKey"></param>
    /// <returns></returns>
    public static T GetValue<T>(string configName, string valueKey)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " is not found");
            return default;
        }

        LuaTable config = _configs[configName];
        object value = config.RawGet<string, T>(valueKey);
        if (value is LuaTable)
        {
            object outValue = null;
            UnPackLuaTable(value, ref outValue);
            return (T)outValue;
        }
        return (T)value;
    }

    /// <summary>
    /// 根据表名、单键、值key获取对应的值
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="key"></param>
    /// <param name="valueKey"></param>
    public static T GetValue<T>(string configName, int key, string valueKey)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " is not found");
            return default;
        }

        LuaTable config = _configs[configName];
        LuaTable row = config.RawGetIndex<LuaTable>(key);
        if (row == null)
        {
            Debug.LogError(key + " is not found");
            return default;
        }

        object value = row.RawGet<string, T>(valueKey);
        if (value is LuaTable)
        {
            object outValue = null;
            UnPackLuaTable(value, ref outValue);
            return (T)outValue;
        }
        return (T)value;
    }

    /// <summary>
    /// 根据表名、双键、值key获取对应的值
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="valueKey"></param>
    /// <returns></returns>
    public static T GetValue<T>(string configName, int key1, int key2, string valueKey)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError(configName + " is not found");
            return default;
        }

        LuaTable config = _configs[configName];
        LuaTable row1 = config.RawGetIndex<LuaTable>(key1);
        if (row1 == null)
        {
            Debug.LogError(key1 + " is not found");
            return default;
        }
        LuaTable row2 = row1.RawGetIndex<LuaTable>(key2);
        if (row2 == null)
        {
            Debug.LogError(key2 + " is not found");
            return default;
        }

        object value = row2.RawGet<string, T>(valueKey);
        if (value is LuaTable)
        {
            object outValue = null;
            UnPackLuaTable(value, ref outValue);
            return (T)outValue;
        }
        return (T)value;
    }

    /// <summary>
    /// 递归解析多层嵌套luaTable
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outValue"></param>
    private static void UnPackLuaTable(object value, ref object outValue)
    {
        if (value is LuaTable)
        {
            outValue = (value as LuaTable).ToArray();
            for (int i = 0; i < (outValue as object[]).Length; i++)
            {
                UnPackLuaTable((outValue as object[])[i], ref (outValue as object[])[i]);
            }
        }
    }

    /// <summary>
    /// 循环遍历配置表的某一列
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="columnName"></param>
    /// <param name="pLoop"></param>
    /// <typeparam name="T"></typeparam>
    public static void LoopColumn<T>(string configName, string columnName, System.Action<int,T> pLoop)
    {
        if (!_configs.ContainsKey(configName))
        {
            Debug.LogError("ConfigManager.Loop->ConfigName Is Not Found!" + "#configName = " + configName);
            return;
        }

        if (columnName == null || columnName == "")
        {
            Debug.LogError("ConfigManager.Loop->ConfigName Is Error!" + "#columnName = " + columnName);
            return;
        }

        LuaTable config = _configs[configName];

        LuaDictTable pLuaDictTable = new LuaDictTable(config);
        foreach(System.Collections.DictionaryEntry kvPair in pLuaDictTable)
        {
            object pValue = ((LuaTable)kvPair.Value).RawGet<string, object>(columnName);
            if (pValue == null)
            {
                Debug.LogError("ConfigManager.Loop->pValue Is Null!" + "#configName = " + configName + "#columnName = " + columnName);
            }
            else
            {
                if (pValue is LuaTable)
                {
                    object outValue = null;
                    UnPackLuaTable(pValue, ref outValue);
                    pLoop?.Invoke((int)kvPair.Key, (T)outValue);
                }
                else
                {
                    pLoop?.Invoke((int)kvPair.Key, (T)pValue);
                }
            }
        }
    }
}