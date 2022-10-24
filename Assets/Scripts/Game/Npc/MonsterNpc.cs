using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNpc : BaseNpc
{
    public bool isInitAttrEntity = false;
    public MonsterNpc(int nCfgId) : base(NpcType.MonsterNpc, nCfgId){}

    public override void OnNpcStart()
    {
        this.IsCanAddQueueNpcList = true;
    }
    
    public override void OnNpcUpdate()
    {
        base.OnNpcUpdate();
    }

    /// <summary>
    /// 计算怪物属性
    /// </summary>
    public override void NpcAttrCalculate()
    {
        if (this.isInitAttrEntity)
        {
            return;
        }
        this.isInitAttrEntity = true;

        foreach(var jData in BaseNpc.GetNpcAttribute(this.cfgId))
        {
            int nCfgId = (int)jData[0];
            int nAttrVal = (int)jData[1];

            AttributeType eType = (AttributeType)ConfigManager.GetValue<int>("Attribute_C", nCfgId, "attributeType");
            AttributeValueType eValueType = (AttributeValueType)ConfigManager.GetValue<int>("Attribute_C", nCfgId, "arithmeticType");

            if (eValueType == AttributeValueType.Base)
            {
                this.attrEntity.SetBaseValue(eType, nAttrVal);
            }
            else
            {
                // this.attrEntity.SetPercentValue(eType, nAttrVal);
            }
        }

        // this.attrEntity.SetBaseValue(AttributeType.Hp, 1000000);

        this.lastHp = this.attrEntity.GetBaseValue(AttributeType.Hp);
    }

    public override void NpcLayerSet()
    {
        this.NpcLayerNameOccupy();
    }

    public override void NpcLayerClear()
    {
        this.NpcLayerNameRelease();
    }
}
