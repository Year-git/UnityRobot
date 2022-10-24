using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 关卡配置Npc管理器
/// </summary>
public class LevelNpcManager : MapObject
{
    /// <summary>
    /// 关卡配置Npc容器
    /// </summary>
    /// <typeparam name="int">关卡配置Npc的索引</typeparam>
    /// <typeparam name="LevelNpcScript">关卡配置Npc脚本</typeparam>
    /// <returns></returns>
    private Dictionary<int,LevelNpcScript> _levelNpcContainer = new Dictionary<int, LevelNpcScript>();

    /// <summary>
    /// 关卡配置Npc从名称到索引的映射
    /// </summary>
    /// <returns></returns>
    private Dictionary<string,List<int>> _nameMappingToIdx = new Dictionary<string, List<int>>();

    /// <summary>
    /// 关卡配置Npc从索引到实例Npc的实例Id的映射
    /// </summary>
    /// <returns></returns>
    private Dictionary<int,int> _idxMappingToInstId = new Dictionary<int,int>();

    /// <summary>
    /// 关卡配置Npc从实例Npc的实例Id到索引的映射
    /// </summary>
    /// <returns></returns>
    private Dictionary<int,int> _instIdMappingToIdx = new Dictionary<int,int>();

    public Model myModel{get; private set;}

