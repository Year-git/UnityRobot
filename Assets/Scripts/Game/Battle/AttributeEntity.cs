using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class AttributeEntity
{
    /// <summary>
    /// 非当前属性集合
    /// </summary>
    Dictionary<AttributeType, AttrbuteValue> _collection = new Dictionary<AttributeType, AttrbuteValue>();

    public Dictionary<AttributeType, AttrbuteValue> attrCollection {get{return this._collection;}}

    public AttributeEntity()
    {
        foreach (AttributeType item in System.Enum.GetValues(typeof(AttributeType)))
        {
            _collection.Add(item, new AttrbuteValue());
        }
    }

    public void InitAttrValue(JArray jAttrInfo)
    {
        foreach(var jData in jAttrInfo)
        {
            int nCfgId = (int)jData[0];
            int nAttrVal = (int)jData[1];

            AttributeType eType = (AttributeType)ConfigManager.GetValue<int>("Attribute_C", nCfgId, "attributeType");
            AttributeValueType eValueType = (AttributeValueType)ConfigManager.GetValue<int>("Attribute_C", nCfgId, "arithmeticType");

            if (eValueType == AttributeValueType.Base)
            {
                SetBaseValue(eType, nAttrVal);
            }
            else{
                // SetPercentValue(eType, nAttrVal);
            }
        }
    }
    
    /// <summary>
    /// 获得属性值
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public int GetValue(AttributeType type)
    {
        if (_collection.ContainsKey(type))
        {
            return _collection[type].GetValue();
        }
        // Debug.LogError("GetValue(AttributeType type) : " + type + "不存在");
        return 0;
    }
    /// <summary>
    /// 获得基础值
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public int GetBaseValue(AttributeType type)
    {
        if (_collection.ContainsKey(type))
        {
            return _collection[type].Base;
        }
        // Debug.LogError(" GetBaseValue : " + type + "不存在");
        return 0;
    }
    /// <summary>
    /// 获得加成百分比比
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetPercentValue(AttributeType type)
    {
        if (_collection.ContainsKey(type))
        {
            return _collection[type].Percent;
        }
        // Debug.LogError("GetPercentValue: " + type + "不存在");
        return 0;
    }
    
    /// <summary>
    /// 设置基础值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void SetBaseValue(AttributeType type, int value)
    {
        if (!_collection.ContainsKey(type))
        {
            _collection.Add(type, new AttrbuteValue());
        }
        _collection[type].Base = value;
        // Debug.LogError("SetBaseValue(AttributeType type, float value): " + type + "不存在");
    }

    /// <summary>
    /// 设置加成值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void SetPercentValue(AttributeType type, int value)
    {
        if (!_collection.ContainsKey(type))
        {
            _collection.Add(type, new AttrbuteValue());
        }
        _collection[type].Percent = value;
        // Debug.LogError("SetPercentValue(AttributeType type, float value) : " + type + "不存在");
    }

    /// <summary>
    /// 增加基础值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void ModfiyBaseValue(AttributeType type, int value)
    {
        if (!_collection.ContainsKey(type))
        {
            _collection.Add(type, new AttrbuteValue());
        }
        _collection[type].Base += value;
        // Debug.LogError(" ModfiyBaseValue" + type + "不存在");
    }

    /// <summary>
    /// 增加百分比值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void ModfiyPercentValue(AttributeType type, int value)
    {
        if (!_collection.ContainsKey(type))
        {
            _collection.Add(type, new AttrbuteValue());
        }
        _collection[type].Percent += value;
        // Debug.LogError("ModfiyPercentValue" + type + "不存在");
    }

}
