using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Framework;
using QFX.IFX;

public abstract class BaseNpc : MapObject
{
    /// <summary>
    /// npc类型
    /// </summary>
    public NpcType type { get; private set; }

    /// <summary>
    /// npc配置ID
    /// </summary>
    public int cfgId { get; private set; }

    /// <summary>
    /// npc名称
    /// </summary>
    public string roleName { get; protected set; } = "";

    public bool isMaster = false;

    /// <summary>
    /// 模型管理
    /// </summary>
    public Model myModel { get; set; }

    /// <summary>
    /// 当前npc是否加载完成
    /// </summary>
    public bool isLoadEnd
    {
        get
        {
            return this.myModel != null ? this.myModel.isLoadEnd : false;
        }
    }

    /// <summary>
    /// 当前npc配件初始化完毕
    /// </summary>
    public bool isPartInitEnd { get; private set; } = false;

    /// <summary>
    /// 玩家缩放
    /// </summary>
    public float myScale { get; protected set; } = 1f;

    public GameObject gameObject { get { return myModel?.gameObject; } }

    public NpcScript myScript { get; private set; }

    /// <summary>
    /// Npc是否启用队列刷新
    /// </summary>
    /// <value></value>
    public virtual bool IsCanAddQueueNpcList { get; set; } = false;

    /// <summary>
    /// Npc行为树控制器
    /// </summary>
    /// <value></value>
    public NpcBehaviacController myBehaviacController { get; private set; }

    /// <summary>
    /// Npc的Buff控制器
    /// </summary>
    /// <value></value>
    public BuffController myBuffController { get; private set; }

    /// <summary>
    /// Npc的技能控制器
    /// </summary>
    /// <value></value>
    public SkillController mySkillController { get; private set; }

    /// <summary>
    /// UI标识
    /// </summary>
    public bool isUiNpc = false;

    /// <summary>
    /// Npc视野范围
    /// </summary>
    /// <value></value>
    public int myViewRange { get; private set; }

    public IFX_AuraCloner myIFX_AuraCloner { get; private set; }

    public BaseNpc(NpcType eNpcType, int nCfgId) : base()
    {
        this.type = eNpcType;
        this.cfgId = nCfgId;
        this.myViewRange = BaseNpc.GetNpcViewRange(nCfgId);
    }

    /// <summary>
    /// 解析服务器数据
    /// </summary>
    public void UnBasePackServer(JArray jNpcInfo)
    {
        this.type = (NpcType)(int)jNpcInfo[0];
        this.UnPackServer(jNpcInfo);
    }

    /// <summary>
    /// 子类型解析
    /// </summary>
    public virtual void UnPackServer(JArray jNpcInfo) { }

    /// <summary>
    /// 加载模型
    /// </summary>
    public virtual void LoadNpc(Vector3 pPosition, Vector3 pEulerAngles, Action<BaseNpc> fLoaded = null)
    {
        int nModelCfgId = BaseNpc.GetNpcModelId(this.cfgId);
        string sModelName = Model.GetModelName(nModelCfgId);
        this.myScale = Model.GetModelScale(nModelCfgId);

        new Model(this, sModelName, pPosition, pEulerAngles, 1f, delegate (Model model)
            {
                this.myModel = model;
                this.myModel.gameObject.name = "Npc_" + type.ToString() + "_" + this.cfgId + "_" + this.InstId;

                // 添加碰撞脚本
                this.myScript = this.myModel.gameObject.GetComponent<NpcScript>();
                this.myScript.myNpc = this;

                // 添加Npc阻挡文件（用于寻路）
                this.myModel.gameObject.AddComponent<NavMeshSourceTag>();

                this.myIFX_AuraCloner = this.myModel.gameObject.GetComponent<IFX_AuraCloner>();
                this.myIFX_AuraCloner.myParentObj = this.myModel.gameObject;

                // 初始化Npc的行为树控制器
                this.myBehaviacController = new NpcBehaviacController(this.myModel.gameObject, this);

                // 初始化Npc的技能控制器
                this.mySkillController = new SkillController(this);

                // 初始化Buff控制器
                this.myBuffController = new BuffController(this);

                // 设置出生点坐标
                this.spawnPositon = pPosition;

                // 设置Npc的Layer
                this.NpcLayerSet();

                // 初始化载体槽位
                int nHoleId = ConfigManager.GetValue<int>("Npc_C", cfgId, "beginSlot");
                //锁敌特效
                CreateTargetMonEffect();
                this.RobotrPartHoleCreate(0, nHoleId, delegate ()
                {
                    FollowLoad(fLoaded);
                });
            }
        );
    }

    public virtual void FollowLoad(Action<BaseNpc> fLoaded = null)
    {
        fLoaded?.Invoke(this);
    }
    public virtual void CreateTargetMonEffect(){}
    /// <summary>
    /// 删除npc
    /// </summary>
    public virtual void NpcDestroy()
    {
        this.myBehaviacController.Clear();
        this.myModel.Destroy();
    }

    /// <summary>
    /// 刷帧
    /// </summary>
    public virtual void OnNpcUpdate()
    {
        if (!this.isPartInitEnd)
        {
            return;
        }

        if (!this.IsEnableState())
        {
            return;
        }

        this.myModel.Update();

        // var fsData = FrameSynchronManager.Instance.fsData;
        // if (fsData != null && !fsData.PauseState)
        // {
        //     // AI刷帧
        //     if (this.robotAIController != null)
        //     {
        //         this.robotAIController.AIControllerUpdate();
        //     }
        // }

        if (!this.isDead)
        {
            RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
                {
                    pPart.Update();
                }
            );

            // 如果当前地图不是战斗地图，用常规Update推进Npc行为树
            if (MapManager.Instance.baseMap.mapType != MapType.Battle)
            {
                // 如果Npc由行为树控制，则技能相关刷新的次数和时机与行为树一致，且必须在刷行为树之前
                this.mySkillController.Update();

                this.myBehaviacController.UpdateBehaviacTree();
            }
        }

