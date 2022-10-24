//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("单位全部进入区域"), KTLevelClassType(TriggerResultType.specified_generator_in_area), KTLevelPlatform(PlatformType.Double)]
public class KTEActionMoveToPosition: KTLevelTriggerAction
{

    [HideInInspector, KTLevelExport("resultType")]
    public TriggerResultType triggerNodeType = TriggerResultType.specified_generator_in_area;

    [KTLevelExport("__result"), KTLevelExport("__result_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelBattleUnitSpawner> spawner;

    [KTLevelExport("__area"), KTLevelExport("__area_uuid")]
    [LabelText("区域"), OnValueChanged("Refresh")]
    public KTLevelPatrolZone zone;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
