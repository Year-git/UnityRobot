#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[KTDisplayName("战斗阶段")]
public class KTLevelFightStage : KTLevelEntityAdjust
{
    protected override string uuidPrefix
    {
        get
        {
            return "fightStages";
        }
    }
    [KTLevelExport("id")]
    public int id;

    [LabelText("出生点"), KTLevelExport("__bornLocation"), KTLevelExport("__bornLocation_uuid")]
    public KTLevelSpawnPoint spawnPoint;

    [LabelText("战斗区域"), KTLevelExport("__area"), KTLevelExport("__area_uuid")]
    public KTLevelPatrolZone area;

    [LabelText("孵化器"), KTLevelExport("__monsterGenerator"), KTLevelExport("__monsterGenerator_uuid")]
    public KTLevelSpawner spawner;

    [HideInInspector, KTLevelExport("__children"), KTLevelExport("__children_uuid")]
    public List<KTLevelEntity> entities;

    public override void PreExport()
    {
        entities = new List<KTLevelEntity>();
        entities=this.GetComponentsInChildren<KTLevelEntity>().Where(item => KTLevelTools.GetAttribute<KTLevelClassIgnoreExportAttribute>(item.GetType()) == null).ToList();//by lijunfeng 过滤掉不可导出实体
        entities.Remove(this);
    }

    public override void PostExport()
    {
        entities = null;
    }
}
#endif
