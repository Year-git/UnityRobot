//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("单位(任意)血量低于百分比"), KTFormatDisplayName("单位(任意)血量低于{0}", "hpPercent"), KTLevelClassType(TriggerConditionType.blood_under_percent), KTLevelPlatform(PlatformType.Server)]
public class KTEConditionCreatureGeneratorHealthLower: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.blood_under_percent;

    [KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelBattleUnitSpawner> spawner;

    [KTLevelExport("bloodPercent")]
    [LabelText("血量百分比"), OnValueChanged("Refresh")]
    public float hpPercent;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
