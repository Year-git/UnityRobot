#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[KTDisplayName("巡逻路线")]
public class KTLevelPatrolPath : KTLevelEntity
{
	protected override string uuidPrefix
	{
		get
		{
			return "patrolPaths";
		}
	}

	[KTLevelExport("pointID1"), ReadOnly]
	[LabelText("端点id1")]
	public int pointID1;

	[KTLevelExport("pointID2"), ReadOnly]
	[LabelText("端点id2")]
	public int pointID2;

	[LabelText("直线距离"), ReadOnly]
	public float dist = 0;

	[KTLevelExport("__children"), KTLevelExport("__children_uuid")]
	[LabelText("巡逻点"), OnValueChanged("RefreshPointIDs")]
	public List<KTLevelPatrolPoint> points;

	private void RefreshPointIDs()
	{
		if (points.Count == 1)
		{
			pointID1 = points[0].id;
			dist = 0;
		}
		else if (points.Count > 1)
		{
			pointID1 = points[0].id;
			pointID2 = points[points.Count - 1].id;
			dist = Vector3.Distance(points[0].transform.position, points[points.Count - 1].transform.position);
		}
		else
		{
			pointID1 = 0;
			pointID2 = 0;
			dist = 0;
		}
	}

	public override void PreExport()
	{
		CheckUniqueList(points, "导出错误, 巡逻路线存在重复点");
	}

	public void OnDrawGizmos()
	{
		if (points == null || points.Count <= 1)
		{
			return;
		}

		for (int i = 0; i < points.Count; ++i)
		{
			var pos = points[i].transform.position;
			var nextPointIndex = (i + 1) % points.Count;
			var nextPos = points[nextPointIndex].transform.position;
			Gizmos.DrawLine(pos, nextPos);
		}
	}
}
#endif