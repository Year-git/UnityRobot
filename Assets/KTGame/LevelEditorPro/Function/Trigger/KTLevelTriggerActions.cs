#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KTEditor.LevelEditor;
using Sirenix.OdinInspector;

/// <summary>
/// by lijunfeng 2018/6/26 所有KTLevelSpawner类型参数 全部都改为 List<KTLevelSpawner>
/// </summary>
public static class KTLevelTriggerActions
{
    [KTDisplayName("开启孵化器"), KTLevelClassType(TriggerResultType.active_spawner), KTLevelPlatform(PlatformType.Server)]
    static void EnableSpawner(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    [KTDisplayName("关闭孵化器"), KTLevelClassType(TriggerResultType.disable_spawner), KTLevelPlatform(PlatformType.Server)]
    static void DisableSpawner(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    [KTDisplayName("孵化器单位全部死亡"), KTLevelClassType(TriggerResultType.all_dead), KTLevelPlatform(PlatformType.Server)]
    static void KillCreatureGenerator(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    [KTDisplayName("孵化器单位全部消失"), KTLevelClassType(TriggerResultType.all_disappear), KTLevelPlatform(PlatformType.Server)]
    static void DisappearCreatureGenerator(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    [KTDisplayName("延迟"), KTFormatDisplayName("延迟{0}秒", "time"), KTLevelClassType(TriggerResultType.delay), KTLevelPlatform(PlatformType.Double)]
    static void Wait(
        [KTDisplayName("时间(秒)"), KTLevelExport("delay")] float time)
    { }

    //[KTDisplayName("关卡结束"), KTLevelClassType(TriggerResultType.level_over), KTLevelPlatform(PlatformType.Server)]
    //static void CloseMap() { }

    [KTDisplayName("激活触发器"), KTLevelClassType(TriggerResultType.active_trigger), KTLevelPlatform(PlatformType.Double)]
    static void EnableTrigger(
        [KTDisplayName("触发器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelTrigger> trigger)
    { }

    [KTDisplayName("停止触发器"), KTLevelClassType(TriggerResultType.stop_trigger), KTLevelPlatform(PlatformType.Double)]
    static void DisableTrigger(
        [KTDisplayName("触发器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelTrigger> trigger)
    { }

    [KTDisplayName("改变单位巡逻行为"), KTLevelClassType(TriggerResultType.change_patrol_path), KTLevelPlatform(PlatformType.Server)]
    static void SetIdleMotion(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
        [KTDisplayName("巡逻设置")] KTLevelBattleUnitSpawner.PatrolPathSettings settings)
    { }

	//[KTDisplayName("改变单位AI"), KTLevelClassType(TriggerResultType.change_ai), KTLevelPlatform(PlatformType.Server)]
	//static void SetAI(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
	//    [KTDisplayName("AI ID"), KTLevelExport("aiId")] int aiId)
	//{ }

	//[KTDisplayName("单位攻击单位"), KTLevelClassType(TriggerResultType.gen_atk_other_gen), KTLevelPlatform(PlatformType.Server)]
	//static void Attack(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
	//    [KTDisplayName("目标孵化器"), KTLevelExport("__targetGenerator"), KTLevelExport("__targetGenerator_uuid")] List<KTLevelBattleUnitSpawner> spawnerTarget)
	//{ }

	//[KTDisplayName("释放技能"), KTLevelClassType(TriggerResultType.use_skill), KTLevelPlatform(PlatformType.Double)]
	//static void UseSkill(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
	//    [KTDisplayName("技能ID"), KTLevelExport("skillID")] int skillId,
	//    [KTDisplayName("指定技能目标"), KTLevelExport("isSkillTargeting")] bool setSkillTarget,
	//    [KTDisplayName("目标孵化器"), KTLevelExport("__targetGenerator"), KTLevelExport("__targetGenerator_uuid")] List<KTLevelBattleUnitSpawner> targetSpawner)
	//{ }

	//[KTDisplayName("单位始终朝向某个点"), KTLevelClassType(TriggerResultType.look_at), KTLevelPlatform(PlatformType.Double)]
	//static void LookAt(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
	//    [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
	//{ }

	//[KTDisplayName("单位改变状态"), KTLevelClassType(TriggerResultType.change_statu), KTLevelPlatform(PlatformType.Server)]
	//static void ChgStatus(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
	//    [KTDisplayName("状态ID"), KTLevelExport("statuID")] int statusId)
	//{ }

	//[KTDisplayName("单位增加删除阵营标记"), KTLevelClassType(TriggerResultType.change_camp_tag), KTLevelPlatform(PlatformType.Server)]
	//static void ChangeFactionRelation(
	//    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
	//    [KTDisplayName("添加"), KTLevelExport("campTagOpType"), KTValueDropdown("kCampTagOpTypeNames")] TriggerResultCampTagOpType opType,
	//    [KTDisplayName("标记参数"), KTLevelExport("campTag")] int flag)
	//{ }

	//[KTDisplayName("NPC附近出生单位"), KTLevelClassType(TriggerResultType.npc_cur_pos_born_unit), KTLevelPlatform(PlatformType.Server)]
	//static void SpawnNear(
	//    [KTDisplayName("NPC附近出生单位"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
	//    [KTDisplayName("内圈半径"), KTLevelExport("innerRadius")] float innerRadius,
	//    [KTDisplayName("外圈半径"), KTLevelExport("outerRadius")] float outerRadius,
	//    [KTDisplayName("目标孵化器"), KTLevelExport("__targetGenerator"), KTLevelExport("__targetGenerator_uuid")] List<KTLevelSpawner> targetSpawner)
	//{ }

	[KTDisplayName("单位全部进入区域"), KTLevelClassType(TriggerResultType.specified_generator_in_area), KTLevelPlatform(PlatformType.Double)]
	static void MoveToPosition(
		[KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
		[KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
	{ }

	[KTDisplayName("开启AI"), KTLevelClassType(TriggerResultType.open_ai), KTLevelPlatform(PlatformType.Server)]
    static void OpenAI(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    [KTDisplayName("关闭AI"), KTLevelClassType(TriggerResultType.close_ai), KTLevelPlatform(PlatformType.Server)]
    static void CloseAI(
        [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    { }

    //[KTDisplayName("交互物开启"), KTLevelClassType(TriggerResultType.touch_enable), KTLevelPlatform(PlatformType.Server)]
    //static void TouchEnable(
    //    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    //{ }

    //[KTDisplayName("交互物关闭"), KTLevelClassType(TriggerResultType.touch_disable), KTLevelPlatform(PlatformType.Server)]
    //static void TouchDisable(
    //    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
    //{ }

  //  [KTDisplayName("区域增加技能状态"), KTLevelClassType(TriggerResultType.spec_area_add_skill_statu), KTLevelPlatform(PlatformType.Server)]
  //  static void BuffPool(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone,
  //      [KTDisplayName("技能ID"), KTLevelExport("skillID")] int skillId,
  //      [KTDisplayName("阵营ID"), KTLevelExport("campId")] int campId)
  //  { }

  //  [KTDisplayName("单位是否无敌"), KTLevelClassType(TriggerResultType.spec_gen_is_invincible), KTLevelPlatform(PlatformType.Server)]
  //  static void StayInvulnerable(
  //      [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelBattleUnitSpawner> spawner,
  //      [KTDisplayName("无敌"), KTLevelExport("isInvincible")] bool invulnerable)
  //  { }

  //  [KTDisplayName("跟随功能"), KTLevelClassType(TriggerResultType.follow_player), KTLevelPlatform(PlatformType.Server)]
  //  static void FollowThePlayer(
  //      [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
  //      [KTDisplayName("开启"), KTLevelExport("toggle")] bool enable)
  //  { }

  //  [KTDisplayName("指派护送"), KTLevelClassType(TriggerResultType.escort_appointed), KTLevelPlatform(PlatformType.Server)]
  //  static void EscortAppointed(
  //      [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
  //  { }

  //  [KTDisplayName("解除护送"), KTLevelClassType(TriggerResultType.escort_dismissed), KTLevelPlatform(PlatformType.Server)]
  //  static void EscortDismissed(
  //      [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
  //  { }

  //  [KTDisplayName("护送成功"), KTLevelClassType(TriggerResultType.escort_finished), KTLevelPlatform(PlatformType.Server)]
  //  static void EscortFinished(
  //      [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
  //  { }

  //  [KTDisplayName("护送失败"), KTLevelClassType(TriggerResultType.escort_unable), KTLevelPlatform(PlatformType.Server)]
  //  static void EscortUnable(
  //    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner)
  //  { }

  //[KTDisplayName("玩家传送到区域"), KTLevelClassType(TriggerResultType.player_jump_to_area), KTLevelPlatform(PlatformType.Server)]
  //  static void ThePlayerJumpToArea(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone,
  //      [KTDisplayName("是否交互"), KTLevelExport("isReact")] bool interaction)
  //  { }

  //  [KTDisplayName("战斗阶段不再触发"), KTLevelClassType(TriggerResultType.spec_fs_disable), KTLevelPlatform(PlatformType.Server)]
  //  static void FinishInstanceStage(
  //      [KTLevelExport("fsId")] int id)
  //  { }

  //  [KTDisplayName("进度标记"), KTLevelClassType(TriggerResultType.progress_tag), KTLevelPlatform(PlatformType.Server)]
  //  static void FinishInstanceProgress(
  //      [KTDisplayName("标记ID"), KTLevelExport("progressId")] int id)
  //  { }

  //  [KTDisplayName("所有玩家传送到区域"), KTLevelClassType(TriggerResultType.all_player_teleport_to), KTLevelPlatform(PlatformType.Server)]
  //  static void AllPlayersJumpToArea(
  //  [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
  //  { }

  //  [KTDisplayName("场景伤害圈"), KTLevelClassType(TriggerResultType.scene_hurt_circle), KTLevelPlatform(PlatformType.Server)]
  //  static void EffectArea(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone,
  //      [KTDisplayName("阵营Id"), KTLevelExport("campId")] int campId,
  //      [KTDisplayName("进入区域增加"), KTLevelExport("buffId1")] int enterAddAura,
  //      [KTDisplayName("进入区域删除"), KTLevelExport("buffId2")] int enterDelAura,
  //      [KTDisplayName("离开区域增加"), KTLevelExport("buffId3")] int leaveAddAura,
  //      [KTDisplayName("离开区域删除"), KTLevelExport("buffId4")] int leaveDelAura)
  //  { }

  //  [KTDisplayName("场景伤害圈-关闭"), KTLevelClassType(TriggerResultType.close_scene_hurt_circle), KTLevelPlatform(PlatformType.Server)]
  //  static void CloseEffectArea(
  //      [KTDisplayName("触发器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] KTLevelTrigger trigger)
  //  { }

  //  [KTDisplayName("开启|关闭震屏效果"), KTFormatDisplayName("震屏效果{0}:{1}", "id", "enable"), KTLevelClassType(TriggerResultType.screen_shake), KTLevelPlatform(PlatformType.Client)]
  //  static void ScreenShake(
  //      [KTLevelExport("shakeEffectId")] int id,
  //      [KTDisplayName("开启"), KTLevelExport("toggleOn")] bool enable)
  //  { }

  //  [KTDisplayName("场景镜头动画"), KTFormatDisplayName("播放场景镜头动画{0}", "id"), KTLevelClassType(TriggerResultType.play_spec_cam_anim), KTLevelPlatform(PlatformType.Client)]
  //  static void CameraAnimation(
  //      [KTDisplayName("动画Id"), KTLevelExport("camId")] int id)
  //  { }

  //  [KTDisplayName("播放音乐"), KTFormatDisplayName("播放音乐{0}", "id"), KTLevelClassType(TriggerResultType.play_music), KTLevelPlatform(PlatformType.Client)]
  //  static void PlayMusic(
  //      [KTDisplayName("音乐ID"), KTLevelExport("musicId")] int id)
  //  { }

  //  [KTDisplayName("播放天气"), KTFormatDisplayName("播放天气{0}", "id"), KTLevelClassType(TriggerResultType.play_weather), KTLevelPlatform(PlatformType.Server)]
  //  static void PlayWeather(
  //      [KTDisplayName("天气ID"), KTLevelExport("weatherId")] int id)
  //  { }

  //  [KTDisplayName("主角死亡"), KTLevelClassType(TriggerResultType.player_dead), KTLevelPlatform(PlatformType.Server)]
  //  static void GoToDie(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
  //  { }

  //  [KTDisplayName("进入位面"), KTFormatDisplayName("进入位面{0}", "id"), KTLevelClassType(TriggerResultType.enter_parallel_mirror), KTLevelPlatform(PlatformType.Server)]
  //  static void EnterParallelMirror(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone,
  //      [KTDisplayName("id"), KTLevelExport("id")] int id)
  //  { }

  //  [KTDisplayName("退出位面"), KTFormatDisplayName("退出位面{0}", "id"), KTLevelClassType(TriggerResultType.leave_parallel_mirror), KTLevelPlatform(PlatformType.Server)]
  //  static void LeaveParallelMirror(
  //      [KTDisplayName("区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone,
  //      [KTDisplayName("id"), KTLevelExport("id")] int id)
  //  { }

    /// <summary>
    /// by lijunfeng 2018/6/25
    /// </summary>
    /// <param name="spawner"></param>
    /// <param name="id"></param>
    [KTDisplayName("单位对话泡泡"), KTFormatDisplayName("单位播放气泡对话{0}", "id"), KTLevelClassType(TriggerResultType.play_bubble), KTLevelPlatform(PlatformType.Double)]
    static void PlayBubble(
    [KTDisplayName("播放目标"), KTLevelExport("bubbleTargetType"), KTValueDropdown("kBubblePlayTargetNames")]BubbleTargetType bubbleTargetType,
    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid"), KTLevelShowIf("IsBubbleTargetTypeGenerators")] List<KTLevelSpawner> spawner,
    [KTDisplayName("指定播放类型"), KTLevelExport("specifiedBubblePlayType"), KTValueDropdown("kBubblePlayTypeNames")]BubblePlayType specifiedBubblePlayType,
    [KTDisplayName("id"), KTLevelExport("bubbleID")] int id)
    { }

    //[KTDisplayName("单位开始对话|答题"), KTLevelClassType(TriggerResultType.spec_dialog), KTLevelPlatform(PlatformType.Server)]
    //static void SpecDialog(
    //    [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
    //    [KTDisplayName("对话类型"), KTLevelExport("dialogType"), KTValueDropdown("kDialogTypeNames")] DialogType type,
    //    [KTDisplayName("id"), KTLevelExport("dialogID")] int id)
    //{}

    [System.Serializable]
    public class AnimSettings
    {
        [KTDisplayName("动画类型"),KTLevelExport("selectedAnimType")]
        public TriggerResultAnimType animType;

        [KTLevelExport("creatureId")]
        [KTUseDataPicker(KTExcels.kItems, "物件ID", new string[] { "物件ID", "备注" }, "KTLevelEditorWindowPro"), ShowIf("IsItemAnim")]
        public int itemId;
        private bool IsItemAnim() { return animType == TriggerResultAnimType.creature_anim; }

        [KTDisplayName("动画名称"), KTLevelExport("animName")]
        public string animName;

        [KTDisplayName("有镜头动画"), KTLevelExport("isHasShotAnim")]
        public bool isHasShotAnim;

        [KTDisplayName("循环播放"), KTLevelExport("isLoop")]
        public bool isLoop;
    }

   // [KTDisplayName("播放动画"), KTLevelClassType(TriggerResultType.play_specified_animation), KTLevelPlatform(PlatformType.Double)]
   // static void PlayCreatureAnimation(
   //     [KTDisplayName("孵化器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
   //     AnimSettings animSettings)
   // { }

   // [KTDisplayName("重置战斗阶段"), KTLevelClassType(TriggerResultType.reset_fs), KTLevelPlatform(PlatformType.Server)]
   // static void ResetFS(
   //     [KTDisplayName("战斗阶段ID"), KTLevelExport("fsId")] int id)
   // { }


   //[KTDisplayName("过场动画"), KTLevelClassType(TriggerResultType.play_cutscene), KTLevelPlatform(PlatformType.Server)]
   // static void PlayCutScene(
   //     [KTDisplayName("过场动画ID"), KTLevelExport("camId")] int id)
   // { }

   // [KTDisplayName("注视单位"), KTLevelClassType(TriggerResultType.look_at_unit), KTLevelPlatform(PlatformType.Client)]
   // static void LookAtUnit(
   //     [KTDisplayName("源单位"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> srcUnit,
   //     [KTDisplayName("目标单位"), KTLevelExport("__targetGenerator"), KTLevelExport("__targetGenerator_uuid")] List<KTLevelSpawner> dstUnit)
   // { }

   // [KTDisplayName("取消注视单位"), KTLevelClassType(TriggerResultType.cancel_look_at_unit), KTLevelPlatform(PlatformType.Client)]
   // static void CancelLookAtUnit(
   //     [KTDisplayName("源单位"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> srcUnit)
   // { }

   // [KTDisplayName("循环结果"), KTLevelClassType(TriggerResultType.once_again), KTLevelPlatform(PlatformType.Double)]
   // static void OnceAgain(
   //     [KTDisplayName("触发器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> trigger)
   // { }

   // [KTDisplayName("不再循环结果"), KTLevelClassType(TriggerResultType.no_more), KTLevelPlatform(PlatformType.Double)]
   // static void NoMore(
   //     [KTDisplayName("触发器"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> trigger)
   // { }

   // [KTDisplayName("打开窗体"), KTLevelClassType(TriggerResultType.open_window), KTLevelPlatform(PlatformType.Client)]
   // static void OpenWindow(
   // [KTDisplayName("窗体名"), KTLevelExport("windowName")]string windowName)
   // { }

   // [KTDisplayName("触发快速通道"), KTLevelClassType(TriggerResultType.transmission), KTLevelPlatform(PlatformType.Client)]
   // static void Transmission(
   // [KTDisplayName("快速通道id"), KTLevelExport("transmissionID")]int transmissionID)
   // { }

   // [KTDisplayName("触发传送"), KTLevelClassType(TriggerResultType.transport), KTLevelPlatform(PlatformType.Client)]
   // static void Transport(
   // [KTDisplayName("传送id"), KTLevelExport("tranportID")]int tranportID)
   // { }

   // [KTDisplayName("解锁快速通道点"), KTLevelClassType(TriggerResultType.unlock_trans_transmission), KTLevelPlatform(PlatformType.Server)]
   // static void UnlockTransmission(
   // [KTDisplayName("通道点id"), KTLevelExport("transmissionID")]int transmissionID)
   // { }

   // [KTDisplayName("解锁传送点"), KTLevelClassType(TriggerResultType.unlock_trans_transport), KTLevelPlatform(PlatformType.Server)]
   // static void UnlockTransport(
   // [KTDisplayName("传送点id"), KTLevelExport("tranportID")]int tranportID)
   // { }

   // [KTDisplayName("改变场景中所有玩家的视野距离"), KTLevelClassType(TriggerResultType.set_players_viewrange), KTLevelPlatform(PlatformType.Server)]
   // static void SetPlayersViewRange(
   // [KTDisplayName("视野距离"), KTLevelExport("viewRange")]int viewRange)
   // { }

   // [KTDisplayName("控制物件自发光"), KTLevelClassType(TriggerResultType.emissive_control), KTLevelPlatform(PlatformType.Client)]
   // static void EmissiveControl(
   // [KTDisplayName("对应单位"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
   // [KTDisplayName("强度"), KTLevelExport("emissiveScale")] float emissiveScale)
   // { }

   // [KTDisplayName("创建NPC"), KTLevelClassType(TriggerResultType.create_local_npc), KTLevelPlatform(PlatformType.Client)]
   // static void CreateLocalNPC(
   // [KTDisplayName("是否是定制NPC"), KTLevelExport("isCustom")] bool isCustom,
   // [KTDisplayName("对应单位"), KTLevelExport("__result"), KTLevelExport("__result_uuid")] List<KTLevelSpawner> spawner,
   // [KTDisplayName("开始时间戳"), KTLevelExport("timeStampStart")] int timeStampStart,
   // [KTDisplayName("结束时间戳"), KTLevelExport("timeStampEnd")] int timeStampEnd,
   // [KTDisplayName("是否随机位置"), KTLevelExport("isRandomPos")] bool isRandomPos,
   // [KTDisplayName("随机区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")] KTLevelPatrolZone zone)
   // { }

   // [KTDisplayName("任务完成"), KTFormatDisplayName("任务{0}完成", "taskId"), KTLevelClassType(TriggerResultType.specified_task_finish), KTLevelPlatform(PlatformType.Server)]
   // static void PlayerCompleteQuest(
   // [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId)
   // {}

   // [KTDisplayName("任务失败"), KTFormatDisplayName("任务{0}失败", "taskId"), KTLevelClassType(TriggerResultType.specified_task_fail), KTLevelPlatform(PlatformType.Server)]
   // static void PlayerQuestFail(
   // [KTDisplayName("任务Id"), KTLevelExport("taskID")] int taskId)
   // {}
}
#endif
