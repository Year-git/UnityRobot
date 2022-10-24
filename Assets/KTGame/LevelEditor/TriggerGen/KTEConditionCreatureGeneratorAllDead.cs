//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("单位全部死亡"), KTLevelClassType(TriggerConditionType.specified_unit_dead), KTLevelPlatform(PlatformType.Double)]
public class KTEConditionCreatureGeneratorAllDead: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_unit_dead;

    [KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
