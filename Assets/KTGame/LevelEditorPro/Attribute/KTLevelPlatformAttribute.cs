#if UNITY_EDITOR
using KTEditor.LevelEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于隐藏ui
/// by lijunfeng 2018/6/23
/// </summary>
[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
public class KTLevelPlatformAttribute : System.Attribute
{
    public PlatformType platformType;

    public KTLevelPlatformAttribute(PlatformType platformType)
    {
        this.platformType = platformType;
    }

    public override string ToString()
    {
        return string.Format("KTLevelPlatform({0}.{1})", platformType.GetType().Name, platformType);
    }
}

#endif

