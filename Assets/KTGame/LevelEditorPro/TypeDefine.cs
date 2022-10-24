using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTEditor.LevelEditor {
	public enum SpawnPointPolicyType
	{
		Sequential,
		Random,
		Random2,
	}
	public enum GeneratorType {
		item = 1,
		interactionItem,
		battleUnit,
        count,
    }

    /// <summary>
    /// 孵化器子类型
    /// by lijunfeng 2018/7/10
    /// </summary>
    public enum SpawnerType
    {
        unit_spawner,
        item_spawner,
        interact_item_spawner,
    }

    public enum RoleType {
        npc = 1,
        little_monster,
        elite_monster,
        little_boss,
        big_boss,
        other,
        reactor,
        trap,
        un_seletable,
        public_carrier,
        spawner_group,//by lijunfeng 2018/6/25
        collection,//by lijunfeng 2018/8/11

        count,
    }

    public enum AbioticType {
        abiotic = 1,

        count,
    }

    public enum ItemNodeType {
        none,
        root,
        camera,
        born_location,
        fight_stage,
        monster_generator,
        trigger,
        patrol_path,
        patrol_area,
        monster,
        patrol_point,
    }

    public enum LevelType {
        manual,
        followPlayer
    }

    public enum ShowType {        
        only_self,
        for_all,
        other_players,//所有其他玩家 by lijunfeng 2018/6/23
        generators      //所有孵化器 by lijunfeng 2018/6/23
    }

    //by lijunfeng 2018/6/23
    public enum UnitType
    {
        self,
        all,
        other_players,//所有其他玩家
        generators      //所有孵化器
    }

    //by lijunfeng 2018/6/23
    public enum TimeType
    {
        World,//自然时间
        Game,//游戏时间
    }

    //by lijunfeng 2018/8/13
    public enum TimeFormat
    {
        Standard,//标准时间
        TimpStamp,//时间戳
    }

    //by lijunfeng 2018/1016
    //数值关系
    public enum RelationType
    {
        Equal,
        Greater,
        Less,
    }

    /// <summary>
    /// 平台类型
    /// </summary>
    public enum PlatformType
    {
        Client,
        Server,
        Double
    }

    //by lijunfeng 2018/6/23
    public enum QuizResult
    {
        Correct,
        Wrong
    }

    public enum MobType {
        interval,
        append
    }

    public enum PatrolType {
        appoint,
        area
    }

    public enum TriggerConditionType {
        player_enter_area,
        specified_unit_dead,
        specified_task_start,
        specified_task_finish,
        reactor,  //玩家交互时
        player_dead,
        blood_under_percent,            //发生器内任意单位血量低于百分比时触发
        specified_generator_play,       //指定发生器内的全部单位播放完后触发
        specified_generator_in_area,    //指定发生器内的全部单位进入指定的区域
        specified_unit_use_skill,       //指定单位技能释放完成

        specified_dialog_over,              // 指定对话结束后
        specified_generator_begin_battle,   // 指定发生器进入战斗后
        specified_generator_end_battle,     // 指定发生器离开战斗后.
        specified_quiz_over,                // 指定答题结束.
        escort_finished,                    // 护送成功
        escort_unable,                      // 护送失败
        period_trigger,                     // 间隔触发
        specified_unit_play_bubble_over,  //指定单位播放指定泡泡完成 by lijunfeng 2018/6/22
        specified_target_unit_count,  //指定单位区域目标单位数量 by lijunfeng 2018/10/17
        specified_unit_enter_path_point,  //指定单位进入指定路径节点 by lijunfeng 2018/6/23
        specified_time,  //指定时间 by lijunfeng 2018/6/23
        world_event_begin,//世界事件发生 by lijunfeng 2018/8/1
        world_event_finish,//世界事件结束 by lijunfeng 2018/8/1
        specified_task_condition_finish,//特定任务条件完成 by lijunfeng 2018/8/17
        out_area,//玩家主角离开区域by lijunfeng 2018/9/18
        specified_task_fail,//特定任务失败 by lijunfeng 2018/10/16
        world_event_fail,//特定世界事件失败 by lijunfeng 2018/10/16
        specified_unit_born, //指定单位出生by lijunfeng 2018/10/16
        specified_unit_act_over,//指定单位指定交互完成by lijunfeng 2018/10/16
        specified_unit_leave,//指定单位消失 lijunfeng 2018/10/17 （本地npc)
        specified_units_enter_unit,//指定数量单位进入指定单位范围 lijunfeng 2018/10/17 （本地npc)
        specified_weather_start,//指定天气开始 lijunfeng 2018/10/17 
        specified_weather_end,//指定天气结束 lijunfeng 2018/10/17
        specified_unit_hit_count,//指定单位被碰撞次数 lijunfeng 2018/10/17 （本地npc)
        specified_unit_dodge_count,//指定单位躲闪碰撞次数 lijunfeng 2018/10/17 （本地npc)
        any_unit_dead,//孵化器内任意一个单位死亡lijunfeng 2018/11/27

        count
    }

    public enum TriggerConditionPlayType {
        bubble_dialog = 1,                         //气泡对话
    }

    public enum TriggerConditionRelationType {
        all,
        one,
        spec, //手动填写

        count
    }

    public enum DialogType {
        dialog = 1,
        answer,
    }

    //泡泡状态 by lijunfeng 2018/6/25
    public enum BubblePlayType
    {
        play=1,//播放
        stop=2,//停止
    }

    //泡泡目标 by lijunfeng 2018/8/17
    public enum BubbleTargetType
    {
        self,//自己
        generators,//其他孵化器
    }

    public enum ToggleType {
        off,
        on,
    }

    public enum TriggerResultType {
        active_spawner,
        disable_spawner,
        all_dead,
        all_disappear,
        delay,
        level_over,
        stop_trigger,
        active_trigger,

        change_patrol_path,             //改变发生器内单位行为路径
        change_ai,                      //改变发生器内单位AI行为
        play_specified_animation,       //播放指定动画
        gen_atk_other_gen,              //指定发生器内的单位攻击指定发生器内单位

        play_bubble,                    //指定单位播放指定的气泡对话
        use_skill,                      //指定发生器内单位释放指定技能
        look_at,                        //指定发生器内单位始终朝向某个点
        change_statu,                   //指定发生器内单位改变状态
        change_camp_tag,                //指定发生器内的单位增加和减少阵营标记
        npc_cur_pos_born_unit,          //指定npc当前位置出生单位
        specified_generator_in_area,    //指定发生器内的全部单位进入指定的区域
        close_ai,                       //关闭AI
        open_ai,                        //开启AI
        touch_enable,                   //指定交互物激活
        touch_disable,                  //指定交互物无法采集
        spec_area_add_skill_statu,      //指定区域增加技能状态
        spec_gen_is_invincible,         //指定发生器内的单位是否无敌

        spec_dialog,                    //指定单位变为对话(答题)功能状态
        stop_bubble,                    //停止播放气泡
        follow_player,                  //指定单位跟随玩家
        player_jump_to_area,            //玩家跳到指定区域
        spec_fs_disable,                //指定战斗阶段编号不再触发
        progress_tag,                   //进度标记(副本)

        do_spec_skill_loop,             //指定发生器内单位释放指定循环技能
        remove_spec_skill,              //删除指定发生器内单位的指定技能
        all_player_teleport_to,         //场景中所有玩家瞬移到指定区域
        scene_hurt_circle,              //场景伤害圈

        screen_shake,                   //播放/关闭屏幕震动
        play_spec_cam_anim,             //播放场景镜头动画

        play_music,                     //播放指定音乐
        play_weather,                   //播放指定天气

        player_dead,                    //主角死亡

        enter_parallel_mirror,          //进入位面
        leave_parallel_mirror,          //离开位面

        close_scene_hurt_circle,        //关闭场景伤害圈

        reset_fs,                       //重置战斗阶段

        escort_appointed,               //指派护送
        escort_dismissed,               //解除护送
        escort_finished,                //护送成功
        escort_unable,                  //护送失败

        play_cutscene,                  //过场动画

        look_at_unit,                   //注视单位
        cancel_look_at_unit,            //取消注视单位    

        once_again,                     //循环结果
        no_more,                        //不再循环结果

        open_window,                    //打开窗体
        transmission,                   //触发快速通道 by lijunfeng 2018/7/2
        transport,                      //触发快速传送 by lijunfeng 2018/7/16
        unlock_trans_transmission,      //解锁快速通道点 by lijunfeng 2018/8/6
        unlock_trans_transport,         //解锁传送点点 by lijunfeng 2018/8/6
        set_players_viewrange,          //改变场景中所有玩家的视野距离 by lijunfeng 2018/8/24
        emissive_control,               //控制场景自发光 by lijunfeng 2018/9/26
        create_local_npc,               //创建本地npcby lijunfeng 2018/10/18
        specified_task_finish,          //触发任务完成 by lijunfeng 2018/11/22
        specified_task_fail,            //触发任务失败 by lijunfeng 2018/11/22

        count
    }

    public enum TriggerResultCampTagOpType {
        op_add,
        op_delete,

        count,
    }

    public enum TriggerResultAnimType {
        monster_anim,
        creature_anim,

        count,
    }

    public enum TriggerResultTargetMode {
        current,
        dps_cure_tank,

        count,
    }
}