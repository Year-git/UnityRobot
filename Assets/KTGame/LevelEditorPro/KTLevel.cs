#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class KTLevel : MonoBehaviour
{
    [LabelText("格式版本")]
    public int ver = 0;
    public float x = 0;
    public float z = 0;
    public float width = 1024;
    public float height = 1024;

    public void CreateDefaultGameObjects()
    {
        Selection.activeGameObject = new GameObject("Task_1");
    }

    private GameObject NewPrimitveGameObject(PrimitiveType type)
    {
        var go = GameObject.CreatePrimitive(type);
        var renderer = go.GetComponent<Renderer>();
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        return go;
    }

    #region 点和区域

	/// <summary>
	/// 位置
	/// </summary>
    public KTLevelPoint NewPoint()
    {
        return NewEntity<KTLevelPoint>(NewPrimitveGameObject(PrimitiveType.Sphere));
    }
	
	/// <summary>
	/// 出生点
	/// </summary>
    public KTLevelSpawnPoint NewSpawnPoint()
    {
        return NewEntity<KTLevelSpawnPoint>(NewPrimitveGameObject(PrimitiveType.Sphere));
    }

	/// <summary>
	/// 巡逻点
	/// </summary>
    public KTLevelPatrolPoint NewPatrolPoint()
    {
        return NewEntity<KTLevelPatrolPoint>(NewPrimitveGameObject(PrimitiveType.Sphere));
    }

	/// <summary>
	/// 巡逻路径
	/// </summary>
    public KTLevelPatrolPath NewPatrolPath()
    {
        return NewEntity<KTLevelPatrolPath>(new GameObject());
    }

	/// <summary>
	/// 区域
	/// </summary>
    public KTLevelPatrolZone NewPatrolZone()
    {
        return NewEntity<KTLevelPatrolZone>(new GameObject());
    }

    #endregion

    #region 孵化器
    public KTLevelItemSpawner NewItemSpawner()
    {        
        return NewEntity<KTLevelItemSpawner>(NewSpawnerGameObject());
    }

    public KTLevelInteractionItemSpawner NewInteractionItemSpawner()
    {
        return NewEntity<KTLevelInteractionItemSpawner>(NewSpawnerGameObject());
    }

    public KTLevelBattleUnitSpawner NewBattleUnitSpawner()
    {
        return NewEntity<KTLevelBattleUnitSpawner>(NewSpawnerGameObject());
    }

	public KTLevelSpawnerGroup NewSpawnerGroup()
	{
		return NewEntity<KTLevelSpawnerGroup>(NewSpawnerGameObject());
	}

	public T NewSpawner<T>() where T : KTLevelSpawnerSingle
	{
		return NewEntity<T>(NewSpawnerGameObject());
	}

	private GameObject NewSpawnerGameObject()
    {
        var go = new GameObject();
        go.AddComponent<KTLevelUnitView>();
        return go;
    }
	#endregion

	#region 战斗阶段&触发器

	public KTLevelTrigger NewServerTrigger()
	{
		return NewEntity<KTLevelTrigger>(new GameObject());
	}

	public KTLevelTriggerClient NewClientTrigger()
	{
		return NewEntity<KTLevelTriggerClient>(new GameObject());
	}

	/// <summary>
	/// 战斗阶段
	/// </summary>
	public KTLevelFightStage NewFightStage()
	{
		return NewEntity<KTLevelFightStage>(new GameObject());
	}

	#endregion

	private T NewEntity<T>(GameObject entityGo) where T : KTLevelEntity
    {
        // NOTE: 把entity挪到关卡场景.
        entityGo.transform.SetParent(this.transform, true);
        entityGo.transform.SetParent(null, true);

        var root = GetNewCommandRoot();
        var rootTrans = root != null ? root.transform : null;
        
        var type = typeof(T);
        entityGo.name = GameObjectUtility.GetUniqueNameForSibling(rootTrans, KTUtils.GetDisplayName(type, type.Name));
        var ret = entityGo.AddComponent<T>();
        ret.GenerateUUID();

        GameObjectUtility.SetParentAndAlign(entityGo, root);
        Undo.RegisterCreatedObjectUndo(entityGo, "Create " + entityGo.name);
        Selection.activeObject = entityGo;

		//scene面板追踪功能
		var sceneView = SceneView.lastActiveSceneView;
		if (sceneView != null)
		{
			sceneView.FrameSelected();
		}

		return ret;
    }

    private GameObject GetNewCommandRoot()
    {
        var selTrans = Selection.activeTransform;
        if (selTrans == null)
        {
            return null;
        }

        var entity = selTrans.GetComponent<KTLevelEntity>();
        if (entity != null)
        {
            return (entity.transform.parent != null) ? entity.transform.parent.gameObject : null;
        }

        return selTrans.gameObject;
    }
}
#endif
