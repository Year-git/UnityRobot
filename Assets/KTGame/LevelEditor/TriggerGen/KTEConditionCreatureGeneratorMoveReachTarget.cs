//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("单位进入区域"), KTLevelClassType(TriggerConditionType.specified_generator_in_area), KTLevelPlatform(PlatformType.Double)]
public class KTEConditionCreatureGeneratorMoveReachTarget: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_generator_in_area;

    [KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    [KTLevelExport("__area"), KTLevelExport("__area_uuid")]
    [LabelText("区域"), OnValueChanged("Refresh")]
    public KTLevelPatrolZone zone;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
