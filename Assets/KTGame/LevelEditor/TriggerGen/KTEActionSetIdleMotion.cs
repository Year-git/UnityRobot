//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("改变单位巡逻行为"), KTLevelClassType(TriggerResultType.change_patrol_path), KTLevelPlatform(PlatformType.Server)]
public class KTEActionSetIdleMotion: KTLevelTriggerAction
{

    [HideInInspector, KTLevelExport("resultType")]
    public TriggerResultType triggerNodeType = TriggerResultType.change_patrol_path;

    [KTLevelExport("__result"), KTLevelExport("__result_uuid")]
    [LabelText("孵化器"), OnValueChanged("Refresh")]
    public List<KTLevelSpawner> spawner;

    
    [LabelText("巡逻设置"), OnValueChanged("Refresh")]
    public KTLevelBattleUnitSpawner.PatrolPathSettings settings;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
