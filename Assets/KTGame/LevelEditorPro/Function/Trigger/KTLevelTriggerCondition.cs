#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KTEditor.LevelEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Reflection;

#pragma warning disable 0414
public class KTLevelTriggerCondition : KTLevelTriggerNode
{
    private readonly static ValueDropdownList<DialogType> kDialogTypeNames = new ValueDropdownList<DialogType>()
        {
            { "对话", DialogType.dialog},
            { "答题", DialogType.answer },
        };

    //by lijunfeng 2018/6/23
    private readonly static ValueDropdownList<QuizResult> kQuizResultNames = new ValueDropdownList<QuizResult>()
        {
            { "正确", QuizResult.Correct},
            { "错误", QuizResult.Wrong },
        };

    //by lijunfeng 2018/6/23
    private readonly static ValueDropdownList<UnitType> kUnitTypeNames = new ValueDropdownList<UnitType>()
        {
            { "所有", UnitType.all},
            { "自己", UnitType.self },
            { "其他玩家", UnitType.other_players },
            { "孵化器", UnitType.generators },
        };

    //by lijunfeng 2018/6/23
    private readonly static ValueDropdownList<TimeType> kTimeTypeNames = new ValueDropdownList<TimeType>()
        {
            { "自然时间", TimeType.World},
            { "游戏时间", TimeType.Game },
        };


    //by lijunfeng 2018/8/13
    private readonly static ValueDropdownList<TimeFormat> kTimeFormatNames = new ValueDropdownList<TimeFormat>()
        {
            { "标准时间", TimeFormat.Standard},
            { "时间戳", TimeFormat.TimpStamp },
        };

    //by lijunfeng 2018/10、16
    private readonly static ValueDropdownList<RelationType> kRelationNames = new ValueDropdownList<RelationType>()
        {
            { "大于", RelationType.Greater},
            { "小于", RelationType.Less },
            { "等于", RelationType.Equal },
        };

    /// <summary>
    /// 根据specifiedTimeFormat选择的格式来隐藏指定ui
    /// by lijunfeng 2018/8/13
    /// </summary>
    /// <returns></returns>
    protected bool IsTimeFormatSelectStandard()
    {
        System.Type t = this.GetType();
        FieldInfo fieldInfo = t.GetField("specifiedTimeFormat", BindingFlags.Public | BindingFlags.Instance);
        return fieldInfo != null && (TimeFormat)fieldInfo.GetValue(this) == TimeFormat.Standard;
    }

    /// <summary>
    /// 根据specifiedTimeFormat选择的格式来隐藏指定ui
    /// by lijunfeng 2018/8/13
    /// </summary>
    /// <returns></returns>
    protected bool IsTimeFormatSelectTimpStamp()
    {
        System.Type t = this.GetType();
        FieldInfo fieldInfo = t.GetField("specifiedTimeFormat", BindingFlags.Public | BindingFlags.Instance);
        return fieldInfo != null && (TimeFormat)fieldInfo.GetValue(this) == TimeFormat.TimpStamp;
    }

    /// <summary>
    /// by lijunfeng 2018/6/23
    /// 用于SpecifiedTargetUnitCount根据选择的specifiedUnitType来隐藏ui
    /// </summary>
    /// <returns></returns>
    protected bool IsSpecfiedSelectGenerator()
    {
        System.Type t = this.GetType();
        FieldInfo fieldInfo = t.GetField("specifiedUnitType", BindingFlags.Public | BindingFlags.Instance);
        return fieldInfo != null && (UnitType)fieldInfo.GetValue(this) == UnitType.generators;
    }

    /// <summary>
    /// by lijunfeng 2018/6/23
    /// 用于SpecifiedTargetUnitCount根据选择的specifiedUnitType来隐藏ui
    /// </summary>
    /// <returns></returns>
    protected bool IsTargetSelectGenerator()
    {
        System.Type t = this.GetType();
        FieldInfo fieldInfo = t.GetField("targetUnitType", BindingFlags.Public | BindingFlags.Instance);
        return fieldInfo != null && (UnitType)fieldInfo.GetValue(this) == UnitType.generators;
    }
}
#endif

