using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public abstract class Buff
{
    protected static int _layerIdx = 1;
    protected BaseNpc myNpc;
    public int curLayer {get; protected set;} = -1;

    public int cfgId {get; private set;}
    public int durationTime {get; private set;}
    public int typeId {get; private set;}
    public JArray paramData {get; private set;}
    public int nameId {get; private set;}
    public string behaviacName {get; private set;}
    public int maxLayer {get; private set;}
    public BuffOverlayType overlayType {get; private set;}
    public bool isDeadRemove {get; private set;}

    public Dictionary<int, int> _layerLastTime {get; private set;} = new Dictionary<int, int>();

    private Buff(){}

    public Buff(BaseNpc pNpc, int nBuffCfgId, BuffOverlayType eType)
    {
        this.myNpc = pNpc;
        this.cfgId = nBuffCfgId;
        this.overlayType = eType;

        // 缓存配置
        this.durationTime = Buff.GetBuffDurationTime(this.cfgId);
        this.typeId = Buff.GetBuffType(this.cfgId);
        this.paramData = Buff.GetBuffParam(this.cfgId);
        this.nameId = Buff.GetBuffNameId(this.typeId);
        this.behaviacName = Buff.GetBuffBehaviacName(this.typeId);
        this.maxLayer = Buff.GetBuffMaxLayer(this.typeId);
        this.isDeadRemove = Buff.GetBuffIsDeadRemove(this.typeId);
    }

    /// <summary>
    /// Buff初始化
    /// </summary>
    public virtual void Init()
    {
        // 派发Buff添加事件到Buff行为树
        this.myNpc.myBehaviacController.DispatchGameEventToBuff(this, BehaviacGameEvent.Buff_OnAdd);
        // 派发Buff层数增加事件到Buff行为树
        this.myNpc.myBehaviacController.DispatchGameEventToBuff(this, BehaviacGameEvent.Buff_OnLayerInc);
    }

    // 获取Buff名称[临时]
    public string GetName()
    {
        return this.nameId.ToString();
    }

    // 获取Buff层数
    public virtual int GetLayer()
    {
        return -1;
    }

    /// <summary>
    /// Buff层数增加
    /// </summary>
    /// <param name="nNum"></param>
    public virtual void LayerInc(int nNum = 1)
    {
        // 派发Buff层数增加事件到Buff行为树
        for(int i = 0; i < nNum; i++)
        {
            this.myNpc.myBehaviacController.DispatchGameEventToBuff(this, BehaviacGameEvent.Buff_OnLayerInc);
        }
    }

    /// <summary>
    /// Buff层数减少
    /// </summary>
    /// <param name="nLayerIdx"></param>
    public virtual void LayerDec(int nLayerIdx = -1)
    {
        // 派发Buff层数减少事件到Buff行为树
        this.myNpc.myBehaviacController.DispatchGameEventToBuff(this, BehaviacGameEvent.Buff_OnLayerDec);
    }

    /// <summary>
    /// Buff移除
    /// </summary>
    public void Remove()
    {
        // 如果Buff还有层数，则循环减少
        int nNum = this.GetLayer();
        if (nNum > 0)
        {
            for(int i = 0; i < nNum; i++)
            {
                this.LayerDec();
            }
        }

        // 派发Buff移除事件到Buff行为树
        this.myNpc.myBehaviacController.DispatchGameEventToBuff(this, BehaviacGameEvent.Buff_OnRemove);
    }

    /// <summary>
    /// 时间检查
    /// </summary>
    public virtual void TimeUpdate(){ }

    // ---------------------------------------------------------------------
    // Buff读表操作
    public static int GetBuffType(int nBuffCfgId)
    {
        return ConfigManager.GetValue<int>("BuffCfg_C", nBuffCfgId, "buffTypeID");
    }

    public static int GetBuffDurationTime(int nBuffCfgId)
    {
        return ConfigManager.GetValue<int>("BuffCfg_C", nBuffCfgId, "duration");
    }
    public static JArray GetBuffParam(int nBuffCfgId)
    {
        return JArray.FromObject(ConfigManager.GetValue<object>("BuffCfg_C", nBuffCfgId, "param"));
    }

    public static int GetBuffNameId(int nBuffTypeId)
    {
        return ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "buffName");
    }

    public static string GetBuffBehaviacName(int nBuffTypeId)
    {
        return ConfigManager.GetValue<string>("BuffTypeCfg_C", nBuffTypeId, "strAiName");
    }

    public static int GetBuffMaxLayer(int nBuffTypeId)
    {
        return ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "layer");
    }

    public static BuffOverlayType GetBuffOverlayType(int nBuffTypeId)
    {
        return (BuffOverlayType)ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "overlayType");
    }

    public static bool GetBuffIsDeadRemove(int nBuffTypeId)
    {
        return ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "disappear") == 0;
    }

    public static int GetBuffElementType(int nBuffTypeId)
    {
        return ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "elementType");
    }

    public static int GetBuffEffectType(int nBuffTypeId)
    {
        return ConfigManager.GetValue<int>("BuffTypeCfg_C", nBuffTypeId, "effectType");
    }
}

