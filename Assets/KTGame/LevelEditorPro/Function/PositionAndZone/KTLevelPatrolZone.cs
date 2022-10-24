#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;
using UnityEditor;

[KTDisplayName("区域")]
public class KTLevelPatrolZone : KTLevelEntityAdjust
{
    protected override string uuidPrefix
    {
        get
        {
            return "patrolAreas";
        }
    }

    [System.Serializable]
    public class CopySettings
    {
        [LabelText("目标孵化器")]
        public KTLevelSpawnerSingle srcSpawner;
        [LabelText("生成数量")]
        public int generateCount = 0;
    }

    [System.Serializable]
    public class GenerateSettings
    {
        public readonly static ValueDropdownList<SpawnerType> kSpawnTypeNames = new ValueDropdownList<SpawnerType>()
        {
            { "战斗单位孵化器", SpawnerType.unit_spawner },
            { "物件孵化器", SpawnerType.item_spawner },
            { "交互物孵化器", SpawnerType.interact_item_spawner },
        };

        [LabelText("孵化器类型"), ValueDropdown("kSpawnTypeNames")]
        public SpawnerType spawnerType = 0;
        [LabelText("生成数量")]
        public int generateCount = 0;
        [LabelText("生成id")]
        public int generateID = 0;
    }

    [KTLevelExport("id"), LabelText("区域ID"),ValidateInput("DistanctID", "id repeat")]
    public int id;

    //[TabGroup("批量复制"), HideLabel]
    //public CopySettings copySettings;
    //[Button("批量复制孵化器", ButtonSizes.Medium), TabGroup("批量复制"), GUIColor(0.0f, 1.0f, 0.0f)]
    //public void BatchCopySpawners()
    //{
    //    if (copySettings.srcSpawner == null)
    //    {
    //        Debug.LogError("please select a spawner before");
    //        return;
    //    }

    //    KTLevel level = FindObjectOfType<KTLevel>();
    //    var srcType = copySettings.srcSpawner.GetType();
    //    var fields = srcType.GetFields().Where(l => 
    //    {
    //        var itemAttr= l.GetCustomAttributes(typeof(KTLevelExportAttribute), true);
    //        return itemAttr != null;
    //    }).ToList();

    //    for (int i = 0; i < copySettings.generateCount; i++)
    //    {
    //        KTLevelSpawnerSingle tempSpawner = null;
    //        if (srcType ==typeof(KTLevelBattleUnitSpawner))
    //        {
    //            tempSpawner = level.NewSpawner<KTLevelBattleUnitSpawner>();
    //        }
    //        else if (srcType == typeof(KTLevelItemSpawner))
    //        {
    //            tempSpawner = level.NewSpawner<KTLevelItemSpawner>();
    //        }
    //        else if (srcType == typeof(KTLevelInteractionItemSpawner))
    //        {
    //            tempSpawner = level.NewSpawner<KTLevelInteractionItemSpawner>();
    //        }

    //        KTLevelTools.CopyFieldsValues(copySettings.srcSpawner, tempSpawner, fields);
    //    }
    //}

    //[TabGroup("批量生成"), HideLabel]
    //public GenerateSettings generateSettings;
    //[Button("批量生成孵化器", ButtonSizes.Medium), TabGroup("批量生成"), GUIColor(0.0f, 1.0f, 0.0f)]
    //public void BatchGenerateSpawners()
    //{
    //    KTLevel level= GameObject.FindObjectOfType<KTLevel>();
        
    //    for (int i=0;i< generateSettings.generateCount; i++)
    //    {
    //        KTLevelSpawnerSingle tempSpawner = null;
    //        if (generateSettings.spawnerType == SpawnerType.unit_spawner)
    //        {
    //            tempSpawner = level.NewBattleUnitSpawner();
    //            (tempSpawner as KTLevelBattleUnitSpawner).id = generateSettings.generateID;
    //        } 
    //        else if(generateSettings.spawnerType == SpawnerType.item_spawner)
    //        {
    //            tempSpawner = level.NewItemSpawner();
    //        }
    //        else if (generateSettings.spawnerType == SpawnerType.interact_item_spawner)
    //        {
    //            tempSpawner = level.NewInteractionItemSpawner();
    //            (tempSpawner as KTLevelInteractionItemSpawner).id = generateSettings.generateID;
    //        }

    //        KTLevelPoint tempPoint = level.NewPoint();
    //        float halfScaleX = this.transform.localScale.x*0.5f;
    //        float halfScaleZ= this.transform.localScale.z*0.5f;
    //        float tempX = this.transform.position.x+Random.Range(-halfScaleX, halfScaleX);
    //        float tempY = this.transform.position.y;
    //        float tempZ = this.transform.position.z+ Random.Range(-halfScaleZ, halfScaleZ);
    //        Vector3 tempPos = new Vector3(tempX, tempY, tempZ);
    //        tempPoint.transform.position = tempPos;
    //        tempPoint.transform.rotation = Quaternion.AngleAxis(Random.value * 360, Vector3.up);

    //        if (tempSpawner.spawnPoints == null)
    //            tempSpawner.spawnPoints = new List<KTLevelPoint>();

    //        tempSpawner.spawnPoints.Add(tempPoint);
    //    }
    //}

    //检查id重复,返回true表示不重复
    public static bool DistanctID(int input=0)
    {
        var entities = FindObjectsOfType(typeof(KTLevelPatrolZone));
        if (entities.Length == 0)
            return true;

        List<int> entities1 = new List<Object>(entities).Select(l => { return (l as KTLevelPatrolZone).id;}).ToList();
        List<int> entities2 = entities1.Distinct().ToList();
        return entities1.Count == entities2.Count;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
#endif