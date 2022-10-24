using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviacController : BaseBehaviacController
{
    // 归属的Npc
    public BaseNpc myNpc{get; private set;}

    public string aiName {get; private set;} = "";

    public NpcBehaviacController(GameObject pGameObj, BaseNpc pNpc) : base(pGameObj)
    {
        this.myNpc = pNpc;
        this.aiName = BaseNpc.GetNpcAiName(this.myNpc.cfgId);
        ((NpcBehaviac)this.myBehaviac).NpcInstId = myNpc.InstId;
        ((NpcBehaviac)this.myBehaviac).ViewRange = myNpc.myViewRange;
    }

    /// <summary>
    /// 初始化行为树脚本组件
    /// </summary>
    /// <param name="pGameObj"></param>
    protected override void InitMyBehaviac(GameObject pGameObj)
    {
        this.myBehaviac = pGameObj.AddComponent<NpcBehaviac>();
    }

    /// <summary>
    /// 重设Ai行为树名称
    /// </summary>
    /// <param name="sAiName"></param>
    public void ResetAiName(string sAiName)
    {
        this.aiName = sAiName;
    }

    /// <summary>
    /// 刷新行为树
    /// </summary>
    /// <param name="sBehaviacTreeName"></param>
    public void UpdateBehaviacTree()
    {
        if (this.myNpc.isUiNpc)
        {
            return;
        }

        if (this.aiName != null && this.aiName != "")
        {
            base.UpdateBehaviacTree("Npc/Ai/", this.aiName);
        }
    }

    /// <summary>
    /// 派发通用游戏事件
    /// </summary>
    /// <param name="eEvent">Ai事件枚举</param>
    /// <param name="nDetailType">细分类型</param>
	public void DispatchGameEvent(BehaviacGameEvent eEvent, params object[] pParam)
	{
        if (this.myNpc.isUiNpc)
        {
            return;
        }

        this.myNpc.myBuffController.BuffLoop(delegate(Buff pBuff)
            {
                this.DispatchGameEventToBuff(pBuff, eEvent, pParam);
            }
        );

        this.DispatchGameEventToAi(eEvent, pParam);
	}

    /// <summary>
    /// 派发游戏事件到Ai
    /// </summary>
    /// <param name="pBuff"></param>
    /// <param name="eEvent"></param>
    /// <param name="pParam"></param>
	public void DispatchGameEventToAi(BehaviacGameEvent eEvent, params object[] pParam)
	{
        if (this.myNpc.isUiNpc)
        {
            return;
        }
        
        if (this.aiName != null && this.aiName != "")
        {
            base.DispatchGameEvent("Npc/Ai/", this.aiName, eEvent, pParam);
        }
	}

    /// <summary>
    /// 派发游戏事件到Buff
    /// </summary>
    /// <param name="pBuff"></param>
    /// <param name="eEvent"></param>
    /// <param name="pParam"></param>
	public void DispatchGameEventToBuff(Buff pBuff, BehaviacGameEvent eEvent, params object[] pParam)
	{
        if (this.myNpc.isUiNpc)
        {
            return;
        }
        
        if (pBuff.behaviacName != null && pBuff.behaviacName != "")
        {
            NpcBehaviac pNpcBehaviac = (NpcBehaviac)this.myBehaviac;
            pNpcBehaviac.BuffCfgId = pBuff.cfgId;
            base.DispatchGameEvent("Npc/Buff/", pBuff.behaviacName, eEvent, pParam);
            pNpcBehaviac.BuffCfgId = -1;
        }
	}

    /// <summary>
    /// 获取依附物坐标X
    /// </summary>
    /// <returns></returns>
	public float GetPosX()
	{
		return this.myGameObj.transform.position.x;
	}

    /// <summary>
    /// 获取依附物坐标Y
    /// </summary>
    /// <returns></returns>
	public float GetPosY()
	{
		return this.myGameObj.transform.position.y;
	}

    /// <summary>
    /// 获取依附物坐标Z
    /// </summary>
    /// <returns></returns>
	public float GetPosZ()
	{
		return this.myGameObj.transform.position.z;
	}

    /// <summary>
    /// 获取依附物转向X
    /// </summary>
    /// <returns></returns>
	public float GetRotX()
	{
		return this.myGameObj.transform.rotation.eulerAngles.x;
	}

    /// <summary>
    /// 获取依附物转向Y
    /// </summary>
    /// <returns></returns>
	public float GetRotY()
	{
		return this.myGameObj.transform.rotation.eulerAngles.y;
	}

    /// <summary>
    /// 获取依附物转向Z
    /// </summary>
    /// <returns></returns>
	public float GetRotZ()
	{
		return this.myGameObj.transform.rotation.eulerAngles.z;
	}

    /// <summary>
    /// 移动到指定坐标
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns>是否成功</returns>
    public bool MoveToPos(Vector3 pPos)
    {
        return this.myNpc.MoveToPos(pPos);
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns>是否成功</returns>
    public bool MoveToPlace(float nAngle, float nDistance)
    {
        return this.myNpc.MoveToPlace(nAngle, nDistance);
    }

    /// <summary>
    /// 移动到指定点
    /// </summary>
    /// <param name="sPointName"></param>
    public bool MoveToPoint(string sPointName)
    {
        return this.myNpc.MoveToPoint(sPointName);
    }

    /// <summary>
    /// 移动到指定Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
    public bool MoveToNpc(int nNpcInstId)
    {
        return this.myNpc.MoveToNpc(nNpcInstId);
    }

    /// <summary>
    /// 移动到玩家
    /// </summary>
    /// <returns></returns>
    public bool MoveToPlayer()
    {
        return this.myNpc.MoveToPlayer();
    }

    /// <summary>
    /// 移动到出生点
    /// </summary>
    public bool MoveToSpawnPositon()
    {
        return this.myNpc.MoveToSpawnPositon();
    }

    /// <summary>
    /// 移动是否到达
    /// </summary>
    /// <returns></returns>
    public bool IsMoveReach()
    {
        return this.myNpc.IsMoveReach();
    }

    /// <summary>
    /// 向指定角度移动
    /// </summary>
    public bool MoveToAngle(int nAngle)
    {
        return this.myNpc.MoveToAngle(nAngle);
    }

    /// <summary>
    /// 移动停止
    /// </summary>
    public void MoveStop()
    {
        this.myNpc.MoveStop();
    }

    /// <summary>
    /// 转向指定坐标
    /// </summary>
    /// <param name="pPos"></param>
    public bool TurnToPos(Vector3 pPos)
    {
        return this.myNpc.TurnToPos(pPos);
    }

    /// <summary>
    /// 转向指定角度
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool TurnToPlace(float nAngle)
    {
        return this.myNpc.TurnToPlace(nAngle);
    }

    /// <summary>
    /// 转向某点
    /// </summary>
    /// <param name="sPointName"></param>
    /// <returns></returns>
    public bool TurnToPoint(string sPointName)
    {
        return this.myNpc.TurnToPoint(sPointName);
    }

    /// <summary>
    /// 转向Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
    public bool TurnToNpc(int nNpcInstId)
    {
        return this.myNpc.TurnToNpc(nNpcInstId);
    }

    /// <summary>
    /// 转向玩家
    /// </summary>
    /// <returns></returns>
    public bool TurnToPlayer()
    {
        return this.myNpc.TurnToPlayer();
    }

    /// <summary>
    /// 转向出生点
    /// </summary>
    /// <returns></returns>
    public bool TurnToSpawnPositon()
    {
        return this.myNpc.TurnToSpawnPositon();
    }

    /// <summary>
    /// 转向停止
    /// </summary>
    public void TurnStop()
    {
        this.myNpc.TurnStop();
    }

    /// <summary>
    /// 转向是否到达
    /// </summary>
    /// <returns></returns>
    public bool IsTurnReach()
    {
        return this.myNpc.IsTurnReach();
    }

    /// <summary>
    /// 目标Npc是否在指定范围内
    /// </summary>
    /// <returns></returns>
    public bool IsInRangeOfNpc(int nNpcInstId, int nRange)
    {
        return this.myNpc.IsInRangeOfNpc(nNpcInstId, nRange);
    }

    /// <summary>
    /// 玩家Npc是否在指定范围内
    /// </summary>
    /// <returns></returns>
    public bool IsInRangeOfPlayerNpc(int nRange)
    {
        return this.myNpc.IsInRangeOfPlayerNpc(nRange);
    }

    /// <summary>
    /// 设置Npc暂停状态
    /// </summary>
    /// <param name="bEnable"></param>
    public void SetPauseState(bool bPauseState)
    {
        this.myNpc.SetPauseState(bPauseState);
    }

    /// <summary>
    /// 是否朝向指定Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nVague"></param>
    /// <returns></returns>
	public bool IsLookAtNpc(int nNpcInstId, int nVague)
	{
        return this.myNpc.IsLookAtNpc(nNpcInstId, nVague);
	}

    /// <summary>
    /// 是否朝向玩家Npc
    /// </summary>
    /// <param name="nVague"></param>
    /// <returns></returns>
	public bool IsLookAtPlayer(int nVague)
	{
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }

        return this.myNpc.IsLookAtNpc(pPlayerNpc.InstId, nVague);
	}

    /// <summary>
    /// 向目标Npc移动一定的距离
    /// </summary>
    /// <returns></returns>
    public bool MoveToNpcByDistance(int nNpcInstId, int nDistance)
    {
        return this.myNpc.MoveToNpcByDistance(nNpcInstId, nDistance);
    }

    /// <summary>
    /// 向玩家Npc移动一定距离
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool MoveToPlayerByDistance(int nDistance)
    {
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }

        return this.myNpc.MoveToNpcByDistance(pPlayerNpc.InstId, nDistance);
    }

    /// <summary>
    /// 移动到与目标Npc连线后某夹角某距离的坐标
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool MoveToRelativeNpcDirect(int nNpcInstId, int nAngle, int nDistance)
    {
        return this.myNpc.MoveToRelativeNpcDirect(nNpcInstId, nAngle, nDistance);
    }

    /// <summary>
    /// 移动到与玩家Npc连线后某夹角某距离的坐标
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool MoveToRelativePlayerDirect(int nAngle, int nDistance)
    {
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }

        return this.myNpc.MoveToRelativeNpcDirect(pPlayerNpc.InstId, nAngle, nDistance);
    }

    /// <summary>
    /// 获取自身Npc实例Id
    /// </summary>
    /// <returns></returns>
    public int GetSelfNpcInstId()
    {
        return this.myNpc.InstId;
    }

    /// <summary>
    /// 获取是否有某配件并未损毁
    /// </summary>
    /// <param name="nPartCfgId"></param>
    /// <returns></returns>
    public bool IsRobotPartExistAndLive(int nPartCfgId)
    {
        foreach(var kvPair in this.myNpc.GetRobotPartHoleContainer())
        {
            var nPartInstId = kvPair.Value.partInstId;
            if (nPartInstId != 0)
            {
                RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
                if (pPart != null)
                {
                    if (pPart.cfgId == nPartCfgId)
                    {
                        foreach(var pElement in pPart.myElementList)
                        {
                            if (!pElement.isDead)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 是否血量低于指定百分比
    /// </summary>
    /// <param name="nPercent"></param>
    /// <returns></returns>
    public bool IsLastPercentHp(int nPercent)
    {
        return base.IsNpcLastPercentHp(this.myNpc.InstId, nPercent);
    }

    /// <summary>
    /// 是否能到达指定位置
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool IsCanMoveToPlace(int nAngle, int nDistance)
    {
        return this.myNpc.IsCanMoveToPos(this.myNpc.GetPosByAngle(nAngle, nDistance));
    }

    /// <summary>
    /// Npc是否有指定配件
    /// </summary>
    /// <param name="nPartCfgId"></param>
    /// <returns></returns>
	public bool NpcIsHaveRobotPart(int nPartCfgId)
    {
        foreach(var kvPair in this.myNpc.GetRobotPartHoleContainer())
        {
            var nPartInstId = kvPair.Value.partInstId;
            if (nPartInstId != 0)
            {
                RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
                if (pPart != null)
                {
                    if (pPart.cfgId == nPartCfgId)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Npc指定配件是否死亡
    /// </summary>
    /// <param name="nPartCfgId"></param>
    /// <returns></returns>
	public bool NpcPartIsDead(int nPartCfgId)
	{
        foreach(var kvPair in this.myNpc.GetRobotPartHoleContainer())
        {
            var nPartInstId = kvPair.Value.partInstId;
            if (nPartInstId != 0)
            {
                RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
                if (pPart != null)
                {
                    if (pPart.cfgId == nPartCfgId)
                    {
                        foreach(var pElement in pPart.myElementList)
                        {
                            if (!pElement.isDead)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
	}

    // -------------------------------------------------------------------------
    // 属性相关的接口代码

    /// <summary>
    /// Npc属性值获取
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="nValType"></param>
    /// <returns></returns>
    public int NpcAttrGet(BehaviacNpcAttrType nType, BehaviacNpcAttrValGetType nValType)
    {
        if (nValType == BehaviacNpcAttrValGetType.All)
        {
            return this.myNpc.attrEntity.GetValue((AttributeType)nType);
        }
        else if (nValType == BehaviacNpcAttrValGetType.Base)
        {
            return this.myNpc.attrEntity.GetBaseValue((AttributeType)nType);
        }
        else if (nValType == BehaviacNpcAttrValGetType.Percent)
        {
            return this.myNpc.attrEntity.GetPercentValue((AttributeType)nType);
        }
        else
        {
            Debug.Log("NpcBehaviacController.NpcAttrGet->Npc Attr Get Failed!" + "#nType = " + nType.ToString() + "#nValType = " + nValType.ToString());
            return 0;
        }
    }

    /// <summary>
    /// Npc属性值设置
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="nValType"></param>
    /// <param name="nVal"></param>
    public void NpcAttrSet(BehaviacNpcAttrType nType, BehaviacNpcAttrValSetType nValType, int nVal)
    {
        if (nValType == BehaviacNpcAttrValSetType.Base)
        {
            this.myNpc.attrEntity.SetBaseValue((AttributeType)nType, nVal);
        }
        else if (nValType == BehaviacNpcAttrValSetType.Percent)
        {
            this.myNpc.attrEntity.SetPercentValue((AttributeType)nType, nVal);
        }
        else
        {
            Debug.Log("NpcBehaviacController.NpcAttrSet->Npc Attr Set Failed!" + "#nType = " + nType.ToString() + "#nValType = " + nValType.ToString() + "#nVal = " + nVal);
        }
    }

    /// <summary>
    /// Npc属性值修改
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="nValType"></param>
    /// <param name="nVal"></param>
    public void NpcAttrModfiy(BehaviacNpcAttrType nType, BehaviacNpcAttrValSetType nValType, int nVal)
    {
        if (nValType == BehaviacNpcAttrValSetType.Base)
        {
            this.myNpc.attrEntity.ModfiyBaseValue((AttributeType)nType, nVal);
        }
        else if (nValType == BehaviacNpcAttrValSetType.Percent)
        {
            this.myNpc.attrEntity.ModfiyPercentValue((AttributeType)nType, nVal);
        }
        else
        {
            Debug.Log("NpcBehaviacController.NpcAttrModfiy->Npc Attr Modfiy Failed!" + "#nType = " + nType.ToString() + "#nValType = " + nValType.ToString() + "#nVal = " + nVal);
        }
    }

    // -------------------------------------------------------------------------
    // Buff相关的接口代码

    private bool _isBuffCanAdd = true;

    /// <summary>
    /// 设置行为树在Buff_OnBuffAdd事件中Buff是否可被添加的值
    /// </summary>
    /// <param name="bAdd"></param>
    /// <returns></returns>
    public void ReturnBuffIsCanAdd(bool bAdd)
    {
        this._isBuffCanAdd = bAdd;
    }

    /// <summary>
    /// 获取行为树在Buff_OnBuffAdd事件中设置的Buff是否可被添加的值
    /// </summary>
    /// <returns></returns>
    public bool IsBuffCanAdd()
    {
        return this._isBuffCanAdd;
    }

    /// <summary>
    /// 重置行为树在Buff_OnBuffAdd事件中设置的Buff是否可被添加的值
    /// </summary>
    public void ResetIsBuffCanAdd()
    {
        this._isBuffCanAdd = true;
    }

    /// <summary>
    /// 获取Buff层数
    /// </summary>
    public int GetBuffLayer(int nBuffCfgId)
    {
        return this.myNpc.myBuffController.GetBuff(nBuffCfgId).GetLayer();
    }

    /// <summary>
    /// 获取敌对Npc
    /// </summary>
    /// <returns></returns>
    public int GetEnemyNpcInView()
    {
        return this.myNpc.GetEnemyNpcInView();
    }

    // -------------------------------------------------------------------------
    // 技能相关的接口代码

    /// <summary>
    /// Npc释放技能
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool DoSkill(int skillID)
    {
        return this.myNpc.mySkillController.SkillDo(skillID);
    }

	public bool SkillIsExist(int nSkillCfgId)
	{
		return this.myNpc.mySkillController.IsHaveSkill(nSkillCfgId);
	}

	public bool SkillIsCooldown(int nSkillCfgId)
	{   
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsCooldown();
	}

	public bool SkillIsViewIn(int nSkillCfgId, int nNpcInstId)
	{
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsViewIn(MapManager.Instance.baseMap.GetNpc(nNpcInstId).myModel.transform.position);
	}

	public bool SkillIsMaxRangeIn(int nSkillCfgId, int nNpcInstId)
	{
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsMaxRangeIn(MapManager.Instance.baseMap.GetNpc(nNpcInstId).myModel.transform.position);
	}

	public bool SkillIsMinRangeOut(int nSkillCfgId, int nNpcInstId)
	{
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsMinRangeOut(MapManager.Instance.baseMap.GetNpc(nNpcInstId).myModel.transform.position);
	}

	public bool SkillIsDo(int nSkillCfgId)
	{
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).isDo;
    }

	public bool SkillIsDoStage(int nSkillCfgId, BehaviacSkillDoStageType eSkillDoStage)
	{
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsDoStage(eSkillDoStage);
    }

	public int SkillGetMaxRange(int nSkillCfgId)
    {
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).maxRange;
    }
	public int SkillGetMinRange(int nSkillCfgId)
    {
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).minRange;
    }

	public bool SkillIsDoEnd(int nSkillCfgId)
    {
        return this.myNpc.mySkillController.GetSkill(nSkillCfgId).IsDoEnd();
    }

	public void SkillSetManualDo(int nSkillCfgId)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).SetManualDo();
    }

	public void SkillManualDo(int nSkillCfgId)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).ManualDo();
    }

	public void SkillSetCooldown(int nSkillCfgId, bool bCooldown)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).SetCooldown(bCooldown);
    }

    public void SkillSetBeforeTime(int nSkillCfgId, int nTime)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).SetBeforeTime(nTime);
    }

    public void SkillResetBeforeTime(int nSkillCfgId)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).ResetBeforeTime();
    }

    public void SkillSetDurationTime(int nSkillCfgId, int nTime)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).SetDurationTime(nTime);
    }

    public void SkillResetDurationTime(int nSkillCfgId)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).ResetDurationTime();
    }

    public void SkillSetAfterTime(int nSkillCfgId, int nTime)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).SetAfterTime(nTime);
    }

    public void SkillResetAfterTime(int nSkillCfgId)
    {
        this.myNpc.mySkillController.GetSkill(nSkillCfgId).ResetAfterTime();
    }
}
