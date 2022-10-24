//自动布局
using UnityEngine;

public interface IKTLevelLayout
{
	GameObject ObjToAdjust { get; set; }//粘附目标
	void AdjustY();//垂直向下粘附
	void AdjustXZ();//横向粘附到某模型中心
}
