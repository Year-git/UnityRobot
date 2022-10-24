using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

public class RobotPartElement : MapObject 
{
    public RobotPart myRobotPart;

    public RobotPartScriptBase myScript;

    public Model myModel;

    public int holeListIdx;

    public AnimationController myAnimationController;

    // ----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 是否加载完成
    /// </summary>
    public bool loadEnd{ get { return this.myModel != null ? this.myModel.isLoadEnd : false; } }

    /// <summary>
    /// 加载配件模型
    /// </summary>
    public void Load(int nModelCfgId, Vector3 pPosition, Vector3 pEulerAngles, float nParentScale, Action<RobotPartElement> fLoaded = null)
    {
        string sModelName = ConfigManager.GetValue<string>("Model_C", nModelCfgId, "strSkinName");
        float nScale = Model.GetModelScale(nModelCfgId) * nParentScale;

        this.myModel = new Model(this, sModelName, pPosition, pEulerAngles, nScale, delegate (Model model)
            {
                this.myModel = model;
                this.myModel.gameObject.SetActive(false);
                this.myAnimationController = new AnimationController(nModelCfgId, this.myModel.gameObject);
                this.RobotPartFragmentInit(delegate()
                    {
                        fLoaded?.Invoke(this);
                    }
                );
            }
        );
    }

    /// <summary>
    /// 获取该零件所属的Npc
    /// </summary>
    /// <returns></returns>
    public BaseNpc GetMyNpc()
    {
        return MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId);
    }

    // ----------------------------------------------------------------------------------------------------------
    // 动作相关代码

    /// <summary>
    /// 零件通知动作控制器播放指定索引的动作
    /// </summary>
    /// <param name="nAnimationIdx"></param>
    public void AnimationPlay(int nAnimationIdx)
    {
        this.myAnimationController.Play(nAnimationIdx);
    }

    private void AnimationUpdate()
    {
        this.myAnimationController.Update();
    }

    // ----------------------------------------------------------------------------------------------------------
    // 战斗相关代码

    /// <summary>
    /// 释放技能
    /// </summary>
    public bool DoSkill(){
        return myScript.DoSkill();
    }

    /// <summary>
    /// 零件初始耐久
    /// </summary>
    /// <value></value>
    public int initDurability
    {
        get
        {
            BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId);
            if (pNpc == null)
            {
                return -1;
            }
            return ((pNpc.initHp * this.myRobotPart.attrEntityCfg.GetValue(AttributeType.Durability)) / 100) / this.myRobotPart.myElementList.Count;
        }
    }

    /// <summary>
    /// 零件剩余耐久万分比
    /// </summary>
    private int _lastDurabilityOfTenThousandRatio = 10000;

    /// <summary>
    /// 零件剩余耐久
    /// </summary>
    /// <value></value>
    public int lastDurability
    {
        get
        {
            return this.initDurability * this._lastDurabilityOfTenThousandRatio / 10000;
        }
        set
        {
            int nNewVal = value * 10000 / this.initDurability;
            if (nNewVal > 10000)
            {
                this._lastDurabilityOfTenThousandRatio = 10000;
            }
            else if(nNewVal < 0)
            {
                this._lastDurabilityOfTenThousandRatio = 0;
            }
            else
            {
                this._lastDurabilityOfTenThousandRatio = nNewVal;
            }
        }
    }

    public bool isDead
    { 
        get
        {
            if (this.myRobotPart.partType == RobotPartType.Body)
            {
                return MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId).isDead;
            }
            else
            {
                return this.lastDurability == 0;
            }
        } 
    }

    /// <summary>
    /// 零件伤害输入
    /// </summary>
    /// <param name="nDamage"></param>
    public void RobotPartElementDamageInput(int nDamage)
    {
        int nFixDamage = nDamage < 0 ? 0 : nDamage;
        int nNewDurability = this.lastDurability - nFixDamage;
        this.lastDurability = nNewDurability >= 0 ? nNewDurability : 0;
        
        this.OnPartElementDamageInput();

        if (this.lastDurability == 0)
        {
            this.OnPartElementDead();
        }
    }

    /// <summary>
    /// 零件治疗输入
    /// </summary>
    /// <param name="nTreatmentVal"></param>
    public void RobotPartElementTreatmentInput(int nTreatmentVal)
    {
        int nFixTreatmentVal = nTreatmentVal < 0 ? 0 : nTreatmentVal;
        int nNewDurability = this.lastDurability + nFixTreatmentVal;
        this.lastDurability = nNewDurability > this.initDurability ? this.initDurability : nNewDurability;
        this.OnPartElementTreatmentInput();
    }

    /// <summary>
    /// 零件设置死亡
    /// </summary>
    private void RobotPartElementSetDead()
    {
        this._lastDurabilityOfTenThousandRatio = 0;
        this.OnPartElementDead();
    }

    /// <summary>
    /// 零件伤害输入过滤器触发
    /// </summary>
    /// <param name="nDamage">传入伤害</param>
    /// <returns>过滤后的伤害</returns>
    public int RobotPartElementDamageInputFilter(int nDamage)
    {
        return this.myScript.Base_OnPartElementDamageInputFilter(nDamage);
    }

    //--------------------------------------------------------------------------------------------------
    // 零件行为相关代码

    /// <summary>
    /// 移动配件的零件移动到某坐标
    /// </summary>
    /// <param name="pPosition"></param>
    /// <param name="fReachCall"></param>
    public bool MovePartElement_MoveToPos(Vector3 pPosition)
    {
#if UNITY_EDITOR
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId);
        Debug.DrawLine(pNpc.myModel.transform.position, pPosition, Color.green, 0.2f);//绿色
