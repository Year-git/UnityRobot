//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("指定发生器离开战斗后"), KTLevelClassType(TriggerConditionType.specified_generator_end_battle), KTLevelPlatform(PlatformType.Server)]
public class KTEConditionSpecifiedGeneratorEndBattle: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_generator_end_battle;

    [KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
