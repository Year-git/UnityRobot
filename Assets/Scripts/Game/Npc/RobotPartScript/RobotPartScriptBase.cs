using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Framework;

public abstract class RobotPartScriptBase : MonoBehaviour
{
#if UNITY_EDITOR
    [HeaderAttribute("显示属性")]
    public bool isShowInfo = false;

    [SerializeField]
    [HeaderAttribute("Npc血量信息")]
    private string _hpInfo = "";
    
    [SerializeField]
    [HeaderAttribute("Npc血量信息")]
    private string _durabilityInfo = "";

    public void UpdateState()
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(myElement.myRobotPart.npcInstId);
        if (pNpc == null)
        {
            return;
        }

        this._hpInfo = pNpc.initHp + "->" + pNpc.lastHp;
        this._durabilityInfo = myElement.initDurability + "->" +  myElement.lastDurability;
    }
#endif

    public RobotPartElement myElement;
    protected List<int> _deadEffectInstId = new List<int>();

    /// <summary>
    /// 音效列表
    /// </summary>
    private PlaySoundScript[] _playSoundScriptList;

    /// <summary>
    /// 损伤特效表
    /// </summary>
    /// <typeparam name="DurabilityEffectPack"></typeparam>
    /// <returns></returns>
    protected List<DurabilityEffectPack> _durabilityEffectCfgIdList = new List<DurabilityEffectPack>();

    /// <summary>
    /// 损伤特效的配置数据包
    /// </summary>
    protected struct DurabilityEffectPack
    {
        public DurabilityEffectPack(int a, int b, JArray c) { durabilityThreshold = a; effectCfgId = b; effectPointList = c; }
        public int durabilityThreshold { get; private set; }
        public int effectCfgId { get; private set; }
        public JArray effectPointList { get; private set; }
    }

    protected List<int> _durabilityEffectInstId = new List<int>();
    private int _durabilityEffectCfgIdListIdx;

    // 保存配件的正常模型组
    public GameObject[] _normalModel;
    // 保存配件的损伤模型组
    public GameObject[] _damageModel;
    // 保存配件的死亡模型组
    public GameObject[] _deadModel;

    // 动作同步的模型组
    public GameObject[] synAnimationModel;
    // 动作同步的车体挂点
    private Transform _bodyPoint;
    // 动作同步的保存车体挂点相对车体配件的基础偏移
    private Vector3 _bodyPointBaseShifting;

    private Dictionary<ModelActiveEnum, GameObject[]> ModelActiveList;
    public ModelActiveEnum curModelActive { get; private set; } = ModelActiveEnum.normal;
    // 是否已经切换到损伤模型组
    private bool isHaveSwitchDamageModel = false;

    /// <summary>
    /// 切换模型状态
    /// </summary>
    /// <param name="modelActiveEnum"></param>
    private void ChangeModelActive(ModelActiveEnum modelActiveEnum)
    {
        // 缓存需要显示的模型
        Dictionary<int, GameObject> mapping = new Dictionary<int, GameObject>();
        foreach (var gObj in this.ModelActiveList[modelActiveEnum])
        {
            mapping[gObj.GetInstanceID()] = gObj;
        }

        foreach (ModelActiveEnum suit in Enum.GetValues(typeof(ModelActiveEnum)))
        {
            if (suit != modelActiveEnum)
            {
                foreach (var gObj in this.ModelActiveList[suit])
                {
                    // 如果现在显示并且下个阶段也需要显示 则不隐藏
                    if (mapping.ContainsKey(gObj.GetInstanceID()))
                    {
                    }
                    else if (gObj.activeSelf)
                    {
                        gObj.SetActive(false);
                    }
                }
            }
        }

        foreach (var key in mapping)
        {
            GameObject gObj = key.Value;
            if (!gObj.activeSelf)
            {
                gObj.SetActive(true);
            }
        }

        // 记录当前激活的模型组枚举
        curModelActive = modelActiveEnum;
    }

    /// <summary>
    /// 获取NPC刚体
    /// </summary>
    public Rigidbody GetNpcRigidbody()
    {
        int npcInstId = this.myElement.myRobotPart.npcInstId;
        BaseNpc npc = MapManager.Instance.baseMap.GetNpc(npcInstId);
        Rigidbody npcrid = npc.gameObject.GetComponent<Rigidbody>();
        return npcrid;
    }

    /// <summary>
    /// 播放配件场景音效
    /// </summary>
    /// <param name="nIdx"></param>
    public void PlayPartSound(int nIdx, Vector3 pPosition)
    {
        if (this._playSoundScriptList.Length > nIdx)
        {
            this._playSoundScriptList[nIdx].Play(pPosition);
        }
    }

    /// <summary>
    /// 播放配件及身音效
    /// </summary>
    /// <param name="nIdx"></param>
    /// <param name="pParent"></param>
    public void PlayPartSound(int nIdx, Transform pParent)
    {
        if (this._playSoundScriptList.Length > nIdx)
        {
            this._playSoundScriptList[nIdx].Play(pParent);
        }
    }

    // ----------------------------------------------------------------------------------------------------------
    // 通知相关的定义

    /// <summary>
    /// 零件实例化完成时
    /// </summary>
    public void Base_OnStart()
    {
        this._playSoundScriptList = GetComponents<PlaySoundScript>();
        this.BaseEvent_OnStart();
    }

    /// <summary>
    /// 通知子类零件实例化完成时
    /// </summary>
    protected virtual void BaseEvent_OnStart() { }

    /// <summary>
    /// 地图数据处理完成时通知
    /// </summary>
    public void Base_OnMapDataDisposed()
    {
        this.BaseEvent_OnMapDataDisposed();
    }

    /// <summary>
    /// 通知子类地图数据处理完成时
    /// </summary>
    protected virtual void BaseEvent_OnMapDataDisposed() { }

    /// <summary>
    /// 零件销毁时
    /// </summary>
    public void Base_OnDestroy()
    {
        // 移除死亡特效
        if (this._deadEffectInstId.Count != 0)
        {
            EffectManager pEffectManager = MapManager.Instance.baseMap.effectManager;
            this.DelEffectFromEffectPointList(this._deadEffectInstId, delegate (int nEffInstId)
                {
                    pEffectManager.EffectDel(nEffInstId);
                }
            );
        }

        this.BaseEvent_OnDestroy();
    }

    /// <summary>
    /// 通知子类零件销毁时
    /// </summary>
    protected virtual void BaseEvent_OnDestroy() { }

    /// <summary>
    /// 零件刷新时
    /// </summary>
    public void Base_OnUpdate()
    {
        // 动作同步的模型组与车体动作的坐标同步
        if (this._bodyPoint != null)
        {
            // 获得车体挂点相对车体配件的当前偏移
            Vector3 pCurBaseShifting = this._bodyPoint.position - this.myElement.myModel.transform.position;
            // 用当前偏移减去基础偏移，求出配件同步车体动作的偏移值
            Vector3 pCurShifting = pCurBaseShifting - this._bodyPointBaseShifting;
            for (int i = 0; i < this.synAnimationModel.Length; i++)
            {
                if (this.synAnimationModel[i] != null && this.synAnimationModel[i].activeSelf)
                {
                    this.synAnimationModel[i].transform.localPosition = pCurShifting;
                }
            }
        }

#if UNITY_EDITOR
        if (this.isShowInfo)
        {
            this.UpdateState();
        }
        else
        {
            if (this._hpInfo != "") this._hpInfo = "";
            if (this._durabilityInfo != "") this._durabilityInfo = "";
        }
#endif

        this.BaseEvent_OnUpdate();
    }

    /// <summary>
    /// 通知子类零件刷新时
    /// </summary>
    protected virtual void BaseEvent_OnUpdate() { }

    /// <summary>
    /// 配件安装时
    /// </summary>
    /// <param name="fLoaded">通知完成时的回调</param>
    public void Base_OnInstall(Action fLoaded)
    {
        // 如果类型不是车体，就初始化动作同步的车体挂点和其坐标
        if (this.myElement.myRobotPart.partType != RobotPartType.Body)
        {
            BaseNpc pMyNpc = this.myElement.GetMyNpc();
            int nHoleCfgId = this.myElement.GetMyNpc().GetRobotPartHole(this.myElement.myRobotPart.holeIdx).cfgId;
            int nHoleListIdx = this.myElement.holeListIdx;
            string sPointName = RobotPartHole.GetRobotPartHolePoint(nHoleCfgId, nHoleListIdx);
            if (sPointName != "")
            {
                RobotPartScriptBody pBodyScript = (RobotPartScriptBody)pMyNpc.GetRobotBodyPart().myElementList[0].myScript;
                if (pBodyScript.IsHaveBodyChildTransform(sPointName))
                {
                    Transform pTarget = pBodyScript.GetBodyChildTransform(sPointName);
                    // 保存车体挂点相对车体配件的基础偏移
                    this._bodyPointBaseShifting = pTarget.position - this.myElement.myModel.transform.position;
                    // 保存车体挂点的Transform
                    this._bodyPoint = pTarget;
                }
                else
                {
                    Debug.LogError("CfgError->RobotPartScriptBase.Base_OnInstall->Target Hole Point Is Not Exist!" + "#nHoleCfgId = " + nHoleCfgId + "#sPointName = " + sPointName);
                }
            }
            else
            {
                // Debug.LogError("CfgWarn->RobotPartScriptBase.Base_OnInstall->Hole Point Is Lost!" + "#nHoleCfgId = " + nHoleCfgId + "#nHoleListIdx = " + nHoleListIdx);
            }
        }

        // 初始化模型
        this.ModelActiveList = new Dictionary<ModelActiveEnum, GameObject[]>()
        {
            [ModelActiveEnum.normal] = this._normalModel,
            [ModelActiveEnum.damage] = this._damageModel,
            [ModelActiveEnum.dead] = this._deadModel,
        };

        // 初始化损伤的特效表
        int nPartCfgId = this.myElement.myRobotPart.cfgId;
        // 判定的耐久值小的在前面

        this._durabilityEffectCfgIdList.Add(
            new DurabilityEffectPack
            (
                RobotPart.GetRobotPartDurabilityForBrokenEffect2(),
                RobotPart.GetRobotPartBrokenEffect2(nPartCfgId),
                RobotPart.GetRobotPartBrokenEffect2PointList(nPartCfgId)
            )
        );

        this._durabilityEffectCfgIdList.Add(
            new DurabilityEffectPack
            (
                RobotPart.GetRobotPartDurabilityForBrokenEffect1(),
                RobotPart.GetRobotPartBrokenEffect1(nPartCfgId),
                RobotPart.GetRobotPartBrokenEffect1PointList(nPartCfgId)
            )
        );

        this._durabilityEffectCfgIdListIdx = this._durabilityEffectCfgIdList.Count;

        this.Base_SwitchNormalModel();

        this.BaseEvent_OnInstall(fLoaded);
    }

    /// <summary>
    /// 通知子类配件安装时
    /// </summary>
    /// <param name="fLoaded">通知完成时的回调</param>
    protected virtual void BaseEvent_OnInstall(Action fLoaded)
    {
        fLoaded?.Invoke();
    }

    /// <summary>
    /// 配件卸载时
    /// </summary>
    public void Base_OnUnInstall()
    {
        this.BaseEvent_OnUnInstall();
    }

    /// <summary>
    /// 通知子类配件卸载时
    /// </summary>
    protected virtual void BaseEvent_OnUnInstall() { }

    /// <summary>
    /// 帧同步逻辑帧数据更新时
    /// </summary>
    /// <param name="jInfo"></param>
    public void Base_OnFmSynLogicDataUpdate(JObject jInfo)
    {
        this.BaseEvent_OnFmSynLogicDataUpdate(jInfo);
    }

    /// <summary>
    /// 通知子类帧同步逻辑帧数据更新时
    /// </summary>
    /// <param name="jInfo"></param>
    protected virtual void BaseEvent_OnFmSynLogicDataUpdate(JObject jInfo) { }

    /// <summary>
    /// 帧同步逻辑帧更新时
    /// </summary>
    public void Base_OnFmSynLogicUpdate()
    {
        this.BaseEvent_OnFmSynLogicUpdate();
    }

    /// <summary>
    /// 通知子类帧同步逻辑帧更新时
    /// </summary>
    protected virtual void BaseEvent_OnFmSynLogicUpdate() { }

    /// <summary>
    /// 帧同步渲染帧更新时
    /// </summary>
    /// <param name="nInterpolation"></param>
    public void Base_OnFmSynRenderUpdate(Fix64 nInterpolation)
    {
        this.BaseEvent_OnFmSynRenderUpdate(nInterpolation);
    }

    /// <summary>
    /// 通知子类帧同步渲染帧更新时
    /// </summary>
    /// <param name="nInterpolation"></param>
    protected virtual void BaseEvent_OnFmSynRenderUpdate(Fix64 nInterpolation) { }

    /// <summary>
    /// 通知零件释放技能
    /// </summary>
    public virtual bool DoSkill() { return true; }

    /// <summary>
    /// 碰撞开始
    /// </summary>
    /// <param name="pCollision"></param>
    public void Base_OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        // 添加怪物之间碰撞弹开
        RobotPartScriptBase pTargetPartScript = pCollision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pTargetPartScript != null)
        {
            BaseNpc pMyNpc = MapManager.Instance.baseMap.GetNpc(this.myElement.myRobotPart.npcInstId);
            BaseNpc pTargetNpc = MapManager.Instance.baseMap.GetNpc(pTargetPartScript.myElement.myRobotPart.npcInstId);
            if ((pMyNpc != null) && (pTargetNpc != null))
            {
                if (pMyNpc.type == pTargetNpc.type)
                {
                    ContactPoint contact = pCollision.GetContact(0);
                    Vector3 point = Vector3.zero;
                    Vector3 normal = Vector3.zero;
                    List<ContactPoint> Contacts = new List<ContactPoint>();
                    int ContactCount = pCollision.GetContacts(Contacts);
                    foreach (ContactPoint item in Contacts)
                    {
                        point += item.point;
                        normal += item.normal;
                    }
                    point /= ContactCount;
                    normal /= ContactCount;
                    Rigidbody targetRigidbody = pTargetNpc.gameObject.GetComponent<Rigidbody>();
                    Vector3 fv = -normal * 100;
                    targetRigidbody.AddForceAtPosition(fv, point, ForceMode.Acceleration);
                }
            }
        }

        this.BaseEvent_OnPartElementCollisionEnter(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 通知子类碰撞开始
    /// </summary>
    /// <param name="pCollision"></param>
    protected virtual void BaseEvent_OnPartElementCollisionEnter(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript) { }

    /// <summary>
    /// 碰撞持续
    /// </summary>
    /// <param name="pCollision"></param>
    public void Base_OnPartElementCollisionStay(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        this.BaseEvent_OnPartElementCollisionStay(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 通知子类碰撞持续
    /// </summary>
    /// <param name="pCollision"></param>
    protected virtual void BaseEvent_OnPartElementCollisionStay(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript) { }

    /// <summary>
    /// 碰撞退出
    /// </summary>
    /// <param name="pCollision"></param>
    public void Base_OnPartElementCollisionExit(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript)
    {
        this.BaseEvent_OnPartElementCollisionExit(pMyCollider, pCollision, pTargetScript);
    }

    /// <summary>
    /// 通知子类碰撞退出
    /// </summary>
    /// <param name="pCollision"></param>
    protected virtual void BaseEvent_OnPartElementCollisionExit(Collider pMyCollider, Collision pCollision, RobotPartScriptBase pTargetScript) { }

    /// <summary>
    /// Npc伤害输入时
    /// </summary>
    public void Base_OnNpcDamageInput()
    {
        this.BaseEvent_OnNpcDamageInput();
    }

    /// <summary>
    /// 通知子类Npc伤害输入时
    /// </summary>
    protected virtual void BaseEvent_OnNpcDamageInput() { }

    /// <summary>
    /// Npc治疗输入时
    /// </summary>
    public void Base_OnNpcTreatmentInput()
    {
        this.BaseEvent_OnNpcTreatmentInput();
    }

    /// <summary>
    /// 通知子类Npc治疗输入时
    /// </summary>
    protected virtual void BaseEvent_OnNpcTreatmentInput() { }

    /// <summary>
    /// Npc死亡时
    /// </summary>
    public void Base_OnNpcDead()
    {
        this.BaseEvent_OnNpcDead();
    }

    /// <summary>
    /// 通知子类Npc死亡时
    /// </summary>
    protected virtual void BaseEvent_OnNpcDead() { }


    /// <summary>
    /// 获取当前模型显示阶段
    /// </summary>
    /// <returns></returns>
    protected ModelActiveEnum GetModelActiveEnum()
    {
        int nSwitchDurability = (int)Math.Floor((double)(this.myElement.initDurability * RobotPart.GetRobotPartSwitchDamageModelDurability() / 100));
        if (this.myElement.isDead)
        {
            return ModelActiveEnum.dead;
        }
        else if (this.myElement.lastDurability < nSwitchDurability)
        {
            return ModelActiveEnum.damage;
        }
        else
        {
            return ModelActiveEnum.normal;
        }
    }

    /// <summary>
    /// 零件伤害输入时
    /// </summary>
    public void Base_OnPartElementDamageInput()
    {
        // 切换到损伤模型组
        if (!this.isHaveSwitchDamageModel)
        {
            if (this.GetModelActiveEnum() == ModelActiveEnum.damage)
            {
                this.Base_SwitchDamageModel();
            }
        }

        // 播放损伤特效
        var pEffectManager = MapManager.Instance.baseMap.effectManager;
        for (int i = 0; i < this._durabilityEffectCfgIdListIdx; i++)
        {
            int nDurabilityLimit = this.myElement.initDurability * this._durabilityEffectCfgIdList[i].durabilityThreshold / 100;
            if (this.myElement.lastDurability <= nDurabilityLimit)
            {
                if (this._durabilityEffectInstId.Count != 0)
                {
                    this.DelEffectFromEffectPointList(this._durabilityEffectInstId, delegate (int nEffInstId)
                        {
                            pEffectManager.EffectDel(nEffInstId);
                        }
                    );
                }
                int nEffectCfgId = this._durabilityEffectCfgIdList[i].effectCfgId;
                if (nEffectCfgId > 0)
                {
                    this._durabilityEffectInstId = this.AddEffectToEffectPointList(nEffectCfgId, this._durabilityEffectCfgIdList[i].effectPointList, delegate (int nEffCfgId, int nPointId)
                        {
                            return this.myElement.BodyEffectAddByPoint(nEffCfgId, nPointId);
                        }
                    );
                }
                this._durabilityEffectCfgIdListIdx = i;
                break;
            }
        }
        this.BaseEvent_OnPartElementDamageInput();
    }

    /// <summary>
    /// 通知子类零件伤害输入时
    /// </summary>
    protected virtual void BaseEvent_OnPartElementDamageInput() { }

    /// <summary>
    /// 零件伤害输入过滤器触发
    /// </summary>
    /// <param name="nDamage">传入伤害</param>
    /// <returns>过滤后的伤害</returns>
    public int Base_OnPartElementDamageInputFilter(int nDamage)
    {
        return this.BaseEvent_OnPartElementDamageInputFilter(nDamage);
    }

    /// <summary>
    /// 通知子类零件伤害输入过滤器触发时
    /// </summary>
    /// <param name="nDamage">传入伤害</param>
    /// <returns>过滤后的伤害</returns>
    protected virtual int BaseEvent_OnPartElementDamageInputFilter(int nDamage)
    {
        return nDamage;
    }

    /// <summary>
    /// 零件治疗输入时
    /// </summary>
    public void Base_OnPartElementTreatmentInput()
    {
        // 去除死亡特效
        this.DelEffectFromEffectPointList(this._deadEffectInstId, delegate (int nEffInstId)
            {
                MapManager.Instance.baseMap.effectManager.EffectDel(nEffInstId);
            }
        );

        // 重置损伤特效索引
        this._durabilityEffectCfgIdListIdx = this._durabilityEffectCfgIdList.Count;

        // 去除损伤特效
        this.DelEffectFromEffectPointList(this._durabilityEffectInstId, delegate (int nEffInstId)
            {
                MapManager.Instance.baseMap.effectManager.EffectDel(nEffInstId);
            }
        );

        // 切换回正常模型
        this.isHaveSwitchDamageModel = false;
        this.ChangeModelActive(ModelActiveEnum.normal);

        this.BaseEvent_OnPartElementTreatmentInput();
    }

    /// <summary>
    /// 通知子类零件治疗输入时
    /// </summary>
    protected virtual void BaseEvent_OnPartElementTreatmentInput() { }

    /// <summary>
    /// 零件死亡时
    /// </summary>
    public void Base_OnPartElementDead()
    {
        EffectManager pEffectManager = MapManager.Instance.baseMap.effectManager;

        // 播放死亡特效
        int nPartCfgId = this.myElement.myRobotPart.cfgId;
        int nDeadEffectCfgId = RobotPart.GetRobotPartDeadEffect(nPartCfgId);
        if (nDeadEffectCfgId > 0)
        {
            this._deadEffectInstId = AddEffectToEffectPointList(nDeadEffectCfgId, RobotPart.GetRobotPartDeadEffectPointList(nPartCfgId), delegate (int nEffCfgId, int nPointId)
                {
                    return this.myElement.BodyEffectAddByPoint(nEffCfgId, nPointId);
                }
            );
        }

        // 删除损毁特效
        this.DelEffectFromEffectPointList(this._durabilityEffectInstId, delegate (int nEffInstId)
            {
                pEffectManager.EffectDel(nEffInstId);
            }
        );

        this.Base_SwitchDeadModel();

        this.BaseEvent_OnPartElementDead();
    }

    /// <summary>
    /// 通知子类零件死亡时
    /// </summary>
    protected virtual void BaseEvent_OnPartElementDead() { }

    /// <summary>
    /// Npc启用状态改变通知
    /// </summary>
    /// <param name="bEnableState"></param>
    public void Base_OnNpcEnableStateChange(bool bEnableState)
    {
        this.BaseEvent_OnNpcEnableStateChange(bEnableState);
    }

    /// <summary>
    /// 通知子类Npc启用状态改变
    /// </summary>
    /// <param name="bEnableState"></param>
    protected virtual void BaseEvent_OnNpcEnableStateChange(bool bEnableState) { }

    /// <summary>
    /// 指定特效添加方法的格式
    /// </summary>
    /// <param name="nEffCfgId">特效配置Id</param>
    /// <param name="nPointId">特效挂点Id</param>
    /// <returns>返回特效实例Id</returns>
    public delegate int AddEffectToEffectPoint(int nEffCfgId, int nPointId);

    /// <summary>
    /// 根据特效配置Id、特效挂点表、特效添加方法添加特效，并返回添加的特效的实例Id表
    /// </summary>
    /// <param name="nEffCfgId">特效配置Id</param>
    /// <param name="pEffectPointList">特效挂点表</param>
    /// <param name="fAddEffectAction">特效添加方法</param>
    /// <returns></returns>
    public List<int> AddEffectToEffectPointList(int nEffCfgId, JArray pEffectPointList, AddEffectToEffectPoint fAddEffectAction)
    {
        EffectManager pEffectManager = MapManager.Instance.baseMap.effectManager;
        List<int> pSaveEffectInstIdList = new List<int>();
        foreach (int nPointId in pEffectPointList)
        {
            int nEffInstId = fAddEffectAction.Invoke(nEffCfgId, nPointId);
            if (nEffInstId > 0)
            {
                pSaveEffectInstIdList.Add(nEffInstId);
            }
        }
        return pSaveEffectInstIdList;
    }

    /// <summary>
    /// 从保存特效实例Id的列表中按照指定的特效移除方法移除特效
    /// </summary>
    /// <param name="pSaveEffectInstIdList">保存特效实例Id的列表</param>
    /// <param name="fDelEffectAction">特效移除方法</param>
    public void DelEffectFromEffectPointList(List<int> pSaveEffectInstIdList, Action<int> fDelEffectAction)
    {
        EffectManager pEffectManager = MapManager.Instance.baseMap.effectManager;
        if (pSaveEffectInstIdList.Count > 0)
        {
            foreach (int nEffInstId in pSaveEffectInstIdList)
            {
                fDelEffectAction.Invoke(nEffInstId);
            }
            pSaveEffectInstIdList.Clear();
        }
    }

    /// <summary>
    /// 零件切换到正常模型
    /// </summary>
    private void Base_SwitchNormalModel()
    {
        this.BaseEvent_SwitchNormalModel();

        this.isHaveSwitchDamageModel = false;
        this.ChangeModelActive(ModelActiveEnum.normal);
    }

    /// <summary>
    /// 通知子类零件切换到正常模型
    /// </summary>
    protected virtual void BaseEvent_SwitchNormalModel() { }

    /// <summary>
    /// 零件切换到损伤模型
    /// </summary>
    private void Base_SwitchDamageModel()
    {
        this.BaseEvent_SwitchDamageModel();

        this.isHaveSwitchDamageModel = true;
        this.ChangeModelActive(ModelActiveEnum.damage);
    }

    /// <summary>
    /// 通知子类零件切换到损伤模型
    /// </summary>
    protected virtual void BaseEvent_SwitchDamageModel() { }

    /// <summary>
    /// 零件切换到死亡模型
    /// </summary>
    private void Base_SwitchDeadModel()
    {
        this.BaseEvent_SwitchDeadModel();

        this.isHaveSwitchDamageModel = false;
        this.ChangeModelActive(ModelActiveEnum.dead);
    }

    /// <summary>
    /// 通知子类零件切换到死亡模型
    /// </summary>
    protected virtual void BaseEvent_SwitchDeadModel() { }

    // ----------------------------------------------------------------------------------------------------------
    // 配件常用计算方法

    /// <summary>
    /// 将角度 转换为 -180~180
    /// </summary>
    public float ChangeAngle(float angle)
    {
        float newAngle = angle;
        if(newAngle < 0){
            newAngle = 360 - Mathf.Abs(newAngle);
        }
        newAngle = (newAngle - 180) > 0 ? (newAngle - 360) : newAngle;
        return newAngle;
    }

    /// <summary>
    /// 计算B 相对于 A 的角度
    /// </summary>
    public float CalcRelativeAngle(float A, float B)
    {
        float newAngle = B - A;
        if (Mathf.Abs(newAngle) > 180)
            newAngle = (newAngle > 0) ? (newAngle - 360) : (newAngle + 360);
        return newAngle;
    }
    
    /// <summary>
    /// 增强平稳性,类似不倒翁
    /// </summary>
    public void ReinforceStable(GameObject obj, Rigidbody rigidbody)
    {
        float x = ChangeAngle(obj.transform.eulerAngles.x);
        float z = ChangeAngle(obj.transform.eulerAngles.z);
        if (Mathf.Abs(x) > 5 || Mathf.Abs(z) > 5)
        {
            x = Mathf.Pow((Mathf.Abs(x) - 5) / 5, 2) * (x > 0 ? -1 : 1);
            z = Mathf.Pow((Mathf.Abs(z) - 5) / 5, 2) * (z > 0 ? -1 : 1);
            rigidbody.AddRelativeTorque(x, 0, z, ForceMode.Acceleration);
        }
    }
}
