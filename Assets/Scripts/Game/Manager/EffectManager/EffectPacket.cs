using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectPacket : MapObject {
    public int cfgId;
    public int useListIdx = -1;
    public int synListIdx = -1;
    public Model myModel;
    public EffectMappingScript effectMappingScript;
    
    // 是否正在等待删除
    public bool isAwaitDelete;

    public EffectPacket(){}
    
    public EffectPacket(int nEffCfgId, float nScale = 1f, Action<EffectPacket> fInitCall = null)
    {
        cfgId = nEffCfgId;
        LoadEffect(nEffCfgId, Vector3.zero, Vector3.zero, nScale, fInitCall);
    }

    public void Destroy()
    {
        myModel.Destroy();
    }

    /// <summary>
    /// 加载特效
    /// </summary>
    public void LoadEffect(int nEffCfgId, Vector3 pPosition, Vector3 pEulerAngles, float nScale, Action<EffectPacket> fInitCall = null)
    {
        string sModelName = "Assets/Res/Prefabs/Effect/" + EffectManager.GetEffectFileName(nEffCfgId);
        myModel = new Model(this, sModelName, pPosition, pEulerAngles, nScale, 
            delegate (Model Model)
                {
                    myModel = Model;
                    fInitCall?.Invoke(this);
                }
        );
    }
}