using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

public class BaseMap : CoroutineManager, IMap
{
    /// <summary>
    /// npc配置ID
    /// </summary>
    public int cfgId { get; private set; }

    /// <summary>
    /// 当前关卡Id
    /// </summary>
    /// <value></value>
    public int gameLevelId = 0;

    /// <summary>
    /// 地图类型
    /// </summary>
    /// <value></value>
    public MapType mapType {get; private set;}

    /// <summary>
    /// 地图名称
    /// </summary>
    public string mapName { get; private set; }

    /// <summary>
    /// 是否加载完成
    /// </summary>
    public bool isLoadEnd = false;

    /// <summary>
    /// 地图数据是否已处理完成
    /// </summary>
    /// <value></value>
    public bool isMapDataDisposed{get; private set;} = false;

    /// <summary>
    /// npc实例集合
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="BaseNpc"></typeparam>
    /// <returns></returns>
    public Dictionary<int, BaseNpc> _npcList{get; private set;} = new Dictionary<int, BaseNpc>();

    /// <summary>
    /// npc队列集合
    /// </summary>
    /// <typeparam name="BaseNpc"></typeparam>
    /// <returns></returns>
    public Queue<BaseNpc> _queueNpcList = new Queue<BaseNpc>();

    /// <summary>
    /// 碰撞扩展器
    /// </summary>
    /// <returns></returns>
    public CollisionExtend collisionExtend {get; private set;} = new CollisionExtend();

    /// <summary>
    /// 特效管理器
    /// </summary>
    /// <returns></returns>
    public EffectManager effectManager = new EffectManager();

    /// <summary>
    /// 地图导演的GameObject
    /// </summary>
    private GameObject mapDirectorGameObj;

    /// <summary>
    /// 地图导演行为树控制器
    /// </summary>
    public MapBehaviacController mapDirectorBehaviacController {get; private set;}

    public LevelNpcManager levelNpcManager {get; private set;} = new LevelNpcManager();
    public LevelPointManager levelPointManager {get; private set;} = new LevelPointManager();
    public LevelTrapManager levelTrapManager {get; private set;} = new LevelTrapManager();
    public ObjectPoolController objectPoolController {get; private set;} = new ObjectPoolController();
    public CommTimer myMapTimer = new CommTimer();
    public JArray npcInfoContainer;

    public BaseMap(){}

    public BaseMap(int nCfgId)
    {
        this.cfgId = nCfgId;
        this.mapType = GetMapType(nCfgId);
        // 地图名称
        this.mapName = ConfigManager.GetValue<string>("Scene_C", nCfgId, "strPathName");
        if (this.mapName == "" || this.mapName == "0")
        {
            this.mapName = "VoidScene";
        }
        this.isLoadEnd = false;
    }

    /// <summary>
    /// 地图进入时调用
    /// </summary>
    public virtual void OnEnter()
    {
        if (this.gameLevelId != 0)
        {
            MyPlayer.cameraHeightType = 2;
            // 初始化地图导演
            this.mapDirectorGameObj = new GameObject("MapDirector");
            this.mapDirectorBehaviacController = new MapBehaviacController(mapDirectorGameObj, this);
            BehaviacManager.Start();
            objectPoolController.Start();
        }
        LuaGEvent.DispatchEventToLua(GacEvent.MapOnEnter, this.cfgId);
    }

    /// <summary>
    /// 地图数据处理完成时调用
    /// </summary>
    private void MapDataDisposed()
    {
        this.isMapDataDisposed = true;

        foreach (BaseNpc pNpc in this._npcList.Values)
        {
            if (pNpc.isLoadEnd)
            {
                pNpc.OnMapDataDisposed();
            }
        }

        this.OnEnter();

        // 玩家组装开始
        //this.PlayerAssembleStart();

        // Network.CSharpSend(GacToGas.K_ReqLoadScene, 100);

        if (this.gameLevelId != 0)
        {
            this.MapFrameSynchronStart();
        }

        this.LoadProgressClose();
    }

