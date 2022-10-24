//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("玩家进入区域"), KTLevelClassType(TriggerConditionType.player_enter_area), KTLevelPlatform(PlatformType.Double)]
public class KTEConditionPlayerEnterArea: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.player_enter_area;

    [KTLevelExport("__area"), KTLevelExport("__area_uuid")]
    [LabelText("区域"), OnValueChanged("Refresh")]
    public KTLevelPatrolZone zone;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
