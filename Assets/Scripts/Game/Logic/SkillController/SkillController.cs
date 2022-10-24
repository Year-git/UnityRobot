using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController
{
    public BaseNpc myNpc {get; private set;}

    /// <summary>
    /// 技能容器
    /// </summary>
    /// <returns></returns>
    private Dictionary<int, Skill> _skillContainer = new Dictionary<int, Skill>();

    private SkillController(){}

    public SkillController(BaseNpc pNpc)
    {
        this.myNpc = pNpc;
    }

    public void Update()
    {
        foreach(Skill pSkill in this._skillContainer.Values)
        {
            pSkill.Update();
            if(pSkill.isAuto && myNpc.IsAutoSkill()){
                pSkill.DoStart();
            }
        }
    }

    public void AddSkill(int nSkillCfgId, int nPartInstId)
    {
        if (nSkillCfgId <= 0)
        {
            return;
        }

        if (!this.IsHaveSkill(nSkillCfgId))
        {
            this._skillContainer.Add(nSkillCfgId, new Skill(nSkillCfgId, this));
        }
        this._skillContainer[nSkillCfgId].Regist(nPartInstId);
    }

    public void DelSkill(int nSkillCfgId, int nPartInstId)
    {
        if (nSkillCfgId <= 0)
        {
            return;
        }

        if (!this.IsHaveSkill(nSkillCfgId))
        {
            return;
        }

        this._skillContainer[nSkillCfgId].UnRegist(nPartInstId);
    }

    public bool IsHaveSkill(int nSkillCfgId)
    {
        if (this._skillContainer.ContainsKey(nSkillCfgId))
        {
            return true;
        }
        return false;
    }

    public Skill GetSkill(int nSkillCfgId)
    {
        if (!this.IsHaveSkill(nSkillCfgId))
        {
            return null;
        }
        return this._skillContainer[nSkillCfgId];
    }

    public bool SkillDo(int nSkillCfgId)
    {
        if (!this.IsHaveSkill(nSkillCfgId))
        {
            return false;
        }

        if (this.myNpc.isDead)
        {
            return false;
        }

        return this._skillContainer[nSkillCfgId].DoStart();
    }
}
