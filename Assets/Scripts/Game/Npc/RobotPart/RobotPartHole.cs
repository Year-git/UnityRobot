using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RobotPartHole : MapObject
{
    /// <summary>
    /// 槽位的配置Id
    /// </summary>
    /// <value></value>
    public int cfgId{get; private set;}
    
    /// <summary>
    /// 槽位中的配件实例Id，没有为0
    /// </summary>
    /// <value></value>
    public int partInstId{get; private set;} = 0;

    /// <summary>
    /// 槽位类型
    /// </summary>
    public RobotPartHoleType holeType;

    /// <summary>
    /// 槽位配件类型
    /// </summary>
    /// <value></value>
    public RobotPartType partType{get;private set;}

    /// <summary>
    /// 槽位缩放
    /// </summary>
    public float myScale { get; protected set; } = 1f;

    /// <summary>
    /// 槽位链接模型
    /// </summary>
    /// <value></value>
    public List<Model> myModelList{get;private set;} = new List<Model>();

    /// <summary>
    /// 槽位挂点Transform
    /// </summary>
    /// <typeparam name="Transform"></typeparam>
    /// <returns></returns>
    public List<Transform> myPointList{get;private set;} = new List<Transform>();

    public BaseNpc myNpc{get;private set;}

    public int listCount = 0;
    public int holeIdx{get; private set;} = -1;

    public RobotPartHole(BaseNpc pNpc, int nHoleIdx)
    {
        this.myNpc = pNpc;
        this.holeIdx = nHoleIdx;
    }

    /// <summary>
    /// 是否已加载完模型
    /// </summary>
    /// <value></value>
    public bool loadEnd
    {
        get 
        {
            bool bEnd = true;
            foreach(var model in myModelList)
            {
                bEnd = bEnd ? (model != null ? model.isLoadEnd : false) : false;
            }
            return bEnd;
        }
    }

    /// <summary>
    /// 在指定模型下创建配件槽位
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="nHolePlaceIdx"></param>
    /// <param name="mParent"></param>
    /// <param name="fLoaded"></param>
    public void Init(int nCfgId, float nParentScale, Action<RobotPartHole> fLoaded = null)
    {
        this.cfgId = nCfgId;
        this.holeType = RobotPartHole.GetRobotPartHoleType(nCfgId);
        this.partType = RobotPartHole.GetRobotPartHolePartType(nCfgId);

        int nModelCfgId = RobotPartHole.GetRobotPartHoleModelId(partType);

        this.myScale = Model.GetModelScale(nModelCfgId) * nParentScale;

        JArray arrPosition = RobotPartHole.GetRobotPartHoleRandomCoord(nCfgId);

        CommAsyncCounter pCounter = new CommAsyncCounter(arrPosition.Count, delegate()
            {
                fLoaded?.Invoke(this);
            }
        );

        for(int i = 0; i < arrPosition.Count; i++)
        {
            int nIdx = i;

            Load(nModelCfgId, Vector3.zero, Vector3.zero, this.myScale, delegate (Model model)
                {
                    myModelList.Add(model);
                    model.gameObject.name = this.holeIdx.ToString();

                    // 根据槽位类型做不同处理
                    if (this.holeType == RobotPartHoleType.Point)
                    {
                        string sPointName = RobotPartHole.GetRobotPartHolePoint(this.cfgId, nIdx);
                        if (sPointName == "")
                        {
                            Debug.LogError("RobotPartHole.Init->Hole BoneName Is Lost!" + "#HoleCfgId = " + this.cfgId + "#nIdx = " + nIdx);
                        }

                        RobotPartScriptBody pBody = (RobotPartScriptBody)this.myNpc.GetRobotBodyPart().myElementList[0].myScript;
                        if (pBody == null)
                        {
                            Debug.LogError("RobotPartHole.Init->Robot Part Body Is Not Exist!");
                        }

                        if (!pBody.IsHaveBodyChildTransform(sPointName))
                        {
                            Debug.LogError("RobotPartHole.Init->BonePoint Is Not Exist!" + "#HoleCfgId = " + this.cfgId + "#nIdx = " + nIdx + "#sPointName = " + sPointName);
                        }

                        model.transform.parent = pBody.GetBodyChildTransform(sPointName);
                        model.transform.localPosition = Vector3.zero;
                        model.transform.localRotation = Quaternion.identity;
                    }
                    else // 默认使用 Normal 类型
                    {
                        JArray jPosition = (JArray)arrPosition[nIdx];
                        Vector3 v3Position = new Vector3((float)jPosition[0], (float)jPosition[1], (float)jPosition[2]);
                        Vector3 v3Euler = new Vector3((float)jPosition[3], (float)jPosition[4], (float)jPosition[5]);
                        model.transform.parent = this.myNpc.myModel.transform;
                        model.transform.localPosition = v3Position;
                        model.transform.localRotation = Quaternion.Euler(v3Euler);
                    }
                    
                    pCounter.Increase();
                }
            );
        }
    }

    /// <summary>
    /// 配件槽位链接模型加载
    /// </summary>
    /// <param name="nModelCfgId">模型配置Id</param>
    /// <param name="fv3Position">坐标</param>
    /// <param name="fv3EulerAngles">转向</param>
    /// <param name="fLoaded">加载完成后回调</param>
    /// <returns></returns>
    public Model Load(int nModelCfgId, Vector3 fv3Position, Vector3 fv3EulerAngles, float nParentScale, Action<Model> fLoaded = null)
    {
        string sModelName = Model.GetModelName(nModelCfgId);
        float nScale = Model.GetModelScale(nModelCfgId) * nParentScale;
        
        return new Model(this, sModelName, fv3Position, fv3EulerAngles, nScale, delegate (Model model)
            {
                // this.HideModel();
                fLoaded?.Invoke(model);
            }
        );
    }

    /// <summary>
    /// 销毁配件槽位
    /// </summary>
    public void Destroy()
    {
        foreach(var model in myModelList)
        {
            model.transform.parent = null;
            model.Destroy();
        }
    }

    /// <summary>
    /// 槽位放入配件
    /// </summary>
    /// <param name="pPart"></param>
    public void RobotPartInsert(RobotPart pPart)
    {
        for (int i = 0; i < pPart.myElementList.Count; i++)
        {
            pPart.myElementList[i].myModel.transform.parent = this.myModelList[i].transform;
            pPart.myElementList[i].myModel.transform.localPosition = Vector3.zero;
            pPart.myElementList[i].myModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
            pPart.myElementList[i].myModel.gameObject.SetActive(true);
            pPart.myElementList[i].holeListIdx = i;
            this.myNpc.NpcChildLayerSet(pPart.myElementList[i].myModel.transform);
        }

        // 槽位记录配件实例Id
        this.partInstId = pPart.InstId;

        // 设置配件的安装信息
        pPart.SetInstallInfo(this.myNpc.InstId, this.holeIdx);
    }

    /// <summary>
    /// 槽位移出配件
    /// </summary>
    public void RobotPartRemove()
    {
        RobotPart pPart = MapManager.Instance.baseMap.GetRobotPart(this.partInstId);
        for (int i = 0; i < pPart.myElementList.Count; i++)
        {
            pPart.myElementList[i].myModel.transform.parent = null;
            pPart.myElementList[i].myModel.gameObject.SetActive(false);
            this.myNpc.NpcChildLayerClear(pPart.myElementList[i].myModel.transform);
        }

        // 槽位清除记录的配件实例Id
        this.partInstId = 0;

        // 设置配件的安装信息
        pPart.SetInstallInfo(0, -1);
    }

    /// <summary>
    /// 显示槽位链接模型
    /// </summary>
    // public void ShowModel()
    // {
    //     foreach(Model pModel in this.myModelList)
    //     {
    //         pModel.meshRenderer.enabled = true;
    //         pModel.collider.enabled = true;
    //     }
    // }
    
    /// <summary>
    /// 隐藏槽位链接模型
    /// </summary>
    // public void HideModel()
    // {
    //     foreach(Model pModel in this.myModelList)
    //     {
    //         pModel.meshRenderer.enabled = false;
    //         pModel.collider.enabled = false;
    //     }
    // }

    public static int GetRobotPartHoleModelListCount(int nCfgId)
    {
        JArray arrHole = GetRobotPartHoleRandomCoord(nCfgId);
        return arrHole.Count;
    }

    //----------------------------------------------------------------------------------------------------
    //读表操作

    public static RobotPartType GetRobotPartHolePartType(int nCfgId)
    {
        return (RobotPartType)ConfigManager.GetValue<int>("Slot_C", nCfgId, "slotType");
    }

    public static JArray GetRobotPartHoleRandomCoord(int nCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("Slot_C", nCfgId, "randomCoord"));
    }

    public static int GetRobotPartHoleModelId(RobotPartType eType)
    {
        int nModelCfgId = 0;
        switch(eType)
        {
            case RobotPartType.Body:
                nModelCfgId = ConfigManager.GetValue<int>("GameParam_C", "MainSoltModel");
                break;
            case RobotPartType.Weapon:
                nModelCfgId = ConfigManager.GetValue<int>("GameParam_C", "AttackSoltModel");
                break;
            case RobotPartType.Move:
                nModelCfgId = ConfigManager.GetValue<int>("GameParam_C", "MoveSoltModel");
                break;
            case RobotPartType.Assist:
                nModelCfgId = ConfigManager.GetValue<int>("GameParam_C", "AuxiliarySoltModel");
                break;
            case RobotPartType.Ornament:
                nModelCfgId = ConfigManager.GetValue<int>("GameParam_C", "DecorateSoltModel");
                break;
        }
        return nModelCfgId;
    }

    /// <summary>
    /// 获取槽位对应的挂点
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public static string GetRobotPartHolePoint(int nCfgId, int nIdx)
    {
        JArray ary = JArray.FromObject(ConfigManager.GetValue<object>("Slot_C", nCfgId, "boneName"));
        if (nIdx >= ary.Count)
        {
            return "";
        }
        return (string)ary[nIdx];
    }

    public static RobotPartHoleType GetRobotPartHoleType(int nCfgId)
    {
        return (RobotPartHoleType)ConfigManager.GetValue<int>("Slot_C", nCfgId, "boneType");
    }
}
