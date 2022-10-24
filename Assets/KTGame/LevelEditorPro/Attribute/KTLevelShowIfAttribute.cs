#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于隐藏ui
/// by lijunfeng 2018/6/23
/// </summary>
[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
public class KTLevelShowIfAttribute : System.Attribute
{
    public string funcName;

    public KTLevelShowIfAttribute(string funcName)
    {
        this.funcName = funcName;
    }

    public override string ToString()
    {
        return string.Format("ShowIf(\"{0}\")", funcName);
    }
}

#endif

