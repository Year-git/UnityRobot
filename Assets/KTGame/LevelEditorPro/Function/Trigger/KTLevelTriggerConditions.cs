#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KTEditor.LevelEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

/// <summary>
/// by lijunfeng 2018/6/26 所有KTLevelSpawner类型参数 全部都改为 List<KTLevelSpawner>
/// by lijunfeng 2018/6/27 所有方法上增加KTLevelPlatform特性，用于显示不同的菜单分类
/// </summary>
public static class KTLevelTriggerConditions
{
    [KTDisplayName("玩家进入区域"), KTLevelClassType(TriggerConditionType.player_enter_area), KTLevelPlatform(PlatformType.Double)]
    static bool PlayerEnterArea(
    [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")]KTLevelPatrolZone zone)
    { return false; }

    //[KTDisplayName("玩家离开区域"), KTLevelClassType(TriggerConditionType.out_area), KTLevelPlatform(PlatformType.Client)]
    //static bool PlayerExitArea(
    //[KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")]KTLevelPatrolZone zone)
    //{ return false; }

    [KTDisplayName("单位全部死亡"), KTLevelClassType(TriggerConditionType.specified_unit_dead), KTLevelPlatform(PlatformType.Double)]
    static bool CreatureGeneratorAllDead(
    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner)
    { return false; }

	[KTDisplayName("指定单位进入指定路径点"), KTLevelClassType(TriggerConditionType.specified_unit_enter_path_point), KTLevelPlatform(PlatformType.Double)]
	static bool SpecifiedUnitEnterPathPoint(
	[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner,
	[KTDisplayName("指定路径"), KTLevelExport("__patrolPath"), KTLevelExport("__patrolPath_uuid")]KTLevelPatrolPath path,
	[KTDisplayName("指定节点"), KTLevelExport("__patrolPoint"), KTLevelExport("__patrolPoint_uuid")]KTLevelPatrolPoint point)
	{ return false; }

	//[KTDisplayName("指定单位出生"), KTLevelClassType(TriggerConditionType.specified_unit_born), KTLevelPlatform(PlatformType.Double)]
	//static bool CreatureGeneratorBorn(
	//[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner)
	//{ return false; }

	//[KTDisplayName("指定单位离开"), KTLevelClassType(TriggerConditionType.specified_unit_leave), KTLevelPlatform(PlatformType.Client)]
	//static bool CreatureGeneratorLeave(
	//[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner)
	//{ return false; }

	//[KTDisplayName("玩家交互时"), KTLevelClassType(TriggerConditionType.reactor), KTLevelPlatform(PlatformType.Server)]
	//static bool PlayerInvestigate(
	//    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelInteractionItemSpawner> spawner)
	//{ return false; }

	//[KTDisplayName("玩家死亡"), KTLevelClassType(TriggerConditionType.player_dead), KTLevelPlatform(PlatformType.Server)]
	//static bool PlayerDead() { return false; }

	[KTDisplayName("单位(任意)血量低于百分比"), KTFormatDisplayName("单位(任意)血量低于{0}", "hpPercent"), KTLevelClassType(TriggerConditionType.blood_under_percent), KTLevelPlatform(PlatformType.Server)]
    static bool CreatureGeneratorHealthLower(
        [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelBattleUnitSpawner> spawner,
        [KTDisplayName("血量百分比"), KTLevelExport("bloodPercent")] float hpPercent)
    { return false; }

    [KTDisplayName("单位进入区域"), KTLevelClassType(TriggerConditionType.specified_generator_in_area), KTLevelPlatform(PlatformType.Double)]
    static bool CreatureGeneratorMoveReachTarget(
        [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner,
        [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
    { return false; }

    //[KTDisplayName("单位技能释放完成"), KTFormatDisplayName("单位技能{0}释放完成", "skillId"), KTLevelClassType(TriggerConditionType.specified_unit_use_skill), KTLevelPlatform(PlatformType.Double)]
    //static bool CreatureSkillFinished(
    //    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelBattleUnitSpawner> spawner,
    //    [KTDisplayName("技能Id"), KTLevelExport("skillID")] int skillId)
    //{ return false; }

    //[KTDisplayName("单位对话结束"), KTFormatDisplayName("单位对话{0}结束", "id"), KTLevelClassType(TriggerConditionType.specified_generator_play), KTLevelPlatform(PlatformType.Server)]
    //static bool CreatureChatFinished(
    //    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner,
    //    [KTDisplayName("id"), KTLevelExport("bubbleID")] int id)
    //{ return false; }

    //[KTDisplayName("对话结束"), KTFormatDisplayName("对话{0}结束", "id"), KTLevelClassType(TriggerConditionType.specified_dialog_over), KTLevelPlatform(PlatformType.Server)]
    //static bool PlayerChatFinished(
    //    [KTDisplayName("对话类型"), KTLevelExport("dialogType"), KTValueDropdown("kDialogTypeNames")] DialogType dialogType,
    //    [KTDisplayName("id"), KTLevelExport("dialogID")] int id)
    //{ return false; }

    [KTDisplayName("指定发生器进入战斗后"), KTLevelClassType(TriggerConditionType.specified_generator_begin_battle), KTLevelPlatform(PlatformType.Server)]
    static bool SpecifiedGeneratorBeginBattle(
        [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner)
    { return false; }

    [KTDisplayName("指定发生器离开战斗后"), KTLevelClassType(TriggerConditionType.specified_generator_end_battle), KTLevelPlatform(PlatformType.Server)]
    static bool SpecifiedGeneratorEndBattle(
        [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner)
    { return false; }

    //[KTDisplayName("护送成功"), KTLevelClassType(TriggerConditionType.escort_finished), KTLevelPlatform(PlatformType.Server)]
    //static bool EscortFinished(
    //    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner)
    //{ return false; }

    //[KTDisplayName("护送失败"), KTLevelClassType(TriggerConditionType.escort_unable), KTLevelPlatform(PlatformType.Server)]
    //static bool EscortUnable(
    //    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")] List<KTLevelSpawner> spawner)
    //{ return false; }

    //[KTDisplayName("间隔触发"), KTLevelClassType(TriggerConditionType.period_trigger), KTLevelPlatform(PlatformType.Double)]
    //static bool PeriodTrigger(
    //    [KTDisplayName("间隔时间"), KTLevelExport("length")] float period)
    //{ return false; }

    //[KTDisplayName("任务开始"), KTFormatDisplayName("任务{0}开始", "taskId"), KTLevelClassType(TriggerConditionType.specified_task_start), KTLevelPlatform(PlatformType.Double)]
    //static bool PlayerAcceptQuest(
    //    [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId)
    //{ return false; }

    [KTDisplayName("任务完成"), KTFormatDisplayName("任务{0}完成", "taskId"), KTLevelClassType(TriggerConditionType.specified_task_finish), KTLevelPlatform(PlatformType.Double)]
    static bool PlayerCompleteQuest(
        [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId)
    { return false; }

    //[KTDisplayName("任务条件完成"), KTFormatDisplayName("任务{0}条件{1}完成", "taskId", "taskCdnIndex"), KTLevelClassType(TriggerConditionType.specified_task_condition_finish), KTLevelPlatform(PlatformType.Client)]
    //static bool PlayerCompleteQuestCondition(
    //    [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId,
    //    [KTDisplayName("任务条件索引"), KTLevelExport("taskCdnIndex")] int taskCdnIndex)
    //{ return false; }

    [KTDisplayName("任务失败"), KTFormatDisplayName("任务{0}失败", "taskId"), KTLevelClassType(TriggerConditionType.specified_task_fail), KTLevelPlatform(PlatformType.Server)]
    static bool PlayerQuestFail(
    [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId)
    { return false; }

    //[KTDisplayName("指定单位播放指定泡泡完成"), KTLevelClassType(TriggerConditionType.specified_unit_play_bubble_over), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedUnitPlayBubbleOver(
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("id"), KTLevelExport("bubbleID")] int id,
    //[KTDisplayName("播放次数"), KTLevelExport("times")] int times)
    //{ return false; }

    //[KTDisplayName("指定单位区域目标单位数量"), KTLevelClassType(TriggerConditionType.specified_target_unit_count), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedTargetUnitCount(
    //[KTDisplayName("指定单位类型"), KTLevelExport("specifiedUnitType"), KTValueDropdown("kUnitTypeNames")]UnitType specifiedUnitType,
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid"), KTLevelShowIf("IsSpecfiedSelectSelf")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("目标单位数量"), KTLevelExport("count")] int count,
    //[KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")]KTLevelPatrolZone zone)
    //{ return false; }

    //[KTDisplayName("指定数量单位进入指定单位范围"), KTLevelClassType(TriggerConditionType.specified_units_enter_unit), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedUnitsEnterUnit(
    //[KTDisplayName("指定单位类型"), KTLevelExport("specifiedUnitType"), KTValueDropdown("kUnitTypeNames")]UnitType specifiedUnitType,
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid"), KTLevelShowIf("IsSpecfiedSelectGenerator")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("目标单位类型"), KTLevelExport("targetUnitType"), KTValueDropdown("kUnitTypeNames")]UnitType targetUnitType,
    //[KTDisplayName("孵化器"), KTLevelExport("__targetGenerator"), KTLevelExport("__targetGenerator_uuid"), KTLevelShowIf("IsTargetSelectGenerator")]List<KTLevelSpawner> targetSpawner,
    //[KTDisplayName("指定单位区域半径"), KTLevelExport("areaRadius")] int areaRadius,
    //[KTDisplayName("目标单位数量"), KTLevelExport("count")] int count)
    //{ return false; }

    //[KTDisplayName("指定时间"), KTLevelClassType(TriggerConditionType.specified_time), KTLevelPlatform(PlatformType.Double)]
    //static bool SpecifiedTime(
    //[KTDisplayName("时间类型"), KTLevelExport("specifiedTimeType"), KTValueDropdown("kTimeTypeNames")]TimeType specifiedTimeType,
    //[KTDisplayName("时间格式"), KTLevelExport("specifiedTimeFormat"), KTValueDropdown("kTimeFormatNames")]TimeFormat specifiedTimeFormat,
    //[KTDisplayName("关系"), KTLevelExport("relationType"), KTValueDropdown("kRelationNames")]RelationType relationType,
    //[KTDisplayName("年月日"), KTLevelExport("ymd"), KTLevelShowIf("IsTimeFormatSelectStandard")]Vector3 ymd,
    //[KTDisplayName("周"), KTLevelExport("week"), KTLevelShowIf("IsTimeFormatSelectStandard")]int week,
    //[KTDisplayName("时分秒"), KTLevelExport("hms"), KTLevelShowIf("IsTimeFormatSelectStandard")]Vector3 hms,
    //[KTDisplayName("是否相对服务器启动时间"), KTLevelExport("isRelativeToServer"), KTLevelShowIf("IsTimeFormatSelectTimpStamp")]bool isRelativeToServer,
    //[KTDisplayName("时间戳"), KTLevelExport("timeStamp"), KTLevelShowIf("IsTimeFormatSelectTimpStamp")]int timeStamp)
    //{ return false; }

    //[KTDisplayName("指定答题结束"), KTLevelClassType(TriggerConditionType.specified_quiz_over), KTLevelPlatform(PlatformType.Server)]
    //static bool SpecifiedQuizOver(
    //[KTDisplayName("答题结果类型"), KTLevelExport("quizResult"), KTValueDropdown("kQuizResultNames")]QuizResult quizResult,
    //[KTDisplayName("对应数量"), KTLevelExport("count")]int count,
    //[KTDisplayName("答题ID"), KTLevelExport("dialogID")]int id)
    //{ return false; }

    //[KTDisplayName("世界事件发生"), KTLevelClassType(TriggerConditionType.world_event_begin), KTLevelPlatform(PlatformType.Server)]
    //static bool WorldEventBegin(
    //[KTDisplayName("世界事件ID"), KTLevelExport("eventID")]int id)
    //{ return false; }

    //[KTDisplayName("世界事件结束"), KTLevelClassType(TriggerConditionType.world_event_finish), KTLevelPlatform(PlatformType.Server)]
    //static bool WorldEventFinish(
    //[KTDisplayName("世界事件ID"), KTLevelExport("eventID")]int id)
    //{ return false; }

    //[KTDisplayName("世界事件失败"), KTLevelClassType(TriggerConditionType.world_event_fail), KTLevelPlatform(PlatformType.Server)]
    //static bool WorldEventFail(
    //[KTDisplayName("世界事件ID"), KTLevelExport("eventID")]int id)
    //{ return false; }

    //[KTDisplayName("指定单位交互完成"), KTLevelClassType(TriggerConditionType.specified_unit_act_over), KTLevelPlatform(PlatformType.Double)]
    //static bool SpecifiedUnitActOver(
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("指定交互ID"), KTLevelExport("actID")] int id)
    //{ return false; }

    //[KTDisplayName("指定天气开始"), KTLevelClassType(TriggerConditionType.specified_weather_start), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedWeatherStart(
    //[KTDisplayName("指定天气ID"), KTLevelExport("weatherID")] int weatherID)
    //{ return false; }

    //[KTDisplayName("指定天气结束"), KTLevelClassType(TriggerConditionType.specified_weather_end), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedWeatherEnd(
    //[KTDisplayName("指定天气ID"), KTLevelExport("weatherID")] int weatherID)
    //{ return false; }

    //[KTDisplayName("指定单位被碰撞次数"), KTLevelClassType(TriggerConditionType.specified_unit_hit_count), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedUnitHitCount(
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("次数"), KTLevelExport("count")]int count)
    //{ return false; }

    //[KTDisplayName("指定单位躲闪碰撞次数"), KTLevelClassType(TriggerConditionType.specified_unit_dodge_count), KTLevelPlatform(PlatformType.Client)]
    //static bool SpecifiedUnitDodagCount(
    //[KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner,
    //[KTDisplayName("次数"), KTLevelExport("count")]int count)
    //{ return false; }

    [KTDisplayName("孵化器内任意一个单位死亡"), KTLevelClassType(TriggerConditionType.any_unit_dead), KTLevelPlatform(PlatformType.Server)]
    static bool AnyUnitDead(
    [KTDisplayName("孵化器"), KTLevelExport("__condition"), KTLevelExport("__condition_uuid")]List<KTLevelSpawner> spawner)
    { return false; }
}
#endif