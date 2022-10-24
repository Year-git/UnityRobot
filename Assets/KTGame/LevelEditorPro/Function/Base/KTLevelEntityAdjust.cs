#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;

public class KTLevelEntityAdjust : KTLevelEntity, IKTLevelLayout
{
	[LabelText("粘附目标"), TabGroup("自动粘附")]
	public GameObject ObjToAdjust { get; set; }

	[Button("Y轴自动粘附", ButtonSizes.Medium), TabGroup("自动粘附"), GUIColor(0.0f, 1.0f, 0.0f)]
	public void AdjustY()
	{
		RaycastHit hitInfo;
		Vector3 oraginPoint = new Vector3(transform.position.x, 10000f, transform.position.z);
		if (Physics.Raycast(oraginPoint, Vector3.down, out hitInfo, 10100f, 1 << LayerMask.NameToLayer("Terrain")))
		{
			this.transform.position = hitInfo.point;
		}
		else
		{
			Debug.LogErrorFormat("can not find terrain to adjust in entity named {0}", this.uuid);
		}
	}

	//[Button("XZ轴自动粘附", ButtonSizes.Medium), TabGroup("自动粘附"), GUIColor(0.0f, 1.0f, 0.0f)]
	public void AdjustXZ()
	{
		if (ObjToAdjust)
		{
			this.transform.position = new Vector3(ObjToAdjust.transform.position.x, this.transform.position.y, ObjToAdjust.transform.position.z);
		}
		else
		{
			Debug.LogError("please select a gameobject before");
		}
	}
}

#endif