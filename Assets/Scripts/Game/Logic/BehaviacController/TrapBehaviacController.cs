using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviacController : BaseBehaviacController
{
    public LevelTrapController myLevelController{get; private set;}
    public string myTreeName{get; private set;}
    public TrapBehaviacController(GameObject pGameObj, LevelTrapController pLevelController) : base(pGameObj)
    {
        this.myLevelController = pLevelController;
        this.myTreeName = pLevelController.aiTreeName;
    }

    /// <summary>
    /// 初始化行为树脚本组件
    /// </summary>
    /// <param name="pGameObj"></param>
    protected override void InitMyBehaviac(GameObject pGameObj)
    {
        // 添加Ai行为树的脚本组件
        this.myBehaviac = pGameObj.AddComponent<TrapBehaviac>();
    }

    /// <summary>
    /// 刷新行为树
    /// </summary>
    /// <param name="sBehaviacTreeName"></param>
    public void UpdateBehaviacTree()
    {
        base.UpdateBehaviacTree("Map/Trap/", this.myTreeName);
    }

    /// <summary>
    /// 派发游戏事件
    /// </summary>
    /// <param name="eEvent">Ai事件枚举</param>
    /// <param name="nDetailType">细分类型</param>
	public void DispatchGameEvent(BehaviacGameEvent eEvent, params object[] pParam)
	{
        // UnityEngine.Debug.Log("Log->TrapBehaviacController.DispatchGameEvent->" + "#eEvent = " + eEvent.ToString() + "#pParam[0] = " + pParam[0].ToString() + "#pParam[1] = " + pParam[1].ToString());
        base.DispatchGameEvent("Map/Trap/", this.myTreeName, eEvent, pParam);
	}
}
