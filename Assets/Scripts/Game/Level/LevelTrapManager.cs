using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 关卡配置Trap管理器
/// </summary>
public class LevelTrapManager : MapObject
{
    /// <summary>
    /// 关卡配置Trap容器
    /// </summary>
    /// <typeparam name="int">关卡配置Trap的索引</typeparam>
    /// <typeparam name="LevelTrapController">关卡配置Trap控制器的脚本</typeparam>
    /// <returns></returns>
    private Dictionary<int,LevelTrapController> _levelTrapContainer = new Dictionary<int, LevelTrapController>();

    /// <summary>
    /// 关卡配置Trap从名字到索引的映射
    /// </summary>
    /// <returns></returns>
    private Dictionary<string,List<int>> _nameMappingToIdx = new Dictionary<string, List<int>>();

    public Model myModel{get; private set;}

    /// <summary>
    /// 初始化关卡配置Trap控制器
    /// </summary>
    public void Init(int nLevelId, Action fLoadEnd)
    {
        MapManager.Instance.baseMap.loadProgressType = LoadProgressType.LevelTrap;

        if (nLevelId == 0)
        {
            MapManager.Instance.baseMap.LoadProgressInc(MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelTrap]);
            fLoadEnd?.Invoke();
        }
        else
        {
            Load(nLevelId, delegate()
                {
                    for(int i = 0; i < this.myModel.transform.childCount; i++)
                    {
                        GameObject pGameObj = this.myModel.transform.GetChild(i).gameObject;
                        LevelTrapController pController = pGameObj.GetComponent<LevelTrapController>();
                        pController.Init();

                        pController.myTrapIdx = i;
                        this._levelTrapContainer.Add(i, pController);
                        this._queueTrapList.Enqueue(pController);

                        string sName = pGameObj.name;
                        if (!this._nameMappingToIdx.ContainsKey(sName))
                        {
                            this._nameMappingToIdx[sName] = new List<int>();
                        }
                        this._nameMappingToIdx[sName].Add(i);
                    }

                    MapManager.Instance.baseMap.LoadProgressInc(MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelTrap]);
                    fLoadEnd?.Invoke();
                }
            );
        }
    }

    /// <summary>
    /// Trap队列集合
    /// </summary>
    /// <typeparam name="CleanObject"></typeparam>
    /// <returns></returns>
    public Queue<LevelTrapController> _queueTrapList = new Queue<LevelTrapController>();

    public void OnQueueTrapFrameSynLogicUpdate()
    {
        // 排队刷帧
        if(this._queueTrapList.Count > 0){
            LevelTrapController pController = null;
            // 是否循环查找
            for(int i = 0; i<this._queueTrapList.Count; i++ )
            {
                pController = this._queueTrapList.Dequeue();

                if (!this._levelTrapContainer.ContainsKey(pController.myTrapIdx)){
                    continue;
                }

                this._queueTrapList.Enqueue(pController);

                if (!pController.IsEnableState())
                {
                    continue;
                }

                if (pController.joinTrapNpcController.Count == 0)
                {
                    continue;
                }

                pController.OnQueueTrapFrameSynLogicUpdate();
                break;
            }
        }
    }

    /// <summary>
    /// 加载关卡配置Trap的Prefab
    /// </summary>
    /// <param name="nLevelId"></param>
    /// <param name="fLoadEnd"></param>
    public void Load(int nLevelId, Action fLoadEnd)
    {
        new Model(this, "Assets/Res/Prefabs/Level/" + nLevelId + "_Trap.prefab", Vector3.zero, Vector3.zero, 1f, delegate (Model model)
            {
                this.myModel = model;
                fLoadEnd?.Invoke();
            }
        );
    }

    /// <summary>
    /// 是否存在指定索引的关卡配置Trap
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool IsExistLevelTrapByIdx(int nIdx)
    {
        if (!this._levelTrapContainer.ContainsKey(nIdx))
        {
            Debug.Log("LevelTrapController.IsExistLevelTrapByIdx->Don't Exist Target nIdx!" + "|nIdx = " + nIdx);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否存在指定名称的关卡配置Trap
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool IsExistLevelTrapByName(string sName)
    {
        if (!this._nameMappingToIdx.ContainsKey(sName))
        {
            Debug.Log("LevelTrapController.IsExistLevelTrapByName->Don't Exist Target sName!" + "|sName = " + sName);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 通过关卡配置Trap的索引获取关卡配置Trap
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public LevelTrapController GetLevelTrap(int nIdx)
    {
        if (!this.IsExistLevelTrapByIdx(nIdx))
        {
            return null;
        }
        return this._levelTrapContainer[nIdx];
    }

    /// <summary>
    /// 通过关卡配置Trap的名称获取所有符合该名称的关卡配置Trap索引列表
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public List<int> GetLevelTrapList(string sName)
    {
        if (!this.IsExistLevelTrapByName(sName))
        {
            return new List<int>();
        }
        return this._nameMappingToIdx[sName];
    }

    /// <summary>
    /// 设置指定索引的Trap是否启用
    /// </summary>
    /// <param name="nIdx"></param>
    /// <param name="bEnable"></param>
    public void SetLevelTrapEnableByIdx(int nIdx, bool bEnable)
    {
        if (!this.IsExistLevelTrapByIdx(nIdx))
        {
            return;
        }
        if (this._levelTrapContainer[nIdx].gameObject.activeSelf == bEnable)
        {
            return;
        }
        this._levelTrapContainer[nIdx].SetEnableState(bEnable);
    }

    /// <summary>
    /// 设置指定名称的Trap组是否启用
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="bEnable"></param>
    public void SetLevelTrapEnableByName(string sName, bool bEnable)
    {
        if (!this.IsExistLevelTrapByName(sName))
        {
            return;
        }
        foreach(int nIdx in this._nameMappingToIdx[sName])
        {
            this.SetLevelTrapEnableByIdx(nIdx, bEnable);
        }
    }

    /// <summary>
    /// 获取指定索引的Trap的是否启用
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool IsLevelTrapEnableByIdx(int nIdx)
    {
        if (!this.IsExistLevelTrapByIdx(nIdx))
        {
            return false;
        }
        return this._levelTrapContainer[nIdx].IsEnableState();
    }

    /// <summary>
    /// 获取指定名称的Trap组是否都是启用
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool IsLevelTrapAllEnableByName(string sName)
    {
        if (!this.IsExistLevelTrapByName(sName))
        {
            return false;
        }

        bool bAllEnable = true;
        foreach(int nIdx in this.GetLevelTrapList(sName))
        {
            if (!this.IsLevelTrapEnableByIdx(nIdx))
            {
                bAllEnable = false;
                break;
            }
        }
        return bAllEnable;
    }
}
