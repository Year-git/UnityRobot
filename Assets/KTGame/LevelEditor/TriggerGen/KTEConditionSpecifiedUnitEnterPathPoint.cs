//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("指定单位进入指定路径点"), KTLevelClassType(TriggerConditionType.specified_unit_enter_path_point), KTLevelPlatform(PlatformType.Double)]
public class KTEConditionSpecifiedUnitEnterPathPoint: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_unit_enter_path_point;

    [KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    [KTLevelExport("__patrolPath"), KTLevelExport("__patrolPath_uuid")]
    [LabelText("指定路径"), OnValueChanged("Refresh")]
    public KTLevelPatrolPath path;

    [KTLevelExport("__patrolPoint"), KTLevelExport("__patrolPoint_uuid")]
    [LabelText("指定节点"), OnValueChanged("Refresh")]
    public KTLevelPatrolPoint point;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
