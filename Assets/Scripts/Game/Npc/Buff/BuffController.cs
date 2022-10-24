using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuffController
{
    public Dictionary<int, Buff> _buffContainer = new Dictionary<int, Buff>();

    private BaseNpc myNpc;

    private BuffController(){}

    public BuffController(BaseNpc pNpc)
    {
        this.myNpc = pNpc;
    }

    /// <summary>
    /// 获取Buff
    /// </summary>
    /// <param name="nBuffCfgId"></param>
    /// <returns></returns>
    public Buff GetBuff(int nBuffCfgId)
    {
        if(!this._buffContainer.ContainsKey(nBuffCfgId))
        {
            return null;
        }
        return this._buffContainer[nBuffCfgId];
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="nBuffCfgId"></param>
    public void BuffAdd(int nBuffCfgId)
    {
        if (nBuffCfgId <= 0)
        {
            return;
        }

        if(this._buffContainer.ContainsKey(nBuffCfgId))
        {
            this._buffContainer[nBuffCfgId].LayerInc();
        }
        else
        {
            // --------------------------------------------------------------------------------------
            // 检查行为树在Buff_OnBuffAdd事件中返回的该Buff是否可被添加，如果有不可以的，则不添加该Buff
            bool bCanAdd = true;
            foreach(Buff pBuff in this._buffContainer.Values)
            {
                // 派发Buff_OnBuffAdd事件到Npc身上所有Buff的行为树中
                this.myNpc.myBehaviacController.DispatchGameEventToBuff(pBuff, BehaviacGameEvent.Buff_OnBuffAdd, nBuffCfgId);
                if (bCanAdd == true && !this.myNpc.myBehaviacController.IsBuffCanAdd())
                {
                    bCanAdd = false;
                }
                this.myNpc.myBehaviacController.ResetIsBuffCanAdd();
            }

            if (bCanAdd == false)
            {
                return;
            }
            // --------------------------------------------------------------------------------------

            Buff pNewBuff;
            BuffOverlayType eType = Buff.GetBuffOverlayType(nBuffCfgId);
            switch(eType)
            {
                case BuffOverlayType.Queue:
                    pNewBuff = new BuffOverlayQueue(this.myNpc, nBuffCfgId, eType);
                    break;
                case BuffOverlayType.Share:
                    pNewBuff = new BuffOverlayShare(this.myNpc, nBuffCfgId, eType);
                    break;
                case BuffOverlayType.Each:
                    pNewBuff = new BuffOverlayEach(this.myNpc, nBuffCfgId, eType);
                    break;
                default:
                    pNewBuff = new BuffOverlayQueue(this.myNpc, nBuffCfgId, eType);
                    break;
            }
            this._buffContainer.Add(nBuffCfgId, pNewBuff);
            pNewBuff.Init();
        }
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    /// <param name="nBuffCfgId"></param>
    public void BuffDel(int nBuffCfgId)
    {
        if (nBuffCfgId <= 0)
        {
            return;
        }

        if (!this._buffContainer.ContainsKey(nBuffCfgId))
        {
            return;
        }

        // 派发Buff_OnBuffRemove事件到Npc身上所有Buff的行为树中
        foreach(Buff pBuff in this._buffContainer.Values)
        {
            this.myNpc.myBehaviacController.DispatchGameEventToBuff(pBuff, BehaviacGameEvent.Buff_OnBuffRemove, nBuffCfgId);
        }

        this._buffContainer[nBuffCfgId].Remove();
        this._buffContainer.Remove(nBuffCfgId);
    }

    /// <summary>
    /// Npc所有Buff刷新
    /// </summary>
    public void BuffTimeUpdate()
    {
        List<int> removeList = new List<int>();
        foreach(var kvPair in this._buffContainer)
        {
            kvPair.Value.TimeUpdate();

            if(kvPair.Value.GetLayer() == 0)
            {
                removeList.Add(kvPair.Key);
            }
        }

        foreach(int nBuffCfgId in removeList)
        {
            this.BuffDel(nBuffCfgId);
        }
    }

    /// <summary>
    /// Npc遍历所有Buff
    /// </summary>
    public void BuffLoop(Action<Buff> pLoop)
    {
        foreach(Buff pBuff in this._buffContainer.Values)
        {
            pLoop?.Invoke(pBuff);
        }
    }

    /// <summary>
    /// Npc死亡通知
    /// </summary>
    public void OnNpcDead()
    {
        List<int> removeList = new List<int>();
        foreach(var kvPair in this._buffContainer)
        {
            if(kvPair.Value.isDeadRemove)
            {
                removeList.Add(kvPair.Key);
            }
        }

        foreach(int nBuffCfgId in removeList)
        {
            this.BuffDel(nBuffCfgId);
        }
    }
}