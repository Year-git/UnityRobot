using UnityEngine;
using FairyGUI;
using LuaInterface;
using Newtonsoft.Json.Linq;

/// <summary>
/// Lua函数扩展
/// </summary>
public static class LuaExtend
{
    public static int GetMapProgressValue()
    {
        return MapManager.Instance.progressValue;
    } 

    public static int GetMapProgressTotal()
    {
        return MapManager.Instance.progressTotal;
    }

    public static int GetMapId(){
        return MapManager.Instance.curMapCfgId;
    }

    /// <summary>
    /// 创建带有UI标识的Npc
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="luaFun"></param>
    /// <param name="luaTable"></param>
    /// <returns>返回Npc实例Id</returns>
    public static int CreatNpcGameObj(int nCfgId, LuaFunction luaFun, LuaTable luaTable = null)
    {
        if (!(nCfgId > 0))
        {
            return 0;
        }

        if (MapManager.Instance.baseMap == null)
        {
            return 0;
        }

        BaseNpc pNewNpc = MapManager.Instance.baseMap.CreatNpc(
            nCfgId,
            Vector3.zero,
            Vector3.zero,
            null,
            delegate(BaseNpc pNpc)
            {
                    pNpc.isUiNpc = true;
                    if (luaFun != null)
                    {
                        luaFun.BeginPCall();
                        if (luaTable != null)
                        {
                            luaFun.Push(luaTable);
                        }
                        luaFun.Push(pNpc.InstId);
                        luaFun.PCall();
                        luaFun.EndPCall();
                    }
            }
        );

        return pNewNpc.InstId;
    }

    /// <summary>
    /// 获取Npc的GameObject
    /// </summary>
    /// <param name="nNpcInstId">Npc实例Id</param>
    /// <returns></returns>
    public static GameObject GetNpcGameObj(int nNpcInstId)
    {
        if (MapManager.Instance.baseMap == null)
        {
            return null;
        }

        return MapManager.Instance.baseMap.GetNpc(nNpcInstId).myModel.gameObject;
    }

    /// <summary>
    /// 删除Npc
    /// </summary>
    /// <param name="nNpcInstId">Npc实例Id</param>
    /// <returns></returns>
    public static void RemoveNpcGameObj(int nNpcInstId)
    {
        MapManager.Instance.baseMap.RemoveNpc(nNpcInstId);
    }

