//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("任务失败"), KTFormatDisplayName("任务{0}失败", "taskId"), KTLevelClassType(TriggerConditionType.specified_task_fail), KTLevelPlatform(PlatformType.Server)]
public class KTEConditionPlayerQuestFail: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_task_fail;

    [KTLevelExport("taskID")]
    [LabelText("任务Id"), OnValueChanged("Refresh")]
    public int taskId;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
