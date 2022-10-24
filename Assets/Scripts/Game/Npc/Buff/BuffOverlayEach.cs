using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffOverlayEach : Buff
{
    public BuffOverlayEach(BaseNpc pNpc, int nBuffCfgId, BuffOverlayType eType) : base(pNpc, nBuffCfgId, eType){}

    /// <summary>
    /// Buff初始化
    /// </summary>
    public override void Init()
    {
        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;
        this._layerLastTime.Add(_layerIdx++, nCurTime + this.durationTime);
        base.Init();
    }

    // 获取Buff层数
    public override int GetLayer()
    {
        return this._layerLastTime.Count;
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

        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;

        for(int i = 0; i < nNum; i++)
        {
            this._layerLastTime.Add(_layerIdx++, nCurTime + this.durationTime);
        }

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

        int nTargetLayerIdx = nLayerIdx;
        if (nTargetLayerIdx == -1)
        {
            foreach(var kvPair in _layerLastTime)
            {
                nTargetLayerIdx = kvPair.Key;
                break;
            }
        }
        this._layerLastTime.Remove(nTargetLayerIdx);

        base.LayerDec(nTargetLayerIdx);
    }

    /// <summary>
    /// 时间检查
    /// </summary>
    public override void TimeUpdate()
    {
        int nCurTime = FrameSynchronManager.Instance.fsData.FrameRunningTime;

        List<int> removeList = new List<int>();
        foreach(var kvPair in this._layerLastTime)
        {
            if (nCurTime >= kvPair.Value)
            {
                removeList.Add(kvPair.Key);
            }
        }
        foreach(int nLayerIdx in removeList)
        {
            this.LayerDec(nLayerIdx);
        }
    }
}