#endif

        // 清理之前的移动配件的零件向指定角度移动的值
        ((RobotPartScriptMove)this.myScript).MoveToAngle(-1);
        // 移动配件的零件移动到某坐标接口
        return ((RobotPartScriptMove)this.myScript).AddTargetPoint(pPosition);
    }

    /// <summary>
    /// 移动配件的零件向指定角度移动
    /// </summary>
    public void MovePartElement_MoveToAngle(int nAngle)
    {
#if UNITY_EDITOR
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId);
        Debug.DrawLine(pNpc.myModel.transform.position, pNpc.GetPosByAngle(0, 1000f), Color.blue, 0.2f);//蓝色
        Debug.DrawLine(pNpc.myModel.transform.position, pNpc.GetPosByAngle(nAngle, 1000f), Color.red, 0.2f);//红色
#endif

        ((RobotPartScriptMove)this.myScript).MoveToAngle((int)nAngle);
    }

    /// <summary>
    /// 移动配件的零件停止移动
    /// </summary>
    public void MovePartElement_MoveStop()
    {
        // 清理之前的移动配件的零件向指定角度移动的值
        ((RobotPartScriptMove)this.myScript).MoveToAngle(-1);
        // 移动配件的零件停止移动接口
        ((RobotPartScriptMove)this.myScript).DownSpace();
    }

    /// <summary>
    /// 移动配件的零件移动是否到达
    /// </summary>
    public bool MovePartElement_IsMoveReach()
    {
        return ((RobotPartScriptMove)this.myScript).IsArriveTarget();
        // return false;
    }

    /// <summary>
    /// 移动配件的零件转向到某坐标
    /// </summary>
    /// <param name="pPosition"></param>
    public void MovePart_TurnToPos(Vector3 pPosition)
    {
#if UNITY_EDITOR
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this.myRobotPart.npcInstId);
        Debug.DrawLine(pNpc.myModel.transform.position, pNpc.GetPosByAngle(0, 1000f), Color.cyan, 0.2f);//青色
        Debug.DrawLine(pNpc.myModel.transform.position, pPosition, Color.yellow, 0.2f);//黄色
