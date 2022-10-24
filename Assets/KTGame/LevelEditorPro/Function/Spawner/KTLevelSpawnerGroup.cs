#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;
using System.Linq;

/// <summary>
/// 孵化器群组
/// 为了兼容之前的孵化器，暂时让孵化器群组继承KTLevelSpawner
/// </summary>

[KTDisplayName("孵化器群组"), KTLevelGroup("spawners")]
public class KTLevelSpawnerGroup : KTLevelSpawner
{
    [HideInInspector, KTLevelExport("selectedRoleType")]
    public RoleType roleType = RoleType.spawner_group;

    [LabelText("孵化器群组")]
    public List<KTLevelSpawner> spawners;

    public override void PreExport()
    {
        CheckUniqueList(spawners, "导出错误, 孵化器群组存在重复点");

        if (spawners != null && spawners.Count > 0)
        {
            spawners.ForEach(s => s.PreExport());
        }
    }

    public override void RefreshView()
    {
        if(spawners!=null && spawners.Count>0)
        {
            spawners.ForEach(s =>s.RefreshView());
        }
    }
}

#endif