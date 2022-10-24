using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffOverlayQueue : Buff
{
    public BuffOverlayQueue(BaseNpc pNpc, int nBuffCfgId, BuffOverlayType eType) : base(pNpc, nBuffCfgId, eType){}

    /// <summary>
    /// Buff初始化
    /// </summary>
    public override void Init()
    {
        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;

        this.curLayer = 1;
        this._layerLastTime.Add(-1, nCurTime + this.durationTime);
        base.Init();
    }

    // 获取Buff层数
    public override int GetLayer()
    {
        return this.curLayer;
    }

    /// <summary>
    /// Buff层数增加
    /// </summary>
    /// <param name="nNum"></param>
    public override void LayerInc(int nNum)
    {
        if (nNum <= 0)
        {
            return;
        }

        this.curLayer += nNum;

        base.LayerInc(nNum);
    }

    /// <summary>
    /// Buff层数减少
    /// </summary>
    /// <param name="nLayerIdx"></param>
    public override void LayerDec(int nLayerIdx = -1)
    {
        if (this.GetLayer() <= 0)
        {
            return;
        }

        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;

        this.curLayer--;
        this._layerLastTime[nLayerIdx] = nCurTime + this.durationTime;

        base.LayerDec(nLayerIdx);
    }

    /// <summary>
    /// 时间检查
    /// </summary>
    public override void TimeUpdate()
    {
        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;
        
        if (nCurTime >= this._layerLastTime[-1])
        {
            this.LayerDec();
        }
    }
}