#endif

        ((RobotPartScriptMove)this.myScript).LookAtPos(pPosition);
    }

    /// <summary>
    /// 移动配件的零件转向停止
    /// </summary>
    /// <param name="pPosition"></param>
    public void MovePart_TurnStop()
    {
        ((RobotPartScriptMove)this.myScript).SlowDown();
    }

    /// <summary>
    /// 移动配件的零件转向是否到达
    /// </summary>
    public bool MovePartElement_IsTurnReach()
    {
        return ((RobotPartScriptMove)this.myScript).IsLookAt();
        // return false;
    }

    // ----------------------------------------------------------------------------------------------------------
    // 零件的特效挂点相关代码

    /// <summary>
    /// 零件的特效挂点表
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="Transform"></typeparam>
    /// <returns></returns>
    private Dictionary<string, Transform> _effectPointList = new Dictionary<string, Transform>();

    /// <summary>
    /// 初始化配件零件的特效挂点
    /// </summary>
    private void PartElementEffectPointInit()
    {
        Transform[] pEffectPointList = this.myModel.gameObject.GetComponentsInChildren<Transform>(true);
        foreach(var pChild in pEffectPointList)
        {
            string sChildName = pChild.name;
            if (sChildName.Length > 12)
            {
                if (sChildName.Substring(0,12) == "EffectPoint_")
                {
                    this._effectPointList.Add(pChild.name, pChild);
                }
            }
        }
    }

    /// <summary>
    /// 获取配件零件挂点的Transform
    /// </summary>
    /// <param name="nId">挂点Id</param>
    /// <returns></returns>
    private Transform GetPartElementEffectPoint(int nId)
    {
        string sId = "EffectPoint_" + nId.ToString();
        if (!this._effectPointList.ContainsKey(sId))
        {
            return null;
        }
        return this._effectPointList[sId];
    }

    /// <summary>
    /// 向配件零件的特效挂点上添加特效
    /// </summary>
    /// <param name="nEffCfgId"></param>
    /// <param name="nPointIdx"></param>
    /// <returns></returns>
	public int BodyEffectAddByPoint(int nEffCfgId, int nPointIdx)
    {
        Transform pEffectPoint = this.GetPartElementEffectPoint(nPointIdx);
        if (pEffectPoint == null){
            return 0;
        }
        return  MapManager.Instance.baseMap.effectManager.BodyEffectAdd(nEffCfgId,pEffectPoint);
    }

    // ----------------------------------------------------------------------------------------------------------
    // 零件碎片的相关代码

    /// <summary>
    /// 零件碎片表
    /// </summary>
    /// <typeparam name="RobotPartFragment"></typeparam>
    /// <returns></returns>
    private List<RobotPartFragment> _fragmentList = new List<RobotPartFragment>();
    
    /// <summary>
    /// 零件碎片的回收时间
    /// </summary>
    private float _fragmentRecoveryIntervalTime = 30f;

    /// <summary>
    /// 零件碎片的回收记录时间
    /// </summary>
    private float _fragmentRecoveryTriggerTime = 0f;

    /// <summary>
    /// 初始化零件碎片
    /// </summary>
    private void RobotPartFragmentInit(Action fDoneCall)
    {
        int nTotal = 5;

        CommAsyncCounter pCounter = new CommAsyncCounter(nTotal, delegate()
            {
                fDoneCall?.Invoke();
            }
        );

        Transform pTransform = this.myModel.transform;
        for(int i = 0; i < nTotal; i++)
        {
            new RobotPartFragment(pTransform.position, pTransform.rotation.eulerAngles, delegate(RobotPartFragment pFragment)
                {
                    this._fragmentList.Add(pFragment);
                    pFragment.myModel.gameObject.SetActive(false);
                    pCounter.Increase();
                }
            );
        }
    }

    /// <summary>
    /// 零件碎片抛出
    /// </summary>
    private void RobotPartFragmentThrowOut()
    {
        foreach(var pFragment in this._fragmentList)
        {
            pFragment.myModel.transform.position = this.myModel.transform.position;
            pFragment.myModel.transform.rotation = Quaternion.identity;
            pFragment.myModel.gameObject.SetActive(true);
            pFragment.RandomThrowOut();
        }
    }

    /// <summary>
    /// 零件碎片回收初始化
    /// </summary>
    private void RobotPartFragmentRecoveryInit()
    {
        FrameSynchronManager pFrameSynManager = FrameSynchronManager.Instance;
        this._fragmentRecoveryTriggerTime = (float)(pFrameSynManager.fsData.FixFrameLen * pFrameSynManager.fsData.GameLogicFrame + this._fragmentRecoveryIntervalTime);
    }

    /// <summary>
    /// 零件碎片回收操作
    /// </summary>
    private void RobotPartFragmentRecovery()
    {
        this._fragmentRecoveryTriggerTime = 0f;

        foreach(var pFragment in _fragmentList)
        {
            pFragment.myModel.gameObject.SetActive(false);
            pFragment.myModel.rigidbody.velocity = Vector3.zero;
            pFragment.myModel.rigidbody.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 零件碎片回收检查
    /// </summary>
    private void RobotPartFragmentCheckRecovery()
    {
        if (this._fragmentRecoveryTriggerTime == 0f)
        {
            return;
        }
        
        FrameSynchronManager pFrameSynManager = FrameSynchronManager.Instance;
        if (pFrameSynManager.fsData.FixFrameLen * pFrameSynManager.fsData.GameLogicFrame < this._fragmentRecoveryTriggerTime)
        {
            return;
        }

        this.RobotPartFragmentRecovery();
    }

    // ----------------------------------------------------------------------------------------------------------
    // 零件事件通知的相关代码

    /// <summary>
    /// 配件实例化完成时
    /// </summary>
	public void OnPartElementStart()
    {
        this.PartElementEffectPointInit();
        this.myScript.Base_OnStart();
    }

    /// <summary>
    /// 地图数据处理完成时通知
    /// </summary>
    public void OnMapDataDisposed()
    {
        this.myScript.Base_OnMapDataDisposed();
    }

    /// <summary>
    /// 配件销毁时
    /// </summary>
    public void OnPartElementDestroy()
    {
        this.myScript.Base_OnDestroy();
        this.myModel.Destroy();
    }

    /// <summary>
    /// 配件刷新时
    /// </summary>
    public void OnPartElementUpdate()
    {
        if (this.myModel != null)
        {
            this.myScript.Base_OnUpdate();
            this.myModel.Update();
        }
    }

    /// <summary>
    /// 配件安装时
    /// </summary>
    /// <param name="fLoaded">通知完成时的回调</param>
    public void OnPartElementInstall(Action fLoaded)
    {
        this.myScript.Base_OnInstall(fLoaded);
    }

    /// <summary>
    /// 配件卸载时
    /// </summary>
    public void OnPartElementUnInstall()
    {
        this.myScript.Base_OnUnInstall();
    }

    /// <summary>
    /// 帧同步逻辑帧数据更新时
    /// </summary>
    /// <param name="jInfo"></param>
    public void OnPartElementFmSynLogicDataUpdate(JObject jInfo)
    {
        this.myScript.Base_OnFmSynLogicDataUpdate(jInfo);
    }

    /// <summary>
    /// 帧同步逻辑帧更新时
    /// </summary>
    public void OnPartElementFmSynLogicUpdate()
    {
        this.RobotPartFragmentCheckRecovery();
        this.AnimationUpdate();
        this.myScript.Base_OnFmSynLogicUpdate();
    }

    /// <summary>
    /// 帧同步渲染帧更新时
    /// </summary>
    /// <param name="nInterpolation"></param>
    public void OnPartElementFmSynRenderUpdate(Fix64 nInterpolation)
    {
        this.myScript.Base_OnFmSynRenderUpdate(nInterpolation);
    }

    /// <summary>
    /// 碰撞开始
    /// </summary>
    /// <param name="pCollision"></param>
    public void OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        this.myScript.Base_OnPartElementCollisionEnter(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 碰撞持续
    /// </summary>
    /// <param name="pCollision"></param>
    public void OnPartElementCollisionStay(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        this.myScript.Base_OnPartElementCollisionStay(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 碰撞退出
    /// </summary>
    /// <param name="pCollision"></param>
    public void OnPartElementCollisionExit(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        this.myScript.Base_OnPartElementCollisionExit(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 配件的零件伤害输入时
    /// </summary>
    private void OnPartElementDamageInput()
    {
        this.myScript.Base_OnPartElementDamageInput();
    }

    /// <summary>
    /// 配件的零件治疗输入时
    /// </summary>
    private void OnPartElementTreatmentInput()
    {
        this.myScript.Base_OnPartElementTreatmentInput();
    }

    /// <summary>
    /// 配件的零件死亡时
    /// </summary>
    private void OnPartElementDead()
    {
        this.RobotPartFragmentThrowOut();
        this.RobotPartFragmentRecoveryInit();
        this.myScript.Base_OnPartElementDead();
    }

    /// <summary>
    /// Npc伤害输入时
    /// </summary>
    public void OnNpcDamageInput()
    {
        this.myScript.Base_OnNpcDamageInput();
    }

    /// <summary>
    /// Npc治疗输入时
    /// </summary>
    public void OnNpcTreatmentInput()
    {
        this.myScript.Base_OnNpcTreatmentInput();
    }

    /// <summary>
    /// Npc死亡时
    /// </summary>
    public void OnNpcDead()
    {
        // 如果是车体零件，则调用其死亡，如果不是，则看是否已死亡
        if (this.myRobotPart.partType == RobotPartType.Body)
        {
            this.RobotPartElementSetDead();
        }
        else
        {
            if (!this.isDead)
            {
                this.RobotPartElementSetDead();
            }
        }

        this.myScript.Base_OnNpcDead();
    }

    // Npc启用状态改变通知
    public void OnNpcEnableStateChange(bool bEnableState)
    {
        this.myScript.Base_OnNpcEnableStateChange(bEnableState);
    }
}
