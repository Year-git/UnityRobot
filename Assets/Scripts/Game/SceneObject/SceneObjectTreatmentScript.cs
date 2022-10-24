using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectTreatmentScript : MonoBehaviour
{
    /// <summary>
    /// 触发治疗的间隔时间
    /// </summary>
    public int _treatmentIntervalTime;

    /// <summary>
    /// 治疗的百分比值
    /// </summary>
    public int _treatmentPercent;

    /// <summary>
    /// 下次可触发治疗的时间
    /// </summary>
    private int _treatmentTriggerTime = 0;

    private void OnTriggerEnter(Collider other)
    {
        TreatmentCheck(other);
    }

    private void OnTriggerStay (Collider other)
    {
        TreatmentCheck(other);
    }

    private void TreatmentCheck(Collider other)
    {
        RobotPartScriptBase pPartScript = other.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pPartScript == null)
        {
            return;
        }

        BaseMap pMap = MapManager.Instance.baseMap;
        if (pMap == null)
        {
            return;
        }
        
        BaseNpc pNpc = pMap.GetNpc(pPartScript.myElement.myRobotPart.npcInstId);
        if (pNpc == null)
        {
            return;
        }

        FrameSynchronManager pFrameSynManager = FrameSynchronManager.Instance;
        int nCurTime = pFrameSynManager.fsData.FrameRunningTime;
        if (_treatmentTriggerTime > nCurTime )
        {
            return;
        }

        _treatmentTriggerTime = nCurTime + _treatmentIntervalTime;
        pNpc.NpcTreatmentPercentInput(_treatmentPercent);
    }
}
