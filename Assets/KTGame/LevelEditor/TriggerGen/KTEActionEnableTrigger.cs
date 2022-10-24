//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("激活触发器"), KTLevelClassType(TriggerResultType.active_trigger), KTLevelPlatform(PlatformType.Double)]
public class KTEActionEnableTrigger: KTLevelTriggerAction
{

    [HideInInspector, KTLevelExport("resultType")]
    public TriggerResultType triggerNodeType = TriggerResultType.active_trigger;

    [KTLevelExport("__result"), KTLevelExport("__result_uuid")]
    [LabelText("触发器"), OnValueChanged("Refresh")]
    public List<KTLevelTrigger> trigger;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
