using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class EffectManager
{
    private Dictionary<int,EffectPacket> _effectManager = new Dictionary<int, EffectPacket>();
    private Dictionary<int,FreeUseList<EffectPacket>> _recoveryContainer = new Dictionary<int, FreeUseList<EffectPacket>>();
    private Dictionary<int,KeyValuePair<float,Action>> _recoveryCheckContainer = new Dictionary<int, KeyValuePair<float, Action>>();
    private GameObject _effectListGameObj;
    public GameObject effectListGameObj 
    {
        get
        {
            if (this._effectListGameObj == null)
            {
                this._effectListGameObj = new GameObject("EffectList");
                this._effectListGameObj.transform.position = Vector3.zero;
                this._effectListGameObj.transform.rotation = Quaternion.identity;
            }
            return this._effectListGameObj;
        }
        private set
        {
            this._effectListGameObj = value;
        }
    }

    /// <summary>
    /// 一次性特效的刷新回收
    /// </summary>
    public void UpdateEffect()
    {
        if(this._recoveryCheckContainer.Count == 0)
        {
            return;
        }
        var nCurTime = GTime.RealtimeSinceStartup;
        var recoverList = new List<int>();
        foreach(var kvPair in this._recoveryCheckContainer)
        {
            if (nCurTime >= kvPair.Value.Key)
            {
                recoverList.Add(kvPair.Key);
            }
        }
        foreach(int i in recoverList)
        {
            this._recoveryCheckContainer[i].Value?.Invoke();
            this._recoveryCheckContainer.Remove(i);
        }
    }

    /// <summary>
    /// 添加到特效回收检查表
    /// </summary>
    /// <param name="pEff">特效实例</param>
    /// <param name="fRecoveCall">触发回收的处理方法</param>
    public void EffectRecoveCheckAdd(EffectPacket pEff, Action fRecoveCall)
    {
        this._recoveryCheckContainer.Add(
            pEff.InstId,
            new KeyValuePair<float, Action>(GTime.RealtimeSinceStartup + EffectManager.GetEffectCfgEndTime(pEff.cfgId) / 1000f, delegate()
                {
                    fRecoveCall?.Invoke();
                }
            )
        );
    }

    /// <summary>
    /// 从特效回收检查表中移除
    /// </summary>
    /// <param name="pEff">特效实例</param>
    /// <param name="fRecoveCall">触发回收的处理方法</param>
    public void EffectRecoveCheckRemove(int nEffInstId)
    {
        if (!this._recoveryCheckContainer.ContainsKey(nEffInstId))
        {
            return;
        }
        this._recoveryCheckContainer.Remove(nEffInstId);
    }

    /// <summary>
    /// 获取特效实例
    /// </summary>
    /// <param name="nEffInstId">特效实例Id</param>
    /// <returns></returns>
    public EffectPacket GetEffectPacket(int nEffInstId)
    {
        if (!this._effectManager.ContainsKey(nEffInstId))
        {
            return null;
        }
        EffectPacket pEffect = this._effectManager[nEffInstId];
        if (pEffect == null)
        {
            return null;
        }
        int nCfgId = pEffect.cfgId;
        if (EffectManager.GetEffectCfgEndTime(nCfgId) > 0)
        {
            if (!this._recoveryContainer.ContainsKey(nCfgId))
            {
                return null;
            }
            if (pEffect.useListIdx < 0)
            {
                return null;
            }
            return this._recoveryContainer[nCfgId].GetUseItem(pEffect.useListIdx);
        }
        return this._effectManager[nEffInstId];
    }

    /// <summary>
    /// 添加特效
    /// </summary>
    /// <param name="nEffCfgId">特效实例Id</param>
    /// <param name="fInitAction">特效初始化处理</param>
    /// <param name="fRecoveAction">特效回收处理</param>
    /// <returns></returns>
    private int EffectAdd(int nEffCfgId, float nScale = 1f, Action<EffectPacket> fInitAction = null)
    {
        EffectPacket pEffect;

        Action<EffectPacket> fEffectAddInitAction = delegate(EffectPacket pEff)
        {
            fInitAction?.Invoke(pEff);
            pEff.myModel.gameObject.SetActive(true);

            if (!pEff.isAwaitDelete)
            {
                return;
            }
            this.EffectDel(pEff.InstId);
        };

        bool bIsLoop = EffectManager.GetEffectCfgIsLoop(nEffCfgId);
        if (bIsLoop)
        {
            pEffect = new EffectPacket(nEffCfgId, nScale, fEffectAddInitAction);
        }
        else
        {
            Action<EffectPacket> fInitActionAttachRecove = delegate(EffectPacket pEff)
                {
                    fEffectAddInitAction?.Invoke(pEff);
                    this.EffectRecoveCheckAdd(pEff, delegate()
                        {
                            this.EffectDel(pEff.InstId);
                        }
                    );
                };

            if (!this._recoveryContainer.ContainsKey(nEffCfgId))
            {
                this._recoveryContainer.Add(nEffCfgId, new FreeUseList<EffectPacket>());
            }

            KeyValuePair<int, EffectPacket> kyPair = this._recoveryContainer[nEffCfgId].Assign(fInitActionAttachRecove, nEffCfgId, nScale, fInitActionAttachRecove);
            pEffect = kyPair.Value;
            pEffect.isAwaitDelete = false;
            pEffect.useListIdx = kyPair.Key;
        }

        if (!this._effectManager.ContainsKey(pEffect.InstId))
        {
            this._effectManager.Add(pEffect.InstId, pEffect);
        }

        return pEffect.InstId;
    }

    /// <summary>
    /// 删除特效
    /// </summary>
    /// <param name="nEffInstId">特效实例Id</param>
    public void EffectDel(int nEffInstId)
    {
        EffectPacket pEffect = this.GetEffectPacket(nEffInstId);
        if (pEffect == null)
        {
            return;
        }

        pEffect.isAwaitDelete = true;
        // 模型没有加载出来等待模型加载回调
        if(pEffect.myModel.gameObject == null){
            return;
        }
        pEffect.myModel.gameObject.SetActive(false);

        if (pEffect.effectMappingScript != null)
        {
            pEffect.effectMappingScript.Destroy();
            pEffect.effectMappingScript = null;
        }

        if (EffectManager.GetEffectCfgIsLoop(pEffect.cfgId))
        {
            this._effectManager.Remove(nEffInstId);
            pEffect.Destroy();
        }
        else
        {
            this.EffectRecoveCheckRemove(nEffInstId);
            pEffect.isAwaitDelete = false;
            this._recoveryContainer[pEffect.cfgId].Recover(pEffect.useListIdx);
            pEffect.useListIdx = -1;
        }
    }


    /// <summary>
    /// 添加场景特效
    /// </summary>
    /// <param name="nEffCfgId">特效实例Id</param>
    /// <param name="pPosition">坐标</param>
    /// <param name="pEulerAngles">转向</param>
    /// <returns>返回特效的实例Id</returns>
	public int SceneEffectAdd(int nEffCfgId, Vector3 pPosition, Quaternion pQuaternion, float nScale = 1f)
    {
        if (nEffCfgId <= 0 || pPosition == null || pQuaternion == null)
        {
            return 0;
        }

        Action<EffectPacket> fInitAction = delegate(EffectPacket pEff)
            {
                pEff.myModel.transform.parent = this.effectListGameObj.transform;
                pEff.myModel.transform.position = pPosition;
                pEff.myModel.transform.rotation = pQuaternion;
                pEff.myModel.transform.localScale = new Vector3(nScale, nScale, nScale);
            };
        
        return this.EffectAdd(nEffCfgId, nScale, fInitAction);
    }

    /// <summary>
    /// 添加及身特效
    /// </summary>
    /// <param name="nEffCfgId"></param>
    /// <param name="pParent"></param>
    /// <returns></returns>
	public int BodyEffectAdd(int nEffCfgId, Transform pParent, float nScale = 1f)
    {
        if (nEffCfgId <= 0 || pParent == null)
        {
            return 0;
        }

        Action<EffectPacket> fInitAction = delegate(EffectPacket pEff)
            {
                pEff.myModel.transform.parent = this.effectListGameObj.transform;
                GameObject newGameObj = new GameObject(EffectManager.GetEffectFileName(pEff.cfgId));
                newGameObj.transform.parent = pParent;
                newGameObj.transform.localPosition = Vector3.zero;
                newGameObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                pEff.effectMappingScript = newGameObj.AddComponent<EffectMappingScript>();
                pEff.effectMappingScript.Init(pEff.InstId);
            };
        
        return this.EffectAdd(nEffCfgId, nScale, fInitAction);
    }

	
    /// <summary>
    /// 清理所有特效
    /// </summary>
	public void Clear()
    {
        foreach(var kvPair in this._effectManager)
        {
            kvPair.Value.Destroy();
        }
        this._effectManager = new Dictionary<int, EffectPacket>();
        this._recoveryContainer = new Dictionary<int, FreeUseList<EffectPacket>>();
    }

    /// <summary>
    /// 通过特效配置Id获取特效名
    /// </summary>
    /// <param name="nCfgId">特效配置Id</param>
    /// <returns></returns>
    public static string GetEffectFileName(int nCfgId)
    {
        return ConfigManager.GetValue<string>("Effect_C", nCfgId, "strName");
    }

    /// <summary>
    /// 通过特效配置Id获取特效是否循环
    /// </summary>
    /// <param name="nCfgId">特效配置Id</param>
    /// <returns></returns>
    public static bool GetEffectCfgIsLoop(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Effect_C", nCfgId, "time") == 0;
    }

    /// <summary>
    /// 通过特效配置Id获取特效结束时间（毫秒）
    /// </summary>
    /// <param name="nCfgId">特效配置Id</param>
    /// <returns></returns>
    public static int GetEffectCfgEndTime(int nCfgId)
    {
        return ConfigManager.GetValue<int>("Effect_C", nCfgId, "time");
    }
}