        this.OnNpcUpdateLua();
    }

    public void OnNpcUpdateLua()
    {
        if (Camera.main == null)
        {
            return;
        }
        
        // 通知ui 玩家信息
        Vector3 tagerPos = new Vector3(this.myModel.transform.position.x, this.myModel.transform.position.y + 4, this.myModel.transform.position.z);
        Vector3 PlayerXY = Camera.main.WorldToScreenPoint(tagerPos);
        LuaGEvent.DispatchEventToLua(GacEvent.NpcLuaUpdate, this.InstId, this.roleName, (double)PlayerXY.x, (double)PlayerXY.y, (double)PlayerXY.z, lastHp, initHp, this.cfgId, IsEnableState());
    }

    /// <summary>
    /// 地图数据处理完成时通知
    /// </summary>
    public void OnMapDataDisposed()
    {
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                pPart.OnMapDataDisposed();
            }
        );
    }

    /// <summary>
    /// 地图关卡游戏开始【帧同步开始】
    /// </summary>
    public virtual void OnMapLevelGameStart()
    {
        this.myBehaviacController.DispatchGameEventToAi(BehaviacGameEvent.Map_OnGameStart);
    }

    /// <summary>
    /// 地图关卡游戏结束
    /// </summary>
    public virtual void OnMapLevelGameEnd()
    {
        this.myBehaviacController.DispatchGameEventToAi(BehaviacGameEvent.Map_OnGameEnd);
    }

    // 帧同步的逻辑帧数据刷新时调用
    public virtual void NpcFrameSynLogicDataUpdate(JObject jInfo)
    {
        if (!this.IsEnableState())
        {
            return;
        }

        if (this.IsPauseState())
        {
            return;
        }

        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
        {
            pPart.FmSynLogicDataUpdate(jInfo);
        });

        // 检测释放技能操作
        if (jInfo["Skill"] != null)
        {
            this.mySkillController.SkillDo((int)jInfo["Skill"]);
        }
    }

    //帧同步的逻辑帧调用
    public virtual void NpcFrameSynLogicUpdate()
    {
        if (!this.IsEnableState())
        {
            return;
        }

        if (this.IsPauseState())
        {
            return;
        }

        // 飞出地图复位
        if (this.myModel.transform.position.y > 80 || this.myModel.transform.position.y < -20)
        {
            this.myModel.transform.position = new Vector3(0, 0, 0);
            this.myModel.rigidbody.velocity = new Vector3(0, 0, 0);
            this.myModel.rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }

        if (!this.isDead)
        {
            // 如果Npc没有设置Ai行为树，则在这里刷新技能控制器
            if (this.myBehaviacController.aiName == "")
            {
                this.mySkillController.Update();
            }

            this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
                {
                    pPart.FmSynLogicUpdate();
                }
            );
        }
    }

    /// <summary>
    /// 排队刷帧
    /// </summary>
    public virtual void OnQueueNpcFrameSynLogicUpdate()
    {
        // 如果当前地图是战斗地图，用帧同步Update推进Npc行为树
        if (MapManager.Instance.baseMap.mapType == MapType.Battle)
        {
            // 如果Npc有设置Ai行为树，则在这里刷新技能控制器
            if (this.myBehaviacController.aiName != "")
            {
                // 如果Npc由行为树控制，则技能相关刷新的次数和时机与行为树一致，且必须在刷行为树之前
                this.mySkillController.Update();
            }

            this.myBehaviacController.UpdateBehaviacTree();
        }
    }

    //帧同步的渲染帧调用
    public virtual void NpcFrameSynRenderUpdate(Fix64 interpolation)
    {
        if (!this.IsEnableState())
        {
            return;
        }

        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                pPart.FmSynRenderUpdate(interpolation);
            }
        );
    }
    //----------------------------------------------------------------------------------------
    //Npc状态相关代码

    /// <summary>
    /// Npc启用状态【为false时：Npc模型不可见、并且所有Update都不会调用、不能发送帧同步操作、不能进行战斗操作、不显示头顶血条】
    /// </summary>
    private bool _enableState = true;
    /// <summary>
    /// 获取Npc启用状态
    /// </summary>
    /// <returns></returns>
    public bool IsEnableState() { return this._enableState; }

    /// <summary>
    /// 设置Npc启用状态
    /// </summary>
    /// <param name="bEnableState"></param>
    public void SetEnableState(bool bEnableState)
    {
        if (this.IsEnableState() == bEnableState)
        {
            return;
        }

        this._enableState = bEnableState;
        this.myModel.gameObject.SetActive(bEnableState);
        this.NpcEnableStateChange(bEnableState);
        OnNpcUpdateLua();
    }

    // Npc启用状态改变调用
    public void NpcEnableStateChange(bool bEnableState)
    {
        // 通知配件Npc启用状态改变
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                pPart.OnNpcEnableStateChange(bEnableState);
            }
        );
    }

    /// <summary>
    /// Npc战斗状态【为false时：不能进行战斗操作】
    /// </summary>
    private bool _combatState = true;
    /// <summary>
    /// 获取Npc战斗状态
    /// </summary>
    /// <returns></returns>
    public bool IsCombatState() { return this._combatState; }
    /// <summary>
    /// 设置Npc战斗状态
    /// </summary>
    /// <param name="bEnable"></param>
    public void SetCombatState(bool bCombatState)
    {
        if (this.IsCombatState() == bCombatState)
        {
            return;
        }
        this._combatState = bCombatState;
    }

    /// <summary>
    /// Npc暂停状态【为true时：非帧同步Update被调用、帧同步逻辑和渲染Update不调用、不能发送帧同步操作】
    /// </summary>
    private bool _pauseState = false;

    /// <summary>
    /// 获取Npc暂停状态
    /// </summary>
    /// <returns></returns>
    public bool IsPauseState() { return this._pauseState; }

    /// <summary>
    /// 设置Npc暂停状态
    /// </summary>
    /// <param name="bEnable"></param>
    public void SetPauseState(bool bPauseState)
    {
        if (this.IsPauseState() == bPauseState)
        {
            return;
        }
        this._pauseState = bPauseState;
    }

    //----------------------------------------------------------------------------------------
    //战斗相关代码

    // Npc初始属性数据
    public AttributeEntity attrEntity { get; protected set; } = new AttributeEntity();

    /// <summary>
    /// Npc初始血量
    /// </summary>
    /// <value></value>
    public int initHp
    {
        get
        {
            return this.attrEntity.GetBaseValue(AttributeType.Hp);
        }
    }

    /// <summary>
    /// Npc剩余血量
    /// </summary>
    /// <value></value>
    public int lastHp { get; protected set; } = -1;

    /// <summary>
    /// Npc是否死亡
    /// </summary>
    /// <value></value>
    public virtual bool isDead
    {
        get
        {
            return this.lastHp == 0;
        }
    }

    /// <summary>
    /// 计算玩家属性
    /// </summary>
    public virtual void NpcAttrCalculate()
    {
        // 清理上次计算的属性
        foreach(var kvPair in this.attrEntity.attrCollection)
        {
            this.attrEntity.SetBaseValue(kvPair.Key, 0);
        }

        int nInitHp = 0;

        // 重新计算当前的属性
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                foreach(var kvPair in pPart.attrEntityCfg.attrCollection)
                {
                    if (kvPair.Key == AttributeType.Hp)
                    {
                        nInitHp = nInitHp + kvPair.Value.GetValue();
                    }
                    else
                    {
                        this.attrEntity.ModfiyBaseValue(kvPair.Key, kvPair.Value.GetValue());
                    }
                }
            }
        );

        if (nInitHp != 0)
        {
            if (nInitHp > this.initHp)
            {
                int nAddHp = nInitHp - this.initHp;
                int nNewLastHp = (this.lastHp == -1 ? 0 : this.lastHp) + nAddHp;
                this.lastHp = nNewLastHp > nInitHp ? nInitHp : nNewLastHp;
            }
            else if (nInitHp < this.initHp)
            {
                int nDecHp = this.initHp - nInitHp;
                this.lastHp = this.lastHp > nInitHp ? nInitHp : this.lastHp;
            }
            this.attrEntity.SetBaseValue(AttributeType.Hp, nInitHp);
        }
        else
        {
            this.attrEntity.SetBaseValue(AttributeType.Hp, -1);
            this.lastHp = -1;
        }
    }

    /// <summary>
    /// 获取Npc指定类型属性
    /// </summary>
    /// <param name="eType"></param>
    /// <returns></returns>
    public int GetNpcAttr(AttributeType eType)
    {
        return this.attrEntity.GetBaseValue(eType);
    }

    /// <summary>
    /// 伤害输出
    /// </summary>
    /// <param name="myElement"></param>
    /// <param name="pTargetElement"></param>
    /// <param name="nDamage"></param>
    public virtual void DamageOutput(RobotPartElement pMyElement, RobotPartElement pTargetElement, int nDamage)
    {
        if (pTargetElement == null || pTargetElement.isDead)
        {
            return;
        }

        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(pTargetElement.myRobotPart.npcInstId);
        if (pNpc == null)
        {
            return;
        }

        if (this.InstId == pNpc.InstId)
        {
            return;
        }

        if (this.type == pNpc.type)
        {
            return;
        }

        if (!pNpc.IsCombatState())
        {
            return;
        }

        pNpc.NpcDamageInput(pTargetElement, nDamage);
    }

    /// <summary>
    /// Npc伤害输入（通过对Npc配件的零件造成伤害而导致Npc也受到伤害）
    /// </summary>
    /// <param name="pMyElement"></param>
    /// <param name="nDamage"></param>
    public virtual void NpcDamageInput(RobotPartElement pMyElement, int nDamage)
    {
        if (pMyElement == null || pMyElement.isDead)
        {
            return;
        }

        if (!this.IsCombatState())
        {
            return;
        }
        
        int nNewDamage = pMyElement.RobotPartElementDamageInputFilter(nDamage);
        // 如果pMyElement不是车体，则对其输入伤害
        if (pMyElement.myRobotPart.partType != RobotPartType.Body)
        {
            pMyElement.RobotPartElementDamageInput(nNewDamage);
        }

        this.NpcDamageInput(nNewDamage);
    }

    // Npc伤害输入（直接对Npc的血量造成伤害）
    public virtual void NpcDamageInput(int nDamage)
    {
        if (!this.IsCombatState())
        {
            return;
        }

        int nFixDamage = nDamage < 0 ? 0 : nDamage;

        // 设置Npc血量
        int nNewHp = this.lastHp - nFixDamage;
        this.lastHp = nNewHp >= 0 ? nNewHp : 0;

        // 向Npc的所有配件发送Npc伤害输入通知
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
        {
            pPart.OnNpcDamageInput();
        });

        if (this.isDead)
        {
            this.Dead();
        }
    }

    /// <summary>
    /// Npc治疗输入（固定值）
    /// </summary>
    /// <param name="nTreatmentVal">治疗值</param>
    public virtual void NpcTreatmentInput(int nTreatmentVal)
    {
        if (this.isDead)
        {
            return;
        }

        if (!this.IsCombatState())
        {
            return;
        }

        int nFixTreatmentVal = nTreatmentVal < 0 ? 0 : nTreatmentVal;

        Dictionary<bool, Dictionary<RobotPartType, List<RobotPartElement>>> pRandomAllPool =
            new Dictionary<bool, Dictionary<RobotPartType, List<RobotPartElement>>>();

        // 向Npc的所有配件发送Npc治疗输入通知
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                pPart.OnNpcTreatmentInput();

                RobotPartType eType = pPart.partType;
                if (eType != RobotPartType.Body)
                {
                    foreach (var pElement in pPart.myElementList)
                    {
                        // 已经满血的不加
                        if (pElement.lastDurability != pElement.initDurability)
                        {
                            bool bIsDead = pElement.isDead;
                            if (!pRandomAllPool.ContainsKey(bIsDead))
                            {
                                pRandomAllPool.Add(bIsDead, new Dictionary<RobotPartType, List<RobotPartElement>>());
                            }

                            if (!pRandomAllPool[bIsDead].ContainsKey(eType))
                            {
                                pRandomAllPool[bIsDead].Add(eType, new List<RobotPartElement>());
                            }

                            pRandomAllPool[bIsDead][eType].Add(pElement);
                        }
                    }
                }
            }
        );

        // 设置Npc血量
        int nNewHp = this.lastHp + nFixTreatmentVal;
        this.lastHp = nNewHp > this.initHp ? this.initHp : nNewHp;

        if (pRandomAllPool.Count > 0)
        {
            Dictionary<RobotPartType, List<RobotPartElement>> pRandomPool;
            if (pRandomAllPool.ContainsKey(true))
            {
                pRandomPool = pRandomAllPool[true];
            }
            else
            {
                pRandomPool = pRandomAllPool[false];
            }

            List<RobotPartElement> pPool;
            if (pRandomPool.ContainsKey(RobotPartType.Weapon))
            {
                pPool = pRandomPool[RobotPartType.Weapon];
            }
            else if (pRandomPool.ContainsKey(RobotPartType.Move))
            {
                pPool = pRandomPool[RobotPartType.Move];
            }
            else if (pRandomPool.ContainsKey(RobotPartType.Assist))
            {
                pPool = pRandomPool[RobotPartType.Assist];
            }
            else
            {
                pPool = pRandomPool[RobotPartType.Ornament];
            }

            int nRandomIdx = SeedRandom.instance.Next(0, pPool.Count);
            RobotPartElement pRandomElement = pPool[nRandomIdx];
            pRandomElement.RobotPartElementTreatmentInput(nFixTreatmentVal);
        }
    }

    /// <summary>
    /// Npc治疗输入（百分比）
    /// </summary>
    /// <param name="nTreatmentPercentHp"></param>
    public virtual void NpcTreatmentPercentInput(int nTreatmentPercentHp)
    {
        this.NpcTreatmentInput(this.initHp * nTreatmentPercentHp / 100);
    }

    //死亡
    public virtual void Dead()
    {
        // 通知Npc死亡事件
        MapManager.Instance.baseMap.OnNpcDead(this.InstId);
        this.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Npc_OnDead);

        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
        {
            // 向Npc的所有配件发送Npc死亡通知
            pPart.OnNpcDead();
        });

        this.myBuffController.OnNpcDead();

        this.myModel.rigidbody.ResetCenterOfMass();

        // 清理Npc的Layer
        this.NpcLayerClear();
    }

    // -----------------------------------------------------------------
    
    public bool _bAttrack = true;

    public virtual bool IsAutoSkill(){
        
        if(!_bAttrack) return false;
        
        if (this.isDead)
        {
            return false;
        }

        return true;
    }

    public void DoNormalAttack()
    {
        if(!_bAttrack) return;
    }

    //----------------------------------------------------------------------------------------
    // 配件功能相关代码

    /// <summary>
    /// 保存Npc所有槽位的容器
    /// </summary>
    /// <typeparam name="int">槽位索引</typeparam>
    /// <typeparam name="RobotPartHole">槽位实例</typeparam>
    /// <returns></returns>
    private Dictionary<int, RobotPartHole> _robotPartHoleContainer = new Dictionary<int, RobotPartHole>();

    /// <summary>
    /// 移动配件的槽位索引
    /// </summary>
    private int _robotPartMoveHoleIdx = -1;

    /// <summary>
    /// 获取Npc的移动配件
    /// </summary>
    /// <returns></returns>
    public RobotPart GetRobotMovePart()
    {
        if (this._robotPartMoveHoleIdx == -1)
        {
            return null;
        }

        RobotPartHole pHole = this.GetRobotPartHole(this._robotPartMoveHoleIdx);
        if (pHole == null)
        {
            Debug.LogError("Error->NpcBase.GetRobotMovePart->pHole == null!");
            return null;
        }

        RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(pHole.partInstId);
        if (pPart == null)
        {
            Debug.LogError("Error->NpcBase.GetRobotMovePart->pPart == null!");
            return null;
        }

        return pPart;
    }

    /// <summary>
    /// 获取Npc的车体配件
    /// </summary>
    public RobotPart GetRobotBodyPart()
    {
        RobotPartHole pBodyHole = this.GetRobotPartHole(0);
        if (pBodyHole.partInstId == 0)
        {
            return null;
        }

        return MapManager.Instance.baseMap.GetRobotPart(pBodyHole.partInstId);
    }

    /// <summary>
    /// 获取槽位信息容器
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, RobotPartHole> GetRobotPartHoleContainer()
    {
        return this._robotPartHoleContainer;
    }

    /// <summary>
    /// 获取指定槽位索引的槽位数据
    /// </summary>
    /// <param name="nHoleIdx"></param>
    /// <returns></returns>
    public RobotPartHole GetRobotPartHole(int nHoleIdx)
    {
        if (!this._robotPartHoleContainer.ContainsKey(nHoleIdx))
        {
            return null;
        }
        return this._robotPartHoleContainer[nHoleIdx];
    }

    /// <summary>
    /// 根据配件类型获取槽位索引
    /// </summary>
    /// <param name="robotPartType">配件类型</param>
    /// <returns></returns>
    // public int GetVoidRobotPartHoleIndex(RobotPartType robotPartType)
    // {
    //     foreach (var item in this._robotPartHoleContainer)
    //     {
    //         RobotPartHole pHole = item.Value;
    //         if (pHole.type == robotPartType && pHole.partInstId == 0)
    //         {
    //             return item.Key;
    //         }
    //     }
    //     return -1;
    // }

    /// <summary>
    /// 根据配件类型获取槽位索引
    /// </summary>
    /// <param name="robotPartType">配件类型</param>
    /// <returns></returns>
    public int GetHolePassPartType(RobotPartType robotPartType)
    {
        foreach (var item in this._robotPartHoleContainer)
        {
            RobotPartHole pHole = item.Value;
            if (pHole.partType == robotPartType)
            {
                return item.Key;
            }

        }
        return -1;
    }

    /// <summary>
    /// 通过指定类型获取NPC所有符合的槽位
    /// </summary>
    /// <param name="robotPartType">槽位类型</param>
    /// <returns></returns>
    public List<int> GetAllHoleByType(RobotPartType robotPartType)
    {
        List<int> allholePosition = new List<int>();
        foreach (var item in this._robotPartHoleContainer)
        {
            RobotPartHole pHole = item.Value;
            if (pHole.partType == robotPartType)
            {
                //JArray arrPosition=RobotPartHole.GetRobotPartHoleRandomCoord(item.Key);
                allholePosition.Add(item.Key);
            }

        }
        return allholePosition;
    }
    /// <summary>
    /// 是否有指定槽位索引的槽位
    /// </summary>
    /// <param name="nHoleIdx">槽位索引</param>
    /// <returns></returns>
    public bool IsHaveRobotPartHole(int nHoleIdx)
    {
        if (this._robotPartHoleContainer.ContainsKey(nHoleIdx))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// /// 添加一个指定槽位索引的槽位
    /// </summary>
    /// <param name="nHoleIdx"></param>
    /// <param name="nCfgId"></param>
    public void RobotrPartHoleCreate(int nHoleIdx, int nCfgId, Action fLoaded = null)
    {
        this._robotPartHoleContainer[nHoleIdx] = new RobotPartHole(this, nHoleIdx);
        this._robotPartHoleContainer[nHoleIdx].Init(nCfgId, 1f, delegate (RobotPartHole robotPartHole)
        {
            this._robotPartHoleContainer[nHoleIdx] = robotPartHole;

            foreach (var pModel in robotPartHole.myModelList)
            {
                this.NpcChildLayerSet(pModel.transform);
            }

            fLoaded?.Invoke();
        });
    }

    /// <summary>
    /// 移除指定槽位索引的槽位
    /// </summary>
    /// <param name="nHoleIdx"></param>
    public void RobotrPartHoleRemove(int nHoleIdx)
    {
        if (!this.IsHaveRobotPartHole(nHoleIdx))
        {
            return;
        }
        var pHole = this._robotPartHoleContainer[nHoleIdx];
        var nOldPartInstId = pHole.partInstId;
        if (nOldPartInstId > 0)
        {
            this.RobotPartUnInstall(nHoleIdx);
        }
        this._robotPartHoleContainer.Remove(nHoleIdx);
        pHole.Destroy();
    }

    /// <summary>
    /// Npc配件遍历方法
    /// </summary>
    /// <param name="fAction"></param>
    public void RobotPartLoop(Action<int, RobotPart> fAction)
    {
        var pContainer = this.GetRobotPartHoleContainer();
        for (int i = 0; i < pContainer.Count; i++)
        {
            var nPartInstId = pContainer[i].partInstId;
            // 槽位中是否有配件
            if (nPartInstId != 0)
            {
                // 槽位中的配件是否有效
                RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
                if (pPart != null)
                {
                    fAction.Invoke(i, pPart);
                }
            }
        }
    }

    /// <summary>
    /// 将配件安装到Npc的配件槽位中
    /// </summary>
    /// <param name="nHoleIdx">配件槽位索引</param>
    /// <param name="nPartInstId">配件实例Id</param>
    /// <returns>是否成功</returns>
    public bool RobotPartInstall(int nHoleIdx, int nPartInstId, Action fLoaded = null)
    {
        RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);
        // 配件不存在
        if (pPart == null)
        {
            return false;
        }

        // 没有指定槽位
        if (!this.IsHaveRobotPartHole(nHoleIdx))
        {
            return false;
        }

        // 槽位类型与配件类型不匹配
        RobotPartType eType = this.GetRobotPartHole(nHoleIdx).partType;
        RobotPartType pPartType = pPart.partType;
        if (pPartType != eType)
        {
            return false;
        }

        // 如果槽位上已经装了配件，则执行卸载
        int nInstId = this.GetRobotPartHole(nHoleIdx).partInstId;
        if (nInstId > 0)
        {
            this.RobotPartUnInstall(nHoleIdx);
        }

        // 如果是移动配件，则记录其槽位
        if (pPart.partType == RobotPartType.Move)
        {
            this._robotPartMoveHoleIdx = nHoleIdx;
        }

        // 槽位放入配件
        this.GetRobotPartHole(nHoleIdx).RobotPartInsert(pPart);

        // 重设Npc的质量中心
        this.myModel.rigidbody.centerOfMass = Vector3.zero;

        // Npc属性计算
        this.NpcAttrCalculate();

        // 配件安装通知
        pPart.OnInstall(fLoaded);

        return true;
    }

    /// <summary>
    /// 将Npc配件槽位上的配件卸下
    /// </summary>
    /// <param name="nHoleIdx">槽位索引</param>
    /// <returns></returns>
    public bool RobotPartUnInstall(int nHoleIdx)
    {
        if (!this.IsHaveRobotPartHole(nHoleIdx))
        {
            return false;
        }

        RobotPartHole pHole = this.GetRobotPartHole(nHoleIdx);
        int nPartInstId = pHole.partInstId;
        if (nPartInstId == 0)
        {
            return false;
        }

        RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(nPartInstId);

        // 配件卸载通知
        pPart.OnUnInstall();

        // 如果是移动配件，则清空记录
        if (pPart.partType == RobotPartType.Move)
        {
            this._robotPartMoveHoleIdx = -1;
        }

        // 槽位移出配件
        pHole.RobotPartRemove();

        // 销毁配件
        MapManager.Instance.baseMap.RobotPartDestroy(nPartInstId);

        // 重设Npc的质量中心
        this.myModel.rigidbody.centerOfMass = Vector3.zero;

        // Npc属性计算
        this.NpcAttrCalculate();

        return true;
    }

    /// <summary>
    /// 初始化Npc配件
    /// </summary>
    /// <param name="jPartInfo"></param>
    /// <param name="fLoadDone">全部加载完成回调</param>
    public void RobotPartInit(JArray jPartInfo, Action fLoadDone = null)
    {
        if (jPartInfo != null && jPartInfo.Count > 0)
        {
            Queue<int> pLoadList = new Queue<int>();
            for (int i = 0; i < jPartInfo.Count; i++)
            {
                pLoadList.Enqueue((int)jPartInfo[i]);
            }
            this.RobotPartLoopLoad(pLoadList, 0, fLoadDone);
        }
        else
        {
            fLoadDone?.Invoke();
        }
    }

    /// <summary>
    /// 递归加载配件
    /// </summary>
    /// <param name="pLoadList">要加载的配件队列</param>
    /// <param name="fLoadDone">全部加载完成回调</param>
    private void RobotPartLoopLoad(Queue<int> pLoadList, int nIdx, Action fLoadDone = null)
    {
        // 如果配件配置Id的队列里已经没有了，则返回
        if (pLoadList.Count <= 0)
        {
            fLoadDone?.Invoke();
            return;
        }

        // 如果没有指定的孔，则返回
        if (!this.IsHaveRobotPartHole(nIdx))
        {
            fLoadDone?.Invoke();
            return;
        }

        // 如果配件配置Id是无效的，则递归下一个
        int nCfgId = pLoadList.Dequeue();
        if (nCfgId <= 0)
        {
            this.RobotPartLoopLoad(pLoadList, nIdx + 1, fLoadDone);
            return;
        }

        // 如果要安装的配件配置Id对应的配件类型与槽位不匹配，则报错并递归下一个
        RobotPartHole pHole = GetRobotPartHole(nIdx);
        RobotPartType robotPartType = RobotPart.GetRobotPartType(nCfgId);
        if (pHole.partType != RobotPart.GetRobotPartType(nCfgId))
        {
            Debug.LogError("Error->NpcBase.RobotPartLoopLoad->Hole Type And Part Type Is Not Equal!" + "#NpcCfgId = " + this.cfgId + "#nIdx = " + nIdx + "#nCfgId = " + nCfgId);
            this.RobotPartLoopLoad(pLoadList, nIdx + 1, fLoadDone);
            return;
        }

        // 实例化配件并安装，然后递归下一个
        int nNum = pHole.myModelList.Count;
        MapManager.Instance.baseMap.RobotPartCreate(nCfgId, nNum, this.myScale, delegate (RobotPart pPart)
            {
                this.RobotPartInstall(nIdx, pPart.InstId, delegate ()
                    {
                        this.RobotPartLoopLoad(pLoadList, nIdx + 1, fLoadDone);
                    }
                );
            }
        );
    }

    /// <summary>
    /// Npc初始化完成调用
    /// </summary>
    public void OnNpcStartCall()
    {
        // this.InitAIController();//初始化NpcAI功能
        this.isPartInitEnd = true;
        this.OnNpcStart();
        this.myBehaviacController.DispatchGameEventToAi(BehaviacGameEvent.Npc_OnStart);
    }

    /// <summary>
    /// Npc初始化完成子类型通知
    /// </summary>
    public virtual void OnNpcStart() { }

    /// <summary>
    /// 获取指定类型的所有零件
    /// </summary>
    public List<RobotPartElement> GetRobotPartElement(RobotPartType eType)
    {
        List<RobotPartElement> listElement = new List<RobotPartElement>();
        this.RobotPartLoop(delegate (int nHoleIdx, RobotPart pPart)
            {
                if (pPart.partType == eType)
                {
                    foreach (var element in pPart.myElementList)
                    {
                        listElement.Add(element);
                    }
                }
            }
        );
        return listElement;
    }

    /// <summary>
    /// Npc配件克隆
    /// </summary>
    /// <param name="pOriginalNpc">源Npc</param>
    /// <param name="pTargetNpc">目标Npc</param>
    /// <param name="fCloneEnd">克隆完成回调</param>
    public static void NpcRobotPartClone(BaseNpc pOriginalNpc, BaseNpc pTargetNpc, Action fCloneEnd = null)
    {
        NpcRobotPartClone(pOriginalNpc, pTargetNpc, fCloneEnd, 0);
    }

    /// <summary>
    /// Npc配件克隆递归
    /// </summary>
    /// <param name="pOriginalNpc">源Npc</param>
    /// <param name="pTargetNpc">目标Npc</param>
    /// <param name="fCloneEnd">克隆完成回调</param>
    /// <param name="nHoleIdx">配件槽索引</param>
    private static void NpcRobotPartClone(BaseNpc pOriginalNpc, BaseNpc pTargetNpc, Action fCloneEnd, int nHoleIdx)
    {
        int nNextHoleIdx = nHoleIdx + 1;
        BaseMap pMap = MapManager.Instance.baseMap;

        if (pOriginalNpc.IsHaveRobotPartHole(nHoleIdx))
        {
            RobotPartHole pOriginalHole = pOriginalNpc.GetRobotPartHole(nHoleIdx);
            RobotPartHole pTargetHole = pTargetNpc.GetRobotPartHole(nHoleIdx);
            if (pOriginalHole.partInstId == 0)
            {
                if (pTargetHole.partInstId != 0)
                {
                    int nPartInstId = pTargetHole.partInstId;
                    pTargetNpc.RobotPartUnInstall(nHoleIdx);
                    pMap.RobotPartDestroy(nPartInstId);
                }
                NpcRobotPartClone(pOriginalNpc, pTargetNpc, fCloneEnd, nNextHoleIdx);
            }
            else
            {
                RobotPart pOriginalPart = pMap.GetRobotPart(pOriginalHole.partInstId);
                if (pTargetHole.partInstId != 0)
                {
                    RobotPart pTargetPart = pMap.GetRobotPart(pOriginalHole.partInstId);
                    if (pOriginalPart.cfgId == pTargetPart.cfgId)
                    {
                        NpcRobotPartClone(pOriginalNpc, pTargetNpc, fCloneEnd, nNextHoleIdx);
                        return;
                    }
                    else
                    {
                        int nPartInstId = pTargetHole.partInstId;
                        pTargetNpc.RobotPartUnInstall(nHoleIdx);
                        pMap.RobotPartDestroy(nPartInstId);
                    }
                }

                int nNum = pOriginalHole.myModelList.Count;
                MapManager.Instance.baseMap.RobotPartCreate(pOriginalPart.cfgId, nNum, pTargetNpc.myScale, delegate (RobotPart pPart)
                    {
                        pTargetNpc.RobotPartInstall(nHoleIdx, pPart.InstId, delegate ()
                            {
                                NpcRobotPartClone(pOriginalNpc, pTargetNpc, fCloneEnd, nNextHoleIdx);
                            }
                        );
                    }
                );
            }
        }
        else
        {
            fCloneEnd?.Invoke();
        }
    }

    //----------------------------------------------------------------------------------------
    // Npc的Layer层相关代码
    public string myLayerName { get; protected set; } = "Obstacle";

    public virtual void NpcLayerSet() { }
    public virtual void NpcLayerClear() { }

    /// <summary>
    /// Npc获取并占用闲置的LayerName
    /// </summary>
    /// <param name="pNpc"></param>
    public void NpcLayerNameOccupy()
    {
        if (this.myLayerName != "Obstacle")
        {
            Debug.LogError("Error->BaseMap.NpcLayerNameOccupy->Npc Have Layer! -> " + "#NpcObjName = " + this.myModel.gameObject.name + "#NpcLayerName = " + LayerMask.LayerToName(this.myModel.gameObject.layer));
            return;
        }

        this.myLayerName = MapManager.Instance.baseMap.NpcLayerNameContainerPop(this.type);
        this.NpcChildLayerSet(this.myModel.transform);
    }

    /// <summary>
    /// Npc释放自己的LayerName
    /// </summary>
    /// <param name="pNpc"></param>
    public void NpcLayerNameRelease()
    {
        if (this.myLayerName == "Obstacle")
        {
            Debug.LogError("Error->BaseMap.NpcLayerNameRelease->Layer Is Default! -> " + "#NpcObjName = " + this.myModel.gameObject.name);
            return;
        }

        MapManager.Instance.baseMap.NpcLayerNameContainerPush(this.type, this.myLayerName);
        this.myLayerName = "Obstacle";
        this.NpcChildLayerClear(this.myModel.transform);
    }

    public void NpcChildLayerSet(Transform pTransform)
    {
        int nLayerId = LayerMask.NameToLayer(this.myLayerName);
        foreach (var pChildTransform in pTransform.GetComponentsInChildren<Transform>(true))
        {
            pChildTransform.gameObject.layer = nLayerId;
        }
    }

    public void NpcChildLayerClear(Transform pTransform)
    {
        int nLayerId = LayerMask.NameToLayer("Obstacle");
        foreach (var pChildTransform in pTransform.GetComponentsInChildren<Transform>(true))
        {
            pChildTransform.gameObject.layer = nLayerId;
        }
    }

    //----------------------------------------------------------------------------------------
    // Npc行为相关代码

    /// <summary>
    /// 出生点坐标
    /// </summary>
    /// <value></value>
    public Vector3 spawnPositon { get; private set; }

    /// <summary>
    /// 获取Npc朝向某坐标的转向角度
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public float GetTurnAngle(Vector3 pPos)
    {
        Vector3 dis = pPos - this.myModel.transform.position;
        float angle = Vector3.Angle(this.myModel.transform.forward, dis);
        Vector3 c = Vector3.Cross(this.myModel.transform.forward, dis.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }

    /// <summary>
    /// 是否能到达指定坐标
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsCanMoveToPos(Vector3 pPos)
    {
        Vector3[] points = RobotFindPath.FindPath(this.myModel.transform.position, pPos);
        if(points != null && points.Length > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移动到指定坐标
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns>是否成功</returns>
    public bool MoveToPos(Vector3 pPos)
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        return pPart.MovePart_MoveToPos(pPos);
    }

    /// <summary>
    /// 获取以Npc为中心的指定角度指定距离的坐标
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public Vector3 GetPosByAngle(float nAngle, float nDistance)
    {
        Vector3 pV3 = Quaternion.Euler(0, nAngle, 0) * this.myModel.transform.forward;
        return this.myModel.transform.position + pV3.normalized * nDistance;
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns>是否成功</returns>
    public bool MoveToPlace(float nAngle, float nDistance)
    {
        Vector3 pPos = this.GetPosByAngle(nAngle, nDistance);
        return this.MoveToPos(pPos);
    }

    /// <summary>
    /// 移动到指定点
    /// </summary>
    /// <param name="sPointName"></param>
    public bool MoveToPoint(string sPointName)
    {
        LevelPointScript pPoint = MapManager.Instance.baseMap.levelPointManager.GetLevelPoint(sPointName);
        return this.MoveToPos(pPoint.transform.position);
    }

    /// <summary>
    /// 移动到指定Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
    public bool MoveToNpc(int nNpcInstId)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        return this.MoveToPos(pNpc.myModel.transform.position);
    }

    /// <summary>
    /// 移动到玩家
    /// </summary>
    /// <returns></returns>
    public bool MoveToPlayer()
    {
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }
        return this.MoveToPos(pPlayerNpc.myModel.transform.position);
    }

    /// <summary>
    /// 移动到出生点
    /// </summary>
    public bool MoveToSpawnPositon()
    {
        return this.MoveToPos(this.spawnPositon);
    }

    /// <summary>
    /// 向目标Npc移动一定的距离
    /// </summary>
    /// <returns></returns>
    public bool MoveToNpcByDistance(int nNpcInstId, int nDistance)
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        float nAngle = this.GetTurnAngle(pNpc.myModel.transform.position);
        Vector3 pPos = this.GetPosByAngle(nAngle, nDistance);
        return this.MoveToPos(pPos);
    }

    /// <summary>
    /// 移动到与目标Npc连线后某夹角某距离的坐标
    /// </summary>
    /// <returns></returns>
    public bool MoveToRelativeNpcDirect(int nNpcInstId, int nAngle, int nDistance)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        float nCurAngle = this.GetTurnAngle(pNpc.myModel.transform.position);
        float nTargetAngle = nCurAngle + nAngle >= 360 ? nCurAngle + nAngle - 360 : nCurAngle + nAngle;
        return this.MoveToPlace(nTargetAngle, nDistance);
    }

    /// <summary>
    /// 向指定角度移动
    /// </summary>
    public bool MoveToAngle(int nAngle)
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        pPart.MovePart_MoveToAngle(nAngle);
        return true;
    }

    /// <summary>
    /// 移动停止
    /// </summary>
    public void MoveStop()
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return;
        }

        pPart.MovePart_MoveStop();
    }

    /// <summary>
    /// 是否到达目标点
    /// </summary>
    /// <returns></returns>
    public bool IsMoveReach()
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        return pPart.MovePart_IsMoveReach();
    }

    /// <summary>
    /// 转向指定坐标
    /// </summary>
    /// <param name="pPos"></param>
    public bool TurnToPos(Vector3 pPos)
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        pPart.MovePart_TurnToPos(pPos);

        return true;
    }

    /// <summary>
    /// 转向指定角度
    /// </summary>
    /// <param name="nAngle"></param>
    /// <param name="nDistance"></param>
    /// <returns></returns>
    public bool TurnToPlace(float nAngle)
    {
        Vector3 pPos = this.GetPosByAngle(nAngle, 1f);
        return this.TurnToPos(pPos);
    }

    /// <summary>
    /// 转向某点
    /// </summary>
    /// <param name="sPointName"></param>
    /// <returns></returns>
    public bool TurnToPoint(string sPointName)
    {
        LevelPointScript pPoint = MapManager.Instance.baseMap.levelPointManager.GetLevelPoint(sPointName);
        return this.TurnToPos(pPoint.transform.position);
    }

    /// <summary>
    /// 转向Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <returns></returns>
    public bool TurnToNpc(int nNpcInstId)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        return this.TurnToPos(pNpc.myModel.transform.position);
    }

    /// <summary>
    /// 转向玩家
    /// </summary>
    /// <returns></returns>
    public bool TurnToPlayer()
    {
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }

        return this.TurnToPos(pPlayerNpc.myModel.transform.position);
    }

    /// <summary>
    /// 转向出生点
    /// </summary>
    /// <returns></returns>
    public bool TurnToSpawnPositon()
    {
        return this.TurnToPos(this.spawnPositon);
    }

    /// <summary>
    /// 转向停止
    /// </summary>
    public void TurnStop()
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return;
        }

        pPart.MovePart_TurnStop();
    }

    /// <summary>
    /// 转向是否到达
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsTurnReach()
    {
        RobotPart pPart = this.GetRobotMovePart();
        if (pPart == null)
        {
            return false;
        }

        return pPart.MovePart_IsTurnReach();
    }

    /// <summary>
    /// 是否朝向指定点
    /// </summary>
    /// <param name="pPos"></param>
    /// <param name="nVague"></param>
    /// <returns></returns>
    public bool IsLookAtPos(Vector3 pPos, int nVague = 0)
    {
        float nAngle = this.GetTurnAngle(pPos);
        if (nVague >= nAngle || nAngle > 360 - nVague)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否朝向指定Npc
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nVague"></param>
    /// <returns></returns>
    public bool IsLookAtNpc(int nNpcInstId, int nVague = 0)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        return this.IsLookAtPos(pNpc.myModel.transform.position, nVague);
    }

    /// <summary>
    /// 某点是否在指定范围内
    /// </summary>
    /// <param name="pPos"></param>
    /// <returns></returns>
    public bool IsInRangeOfPos(Vector3 pPos, int nRange)
    {
        Vector3 pDis = this.myModel.transform.position - pPos;
        bool bRet = (nRange * nRange) >= Vector3.SqrMagnitude(pDis);
        return bRet;
    }

    /// <summary>
    /// Npc是否在指定范围内
    /// </summary>
    /// <returns></returns>
    public bool IsInRangeOfNpc(int nNpcInstId, int nRange)
    {
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
        if (pNpc == null)
        {
            return false;
        }

        return this.IsInRangeOfPos(pNpc.myModel.transform.position, nRange);
    }

    /// <summary>
    /// 玩家Npc是否在指定范围内
    /// </summary>
    /// <returns></returns>
    public bool IsInRangeOfPlayerNpc(int nRange)
    {
        BaseNpc pPlayerNpc = MyPlayer.player;
        if (pPlayerNpc == null)
        {
            return false;
        }

        if (pPlayerNpc.isDead)
        {
            return false;
        }

        return this.IsInRangeOfNpc(pPlayerNpc.InstId, nRange);
    }

    /// <summary>
    /// 获取敌对Npc
    /// </summary>
    /// <returns></returns>
    public virtual int GetEnemyNpcInView()
    {
        if (!this.IsInRangeOfPlayerNpc(this.myViewRange))
        {
            return 0;
        }

        return MyPlayer.player.InstId;
    }

    //----------------------------------------------------------------------------------------
    // 读表操作

    /// <summary>
    /// 通过Npc配置Id获取Npc类型
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static NpcType GetNpcTypeByCfgId(int nCfgId)
    {
        return (NpcType)ConfigManager.GetValue<int>("Npc_C", nCfgId, "NPCType");
    }

    /// <summary>
    /// 通过Npc配置Id获取怪物类型
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public MonsterType GetMonsterType()
    {
        return (MonsterType)ConfigManager.GetValue<int>("Npc_C", this.cfgId, "monsterType");
    }

    /// <summary>
    /// 通过Npc配置Id获取怪物类型
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static MonsterType GetMonsterTypeByCfgId(int nCfgId)
    {
        return (MonsterType)ConfigManager.GetValue<int>("Npc_C", nCfgId, "monsterType");
    }

    /// <summary>
    /// 通过Npc配置Id获取Npc模型Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetNpcModelId(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Npc_C", nCfgId, "modelId");
    }

    /// <summary>
    /// 通过Npc配置Id获取NpcAi名称
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static string GetNpcAiName(int nCfgId)
    {
        return ConfigManager.GetValue<string>("Npc_C", nCfgId, "strAiName");
    }

    /// <summary>
    /// 通过Npc配置Id获取Npc名字的国际化字符串Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetNpcNameByCfgId(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Npc_C", nCfgId, "NPCName");
    }

    /// <summary>
    /// 获取Npc属性集合
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static JArray GetNpcAttribute(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Npc_C", nCfgId, "NPCAttribute"));
    }

    /// <summary>
    /// 获取Npc视野范围
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetNpcViewRange(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Npc_C", nCfgId, "viewRange");
    }

    //----------------------------------------------------------------------------------------
    
    // 临时代码
    public virtual void SetNpcIsMove(){}
    public virtual void SetNpcIsStand(){}
}