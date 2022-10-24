using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildNpc : BaseNpc
{
    public BuildNpc(int nCfgId) : base(NpcType.BuildNpc, nCfgId)
    {
    }

    public override void OnNpcStart()
    {
        this.myModel.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public override void NpcLayerSet()
    {
        this.myLayerName = "Obstacle";
    }

    /// <summary>
    /// 计算建筑属性
    /// </summary>
    public override void NpcAttrCalculate()
    {
        if (this.attrEntity != null)
        {
            return;
        }

        this.attrEntity = new AttributeEntity();
        
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

        this.lastHp = this.attrEntity.GetBaseValue(AttributeType.Hp);
    }
}