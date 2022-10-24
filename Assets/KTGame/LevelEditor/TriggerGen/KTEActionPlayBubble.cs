//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("单位对话泡泡"), KTFormatDisplayName("单位播放气泡对话{0}", "id"), KTLevelClassType(TriggerResultType.play_bubble), KTLevelPlatform(PlatformType.Double)]
public class KTEActionPlayBubble: KTLevelTriggerAction
{

    [HideInInspector, KTLevelExport("resultType")]
    public TriggerResultType triggerNodeType = TriggerResultType.play_bubble;

    [KTLevelExport("bubbleTargetType"), ValueDropdown("kBubblePlayTargetNames")]
    [LabelText("播放目标"), OnValueChanged("Refresh")]
    public KTEditor.LevelEditor.BubbleTargetType bubbleTargetType;

    [KTLevelExport("__result"), KTLevelExport("__result_uuid"), ShowIf("IsBubbleTargetTypeGenerators")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    [KTLevelExport("specifiedBubblePlayType"), ValueDropdown("kBubblePlayTypeNames")]
    [LabelText("指定播放类型"), OnValueChanged("Refresh")]
    public KTEditor.LevelEditor.BubblePlayType specifiedBubblePlayType;

    [KTLevelExport("bubbleID")]
    [LabelText("id"), OnValueChanged("Refresh")]
    public int id;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
