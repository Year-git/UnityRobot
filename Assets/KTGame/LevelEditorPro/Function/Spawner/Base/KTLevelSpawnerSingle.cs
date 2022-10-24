#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;
using System.Linq;

/// <summary>
/// 单一孵化器类
/// </summary>
public class KTLevelSpawnerSingle : KTLevelSpawner
{


    [System.Serializable]
    public class PeriodicSpawnPolicy
    {
        [KTLevelExport("generateNum")]
        [LabelText("最大数量")]
        public int maxNum;

        [KTLevelExport("intervalTime")]
        [LabelText("时间间隔")]
        public float interval;
    }

    [System.Serializable]
    public class ReplenishSpawnPolicy
    {
        [KTLevelExport("generateNum")]
        [LabelText("初始数量")]
        public int initNum;
        [KTLevelExport("appendNum")]
        [LabelText("最大数量")]
        public int maxNum;
        [KTLevelExport("appendTime")]
        [LabelText("补充时间(s)")]
        public float interval;
    }

    public readonly static ValueDropdownList<MobType> kSpawnPolicyNames = new ValueDropdownList<MobType>()
        {
            { "按时间刷怪", MobType.interval },
            { "补充刷怪", MobType.append },
        };

    public readonly static ValueDropdownList<SpawnPointPolicyType> kSpawnPointPolicyNames = new ValueDropdownList<SpawnPointPolicyType>()
        {
            { "正常模式", SpawnPointPolicyType.Sequential },
            { "随机模式", SpawnPointPolicyType.Random },
            { "随机模式2", SpawnPointPolicyType.Random2 },
        };

    [KTLevelExport("selectedMobType")]
    [LabelText("刷怪方式"), ValueDropdown("kSpawnPolicyNames")]
    public MobType spawnPolicy = MobType.interval;

    [HideLabel, ShowIf("IsPeriodicPolicy")]
    public PeriodicSpawnPolicy periodicSpawnPolicy;

    [HideLabel, ShowIf("IsReplenishPolicy")]
    public ReplenishSpawnPolicy replenishSpawnPolicy;

    [KTLevelExport("randomMode")]
    [LabelText("选择刷怪位置方式"), ValueDropdown("kSpawnPointPolicyNames")]
    public SpawnPointPolicyType spawnPointPolicy;

    [KTLevelExport("__children"), KTLevelExport("__children_uuid")]
    [LabelText("刷怪点"), OnValueChanged("OnSpawnPointsChanged")]
    public List<KTLevelPoint> spawnPoints;

    [HideInInspector]
    public List<KTLevelPoint> prevSpawnPoints;

    // protected virtual bool IsZonePatrol() { return false; }

    public override void PreExport()
    {
        CheckUniqueList(spawnPoints, "导出错误, 孵化器存在重复刷怪点");
    }

    private void OnSpawnPointsChanged()
    {
        if (prevSpawnPoints != null)
        {
            foreach (var removedPoint in prevSpawnPoints.Except(spawnPoints))
            {
                removedPoint.RemoveUnitView();
            }
        }

        if (prevSpawnPoints == null)
        {
            prevSpawnPoints = new List<KTLevelPoint>();
        }

        prevSpawnPoints.Clear();
        prevSpawnPoints.AddRange(spawnPoints);

        RefreshView();
    }

    [LabelText("刷怪点预览模型"), OnValueChanged("TogglePreviewPoints")]
    public bool previewSpawnPoints;

    private bool IsPeriodicPolicy() { return spawnPolicy == MobType.interval; }
    private bool IsReplenishPolicy() { return spawnPolicy == MobType.append; }

    public void RefreshUnitView(string tableName, int id)
    {
        var view = GetComponent<KTLevelUnitView>();
        if (view != null)
        {
            view.RefreshView(tableName, id);
        }

        if (previewSpawnPoints)
        {
            foreach (var point in spawnPoints)
            {
                if(point!=null)
                    point.RefreshUnitView(tableName, id);
            }
        }
    }

    private void TogglePreviewPoints()
    {
        if (previewSpawnPoints)
        {
            RefreshView();
        }
        else
        {
            RemovePointUnitView();
        }
    }

    private void RemovePointUnitView()
    {
        foreach (var point in spawnPoints)
        {
            if (point != null)
                point.RemoveUnitView();
        }
    }
}
#endif