    public virtual void OnUpdate()
    {
        foreach (BaseNpc pNpc in this._npcList.Values)
        {
            if (pNpc.isLoadEnd)
            {
                pNpc.OnNpcUpdate();
            }
        }
        this.effectManager.UpdateEffect();
    }

    public virtual void OnLateUpdate(){}
    public virtual void OnFixedUpdate(){}

    public virtual void OnExit()
    {
        this.effectManager.Clear();

        // 清理地图导演
        if (this.mapDirectorBehaviacController != null)
        {
            this.mapDirectorBehaviacController.Clear();
        }

        // 清理玩家
        MyPlayer.player = null;
        this.ClearNpcList();
        //_____Temp_____________________
        ///EntitiesPoolManager.instance.OnDestroy();
    }

    /// <summary>
    /// 获取地图所有Npc的容器
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, BaseNpc> GetNpcContainer()
    {
        return this._npcList;
    }

    /// <summary>
    /// 移除npc
    /// </summary>
    public void RemoveNpc(int nInstId)
    {
        if (this._npcList.ContainsKey(nInstId))
        {
            this._npcList[nInstId].NpcDestroy();
            this._npcList.Remove(nInstId);
        }
    }

    /// <summary>
    /// 清空npc
    /// </summary>
    public void ClearNpcList()
    {
        foreach (var npc in this._npcList)
        {
            npc.Value.NpcDestroy();
        }
        this._npcList.Clear();
        this._queueNpcList.Clear();
    }

    /// <summary>
    /// 获取npc
    /// </summary>
    /// <param name="nInstId"></param>
    /// <returns></returns>
    public BaseNpc GetNpc(int nInstId)
    {
        if (this._npcList.ContainsKey(nInstId))
        {
            return this._npcList[nInstId];
        }
        return null;
    }

    /// <summary>
    /// 获取地图中第几个Npc
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public BaseNpc GetNpcByIdx(int nIdx)
    {
        int i = 0;
        foreach(var kvPair in this._npcList)
        {
            if (i == nIdx)
            {
                return kvPair.Value;
            }
            i++;
        }
        return null;
    }

    /// <summary>
    /// 创建npc
    /// </summary>
    /// <param name="nCfgId">Npc配置Id</param>
    /// <param name="pPosition">坐标</param>
    /// <param name="pEulerAngles">转向</param>
    /// <param name="jNpcInfo">Npc数据</param>
    /// <param name="fLoaded">Npc创建完成后回调</param>
    /// <param name="bTemp">是否临时【临时Npc不会加到地图管理中】</param>
    /// <returns></returns>
    public BaseNpc CreatNpc(int nCfgId, Vector3 pPosition, Vector3 pEulerAngles, JArray jNpcInfo, Action<BaseNpc> fLoaded = null)
    {
        NpcType eType;
        int nNpcInstId = 0;

        if (jNpcInfo != null)
        {
            nNpcInstId = (int)jNpcInfo[2];
            if (this._npcList.ContainsKey(nNpcInstId))
            {
                BaseNpc pCurNpc = this.GetNpc(nNpcInstId);
                if (pCurNpc != null) 
                {
                    return pCurNpc;
                }
                else
                {
                    Debug.LogError("BaseMap.CreatNpc->Had Create Npc Is Lost!" + "|nCfgId="+nCfgId+"|jNpcInfo="+jNpcInfo.ToString());
                    return null;
                }
            }
            else
            {
                eType = (NpcType)(int)jNpcInfo[0];
            }
        }
        else
        {
            eType = BaseNpc.GetNpcTypeByCfgId(nCfgId);
        }

        BaseNpc pNewNpc = null;
        switch (eType)
        {
            case NpcType.PlayerNpc:
                pNewNpc = new PlayerNpc(nCfgId);
                break;
            case NpcType.BuildNpc:
                pNewNpc = new BuildNpc(nCfgId);
                break;
            case NpcType.MonsterNpc:
                pNewNpc = new MonsterNpc(nCfgId);
                break;
            default:
                Debug.LogError("Not Define Npc Type!");
                break;
        }

        Action<BaseNpc> fRobotPartInitDone = delegate(BaseNpc pNpc)
            {
                fLoaded?.Invoke(pNpc);
                pNpc.OnNpcStartCall();

                if(pNpc.IsCanAddQueueNpcList){
                    this._queueNpcList.Enqueue(pNpc);
                }
            };

        pNewNpc.LoadNpc(pPosition, pEulerAngles, delegate (BaseNpc pNpc)
            {
                // 将npc加到缓存列表里
                this._npcList.Add(pNpc.InstId, pNpc);
                if (jNpcInfo != null)
                {
                    pNpc.UnBasePackServer(jNpcInfo);

                    pNpc.RobotPartInit((JArray)jNpcInfo[5], delegate ()
                        {
                            fRobotPartInitDone(pNpc);
                        }
                    );
                }
                else
                {
                    // 从配置表中拿配件数据
                    object objPartInfo = ConfigManager.GetValue<object>("Npc_C", nCfgId, "partID");
                    if (objPartInfo != null)
                    {
                        JArray jPartInfo = JArray.FromObject(objPartInfo);
                        pNpc.RobotPartInit(jPartInfo, delegate ()
                            {
                                fRobotPartInitDone(pNpc);
                            }
                        );
                    }
                    else
                    {
                        fRobotPartInitDone(pNpc);
                    }
                }
            }
        );
        return pNewNpc;
    }