    /// <summary>
    /// 初始化关卡配置Npc控制器
    /// </summary>
    /// <param name="nLevelId"></param>
    /// <param name="fLoadEnd"></param>
    public void Init(int nLevelId, Action fLoadEnd)
    {
        MapManager.Instance.baseMap.loadProgressType = LoadProgressType.LevelNpc;

        if (nLevelId == 0)
        {
            MapManager.Instance.baseMap.LoadProgressInc(MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelNpc]);
            fLoadEnd?.Invoke();
        }
        else
        {
            Load(nLevelId, delegate()
                {
                    for(int i = 0; i < this.myModel.transform.childCount; i++)
                    {
                        Transform pTransform = this.myModel.transform.GetChild(i);
                        LevelNpcScript pScript = pTransform.gameObject.GetComponent<LevelNpcScript>();
                        this._levelNpcContainer.Add(i, pScript);

                        string sName = pTransform.name;
                        if (!this._nameMappingToIdx.ContainsKey(sName))
                        {
                            this._nameMappingToIdx[sName] = new List<int>();
                        }
                        this._nameMappingToIdx[sName].Add(i);
                    }

                    this.myModel.gameObject.SetActive(false);

                    this.LevelNpcObjectInit(delegate()
                        {
                            fLoadEnd?.Invoke();
                        }
                    );
                }
            );
        }
    }

    /// <summary>
    /// 加载关卡配置Npc的Prefab
    /// </summary>
    /// <param name="nLevelId"></param>
    /// <param name="fLoadEnd"></param>
    public void Load(int nLevelId, Action fLoadEnd)
    {
        new Model(this, "Assets/Res/Prefabs/Level/" + nLevelId + "_Npc.prefab", Vector3.zero, Vector3.zero, 1f, delegate (Model model)
            {
                this.myModel = model;
                fLoadEnd?.Invoke();
            }
        );
    }

    /// <summary>
    /// 是否存在指定索引的关卡配置Npc
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool IsExistLevelNpcByIdx(int nIdx)
    {
        if (!this._levelNpcContainer.ContainsKey(nIdx))
        {
            Debug.Log("LevelNpcController.IsExistLevelNpcByIdx->Don't Exist Target nIdx!" + "|nIdx = " + nIdx);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否存在指定名称的关卡配置Npc
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool IsExistLevelNpcByName(string sName)
    {
        if (!this._nameMappingToIdx.ContainsKey(sName))
        {
            Debug.Log("LevelNpcController.IsExistLevelNpcByName->Don't Exist Target sName!" + "|sName = " + sName);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 通过关卡配置Npc的索引获取关卡配置Npc
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public LevelNpcScript GetLevelNpc(int nIdx)
    {
        if (!IsExistLevelNpcByIdx(nIdx))
        {
            return null;
        }
        return this._levelNpcContainer[nIdx];
    }

    /// <summary>
    /// 通过关卡配置Npc的索引获取Npc
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public BaseNpc GetNpc(int nIdx)
    {
        if (!this._idxMappingToInstId.ContainsKey(nIdx))
        {
            return null;
        }

        return MapManager.Instance.baseMap.GetNpc(this._idxMappingToInstId[nIdx]);
    }

    /// <summary>
    /// 通过关卡配置Npc的名称获取所有符合该名称的关卡配置Npc的索引列表
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public List<int> GetLevelNpcList(string sName)
    {
        if (!IsExistLevelNpcByName(sName))
        {
            return new List<int>();
        }
        return this._nameMappingToIdx[sName];
    }

    /// <summary>
    /// 通过关卡配置Npc的索引，刷出对应的Npc实例
    /// </summary>
    /// <param name="nIdx"></param>
    private void SpawnLevelNpcByIdx(int nIdx, Action<BaseNpc> fLoadEnd = null)
    {
        LevelNpcScript pScript = this.GetLevelNpc(nIdx);
        MapManager.Instance.baseMap.CreatNpc(
            pScript.npcCfgId, 
            pScript.transform.position,
            pScript.transform.rotation.eulerAngles,
            null,
            delegate(BaseNpc pNpc){
                this._idxMappingToInstId[nIdx] = pNpc.InstId;
                this._idxMappingToInstId[pNpc.InstId] = nIdx;

                fLoadEnd?.Invoke(pNpc);
            }
        );
    }

    /// <summary>
    /// 设置指定索引的关卡配置Npc是否启用
    /// </summary>
    /// <param name="nIdx"></param>
    /// <param name="bEnable"></param>
    public void SetLevelNpcEnableByIdx(int nIdx, bool bEnable)
    {
        if (!this.IsExistLevelNpcByIdx(nIdx))
        {
            return;
        }

        if (this._idxMappingToInstId.ContainsKey(nIdx))
        {
            int nNpcInstId = _idxMappingToInstId[nIdx];
            BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstId);
            if(pNpc.IsEnableState() == bEnable){
                return;
            }   
            if (pNpc != null)
            {
                // 如果是启用，则在设置Npc启用状态前，设置Npc的Layer
                if (bEnable == true)
                {
                    // 播放出场特效
                    MonsterType monsterType = pNpc.GetMonsterType();
                    if(monsterType != MonsterType.Common){
                        MapManager.Instance.baseMap.effectManager.SceneEffectAdd(12, pNpc.gameObject.transform.position,Quaternion.identity);
                    }else{
                        MapManager.Instance.baseMap.effectManager.SceneEffectAdd(13, pNpc.gameObject.transform.position,Quaternion.identity);
                    }                    
                    pNpc.NpcLayerSet();
                }

                pNpc.SetEnableState(bEnable);

                // 如果是不启用，则在设置Npc启用状态后，清理Npc的Layer
                if (bEnable == false)
                {
                    pNpc.NpcLayerClear();
                }
            }
        }
    }

    /// <summary>
    /// 设置指定名称的关卡配置Npc是否启用
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="fLoadEnd"></param>
    /// <param name="fNpcInit"></param>
    public void SetLevelNpcEnableByName(string sName, bool bEnabel)
    {
        if (!this.IsExistLevelNpcByName(sName))
        {
            return;
        }
        foreach(var nLevelNpcIdx in this.GetLevelNpcList(sName))
        {
            this.SetLevelNpcEnableByIdx(nLevelNpcIdx, bEnabel);
        }
    }

    /// <summary>
    /// 获取指定索引的关卡配置Npc是否启用
    /// </summary>
    /// <returns></returns>
    public bool IsLevelNpcEnableByIdx(int nIdx)
    {
        if (!this.IsExistLevelNpcByIdx(nIdx))
        {
            return false;
        }

        if (!this._idxMappingToInstId.ContainsKey(nIdx))
        {
            return false;
        }

        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this._idxMappingToInstId[nIdx]);
        if (pNpc == null)
        {
            return false;
        }

        return pNpc.IsEnableState();
    }

    /// <summary>
    /// 获取指定名的关卡配置Npc是否全部启用
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool IsLevelNpcAllEnableByName(string sName)
    {
        if (!this.IsExistLevelNpcByName(sName))
        {
            return false;
        }
        
        bool bAllEnable = true;
        foreach(var nLevelNpcIdx in this.GetLevelNpcList(sName))
        {
            if (!this.IsLevelNpcEnableByIdx(nLevelNpcIdx))
            {
                bAllEnable = false;
                break;
            }
        }
        return bAllEnable;
    }

    /// <summary>
    /// 初始化所有关卡配置Npc的实例对象
    /// </summary>
    private void LevelNpcObjectInit(Action fLoadEnd = null)
    {
        Queue<int> pLoadList = new Queue<int>();
        foreach(var kvPair in this._levelNpcContainer)
        {
            pLoadList.Enqueue(kvPair.Key);
        }
        this.LevelNpcLoopLoad(pLoadList, fLoadEnd);
    }

    /// <summary>
    /// 递归加载Npc
    /// </summary>
    /// <param name="pLoadList">要加载的配件队列</param>
    /// <param name="fLoadEnd">全部加载完成回调</param>
    private void LevelNpcLoopLoad(Queue<int> pLoadList, Action fLoadEnd = null, int nTotal = -1)
    {
        int nLoadTotal = nTotal == -1 ? pLoadList.Count : nTotal;
        int nLoadProgress = MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelNpc];

        if (pLoadList.Count <= 0)
        {
            MapManager.Instance.baseMap.LoadProgressInc(nLoadTotal == 0 ? nLoadProgress : nLoadProgress % nLoadTotal);
            fLoadEnd?.Invoke();
            return;
        }

        int nIdx = pLoadList.Dequeue();
        SpawnLevelNpcByIdx(
            nIdx,
            delegate(BaseNpc pNpc)
                {
                    if (!this._levelNpcContainer[nIdx].isEnable) 
                    {
                        // 将带有默认不开启标记的关卡配置Npc设置为未启用
                        this.SetLevelNpcEnableByIdx(nIdx, false);
                    }

                    MapManager.Instance.baseMap.LoadProgressInc(nLoadProgress / nLoadTotal);
                    this.LevelNpcLoopLoad(pLoadList, fLoadEnd, nLoadTotal);
                }
        );
    }

    /// <summary>
    /// 通过关卡配置Npc的索引获取实例Npc是否死亡
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool GetLevelNpcIsDead(int nIdx)
    {
        if (!this.IsExistLevelNpcByIdx(nIdx))
        {
            return true;
        }

        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(this._idxMappingToInstId[nIdx]);
        if (pNpc == null)
        {
            Debug.LogError("LevelNpcController.pNpc == null!" + "|nIdx = " + nIdx + "|nNpcInstId = " + this._idxMappingToInstId[nIdx]);
            return true;
        }

        if (pNpc.isDead)
        {
            return true;
        }
        
        return false;
    }

    // / <summary>
    /// 通过关卡配置Npc的名称获取所有实例Npc是否死亡
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool GetLevelNpcIsAllDead(string sName)
    {
        if (!this.IsExistLevelNpcByName(sName))
        {
            return true;
        }
        
        bool bAllDead = true;
        foreach(var nLevelNpcIdx in this.GetLevelNpcList(sName))
        {
            if (!this.GetLevelNpcIsDead(nLevelNpcIdx))
            {
                bAllDead = false;
                break;
            }
        }
        return bAllDead;
    }
}
