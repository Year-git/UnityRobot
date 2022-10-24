using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class EffectMappingScript : MonoBehaviour
{
    private int _effectInstId;

    public void Init(int nEffInstId)
    {
        _effectInstId = nEffInstId;
    }

    public void Update()
    {
        BaseMap pMap = MapManager.Instance.baseMap;
        if (pMap == null)
        {
            return;
        }

        EffectManager pEffectManager = pMap.effectManager;
        if (pEffectManager == null)
        {
            return;
        }
        
        EffectPacket pEffect = pEffectManager.GetEffectPacket(_effectInstId);
        if (pEffect == null)
        {
            return;
        }
        pEffect.myModel.transform.position = transform.position;
        pEffect.myModel.transform.rotation = transform.rotation;
    }
    
    private void OnDestroy()
    {
        // 如果游戏不再运行，则直接返回
        if(!GameApplication.isGameRun){
            return;
        }
        MapManager.Instance.baseMap.effectManager.EffectDel(_effectInstId);
    }

    public void Destroy()
    {
        UnityEngine.GameObject.Destroy(gameObject);
    }
}