    /// <summary>
    /// 地图中Npc死亡通知
    /// </summary>
    /// <param name="nNpcInstId"></param>
    public void OnNpcDead(int nNpcInstId)
    {
        // 通知导演行为树事件
        this.mapDirectorBehaviacController.DispatchGameEvent(BehaviacGameEvent.Map_OnNpcDead, nNpcInstId);
        // 通知地图中所有Npc行为树事件
        foreach(var kvPair in this._npcList)
        {
            kvPair.Value.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Map_OnNpcDead, nNpcInstId);
        }
        
        BaseNpc pNpc = this.GetNpc(nNpcInstId);
        if (pNpc != null)
        {
            if (pNpc.type != NpcType.PlayerNpc)
            {
                // 非玩家Npc死亡清理添加
                this.DeadNpcClearAdd(nNpcInstId);
            }
        }
    }

    /// <summary>
    /// 初始化从服务器传来的地图Npc
    /// </summary>
    public void MapLoadAllNpc(Action fLoadEnd)
    {
        // --------------------------------------------------------
        // 不再使用雪人项目的Npc数据结构，而是重新组织一份
        /*
        {
            [--单个Npc信息
                0->Npc类型,
                1->Npc配置Id,
                2->Npc实例Id,
                3->Npc角色Id,
                4->Npc角色名字
                5->[
                    配件数据集合
                ]
            ]
        }
        */

        JArray jNewAllNpcInfo = new JArray(); // this.npcInfoContainer;

        if (this.gameLevelId != 0)
        {
            jNewAllNpcInfo.Add(new JArray()
                {
                    (int)NpcType.PlayerNpc,
                    101,
                    -1,
                    MyPlayer.roleID,
                    MyPlayer.myName,
                    //new JArray(){}
                    BaseMap.playerAssembleData[this.gameLevelId]
                }
            );
        }

        this.loadProgressType = LoadProgressType.ServerNpc;
        int nTotal = jNewAllNpcInfo.Count;
        int nNpcProgressTotal = this.loadProgressList[LoadProgressType.ServerNpc];

        if (nTotal != 0)
        {
            int nLoadProgressNpcVal = nNpcProgressTotal / nTotal;
            
            CommAsyncCounter pCounter = new CommAsyncCounter(nTotal, delegate()
                {
                    this.LoadProgressInc(nNpcProgressTotal % nTotal);
                    fLoadEnd?.Invoke();
                }
            );

            for(int i = 0; i < nTotal; i++)
            {
                JArray jNpcInfo = (JArray)jNewAllNpcInfo[i];
                int nCfgId = (int)jNpcInfo[1];

                LevelPointScript pPoint = this.levelPointManager.GetLevelPoint("PlayerSpawn");
                this.CreatNpc(nCfgId, pPoint.transform.position, pPoint.transform.rotation.eulerAngles, jNpcInfo, delegate (BaseNpc pNpc)
                    {
                        this.LoadProgressInc(nLoadProgressNpcVal);
                        pCounter.Increase();
                    }
                );
            }
        }
        else
        {
            this.LoadProgressInc(nNpcProgressTotal);
            fLoadEnd?.Invoke();
        }
    }

    /// <summary>
    /// 游戏开始【帧同步开始】
    /// </summary>
    public virtual void MapLevelGameStart()
    {    
        this.mapDirectorBehaviacController.DispatchGameEvent(BehaviacGameEvent.Map_OnGameStart);

        foreach (var npc in this._npcList)
        {
            npc.Value.OnMapLevelGameStart();
        }

        LuaGEvent.DispatchEventToLua(GacEvent.MapLevelGameStart);

        // 第一版demo 倒计时5秒先不用
        // this.MapLevelGameStartWaitBegin();
    }

    /// <summary>
    /// 玩家组装结束调用方法
    /// </summary>
    private Action playerAssembleEndCall;

    /// <summary>
    /// 玩家组装开始
    /// </summary>
    public void PlayerAssembleStart(Action fEndCall = null)
    {
        this.playerAssembleEndCall = fEndCall;

        if (FrameSynchronManager.Instance.isStart)
        {
            FrameSynchronManager.Instance.fsData.PauseState = true;
        }
        
        // MyPlayer.player.SetCombatState(false);
        // MyPlayer.player.SetPauseState(true);

        LuaGEvent.DispatchEventToLua(GacEvent.PlayerAssembleStart);
    }

    /// <summary>
    /// 玩家组装结束
    /// </summary>
    public void PlayerAssembleEnd()
    {
        this.playerAssembleEndCall?.Invoke();
        this.playerAssembleEndCall = null;

        // MyPlayer.player.SetCombatState(true);
        // MyPlayer.player.SetPauseState(false);

        if (FrameSynchronManager.Instance.isStart)
        {
            FrameSynchronManager.Instance.fsData.PauseState = false;
        }
        else
        {
            // Network.CSharpSend(GacToGas.K_ReqLoadScene, 100);
            this.MapFrameSynchronStart();
        }
    }

    /// <summary>
    /// 客户端接管服务器的帧同步开始【临时】
    /// </summary>
    public void MapFrameSynchronStart()
    {
        MapManager.Instance.baseMap.MapLevelGameStart();
        FrameSynchronManager.Instance.Start();
        LocalFrameSynServer.SynFrameData();
    }

    /// <summary>
    /// 关卡结束
    /// </summary>
    /// <param name="bSuc">通关/失败</param>
    public void MapLevelEnd(bool bSuc)
    {
        // 向服务器发送关卡结束通知
        if (bSuc)
        {
            Network.CSharpSend(GacToGas.K_ReqGameLevelEnd, this.gameLevelId, bSuc);
        }

        // 发送关卡结束通知
        this.OnMapLevelGameEnd();

        // 延迟打开结算界面
        this.myMapTimer.Add(
            "WaitEnd",
            3000,
            false,
            delegate(string sName){
                // 通知Lua打开结算界面
                LuaGEvent.DispatchEventToLua(GacEvent.GameLevelEnd, this.gameLevelId, bSuc);
            },
            FrameSynchronManager.Instance.fsData.FrameRunningTime
        );

        // Network.CSharpSend(GacToGas.K_LeaveGS);
    }

    /// <summary>
    /// 关卡结束通知
    /// </summary>
    public void OnMapLevelGameEnd()
    {
        this.mapDirectorBehaviacController.DispatchGameEvent(BehaviacGameEvent.Map_OnGameEnd);

        foreach (var npc in this._npcList)
        {
            npc.Value.OnMapLevelGameEnd();
        }
    }

    /// <summary>
    /// 地图关卡游戏开始等待开启
    /// </summary>
    public void MapLevelGameStartWaitBegin()
    {
        foreach (var npc in this._npcList)
        {
            npc.Value.SetPauseState(true);
        }
        LuaGEvent.DispatchEventToLua(GacEvent.GameStartWait, BaseMap.GetGameStartWait(this.cfgId));
        
        this.myMapTimer.Add(
            "WaitStart",
            BaseMap.GetGameStartWait(this.cfgId),
            false,
            delegate(string sName){
                foreach (var npc in this._npcList)
                {
                    npc.Value.SetPauseState(false);
                }
            },
            FrameSynchronManager.Instance.fsData.FrameRunningTime
        );
    }

    public virtual void OnLoad(int nMapCfgID, Action fCallBack)
    {
        // 在地图加载之前就初始化随机类，让其所有的随机都一致
        SeedRandom.Init(0);

        this.LoadProgressOpen();

        // 开始协程加载地图
        StartCoroutine(
            this.Load(this.mapName, delegate()
            {
                this.LoadMapLevel( delegate()
                {
                    this.MapLoadAllNpc(delegate()
                    {
                        this.MapDataDisposed();
                        fCallBack?.Invoke();
                    });
                });
            })
        );
    }

    /// <summary>
    /// 加载地图关卡的数据
    /// </summary>
    /// <param name="fLoadEnd">加载完成回调</param>
    public void LoadMapLevel(Action fLoadEnd)
    {
        this.levelNpcManager.Init(this.gameLevelId, delegate()
            {
                this.levelPointManager.Init(this.gameLevelId, delegate()
                    {
                        this.levelTrapManager.Init(this.gameLevelId, delegate()
                            {
                                fLoadEnd?.Invoke();
                            }
                        );
                    }
                );
            }
        );
    }

    /// <summary>
    /// 帧同步逻辑帧刷新
    /// </summary>
    /// <param name="jInfo"></param>
    public void UpdateFrameSynLogic(JArray jInfo)
    {
        FrameSynchronData fsData = FrameSynchronManager.Instance.fsData;
        // 推进Behaviac行为树的等待时间接口的时间
        BehaviacManager.SynSinceStartup(fsData.FrameRunningTime);
        // 推进Behaviac行为树的等待帧数接口的帧数值
        BehaviacManager.SynFrameSinceStartup(fsData.GameLogicFrame);

        // 刷新计时器
        this.myMapTimer.Update(fsData.FrameRunningTime);

        // 死亡Npc清理检查
        this.DeadNpcClearCheck();

        // 刷新地图中所有Npc身上Buff的持续时间
        foreach(BaseNpc pNpc in MapManager.Instance.baseMap.GetNpcContainer().Values)
        {
            pNpc.myBuffController.BuffTimeUpdate();
        }

        this.mapDirectorBehaviacController.UpdateBehaviacTree();

        foreach (JArray OperationData in jInfo)
        {
            int nType = (int)OperationData[0];
            int nNpcID = (int)OperationData[1];
            BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcID);
            string sInfo = (string)OperationData[2];
            JObject joInfo = JsonConvert.DeserializeObject<JObject>(sInfo);
            pNpc.NpcFrameSynLogicDataUpdate(joInfo);
        }

        foreach (var npc in this._npcList)
        {
            npc.Value.NpcFrameSynLogicUpdate();
        }

        // 排队刷新Trap的行为树
        this.levelTrapManager.OnQueueTrapFrameSynLogicUpdate();

        // 排队刷帧
        if(this._queueNpcList.Count > 0){
            BaseNpc pNpc = null;
            // 是否循环查找
            for(int i = 0; i<this._queueNpcList.Count; i++ ){
                pNpc = this._queueNpcList.Dequeue();
                if (!this._npcList.ContainsKey(pNpc.InstId)){
                    continue;
                }
                this._queueNpcList.Enqueue(pNpc);
                if(!pNpc.IsEnableState()){
                    continue;
                }
                if(pNpc.IsPauseState()){
                    continue;
                }
                if(pNpc.isDead){
                    continue;
                }
                pNpc.OnQueueNpcFrameSynLogicUpdate();
                break;
            }
        }
    }

    /// <summary>
    /// 帧同步渲染帧刷新
    /// </summary>
    /// <param name="nInterpolation"></param>
    public void UpdateFrameSynRender(Fix64 nInterpolation)
    {
        MyPlayer.UpdateFrameSynRender(nInterpolation);

        foreach (var npc in this._npcList)
        {
            npc.Value.NpcFrameSynRenderUpdate(nInterpolation);
        }
    }

    #region AB资源
    AssetBundle LoadMapAB(string sMapName)
    {
        if (!MapManager.Instance.MapABAssets.ContainsKey(sMapName))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Android/scene_" + sMapName.ToLower());
            AssetBundle MapAb = null;
            MapAb = AssetBundle.LoadFromFile(path);
            MapManager.Instance.MapABAssets.Add(sMapName, MapAb);
            return MapAb;
        }
        else
        {

            return MapManager.Instance.MapABAssets[sMapName];
        }
    }
    #endregion

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <returns></returns>
    IEnumerator Load(string sMapName, Action fCallBack)
    {
        yield return new WaitForEndOfFrame();
#if GAME_VERSION_ENABLED
        LoadMapAB(sMapName);
#endif
        AsyncOperation op = SceneManager.LoadSceneAsync(sMapName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            this.LoadProgressSet((int)(op.progress * (float)this.loadProgressList[LoadProgressType.Map]));
            yield return new WaitForEndOfFrame();
        }

        op.allowSceneActivation = true;

        while(true)
        {
            string sSceneName = SceneManager.GetActiveScene().name;
            if (sSceneName == sMapName)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        isLoadEnd = true;

        this.LoadProgressSet(this.loadProgressList[LoadProgressType.Map]);

        if (fCallBack != null)
        {
            fCallBack();
            fCallBack = null;
        }
    }

    // ---------------------------------------------------------------------------
    // 地图进度加载相关代码

    /// <summary>
    /// 当前的进度值
    /// </summary>
    /// <value></value>
    public int loadProgressVal { get; private set; }

    /// <summary>
    /// 总的进度值
    /// </summary>
    /// <value></value>
    public int loadProgressTotal { get; private set; } = -1;

    public LoadProgressType loadProgressType;

    /// <summary>
    /// 加载项的进度值
    /// </summary>
    /// <value></value>
    public Dictionary<LoadProgressType, int> loadProgressList { get; private set; } = new Dictionary<LoadProgressType, int>{
        [LoadProgressType.Map] = 10,
        [LoadProgressType.LevelNpc] = 75,
        [LoadProgressType.LevelPoint] = 5,
        [LoadProgressType.LevelTrap] = 5,   
        [LoadProgressType.ServerNpc] = 5,
    };

    /// <summary>
    /// 加载进度开始
    /// </summary>
    public void LoadProgressOpen()
    {
        this.loadProgressVal = 0;
        this.loadProgressTotal = 0;
        foreach(int nVal in this.loadProgressList.Values)
        {
            this.loadProgressTotal += nVal;
        }

        // 通知Lua显示加载界面
        LuaGEvent.DispatchEventToLua(GacEvent.LoadProgressOpen);
    }

    /// <summary>
    /// 加载进度结束
    /// </summary>
    public void LoadProgressClose()
    {
        this.loadProgressVal = 0;
    }

    /// <summary>
    /// 加载进度刷新
    /// </summary>
    /// <param name="nVal"></param>
    public void LoadProgressUpdate()
    {
        Debug.Log("BaseMap.LoadProgressUpdate->" + "#loadProgressVal = " + this.loadProgressVal);

        // 通知Lua刷新加载界面
        LuaGEvent.DispatchEventToLua(GacEvent.LoadProgressUpdate);
    }

    public void LoadProgressSet(int nVal)
    {
        this.loadProgressVal = nVal;
        this.LoadProgressUpdate();
    }

    public void LoadProgressInc(int nInc)
    {
        this.loadProgressVal += nInc;
        this.LoadProgressUpdate();
    }

    // ---------------------------------------------------------------------------
    // 死亡Npc清理

    // 死亡Npc清理表
    private Dictionary<int,int> _deadNpcClear = new Dictionary<int,int>();

    // 添加死亡Npc清理
    private void DeadNpcClearAdd(int nNpcInstId)
    {
        this._deadNpcClear.Add(nNpcInstId, FrameSynchronManager.Instance.fsData.FrameRunningTime + 10000);
    }

    // 死亡Npc清理检查
    private void DeadNpcClearCheck()
    {
        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;
        List<int> haveClear = new List<int>();
        foreach(var kvPair in this._deadNpcClear)
        {
            if (nCurTime >= kvPair.Value)
            {
                haveClear.Add(kvPair.Key);
                BaseNpc pNpc = this.GetNpc(kvPair.Key);
                if (pNpc != null)
                {
                    pNpc.SetEnableState(false);
                }
            }
        }

        foreach(int nNpcInstId in haveClear)
        {
            this._deadNpcClear.Remove(nNpcInstId);
        }
    }

    // ---------------------------------------------------------------------------
    //配件功能相关代码

    /// <summary>
    /// 地图中所有配件容器
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="RobotPart"></typeparam>
    /// <returns></returns>
    private Dictionary<int, RobotPart> _robotPartContainer =
        new Dictionary<int, RobotPart>();

    /// <summary>
    /// 获取配件管理容器
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, RobotPart> GetRobotPartContainer()
    {
        return this._robotPartContainer;
    }

    /// <summary>
    /// 通过配件实例Id获取配件实例
    /// </summary>
    /// <param name="nPartInstId">配件实例Id</param>
    /// <returns></returns>
    public RobotPart GetRobotPart(int nPartInstId)
    {
        if (!this._robotPartContainer.ContainsKey(nPartInstId))
        {
            return null;
        }
        return this._robotPartContainer[nPartInstId];
    }

    /// <summary>
    /// 创建配件
    /// </summary>
    /// <param name="nCfgId">配件配置Id</param>
    /// <param name="nNum">配件实体数量</param>
    /// <param name="fLoaded">加载完成后回调</param>
    public void RobotPartCreate(int nCfgId, int nNum, float nParentScale, Action<RobotPart> fLoaded = null)
    {
        int nModelCfgId = RobotPart.GetRobotPartModelId(nCfgId);

        RobotPart pNewPart = new RobotPart(nCfgId);

        this._robotPartContainer[pNewPart.InstId] = pNewPart;
        pNewPart.Load(nNum, nModelCfgId, Vector3.zero, Vector3.zero, nParentScale, delegate ()
        {
            fLoaded?.Invoke(pNewPart);
        });
    }

    /// <summary>
    /// 配件销毁
    /// </summary>
    /// <param name="nPartInstId">配件实例Id</param>
    public void RobotPartDestroy(int nPartInstId)
    {
        RobotPart pPart = GetRobotPart(nPartInstId);
        this._robotPartContainer.Remove(nPartInstId);
        pPart.Destroy();
    }

    //---------------------------------------------------------------------------
    // Npc的Layer层相关代码
    private Dictionary<NpcType, HashSet<string>> _npcLayerContainer = new Dictionary<NpcType, HashSet<string>>()
    {
        [NpcType.MonsterNpc] = new HashSet<string>()
            {
                "Npc1",
                "Npc2",
                "Npc3",
                "Npc4",
                "Npc5",
                "Npc6",
                "Npc7",
                "Npc8",
                "Npc9",
                "Npc10",
                "Npc11",
                "Npc12",
                "Npc13",
                "Npc14",
                "Npc15"
            },
        [NpcType.PlayerNpc] = new HashSet<string>()
            {
                "Npc0"
            }
    };

    /// <summary>
    /// 从Npc的Layer名称容器中弹出一个指定Npc类型的Layer名
    /// </summary>
    /// <param name="eType"></param>
    /// <returns></returns>
    public string NpcLayerNameContainerPop(NpcType eType)
    {
        if (!this._npcLayerContainer.ContainsKey(eType))
        {
            return "Default";
        }

        if (this._npcLayerContainer[eType].Count == 0)
        {
            Debug.LogError("Error->BaseMap.NpcLayerNameContainerPop->Layer Name Have Max Out! -> " + "#NpcType = " + eType.ToString());
            return "Default";
        }

        string sLayerName = "";
        foreach(string sName in this._npcLayerContainer[eType])
        {
            sLayerName = sName;
            break;
        }
        this._npcLayerContainer[eType].Remove(sLayerName);
        return sLayerName;
    }

    /// <summary>
    /// 向Npc的Layer名称容器中放入一个指定Npc类型的Layer名
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="sLayerName"></param>    
    public void NpcLayerNameContainerPush(NpcType eType, string sLayerName)
    {
        if (!this._npcLayerContainer.ContainsKey(eType))
        {
            return;
        }

        if (this._npcLayerContainer[eType].Contains(sLayerName))
        {
            Debug.LogError("Error->BaseMap.NpcLayerNameContainerPush->Have Exist Target NpcType Layer Name! -> " + "#NpcType = " + eType.ToString() + "#LayerName = " + sLayerName);
            return;
        }

        this._npcLayerContainer[eType].Add(sLayerName);
    }
    
    // ---------------------------------------------------------------------------
    //读表操作

    // 场景表

    public static JArray GetNpcSpawnPos(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Scene_C", nCfgId, "pos"));
    }

    public static MapType GetMapType(int nCfgId)
    {
        return (MapType)ConfigManager.GetValue<int>("Scene_C", nCfgId, "playType");
    }

    /// <summary>
    /// 通过地图配置Id获取导演Ai名称
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static string GetDirectorAiName(int nCfgId)
    {
        return ConfigManager.GetValue<string>("Scene_C", nCfgId, "strAiName");
    }

    /// <summary>
    /// 获取游戏开始等待时间
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetGameStartWait(int nCfgId)
    {
        return 5000;
    }

    // 关卡表 ---------------------------------------------------------------------------------

    /// <summary>
    /// 获取关卡场景Id
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static int GetGameLevelSceneId(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Level_C", nCfgId, "sceneId");
    }

    /// <summary>
    /// 获取关卡导演名
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static string GetGameLevelDirectorName(int nCfgId)
    {
        return ConfigManager.GetValue<string>("Level_C", nCfgId, "strDirectorAi");
    }

    // 临时代码 ----------------------------------------------------------------------------------
    public static Dictionary<int, JArray> playerAssembleData = new Dictionary<int, JArray>()
    {
        // [1001] = new JArray(){10003,20500,20501,0,30100,40300,40100,40100},
        [1001] = new JArray(){10003,20602,20603,0,30400,0,40400,40400},
        [1002] = new JArray(){10003,20602,20603,0,30400,0,40400,40400},
        [1003] = new JArray(){10003,20602,20603,0,30400,0,40400,40400},
        [1004] = new JArray(){10003,20602,20603,0,30400,0,40400,40400},
        [1005] = new JArray(){10003,20105,20106,0,30400,0,40400,40400},
        [1006] = new JArray(){10003,20105,20106,0,30400,0,40400,40400},
        [1007] = new JArray(){10003,20105,20106,0,30400,0,40400,40400},
        [1008] = new JArray(){10003,0,0,20203,30400,0,40400,40400},
        [1009] = new JArray(){10003,0,0,20203,30400,0,40400,40400},
        [1010] = new JArray(){10003,20105,20106,0,30400,0,40400,40400},
    };
}