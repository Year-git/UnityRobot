//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("任务完成"), KTFormatDisplayName("任务{0}完成", "taskId"), KTLevelClassType(TriggerConditionType.specified_task_finish), KTLevelPlatform(PlatformType.Double)]
public class KTEConditionPlayerCompleteQuest: KTLevelTriggerCondition
{

    [HideInInspector, KTLevelExport("conditionType")]
    public TriggerConditionType triggerNodeType = TriggerConditionType.specified_task_finish;

    [KTLevelExport("taskID")]
    [LabelText("任务Id"), OnValueChanged("Refresh")]
    public int taskId;

    public override void Refresh()
    {
        gameObject.name = KTUtils.GetDisplayName(this);
    }

}
#endif
