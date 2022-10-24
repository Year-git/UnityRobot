#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;

[KTDisplayName("巡逻点")]
public class KTLevelPatrolPoint : KTLevelEntityAdjust
{
	protected override string uuidPrefix
	{
		get
		{
			return "patrolPoints";
		}
	}

	[KTLevelExport("id")]
	[LabelText("id")]
	public int id = 0;
	[KTLevelExport("isPathPoint")]
	[LabelText("是否是寻路点")]
	public bool isPathPoint = false;//是否是网格寻路点
	[KTLevelExport("stayRangeLow")]
	[LabelText("最短停留时间")]
	public float stayTimeMin = 0.0f;
	[KTLevelExport("stayRangeHigh")]
	[LabelText("最长停留时间")]
	public float stayTimeMax = 0.0f;
	[KTLevelExport("moveSpeed")]
	[LabelText("移动速度")]
	public float moveSpeed = 0.0f;

}
#endif
