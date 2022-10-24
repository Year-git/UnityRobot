using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SkillController myController {get; private set;}

    /// <summary>
    /// 技能配置Id
    /// </summary>
    /// <value></value>
    public int cfgId{get; private set;}

    /// <summary>
    /// 技能冷却时间配置
    /// </summary>
    /// <value></value>
    public int cooldownTime{get; private set;}

    /// <summary>
    /// 技能冷却时间记录
    /// </summary>
    private int _cooldownTimeRecord = 0;

    /// <summary>
    /// 技能最大范围配置
    /// </summary>
    /// <value></value>
    public int maxRange{get; private set;}

    /// <summary>
    /// 技能最小范围配置
    /// </summary>
    /// <value></value>
    public int minRange{get; private set;}

    /// <summary>
    /// 技能朝向角度配置
    /// </summary>
    /// <value></value>
    public int viewAngle{get; private set;}

    /// <summary>
    /// 技能动作索引配置
    /// </summary>
    /// <value></value>
    public int animationIdx{get; private set;}

    /// <summary>
    /// 技能是否在释放中
    /// </summary>
    /// <value></value>
    public bool isDo{get; private set;} = false;

    /// <summary>
    /// 技能前摇时间配置
    /// </summary>
    /// <value></value>
    public int beforeTime{get; private set;}
    /// <summary>
    /// 技能持续时间配置
    /// </summary>
    /// <value></value>
    public int durationTime{get; private set;}
    /// <summary>
    /// 技能后摇时间配置
    /// </summary>
    /// <value></value>
    public int afterTime{get; private set;}

    /// <summary>
    /// 技能阶段释放时间记录
    /// </summary>
    public int _stageTimeRecord = 0;

    /// <summary>
    /// 技能阶段进入记录
    /// </summary>
    private bool _stageEnterRecord = false;

    /// <summary>
    /// 是否自动释放
    /// </summary>
    /// <value></value>
    public bool isAuto{get; private set;}

    /// <summary>
    /// 是否预警
    /// </summary>
    /// <value></value>
    public bool isWarning{get; private set;}

    /// <summary>
    /// 技能释放阶段记录容器
    /// </summary>
    /// <typeparam name="BehaviacSkillDoStageType"></typeparam>
    /// <returns></returns>
    private HashSet<BehaviacSkillDoStageType> _doStageContainer = new HashSet<BehaviacSkillDoStageType>();

    /// <summary>
    /// 是否手动释放
    /// </summary>
    private bool _isManualDo = false;

    /// <summary>
    /// 技能映射的配件实例Id容器
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    public HashSet<int> _partInstIdContainer = new HashSet<int>();

    private Skill(){}

    public Skill(int nSkillCfgId, SkillController pMyController)
    {
        this.myController = pMyController;
        this.cfgId = nSkillCfgId;
        this.cooldownTime = Skill.GetSkillCooldownTime(nSkillCfgId);
        this.maxRange = Skill.GetSkillMaxRange(nSkillCfgId);
        this.minRange = Skill.GetSkillMinRange(nSkillCfgId);
        this.viewAngle = Skill.GetSkillViewRange(nSkillCfgId);
        this.animationIdx = Skill.GetSkillAnimationIdx(nSkillCfgId);
        this.beforeTime = Skill.GetSkillBeforeTime(nSkillCfgId);
        this.durationTime = Skill.GetSkillDurationTime(nSkillCfgId);
        this.afterTime = Skill.GetSkillAfterTime(nSkillCfgId);
        this.isAuto = Skill.GetSkillAuto(nSkillCfgId);
        this.isWarning = Skill.GetSkillIsWarning(nSkillCfgId);
    }

    public void Update()
    {
        if (!this.isDo)
        {
            return;
        }

        if (this._doStageContainer.Contains(BehaviacSkillDoStageType.BeforeUpdate))
        {
            this.BeforeStageUpdate();
        }
        else if (this._doStageContainer.Contains(BehaviacSkillDoStageType.DurationUpdate))
        {
            this.DurationStageUpdate();
        }
        else if (this._doStageContainer.Contains(BehaviacSkillDoStageType.AfterUpdate))
        {
            this.AfterStageUpdate();
        }
    }

    private void BeforeStageStart()
    {
        this._stageEnterRecord = false;
        this._doStageContainer.Clear();
        this._isManualDo = false;
        this.isDo = true;

        if (this.beforeTime == 0)
        {
            this.BeforeStageEnd();
        }
        else
        {
            this._doStageContainer.Add(BehaviacSkillDoStageType.BeforeEnter);
            this._doStageContainer.Add(BehaviacSkillDoStageType.BeforeUpdate);
            this._stageTimeRecord = this.beforeTime + FrameSynchronManager.Instance.fsData.FrameRunningTime;
            this._stageEnterRecord = true;

            if (this.isWarning)
            {
                this.myController.myNpc.myIFX_AuraCloner.CloneLifeTime = (float)this.beforeTime / 1000f;
                this.myController.myNpc.myIFX_AuraCloner.MakeCloneOnce();
            }
        }
    }

    private void BeforeStageUpdate()
    {
        if (this._stageEnterRecord)
        {
            this._stageEnterRecord = false;
        }
        else
        {
            if (this._doStageContainer.Contains(BehaviacSkillDoStageType.BeforeEnter))
            {
                this._doStageContainer.Remove(BehaviacSkillDoStageType.BeforeEnter);
            }

            if (FrameSynchronManager.Instance.fsData.FrameRunningTime >= this._stageTimeRecord)
            {
                this.BeforeStageEnd();
            }
        }
    }

    private void BeforeStageEnd()
    {
        this.DurationStageStart();
    }

    private void DurationStageStart()
    {
        this._doStageContainer.Clear();
        this.Do();
        
        if (this.durationTime == 0)
        {
            this.DurationStageEnd();
        }
        else
        {
            this._doStageContainer.Add(BehaviacSkillDoStageType.DurationEnter);
            this._doStageContainer.Add(BehaviacSkillDoStageType.DurationUpdate);
            this._stageTimeRecord = this.durationTime + FrameSynchronManager.Instance.fsData.FrameRunningTime;
        }
    }

    private void DurationStageUpdate()
    {
        if (this._doStageContainer.Contains(BehaviacSkillDoStageType.DurationEnter))
        {
            this._doStageContainer.Remove(BehaviacSkillDoStageType.DurationEnter);
        }

        if (FrameSynchronManager.Instance.fsData.FrameRunningTime >= this._stageTimeRecord)
        {
            this.DurationStageEnd();
        }
    }

    private void DurationStageEnd()
    {
        this.AfterStageStart();
    }

    private void AfterStageStart()
    {
        this._doStageContainer.Clear();

        if (this.afterTime == 0)
        {
            this.AfterStageEnd();
        }
        else
        {
            this._doStageContainer.Add(BehaviacSkillDoStageType.AfterEnter);
            this._doStageContainer.Add(BehaviacSkillDoStageType.AfterUpdate);
            this._stageTimeRecord = this.afterTime + FrameSynchronManager.Instance.fsData.FrameRunningTime;
        }
    }

    private void AfterStageUpdate()
    {
        if (this._doStageContainer.Contains(BehaviacSkillDoStageType.AfterEnter))
        {
            this._doStageContainer.Remove(BehaviacSkillDoStageType.AfterEnter);
        }

        if (FrameSynchronManager.Instance.fsData.FrameRunningTime >= this._stageTimeRecord)
        {
            if (this._doStageContainer.Contains(BehaviacSkillDoStageType.AfterExit))
            {
                this.AfterStageEnd();
            }
            else
            {
                this._doStageContainer.Add(BehaviacSkillDoStageType.AfterExit);
            }
        }
    }

    private void AfterStageEnd()
    {
        this._doStageContainer.Clear();
        this._isManualDo = false;
        this.isDo = false;
        this.SetCooldown();
    }

    /// <summary>
    /// 注册配件关联
    /// </summary>
    /// <param name="nPartInstId"></param>
    public void Regist(int nPartInstId)
    {
        if (this._partInstIdContainer.Contains(nPartInstId))
        {
            CommFunc.LogError("Skill.Insert->Part Skill Is Have Exist!" + "#NpcName = " + this.myController.myNpc.myModel.gameObject.name + "#SkillCfgId = " + this.cfgId + "#PartInstId = " + nPartInstId);
            return;
        }
        this._partInstIdContainer.Add(nPartInstId);
    }

    /// <summary>
    /// 反注册配件关联
    /// </summary>
    /// <param name="nPartInstId"></param>
    public void UnRegist(int nPartInstId)
    {
        if (!this._partInstIdContainer.Contains(nPartInstId))
        {
            CommFunc.LogError("Skill.Remove->Part Skill Is Not Exist!" + "#NpcName = " + this.myController.myNpc.myModel.gameObject.name + "#SkillCfgId = " + this.cfgId + "#PartInstId = " + nPartInstId);
            return;
        }
        this._partInstIdContainer.Remove(nPartInstId);
    }

    public bool DoStart()
    {
        if (this.IsCooldown())
        {
            return false;
        }

        if (this.isDo)
        {
            return false;
        }

        this.BeforeStageStart();
        
        return true;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <returns></returns>
    private void Do()
    {
        // 用来缓存要播放动作的配件列表
        HashSet<RobotPart> playAnimationPart = new HashSet<RobotPart>();
        foreach(int nPartInstId in this._partInstIdContainer)
        {
            RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
            if (pPart == null || !pPart.isDead)
            {
                continue;
            }

            if (pPart.skillId != this.cfgId)
            {
                continue;
            }

            pPart.DoSkill();

            // 添加释放技能的配件到缓存列表
            playAnimationPart.Add(pPart);
        }

        if (playAnimationPart.Count > 0)
        {
            // 添加联动的车体配件到缓存列表
            RobotPart pBodyPart = this.myController.myNpc.GetRobotBodyPart();
            if (pBodyPart != null)
            {
                if (!playAnimationPart.Contains(pBodyPart))
                {
                    playAnimationPart.Add(pBodyPart);
                }
            }

            // 添加联动的移动配件到缓存列表
            RobotPart pMovePart = this.myController.myNpc.GetRobotMovePart();
            if (pMovePart != null)
            {
                if (!playAnimationPart.Contains(pMovePart))
                {
                    playAnimationPart.Add(pMovePart);
                }
            }

            // 将缓存列表中配件全部播动作
            foreach(RobotPart pPart in playAnimationPart)
            {
                pPart.AnimationPlay(this.animationIdx);
            }
        }
    }

    /// <summary>
    /// 是否在冷却中
    /// </summary>
    /// <param name="nSkillCfgId"></param>
    /// <returns></returns>
    public bool IsCooldown()
    {
        if (FrameSynchronManager.Instance.fsData.FrameRunningTime >= this._cooldownTimeRecord)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 设置是否冷却
    /// </summary>
    /// <param name="bCoolDown"></param>
    public void SetCooldown(bool bCoolDown = true)
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.SetCooldown->Can Not Call In Skill Is Doing!");
            return;
        }

        if (bCoolDown)
        {
            this._cooldownTimeRecord = FrameSynchronManager.Instance.fsData.FrameRunningTime + this.cooldownTime;
        }
        else
        {
            this._cooldownTimeRecord = 0;
        }
    }

    /// <summary>
    /// 是否在最大范围内
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsMaxRangeIn(Vector3 pPos)
    {
        return this.myController.myNpc.IsInRangeOfPos(pPos, this.maxRange);
    }

    /// <summary>
    /// 是否在最小范围外
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsMinRangeOut(Vector3 pPos)
    {
        return !this.myController.myNpc.IsInRangeOfPos(pPos, this.minRange);
    }

    /// <summary>
    /// 是否在范围中
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsRangeIn(Vector3 pPos)
    {
        if (this.IsMaxRangeIn(pPos) && this.IsMinRangeOut(pPos))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否在朝向范围中
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsViewIn(Vector3 pPos)
    {
        if (this.myController.myNpc.IsLookAtPos(pPos, this.viewAngle))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否是指定的技能释放阶段
    /// </summary>
    /// <param name="eStage"></param>
    /// <returns></returns>
    public bool IsDoStage(BehaviacSkillDoStageType eStage)
    {
        if (this._doStageContainer.Contains(eStage))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否是技能释放结束
    /// </summary>
    /// <returns></returns>
    public bool IsDoEnd()
    {
        if (this.IsDoStage(BehaviacSkillDoStageType.AfterUpdate) && this.IsDoStage(BehaviacSkillDoStageType.AfterExit))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置手动释放
    /// </summary>
    public void SetManualDo()
    {
        if (!this.isDo)
        {
            
            CommFunc.LogError("Skill.SetManualDo->Can Not Call In Skill Is Not Doing!");
            return;
        }

        this._isManualDo = true;
    }

    /// <summary>
    /// 手动释放配件能力
    /// </summary>
    public void ManualDo()
    {
        if (!this.isDo)
        {
            CommFunc.LogError("Skill.ManualDo->Can Not Call In Skill Is Not Doing!");
            return;
        }

        this.Do();
    }

    /// <summary>
    /// 设置前摇时间
    /// </summary>
    /// <param name="nTime"></param>
    public void SetBeforeTime(int nTime)
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.SetBeforeTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.beforeTime = nTime;
    }

    /// <summary>
    /// 重置前摇时间
    /// </summary>
    public void ResetBeforeTime()
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.ResetBeforeTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.beforeTime = Skill.GetSkillBeforeTime(this.cfgId);
    }

    /// <summary>
    /// 设置持续时间
    /// </summary>
    /// <param name="nTime"></param>
    public void SetDurationTime(int nTime)
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.SetDurationTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.durationTime = nTime;
    }

    /// <summary>
    /// 重置持续时间
    /// </summary>
    public void ResetDurationTime()
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.ResetDurationTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.durationTime = Skill.GetSkillDurationTime(this.cfgId);
    }

    /// <summary>
    /// 设置后摇时间
    /// </summary>
    /// <param name="nTime"></param>
    public void SetAfterTime(int nTime)
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.SetAfterTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.afterTime = nTime;
    }

    /// <summary>
    /// 重置后摇时间
    /// </summary>
    public void ResetAfterTime()
    {
        if (this.isDo)
        {
            CommFunc.LogError("Skill.ResetAfterTime->Can Not Call In Skill Is Doing!");
            return;
        }

        this.afterTime = Skill.GetSkillAfterTime(this.cfgId);
    }

    //----------------------------------------------------------------------------------------
    // 读表操作

    /// <summary>
    /// 获取技能冷却时间
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillCooldownTime(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "skillCd");
    }

    /// <summary>
    /// 获取技能最大范围
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillMaxRange(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "maxRange");
    }

    /// <summary>
    /// 获取技能最小范围
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillMinRange(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "minRange");
    }

    /// <summary>
    /// 获取技能朝向范围
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillViewRange(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "directionRange");
    }

    /// <summary>
    /// 获取技能动作索引
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillAnimationIdx(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "actionIndex");
    }

    /// <summary>
    /// 获取技能释放前摇时间
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillBeforeTime(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "beforeTime");
    }

    /// <summary>
    /// 获取技能释放持续时间
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillDurationTime(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "durationTime");
    }

    /// <summary>
    /// 获取技能释放后摇时间
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetSkillAfterTime(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "afterTime");
    }

    /// <summary>
    /// 是否自动释放技能
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static bool GetSkillAuto(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "auto") == 1;
    }

    /// <summary>
    /// 是否播放预警
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static bool GetSkillIsWarning(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Skill_C", nCfgId, "warning") == 1;
    }
}
