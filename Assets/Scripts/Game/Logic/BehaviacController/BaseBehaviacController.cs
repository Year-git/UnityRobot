using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBehaviacController
{
    // Ai行为树
    public BaseBehaviac myBehaviac{get; protected set;}
    // 行为树依附物体
    public GameObject myGameObj{get; protected set;}

    /// <summary>
    /// 缓存本次调用的行为树路径
    /// </summary>
    /// <value></value>
    public string curCallBehaviacTreePath{get; private set;}

    /// <summary>
    /// 缓存本次调用的行为树名称
    /// </summary>
    /// <value></value>
    public string curCallBehaviacTreeName{get; private set;}

    /// <summary>
    /// 缓存本次调用的行为树调用类型
    /// </summary>
    private BehaviacCallType curBehaviacCallType;

    /// <summary>
    /// 缓存本次事件调用的事件类型
    /// </summary>
    private BehaviacGameEvent curBehaviacGameEvent;

    /// <summary>
    /// 缓存本次事件调用的事件参数
    /// </summary>
    private object[] curBehaviacGameEventParam;

    protected BaseBehaviacController(GameObject pGameObj)
    {
        if (pGameObj == null)
        {
            return;
        }

        this.myGameObj = pGameObj;

        this.InitMyBehaviac(pGameObj);

        // 关联Ai控制器与Ai行为树
        this.myBehaviac.myControlloer = this;
    }

    /// <summary>
    /// 初始化行为树脚本组件
    /// </summary>
    /// <param name="pGameObj"></param>
    protected virtual void InitMyBehaviac(GameObject pGameObj)
    {
        this.myBehaviac = pGameObj.AddComponent<BaseBehaviac>();
    }

    /// <summary>
    /// 清理行为树
    /// </summary>
    public virtual void Clear()
    {
        this.myBehaviac = null;
    }

    /// <summary>
    /// 调用行为树
    /// </summary>
    protected virtual void CallBehaviacTree(string sTreePath, string sTreeName)
    {
        if (this.myBehaviac == null || sTreeName == null || sTreeName == "")
        {
            return;
        }

        if (behaviac.Config.IsSocketing == true)
        {
            behaviac.Workspace.Instance.DebugUpdate();
        }

        this.curCallBehaviacTreePath = sTreePath;
        this.curCallBehaviacTreeName = sTreeName;
        // UnityEngine.Debug.Log("Log->BaseBehaviacController.CallBehaviacTree->" + "|sTreePath = " + sTreePath + "|sTreeName = " + sTreeName);

        string sTree = sTreePath + sTreeName;

        // 加载行为树
        bool bRet = this.myBehaviac.btload(sTree);
        if (bRet)
        {
            this.myBehaviac.btsetcurrent(sTree);
            behaviac.EBTStatus eStatus = this.myBehaviac.btexec();
            this.curCallBehaviacTreePath = "";
            this.curCallBehaviacTreeName = "";

            if (eStatus == behaviac.EBTStatus.BT_RUNNING)
            {
                UnityEngine.Debug.LogError("Error->BaseBehaviacController.CallBehaviacTree->BehaviacTree Can Not Use BT_RUNNING As Return Value!");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Error->BaseBehaviacController.CallBehaviacTree->BehaviacTree Load Failed!");
        }
    }

    /// <summary>
    /// 刷新行为树
    /// </summary>
    /// <param name="sTreeName"></param>
    protected virtual void UpdateBehaviacTree(string sTreePath, string sTreeName)
    {
        // 检查刷新时间
        int nCurTime;
        if (FrameSynchronManager.Instance.isStart)
        {
            nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;
        }
        else
        {
            nCurTime = (int)(Framework.GTime.RealtimeSinceStartup * 1000);
        }

        if (this.myBehaviac == null || sTreeName == null || sTreeName == "")
        {
            return;
        }

        // 设置本次调用缓存信息
        this.curBehaviacCallType = BehaviacCallType.Update;

        // Debug.Log("BaseBehaviacController.UpdateBehaviacTree->" + "#BehaviacCallType = " + BehaviacCallType.Update.ToString());

        // 调用行为树
        this.CallBehaviacTree(sTreePath, sTreeName);
        
        // 清理本次调用缓存信息
        this.curBehaviacCallType = BehaviacCallType.None;
    }

    /// <summary>
    /// 派发游戏事件
    /// </summary>
    /// <param name="eEvent">Ai事件枚举</param>
    /// <param name="nDetailType">细分类型</param>
	protected virtual void DispatchGameEvent(string sTreePath, string sTreeName, BehaviacGameEvent eEvent, params object[] pParam)
	{
        if (this.myBehaviac == null || sTreeName == null || sTreeName == "")
        {
            return;
        }
        
        // 设置本次调用缓存信息
        this.curBehaviacCallType = BehaviacCallType.Event;
        this.curBehaviacGameEvent = eEvent;
        this.curBehaviacGameEventParam = pParam;

        // Debug.Log("BaseBehaviacController.DispatchGameEvent->" + "#BehaviacCallType = " + BehaviacCallType.Event.ToString() + "#BehaviacGameEvent = " + eEvent.ToString());

        // 调用行为树
        this.CallBehaviacTree(sTreePath, sTreeName);

        // 清理本次调用缓存信息
        this.curBehaviacCallType = BehaviacCallType.None;
        this.curBehaviacGameEvent = BehaviacGameEvent.None;
        this.curBehaviacGameEventParam = null;
	}

    //-----------------------------------------------------------------------------------------------------------
    // 为行为树提供接口的相关代码

    /// <summary>
    /// 获取当前要调用的行为树名称
    /// </summary>
    /// <returns></returns>
    public string GetCurCallTreeName()
    {
        return this.curCallBehaviacTreePath + this.curCallBehaviacTreeName;
    }

    /// <summary>
    /// 是否是指定的调用类型
    /// </summary>
    /// <param name="eType"></param>
    /// <returns></returns>
    public bool IsCurCallType(BehaviacCallType eType)
    {
        return this.curBehaviacCallType == eType;
    }

    /// <summary>
    /// 是否是指定游戏事件
    /// </summary>
    /// <param name="eEvent"></param>
    /// <param name="nDetailType"></param>
    /// <returns></returns>
	public bool IsTargetGameEvent(BehaviacGameEvent eEvent)
	{
        return this.curBehaviacGameEvent == eEvent;
	}

    /// <summary>
    /// 获取指定索引的事件参数[整数]
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public int GetEventParamInt(int nIdx)
    {
        if (this.curBehaviacGameEventParam.Length > nIdx)
        {
            try
            {
                return (int)this.curBehaviacGameEventParam[nIdx];
            }
            catch(System.InvalidCastException)
            {
                Debug.LogError("Error->BaseBehaviacController.GetEventParamInt->DataType Convert Error!");
                return 0;
            }
        }
        else
        {
            Debug.LogError("Error->BaseBehaviacController.GetEventParamInt->Target nIdx[" + nIdx +  "] Param Not Exist In BehaviacGameEventParam!");
            return -1;
        }
    }

    /// <summary>
    /// 获取指定索引的事件参数[字符串]
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public string GetEventParamString(int nIdx)
    {
        if (this.curBehaviacGameEventParam.Length > nIdx)
        {
            try
            {
                return (string)this.curBehaviacGameEventParam[nIdx];
            }
            catch(System.InvalidCastException)
            {
                Debug.LogError("Error->BaseBehaviacController.GetEventParamString->DataType Convert Error!");
                return "";
            }
        }
        else
        {
            Debug.LogError("Error->BaseBehaviacController.GetEventParamString->Target nIdx[" + nIdx +  "] Param Not Exist In BehaviacGameEventParam!");
            return "";
        }
    }

    /// <summary>
    /// 获取指定索引的事件参数[布尔]
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool GetEventParamBool(int nIdx)
    {
        if (this.curBehaviacGameEventParam.Length > nIdx)
        {
            try
            {
                return (bool)this.curBehaviacGameEventParam[nIdx];
            }
            catch(System.InvalidCastException)
            {
                Debug.LogError("Error->BaseBehaviacController.GetEventParamBool->DataType Convert Error!");
                return false;
            }
        }
        else
        {
            Debug.LogError("Error->BaseBehaviacController.GetEventParamBool->Target nIdx[" + nIdx +  "] Param Not Exist In BehaviacGameEventParam!");
            return false;
        }
    }



    /// <summary>
    /// 是否是怪物Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
	public bool IsMonsterNpc(int nNpcInstId)
	{
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }
		return pNpc.type == NpcType.MonsterNpc;
	}

    /// <summary>
    /// 是否是建筑Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
	public bool IsBuildNpc(int nNpcInstId)
	{
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }
		return pNpc.type == NpcType.BuildNpc;
	}

    /// <summary>
    /// 是否是玩家Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
	public bool IsPlayerNpc(int nNpcInstId)
	{
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }
		return pNpc.type == NpcType.PlayerNpc;
	}

    /// <summary>
    /// Npc百分比治疗输入
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nTreatmentPercent"></param>
	public void NpcTreatmentPercentInput(int nNpcInstId, int nTreatmentPercent)
	{
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return;
        }
        pNpc.NpcTreatmentPercentInput(nTreatmentPercent);
	}

    /// <summary>
    /// Npc固定值治疗输入
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nTreatmentVal"></param>
	public void NpcTreatmentInput(int nNpcInstId, int nTreatmentVal)
	{
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return;
        }
        pNpc.NpcTreatmentInput(nTreatmentVal);
	}

    /// <summary>
    /// 添加Npc到地图的指定坐标
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="nPosX"></param>
    /// <param name="nPosY"></param>
    /// <param name="nPosZ"></param>
    /// <param name="nRotX"></param>
    /// <param name="nRotY"></param>
    /// <param name="nRotZ"></param>
    /// <returns></returns>
    public int AddNpcToMapByPos(int nCfgId, float nPosX, float nPosY, float nPosZ, float nRotX, float nRotY, float nRotZ)
    {
        if (!(nCfgId > 0))
        {
            return 0;
        }

        if (MapManager.Instance.baseMap == null)
        {
            return 0;
        }

        BaseNpc pNewNpc = MapManager.Instance.baseMap.CreatNpc(
            nCfgId,
            new Vector3(nPosX, nPosY, nPosZ),
            new Vector3(nRotX, nRotY, nRotZ),
            null,
            null
        );

        return pNewNpc.InstId;
    }

    /// <summary>
    /// 添加Npc到地图的指定点
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="sPointName"></param>
    /// <returns></returns>
    public int AddNpcToMapByPoint(int nCfgId, string sPointName)
    {
        if (!(nCfgId > 0))
        {
            return 0;
        }

        if (MapManager.Instance.baseMap == null)
        {
            return 0;
        }

        if (sPointName == "")
        {
            return 0;
        }

        BaseMap pMap = MapManager.Instance.baseMap;

        List<int> pPointInstList = pMap.levelTrapManager.GetLevelTrapList(sPointName);
        if (pPointInstList.Count == 0)
        {
            return 0;
        }

        LevelPointScript pPoint = pMap.levelPointManager.GetLevelPoint(pPointInstList[0]);
        if  (pPoint == null)
        {
            return 0;
        }

        BaseNpc pNewNpc = pMap.CreatNpc(
            nCfgId,
            pPoint.transform.position,
            pPoint.transform.rotation.eulerAngles,
            null,
            null
        );

        return pNewNpc.InstId;
    }

    /// <summary>
    /// 获取玩家Npc是否已死亡
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerNpcDead()
    {
        if (MyPlayer.player == null)
        {
            return false;
        }
        return MyPlayer.player.isDead;
    }

    /// <summary>
    /// 是否是Npc血量低于指定百分比
    /// </summary>
    /// <returns></returns>
    public bool IsNpcLastPercentHp(int nNpcInstId, int nPercentVal)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        if (nPercentVal > ((float)pNpc.lastHp / (float)pNpc.initHp * 100))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取玩家Npc实例Id
    /// </summary>
    /// <returns></returns>
    public int GetPlayerNpcInstId()
    {
        if (MyPlayer.player == null)
        {
            return 0;
        }

        return MyPlayer.player.InstId;
    }

    /// <summary>
    /// Npc添加及身特效
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nEffId"></param>
    /// <returns></returns>
    public int AddNpcBodyEffect(int nNpcInstId, int nEffId)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        return pMap.effectManager.BodyEffectAdd(nEffId, pNpc.myModel.transform, pNpc.myScale);
    }

    /// <summary>
    /// Npc添加Buff
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nBuffCfgId"></param>
    public void NpcBuffAdd(int nNpcInstId, int nBuffCfgId)
    {
        MapManager.Instance.baseMap.GetNpc(nNpcInstId).myBuffController.BuffAdd(nBuffCfgId);
    }

    /// <summary>
    /// Npc移除Buff
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nBuffCfgId"></param>
    public void NpcBuffRemove(int nNpcInstId, int nBuffCfgId)
    {
        MapManager.Instance.baseMap.GetNpc(nNpcInstId).myBuffController.BuffDel(nBuffCfgId);
    }
}