    /// <summary>
    /// 克隆Npc配件
    /// </summary>
    /// <param name="nOriginalNpcInstId">源Npc实例Id</param>
    /// <param name="nTargetNpcInstId">目标Npc实例Id</param>
    /// <param name="luaFun"></param>
    /// <param name="luaTable"></param>
    public static void NpcRobotPartClone(int nOriginalNpcInstId, int nTargetNpcInstId, LuaFunction luaFun, LuaTable luaTable = null)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc.NpcRobotPartClone(
            pMap.GetNpc(nOriginalNpcInstId),
            pMap.GetNpc(nTargetNpcInstId),
            delegate(){
                if (luaFun != null)
                {
                    luaFun.BeginPCall();
                    if (luaTable != null)
                    {
                        luaFun.Push(luaTable);
                    }
                    luaFun.PCall();
                    luaFun.EndPCall();
                }
            }
        );
    }

    /// <summary>
    /// Npc配件安装
    /// </summary>
    /// <param name="nNpcInstId">Npc实例Id</param>
    /// <param name="nHoleIdx">Npc槽位索引</param>
    /// <param name="nPartCfgId">配件配置Id</param>
    /// <param name="luaFun"></param>
    /// <param name="luaTable"></param>
    public static void NpcRobotPartInstall(int nNpcInstId, int nHoleIdx, int nPartCfgId, LuaFunction luaFun, LuaTable luaTable = null)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        RobotPartHole pHole = pNpc.GetRobotPartHole(nHoleIdx);
        RobotPartType pPartType = RobotPart.GetRobotPartType(nPartCfgId);
        if (pPartType != pHole.partType)
        {
            Debug.LogError("类型不符   零件类型=="+pPartType+"  槽位类型=="+pHole.partType);
            return;//类型不符
        }
        int nNum = pHole.myModelList.Count;
        MapManager.Instance.baseMap.RobotPartCreate(nPartCfgId, nNum, pNpc.myScale, delegate (RobotPart pPart)
            {
                pNpc.RobotPartInstall(nHoleIdx, pPart.InstId, delegate ()
                    {
                        if (luaFun != null)
                        {
                            luaFun.BeginPCall();
                            if (luaTable != null)
                            {
                                luaFun.Push(luaTable);
                            }
                            luaFun.PCall();
                            luaFun.EndPCall();
                        }
                    }
                );
            }
        );
    }

    /// <summary>
    /// Npc配件卸载
    /// </summary>
    /// <param name="nNpcInstId">Npc实例Id</param>
    /// <param name="nHoleIdx">配件实例Id</param>
    public static void NpcRobotPartUnInstall(int nNpcInstId, int nHoleIdx)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        int nPartInstId = pNpc.GetRobotPartHole(nHoleIdx).partInstId;
        pNpc.RobotPartUnInstall(nHoleIdx);
    }

    /// <summary>
    /// 玩家组装开始通知
    /// </summary>
    public static void PlayerAssembleStart()
    {
        MapManager.Instance.baseMap.PlayerAssembleStart();
    }

    /// <summary>
    /// 玩家组装结束通知
    /// </summary>
    public static void PlayerAssembleEnd()
    {
        MapManager.Instance.baseMap.PlayerAssembleEnd();
    }

    /// <summary>
    /// 获取玩家已通关的关卡Id
    /// </summary>
    /// <returns></returns>
    public static int GetPlayerPassGameLevel()
    {
        return MyPlayer.passGameLevelId;
    }

    public static void EnterMap(int nMapCfgID)
    {
        MapManager.Instance.LoadMap(nMapCfgID);
    }

    /// <summary>
    /// 进入游戏关卡【客户端临时接管服务器服务器的C_EnterMap回调】
    /// </summary>
    /// <param name="GameLevel"></param>
    public static void EnterGameLevel(int GameLevel)
    {
        MapManager.Instance.ServerToEnterGameLevel(GameLevel, new JArray());
    }

    /// <summary>
    /// 离开关卡
    /// </summary>
    public static void LeaveGameLevel()
    {
        MapManager.Instance.ServerToLeaveMap();
    }

    /// <summary>
    /// 获取Npc槽位数量
    /// </summary>
    /// <returns></returns>
    public static int GetNpcHoleCount(int nNpcInstId)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        return pNpc.GetRobotPartHoleContainer().Count;
    }

    /// <summary>
    /// 获取玩家槽位数量
    /// </summary>
    /// <returns></returns>
    public static int GetPlayerHoleCount()
    {
        return GetNpcHoleCount(MyPlayer.playerInstId);
    }

    /// <summary>
    /// 获取Npc指定索引的槽位类型
    /// </summary>
    /// <param name="nNpcInstId"></param>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public static int GetNpcHoleType(int nNpcInstId, int nIdx)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        return (int)pNpc.GetRobotPartHole(nIdx).partType;
    }

    /// <summary>
    /// 获取玩家指定索引的槽位类型
    /// </summary>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public static int GetPlayerHoleType(int nIdx)
    {
        return GetNpcHoleType(MyPlayer.player.InstId, nIdx);
    }

    /// <summary>
    /// 获取Npc指定索引的槽位中的配件配置Id
    /// </summary>
    /// <param name="nNpcInstId"></param>
    public static int GetNpcPartCfgIdByHoleIdx(int nNpcInstId, int nIdx)
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        BaseNpc pNpc = pMap.GetNpc(nNpcInstId);
        RobotPart pPart = pMap.GetRobotPart(pNpc.GetRobotPartHole(nIdx).partInstId);
        if (pPart == null)
        {
            return 0;
        }
        return pPart.cfgId;
    }

    /// <summary>
    /// 游戏暂停
    /// </summary>
    /// <param name="bPause"></param>
    public static void SetGamePause(bool bPause)
    {
        FrameSynchronManager.Instance.fsData.PauseState = bPause;
    }

    /// <summary>
    /// 槽位特效
    /// </summary>
    public static int CreatHoleEffect(int holeid,int InstId)
    {
        //先销毁
        DestroyHoleEffect(InstId);
        
        BaseNpc mynpc= MapManager.Instance.baseMap.GetNpc(MyPlayer.playerInstId);
        foreach(Transform item in mynpc.myModel.transform.GetComponentInChildren<Transform>())
        {
            if(item.name==holeid.ToString())
            {
               return MapManager.Instance.baseMap.effectManager.BodyEffectAdd(11,item);
            }
        }
        return -1;	
        
    }
    ///<summary>
    /// 删除槽位特效
    ///</summary>
    public static void DestroyHoleEffect(int InstId)
    {
        EffectPacket effinst= MapManager.Instance.baseMap.effectManager.GetEffectPacket(InstId);
        if(effinst!=null)
        {
            MapManager.Instance.baseMap.effectManager.EffectDel(effinst.InstId);
        }
    }

    ///<summary>
    ///战力示意图
    ///</summary>
    public static void PlayerBattleMap(Shape shape)
    {
        shape.DrawRegularPolygon(5, 2, Color.white, shape.color, shape.color, -18, new float[] { 1f, 1f, 1f, 1f, 1f });
    }

    ///<summary>
    ///加载战斗胜利特效
    ///</summary>
    public static void CreateVictoryObj(string sModelName,LuaFunction luaFun,LuaTable luaTable = null)
    {
        Model mo=new Model(null, sModelName, Vector3.zero, Vector3.zero, 
		1f, delegate (Model model)
            {
				if(model!=null)
				{
					if (luaFun != null)
                    {
                        luaFun.BeginPCall();
                        if (luaTable != null)
                        {
                            luaFun.Push(luaTable);
                        }
                        luaFun.Push(model.gameObject);
                        luaFun.PCall();
                        luaFun.EndPCall();
                    }
                }
            });
    }
}