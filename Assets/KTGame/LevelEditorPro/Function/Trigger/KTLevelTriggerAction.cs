#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KTEditor.LevelEditor;
using Sirenix.OdinInspector;
using System.Reflection;

#pragma warning disable 0414
public class KTLevelTriggerAction : KTLevelTriggerNode
{
    private readonly static ValueDropdownList<TriggerResultCampTagOpType> kCampTagOpTypeNames = new ValueDropdownList<TriggerResultCampTagOpType>()
        {
            { "添加", TriggerResultCampTagOpType.op_add },
            { "删除", TriggerResultCampTagOpType.op_delete },
        };

    private readonly static ValueDropdownList<DialogType> kDialogTypeNames = new ValueDropdownList<DialogType>()
        {
            { "对话", DialogType.dialog},
            { "答题", DialogType.answer},
        };

    private readonly static ValueDropdownList<BubblePlayType> kBubblePlayTypeNames = new ValueDropdownList<BubblePlayType>()
        {
            { "播放", BubblePlayType.play},
            { "停止", BubblePlayType.stop},
        };

    private readonly static ValueDropdownList<BubbleTargetType> kBubblePlayTargetNames = new ValueDropdownList<BubbleTargetType>()
        {
            { "玩家自己", BubbleTargetType.self},
            { "NPC孵化器", BubbleTargetType.generators},
        };

    private readonly static ValueDropdownList<TriggerResultAnimType> kAnimTypeNames = new ValueDropdownList<TriggerResultAnimType>()
        {
            { "怪物动画", TriggerResultAnimType.monster_anim},
            { "物件动画", TriggerResultAnimType.creature_anim},
        };

    /// <summary>
    /// bubbleTargetType
    /// by lijunfeng 2018/8/17
    /// </summary>
    /// <returns></returns>
    protected bool IsBubbleTargetTypeGenerators()
    {
        System.Type t = this.GetType();
        FieldInfo fieldInfo = t.GetField("bubbleTargetType", BindingFlags.Public | BindingFlags.Instance);
        return fieldInfo != null && (BubbleTargetType)fieldInfo.GetValue(this) == BubbleTargetType.generators;
    }
}
#endif
