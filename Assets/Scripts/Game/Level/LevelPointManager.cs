using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 关卡配置点管理器
/// </summary>
public class LevelPointManager : MapObject
{
    /// <summary>
    /// 关卡配置点容器
    /// </summary>
    /// <typeparam name="int">关卡配置点的索引</typeparam>
    /// <typeparam name="LevelPointScript">关卡配置点的脚本</typeparam>
    /// <returns></returns>
    private Dictionary<int,LevelPointScript> _levelPointContainer = new Dictionary<int, LevelPointScript>();

    /// <summary>
    /// 关卡配置点从名字到索引的映射
    /// </summary>
    /// <returns></returns>
    private Dictionary<string,List<int>> _nameMappingToIdx = new Dictionary<string, List<int>>();

    public Model myModel{get; private set;}

    /// <summary>
    /// 初始化关卡配置点控制器
    /// </summary>
    public void Init(int nLevelId, Action fLoadEnd)
    {
        MapManager.Instance.baseMap.loadProgressType = LoadProgressType.LevelPoint;

        if (nLevelId == 0)
        {
            MapManager.Instance.baseMap.LoadProgressInc(MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelPoint]);
            fLoadEnd?.Invoke();
        }
        else
        {
            Load(nLevelId, delegate()
                {
                    for(int i = 0; i < this.myModel.transform.childCount; i++)
                    {
                        Transform pTransform = this.myModel.transform.GetChild(i);
                        LevelPointScript pScript = pTransform.gameObject.GetComponent<LevelPointScript>();
                        this._levelPointContainer.Add(i, pScript);

                        string sName = pTransform.name;
                        if (!this._nameMappingToIdx.ContainsKey(sName))
                        {
                            this._nameMappingToIdx[sName] = new List<int>();
                        }
                        this._nameMappingToIdx[sName].Add(i);
                    }
                    this.myModel.gameObject.SetActive(false);

                    MapManager.Instance.baseMap.LoadProgressInc(MapManager.Instance.baseMap.loadProgressList[LoadProgressType.LevelPoint]);
                    fLoadEnd?.Invoke();
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
        new Model(this, "Assets/Res/Prefabs/Level/" + nLevelId + "_Point.prefab", Vector3.zero, Vector3.zero, 1f, delegate (Model model)
            {
                this.myModel = model;
                fLoadEnd?.Invoke();
            }
        );
    }

    /// <summary>
    /// 是否存在指定索引的关卡配置点
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public bool IsExistLevelPointByIdx(int nIdx)
    {
        if (!this._levelPointContainer.ContainsKey(nIdx))
        {
            Debug.Log("LevelPointController.IsExistLevelPointByIdx->Don't Exist Target nIdx!" + "|nIdx = " + nIdx);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否存在指定名称的关卡配置点
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public bool IsExistLevelPointByName(string sName)
    {
        if (!this._nameMappingToIdx.ContainsKey(sName))
        {
            Debug.Log("LevelPointController.IsExistLevelPointByName->Don't Exist Target sName!" + "|sName = " + sName);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 通过关卡配置点的索引获取关卡配置点
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public LevelPointScript GetLevelPoint(int nIdx)
    {
        if (!this.IsExistLevelPointByIdx(nIdx))
        {
            return null;
        }
        return this._levelPointContainer[nIdx];
    }

    /// <summary>
    /// 通过关卡配置点的名称获取所有符合该名称的关卡配置点索引列表
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public List<int> GetLevelPointList(string sName)
    {
        if (!this.IsExistLevelPointByName(sName))
        {
            return new List<int>();
        }
        return this._nameMappingToIdx[sName];
    }

    /// <summary>
    /// 通过关卡配置点的名称获取一个关卡配置点
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    public LevelPointScript GetLevelPoint(string sName)
    {
        List<int> listPointIdx = this.GetLevelPointList(sName);
        if (listPointIdx.Count == 0)
        {
            return null;
        }

        return this.GetLevelPoint(listPointIdx[0]);
    }
}
