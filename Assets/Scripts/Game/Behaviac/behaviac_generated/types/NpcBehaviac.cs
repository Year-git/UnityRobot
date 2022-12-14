// -------------------------------------------------------------------------------
// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
// -------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

///<<< BEGIN WRITING YOUR CODE FILE_INIT

///<<< END WRITING YOUR CODE

public class NpcBehaviac : BaseBehaviac
///<<< BEGIN WRITING YOUR CODE NpcBehaviac
///<<< END WRITING YOUR CODE
{
	public int BuffCfgId = -1;

	public int NpcInstId = -1;

	public int ViewRange = 0;

	public void ReturnBuffIsCanAdd(bool bAdd)
	{
///<<< BEGIN WRITING YOUR CODE ReturnBuffIsCanAdd
		this.GetMyController().ReturnBuffIsCanAdd(bAdd);
///<<< END WRITING YOUR CODE
	}

	public int GetBuffLayer()
	{
///<<< BEGIN WRITING YOUR CODE GetBuffLayer
		return this.GetMyController().GetBuffLayer(this.BuffCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool IsInRangeOfNpc(int nNpcInstId, int nRange)
	{
///<<< BEGIN WRITING YOUR CODE IsInRangeOfNpc
		return this.GetMyController().IsInRangeOfNpc(nNpcInstId, nRange);
///<<< END WRITING YOUR CODE
	}

	public int NpcAttrGet(BehaviacNpcAttrType nType, BehaviacNpcAttrValGetType nValType)
	{
///<<< BEGIN WRITING YOUR CODE NpcAttrGet
		return this.GetMyController().NpcAttrGet(nType, nValType);
///<<< END WRITING YOUR CODE
	}

	public void NpcAttrSet(BehaviacNpcAttrType nType, BehaviacNpcAttrValSetType nValType, int nVal)
	{
///<<< BEGIN WRITING YOUR CODE NpcAttrSet
		this.GetMyController().NpcAttrSet(nType, nValType, nVal);
///<<< END WRITING YOUR CODE
	}

	public void NpcAttrModfiy(BehaviacNpcAttrType nType, BehaviacNpcAttrValSetType nValType, int nVal)
	{
///<<< BEGIN WRITING YOUR CODE NpcAttrModfiy
		this.GetMyController().NpcAttrModfiy(nType, nValType, nVal);
///<<< END WRITING YOUR CODE
	}

	public int GetEnemyNpcInView()
	{
///<<< BEGIN WRITING YOUR CODE GetEnemyNpcInView
		return this.GetMyController().GetEnemyNpcInView();
///<<< END WRITING YOUR CODE
	}

	public bool IsRobotPartExistAndLive(int nPartCfgId)
	{
///<<< BEGIN WRITING YOUR CODE IsRobotPartExistAndLive
		return this.GetMyController().IsRobotPartExistAndLive(nPartCfgId);
///<<< END WRITING YOUR CODE
	}

	public float GetRotX()
	{
///<<< BEGIN WRITING YOUR CODE GetRotX
		return this.GetMyController().GetRotX();
///<<< END WRITING YOUR CODE
	}

	public float GetRotY()
	{
///<<< BEGIN WRITING YOUR CODE GetRotY
		return this.GetMyController().GetRotY();
///<<< END WRITING YOUR CODE
	}

	public float GetRotZ()
	{
///<<< BEGIN WRITING YOUR CODE GetRotZ
		return this.GetMyController().GetRotZ();
///<<< END WRITING YOUR CODE
	}

	public int GetSelfNpcInstId()
	{
///<<< BEGIN WRITING YOUR CODE GetSelfNpcInstId
		return this.GetMyController().GetSelfNpcInstId();
///<<< END WRITING YOUR CODE
	}

	public float GetPosX()
	{
///<<< BEGIN WRITING YOUR CODE GetPosX
		return this.GetMyController().GetPosX();
///<<< END WRITING YOUR CODE
	}

	public float GetPosY()
	{
///<<< BEGIN WRITING YOUR CODE GetPosY
		return this.GetMyController().GetPosY();
///<<< END WRITING YOUR CODE
	}

	public float GetPosZ()
	{
///<<< BEGIN WRITING YOUR CODE GetPosZ
		return this.GetMyController().GetPosZ();
///<<< END WRITING YOUR CODE
	}

	public bool NpcPartIsDead(int nPartCfgId)
	{
///<<< BEGIN WRITING YOUR CODE NpcPartIsDead
		return this.GetMyController().NpcPartIsDead(nPartCfgId);
///<<< END WRITING YOUR CODE
	}

	public void SetPauseState(bool bEnable)
	{
///<<< BEGIN WRITING YOUR CODE SetPauseState
		this.GetMyController().SetPauseState(bEnable);
///<<< END WRITING YOUR CODE
	}

	public bool IsLookAtPlayer(int nVague)
	{
///<<< BEGIN WRITING YOUR CODE IsLookAtPlayer
		return this.GetMyController().IsLookAtPlayer(nVague);
///<<< END WRITING YOUR CODE
	}

	public bool IsLookAtNpc(int nNpcInstId, int nVague)
	{
///<<< BEGIN WRITING YOUR CODE IsLookAtNpc
		return this.GetMyController().IsLookAtNpc(nNpcInstId, nVague);
///<<< END WRITING YOUR CODE
	}

	public bool IsCanMoveToPlace(int nAngle, int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE IsCanMoveToPlace
		return this.GetMyController().IsCanMoveToPlace(nAngle, nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool IsLastPercentHp(int nPercent)
	{
///<<< BEGIN WRITING YOUR CODE IsLastPercentHp
		return this.GetMyController().IsLastPercentHp(nPercent);
///<<< END WRITING YOUR CODE
	}

	public bool NpcIsHaveRobotPart(int nPartCfgId)
	{
///<<< BEGIN WRITING YOUR CODE NpcIsHaveRobotPart
		return this.GetMyController().NpcIsHaveRobotPart(nPartCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool IsInRangeOfPlayerNpc(int nRange)
	{
///<<< BEGIN WRITING YOUR CODE IsInRangeOfPlayerNpc
		return this.GetMyController().IsInRangeOfPlayerNpc(nRange);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToNpcByDistance(int nNpcInstId, int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE MoveToNpcByDistance
		return this.GetMyController().MoveToNpcByDistance(nNpcInstId, nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToPlayerByDistance(int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE MoveToPlayerByDistance
		return this.GetMyController().MoveToPlayerByDistance(nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToAngle(int nAngle)
	{
///<<< BEGIN WRITING YOUR CODE MoveToAngle
		return this.GetMyController().MoveToAngle(nAngle);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToSpawnPositon()
	{
///<<< BEGIN WRITING YOUR CODE MoveToSpawnPositon
		return this.GetMyController().MoveToSpawnPositon();
///<<< END WRITING YOUR CODE
	}

	public bool MoveToPlayer()
	{
///<<< BEGIN WRITING YOUR CODE MoveToPlayer
		return this.GetMyController().MoveToPlayer();
///<<< END WRITING YOUR CODE
	}

	public bool MoveToRelativeNpcDirect(int nNpcInstId, int nAngle, int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE MoveToRelativeNpcDirect
		return this.GetMyController().MoveToRelativeNpcDirect(nNpcInstId, nAngle, nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToRelativePlayerDirect(int nAngle, int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE MoveToRelativePlayerDirect
		return this.GetMyController().MoveToRelativePlayerDirect(nAngle, nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToNpc(int nNpcInstId)
	{
///<<< BEGIN WRITING YOUR CODE MoveToNpc
		return this.GetMyController().MoveToNpc(nNpcInstId);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToPoint(string sPointName)
	{
///<<< BEGIN WRITING YOUR CODE MoveToPoint
		return this.GetMyController().MoveToPoint(sPointName);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToPlace(int nAngle, int nDistance)
	{
///<<< BEGIN WRITING YOUR CODE MoveToPlace
		return this.GetMyController().MoveToPlace(nAngle, nDistance);
///<<< END WRITING YOUR CODE
	}

	public bool MoveToPos(float nPosX, float nPosY, float nPosZ)
	{
///<<< BEGIN WRITING YOUR CODE MoveToPos
		return this.GetMyController().MoveToPos(new UnityEngine.Vector3(nPosX, nPosY, nPosZ));
///<<< END WRITING YOUR CODE
	}

	public bool IsMoveReach()
	{
///<<< BEGIN WRITING YOUR CODE IsMoveReach
		return this.GetMyController().IsMoveReach();
///<<< END WRITING YOUR CODE
	}

	public void MoveStop()
	{
///<<< BEGIN WRITING YOUR CODE MoveStop
		this.GetMyController().MoveStop();
///<<< END WRITING YOUR CODE
	}

	public bool TurnToNpc(int nNpcInstId)
	{
///<<< BEGIN WRITING YOUR CODE TurnToNpc
		return this.GetMyController().TurnToNpc(nNpcInstId);
///<<< END WRITING YOUR CODE
	}

	public bool TurnToSpawnPositon()
	{
///<<< BEGIN WRITING YOUR CODE TurnToSpawnPositon
		return this.GetMyController().TurnToSpawnPositon();
///<<< END WRITING YOUR CODE
	}

	public bool TurnToPoint(string sPointName)
	{
///<<< BEGIN WRITING YOUR CODE TurnToPoint
		return this.GetMyController().TurnToPoint(sPointName);
///<<< END WRITING YOUR CODE
	}

	public bool IsTurnReach()
	{
///<<< BEGIN WRITING YOUR CODE IsTurnReach
		return this.GetMyController().IsTurnReach();
///<<< END WRITING YOUR CODE
	}

	public void TurnStop()
	{
///<<< BEGIN WRITING YOUR CODE TurnStop
		this.GetMyController().TurnStop();
///<<< END WRITING YOUR CODE
	}

	public bool TurnToPlayer()
	{
///<<< BEGIN WRITING YOUR CODE TurnToPlayer
		return this.GetMyController().TurnToPlayer();
///<<< END WRITING YOUR CODE
	}

	public bool TurnToPlace(int nAngle)
	{
///<<< BEGIN WRITING YOUR CODE TurnToPlace
		return this.GetMyController().TurnToPlace(nAngle);
///<<< END WRITING YOUR CODE
	}

	public bool TurnToPos(float nPosX, float nPosY, float nPosZ)
	{
///<<< BEGIN WRITING YOUR CODE TurnToPos
		return this.GetMyController().TurnToPos(new UnityEngine.Vector3(nPosX, nPosY, nPosZ));
///<<< END WRITING YOUR CODE
	}

	public int SkillGetMaxRange(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillGetMaxRange
		return this.GetMyController().SkillGetMaxRange(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public int SkillGetMinRange(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillGetMinRange
		return this.GetMyController().SkillGetMinRange(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsExist(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsExist
		return this.GetMyController().SkillIsExist(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsCooldown(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsCooldown
		return this.GetMyController().SkillIsCooldown(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsDo(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsDo
		return this.GetMyController().SkillIsDo(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsDoStage(int nSkillCfgId, BehaviacSkillDoStageType eSkillDoStage)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsDoStage
		return this.GetMyController().SkillIsDoStage(nSkillCfgId, eSkillDoStage);
///<<< END WRITING YOUR CODE
	}

	public void SkillSetDurationTime(int nSkillCfgId, int nTime)
	{
///<<< BEGIN WRITING YOUR CODE SkillSetDurationTime
		this.GetMyController().SkillSetDurationTime(nSkillCfgId, nTime);
///<<< END WRITING YOUR CODE
	}

	public void SkillSetAfterTime(int nSkillCfgId, int nTime)
	{
///<<< BEGIN WRITING YOUR CODE SkillSetAfterTime
		this.GetMyController().SkillSetAfterTime(nSkillCfgId, nTime);
///<<< END WRITING YOUR CODE
	}

	public void SkillSetCooldown(int nSkillCfgId, bool bCooldown)
	{
///<<< BEGIN WRITING YOUR CODE SkillSetCooldown
		this.GetMyController().SkillSetCooldown(nSkillCfgId, bCooldown);
///<<< END WRITING YOUR CODE
	}

	public void SkillSetBeforeTime(int nSkillCfgId, int nTime)
	{
///<<< BEGIN WRITING YOUR CODE SkillSetBeforeTime
		this.GetMyController().SkillSetBeforeTime(nSkillCfgId, nTime);
///<<< END WRITING YOUR CODE
	}

	public void SkillSetManualDo(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillSetManualDo
		this.GetMyController().SkillSetManualDo(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsDoEnd(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsDoEnd
		return this.GetMyController().SkillIsDoEnd(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsViewIn(int nSkillCfgId, int nNpcInstId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsViewIn
		return this.GetMyController().SkillIsViewIn(nSkillCfgId, nNpcInstId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsMaxRangeIn(int nSkillCfgId, int nNpcInstId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsMaxRangeIn
		return this.GetMyController().SkillIsMaxRangeIn(nSkillCfgId, nNpcInstId);
///<<< END WRITING YOUR CODE
	}

	public bool SkillIsMinRangeOut(int nSkillCfgId, int nNpcInstId)
	{
///<<< BEGIN WRITING YOUR CODE SkillIsMinRangeOut
		return this.GetMyController().SkillIsMinRangeOut(nSkillCfgId, nNpcInstId);
///<<< END WRITING YOUR CODE
	}

	public bool DoSkill(int nIdx)
	{
///<<< BEGIN WRITING YOUR CODE DoSkill
		// nIdx 是技能配置Id
		return this.GetMyController().DoSkill(nIdx);
///<<< END WRITING YOUR CODE
	}

	public void SkillManualDo(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillManualDo
		this.GetMyController().SkillManualDo(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public void SkillResetDurationTime(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillResetDurationTime
		this.GetMyController().SkillResetDurationTime(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public void SkillResetAfterTime(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillResetAfterTime
		this.GetMyController().SkillResetAfterTime(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

	public void SkillResetBeforeTime(int nSkillCfgId)
	{
///<<< BEGIN WRITING YOUR CODE SkillResetBeforeTime
		this.GetMyController().SkillResetBeforeTime(nSkillCfgId);
///<<< END WRITING YOUR CODE
	}

///<<< BEGIN WRITING YOUR CODE CLASS_PART
	public NpcBehaviacController GetMyController()
	{
		return (NpcBehaviacController)this.myControlloer;
	}
///<<< END WRITING YOUR CODE

}

///<<< BEGIN WRITING YOUR CODE FILE_UNINIT

///<<< END WRITING YOUR CODE

