//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("延迟"), KTFormatDisplayName("延迟{0}秒", "time"), KTLevelClassType(TriggerResultType.delay), KTLevelPlatform(PlatformType.Double)]
public class KTEActionWait: KTLevelTriggerAction
{

    [HideInInspector, KTLevelExport("resultType")]
    public TriggerResultType triggerNodeType = TriggerResultType.delay;

    [KTLevelExport("delay")]
    [LabelText("时间(秒)"), OnValueChanged("Refresh")]
    public float time;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
