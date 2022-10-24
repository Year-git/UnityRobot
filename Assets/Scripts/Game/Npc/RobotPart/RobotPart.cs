using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RobotPart : MapObject
{
    /// <summary>
    /// 所属的Npc实例Id
    /// </summary>
    public int npcInstId;

    /// <summary>
    /// 配件配置ID
    /// </summary>
    public int cfgId { get; private set; }

    /// <summary>
    /// 配件类型
    /// </summary>
    public RobotPartType partType;

    /// <summary>
    /// 配件模型
    /// </summary>
    public List<RobotPartElement> myElementList{get;private set;} = new List<RobotPartElement>();

    /// <summary>
    /// 对应槽位
    /// </summary>
    public int holeIdx {get;private set;} = -1;

    /// <summary>
    /// 配件缩放
    /// </summary>
    public float myScale { get; set; } = 1f;

    /// <summary>
    /// 配件属性
    /// </summary>
    public AttributeEntity attrEntityCfg {get;set;}

    public int skillId {get; private set;}

    public bool isDead
    {
        get
        {
            bool bRet = false;
            foreach(var pElement in this.myElementList)
            {
                if (!pElement.isDead)
                {
                    bRet = true;
                    break;
                }
            }
            return bRet;
        }
    }

    public RobotPart(int nCfgId) : base()
    {
        this.cfgId = nCfgId;
        this.partType = RobotPart.GetRobotPartType(nCfgId);
        this.skillId = RobotPart.GetRobotPartSkillId(nCfgId);
        this.InitAttrCfg();
    }

    public void Load(int nNum, int nModelCfgId, Vector3 fv3Position, Vector3 fv3EulerAngles, float nParentScale, Action fLoaded = null)
    {
        this.myScale = Model.GetModelScale(nModelCfgId) * nParentScale;

        CommAsyncCounter pCounter = new CommAsyncCounter(nNum, delegate()
            {
                this.Start();
                fLoaded?.Invoke();
            }
        );

        for(int i = 0; i < nNum; i++)
        {
            RobotPartElement element = new RobotPartElement();
            element.myRobotPart = this;
            this.myElementList.Add(element);
            element.Load(nModelCfgId, Vector3.zero, Vector3.zero, this.myScale, delegate (RobotPartElement pElement)
            {
                pElement.myScript = pElement.myModel.gameObject.GetComponent<RobotPartScriptBase>();
                pElement.myScript.myElement = pElement;

                pCounter.Increase();
            });
        }
    }

    /// <summary>
    /// 设置配件安装信息
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nHoleIdx"></param>
    public void SetInstallInfo(int nNpcInstId, int nHoleIdx)
    {
        this.holeIdx = nHoleIdx;
        this.npcInstId = nNpcInstId;
    }

    /// <summary>
    /// 初始化配件配置属性
    /// </summary>
    private void InitAttrCfg()
    {
        this.attrEntityCfg = new AttributeEntity();
        this.attrEntityCfg.InitAttrValue(GetRobotPartRandomAttribute(this.cfgId));
    }

    /// <summary>
    /// 配件实例化后调用通知
    /// </summary>
    public void Start()
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementStart();
        }
    }

    /// <summary>
    /// 地图数据处理完成时通知
    /// </summary>
    public void OnMapDataDisposed()
    {
        foreach(var element in this.myElementList)
        {
            element.OnMapDataDisposed();
        }
    }

    /// <summary>
    /// 配件删除处理
    /// </summary>
    public void Destroy()
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementDestroy();
        }
    }

    /// <summary>
    /// 配件刷新
    /// </summary>
    public void Update()
    {
        foreach(var element in this.myElementList)
        {
            if (element.loadEnd)
            {
                element.OnPartElementUpdate();
            }
        }
    }

    /// <summary>
    /// 配件安装时的调用通知
    /// </summary>
    /// <param name="nNpcInstId"></param>
    public void OnInstall(Action fLoaded = null)
    {
        CommAsyncCounter pCounter = new CommAsyncCounter(this.myElementList.Count, delegate()
            {
                MapManager.Instance.baseMap.GetNpc(this.npcInstId).mySkillController.AddSkill(
                    this.skillId,
                    this.InstId
                );
                fLoaded?.Invoke();
            }
        );

        foreach(var element in this.myElementList)
        {
            element.OnPartElementInstall(delegate()
            {
                pCounter.Increase();
            });
        }
    }

    /// <summary>
    /// 配件卸载时的调用通知
    /// </summary>
    public void OnUnInstall()
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementUnInstall();
        }

        MapManager.Instance.baseMap.GetNpc(this.npcInstId).mySkillController.DelSkill(
            this.skillId,
            this.InstId
        );
    }

    /// <summary>
    /// 帧同步逻辑帧数据更新调用通知
    /// </summary>
    /// <param name="jInfo"></param>
    public void FmSynLogicDataUpdate(JObject jInfo)
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementFmSynLogicDataUpdate(jInfo);
        }
    }

    /// <summary>
    /// 帧同步逻辑帧更新调用通知
    /// </summary>
    public void FmSynLogicUpdate()
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementFmSynLogicUpdate();
        }
    }
    
    /// <summary>
    /// 帧同步渲染帧更新调用通知
    /// </summary>
    /// <param name="nInterpolation"></param>
    public void FmSynRenderUpdate(Fix64 nInterpolation)
    {
        foreach(var element in this.myElementList)
        {
            element.OnPartElementFmSynRenderUpdate(nInterpolation);
        }
    }

    // ----------------------------------------------------------------------------------------------------------
    // 动作相关代码

    /// <summary>
    /// 配件通知零件播放指定索引的动作
    /// </summary>
    /// <param name="nAnimationIdx"></param>
    public void AnimationPlay(int nAnimationIdx)
    {
        foreach(var pElement in this.myElementList)
        {
            pElement.AnimationPlay(nAnimationIdx);
        }
    }

    // ----------------------------------------------------------------------------------------------------------
    // 战斗相关代码

    /// <summary>
    /// Npc伤害输入通知
    /// </summary>
    public void OnNpcDamageInput()
    {
        foreach(var element in this.myElementList)
        {
            element.OnNpcDamageInput();
        }
    }

    /// <summary>
    /// Npc治疗输入通知
    /// </summary>
    public void OnNpcTreatmentInput()
    {
        foreach(var element in this.myElementList)
        {
            element.OnNpcTreatmentInput();
        }
    }

    /// <summary>
    /// Npc死亡通知
    /// </summary>
    public void OnNpcDead()
    {
        foreach(var element in this.myElementList)
        {
            element.OnNpcDead();
        }
    }

    // Npc启用状态改变通知
    public void OnNpcEnableStateChange(bool bEnableState)
    {
        foreach(var element in this.myElementList)
        {
            element.OnNpcEnableStateChange(bEnableState);
        }
    }

    //--------------------------------------------------------------------------------------------------
    // 配件行为相关代码

    public bool DoSkill()
    {
        foreach(RobotPartElement element in this.myElementList)
        {
            element.DoSkill();
        }
        return true;
    }


    /// <summary>
    /// 移动配件移动到某坐标
    /// </summary>
    /// <param name="pPosition"></param>
    /// <param name="fReachCall"></param>
    public bool MovePart_MoveToPos(Vector3 pPosition)
    {
        if (this.partType != RobotPartType.Move)
        {
            return false;
        }

        bool bCanReach = false;
        foreach(RobotPartElement pElement in this.myElementList)
        {
            if (pElement.MovePartElement_MoveToPos(pPosition))
            {
                bCanReach = true;
            }
        }
        return bCanReach;
    }

    /// <summary>
    /// 移动配件向指定角度移动
    /// </summary>
    public void MovePart_MoveToAngle(int nAngle)
    {
        if (this.partType != RobotPartType.Move)
        {
            return;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            pElement.MovePartElement_MoveToAngle(nAngle);
        }
    }

    /// <summary>
    /// 移动配件停止移动
    /// </summary>
    public void MovePart_MoveStop()
    {
        if (this.partType != RobotPartType.Move)
        {
            return;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            pElement.MovePartElement_MoveStop();
        }
    }

    /// <summary>
    /// 移动配件移动是否到达
    /// </summary>
    public bool MovePart_IsMoveReach()
    {
        if (this.partType != RobotPartType.Move)
        {
            return false;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            return pElement.MovePartElement_IsMoveReach();
        }

        return false;
    }

    /// <summary>
    /// 移动配件转向到某坐标
    /// </summary>
    /// <param name="pPosition"></param>
    public void MovePart_TurnToPos(Vector3 pPosition)
    {
        if (this.partType != RobotPartType.Move)
        {
            return;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            pElement.MovePart_TurnToPos(pPosition);
        }
    }

    /// <summary>
    /// 移动配件转向停止
    /// </summary>
    /// <param name="pPosition"></param>
    public void MovePart_TurnStop()
    {
        if (this.partType != RobotPartType.Move)
        {
            return;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            pElement.MovePart_TurnStop();
        }
    }

    /// <summary>
    /// 移动配件转向是否到达
    /// </summary>
    public bool MovePart_IsTurnReach()
    {
        if (this.partType != RobotPartType.Move)
        {
            return false;
        }

        foreach(RobotPartElement pElement in this.myElementList)
        {
            return pElement.MovePartElement_IsTurnReach();
        }
        
        return false;
    }


    //--------------------------------------------------------------------------------------------------
    // 读表操作

    /// <summary>
    /// 获取配件类型
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static RobotPartType GetRobotPartType(int nCfgId)
    {
        return (RobotPartType)ConfigManager.GetValue<int>("Part_C", nCfgId, "partType");
    }

    /// <summary>
    /// 获取配件模型Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartModelId(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "normalModel");
    }

    /// <summary>
    /// 获取主体配件的槽位列表
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetRobotPartBodyHoleList(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Part_C", nCfgId, "bodySlot"));
    }

    /// <summary>
    /// 获取配件随机属性集合
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetRobotPartRandomAttribute(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Part_C", nCfgId, "randomAttribute"));
    }

    /// <summary>
    /// 获取配件损伤特效1的Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartBrokenEffect1(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "brokenEffects1");
    }

    /// <summary>
    /// 获取配件损伤特效1的特效挂点表
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetRobotPartBrokenEffect1PointList(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Part_C", nCfgId, "brokenEffects1Point"));
    }

    /// <summary>
    /// 获取播放配件损伤特效1的耐久百分比
    /// </summary>
    /// <returns></returns>
    public static int GetRobotPartDurabilityForBrokenEffect1()
    {
        return ConfigManager.GetValue<int>("GameParam_C", "BrokenEffects1Durability");
    }

    /// <summary>
    /// 获取配件损伤特效2的Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartBrokenEffect2(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "brokenEffects2");
    }

    /// <summary>
    /// 获取配件损伤特效2的特效挂点表
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetRobotPartBrokenEffect2PointList(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Part_C", nCfgId, "brokenEffects2Point"));
    }

    /// <summary>
    /// 获取播放配件损伤特效2的耐久百分比
    /// </summary>
    /// <returns></returns>
    public static int GetRobotPartDurabilityForBrokenEffect2()
    {
        return ConfigManager.GetValue<int>("GameParam_C", "BrokenEffects2Durability");
    }

    /// <summary>
    /// 获取配件损毁特效Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartDeadEffect(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "deadEffect");
    }

    /// <summary>
    /// 获取配件损毁特效的特效挂点表
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetRobotPartDeadEffectPointList(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Part_C", nCfgId, "deadEffectPoint"));
    }

    /// <summary>
    /// 获取配件攻击特效Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartHitEffect(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "hitEffect");
    }

    /// <summary>
    /// 获取配件运行特效Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetRobotPartRunEffect(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "runEffect");
    }

    /// <summary>
    /// 获取配件技能Id
    /// </summary>
    /// <returns></returns>
    public static int GetRobotPartSkillId(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Part_C", nCfgId, "skillID");
    }

    /// <summary>
    /// 获取配件切换到损伤模型的耐久
    /// </summary>
    /// <returns></returns>
    public static int GetRobotPartSwitchDamageModelDurability()
    {
        return ConfigManager.GetValue<int>("GameParam_C", "BrokeSoltSwitchDurability");
    }
}




