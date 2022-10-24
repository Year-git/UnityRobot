using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBehaviacController : BaseBehaviacController
{
    public BaseMap myMap{get; private set;}
    public string myTreeName{get; private set;}
    public MapBehaviacController(GameObject pGameObj, BaseMap pMap) : base(pGameObj)
    {
        myMap = pMap;
        myTreeName = BaseMap.GetGameLevelDirectorName(pMap.gameLevelId);
    }

    /// <summary>
    /// 初始化行为树脚本组件
    /// </summary>
    /// <param name="pGameObj"></param>
    protected override void InitMyBehaviac(GameObject pGameObj)
    {
        // 添加Ai行为树的脚本组件
        this.myBehaviac = pGameObj.AddComponent<MapBehaviac>();
    }

    /// <summary>
    /// 刷新行为树
    /// </summary>
    /// <param name="sBehaviacTreeName"></param>
    public void UpdateBehaviacTree()
    {
        base.UpdateBehaviacTree("Map/Director/", this.myTreeName);
    }

    /// <summary>
    /// 派发游戏事件
    /// </summary>
    /// <param name="eEvent">Ai事件枚举</param>
    /// <param name="nDetailType">细分类型</param>
	public void DispatchGameEvent(BehaviacGameEvent eEvent, params object[] pParam)
	{
        base.DispatchGameEvent("Map/Director/", this.myTreeName, eEvent, pParam);
	}
}
