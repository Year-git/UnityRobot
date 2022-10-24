using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNpcScript : MonoBehaviour
{
    [HeaderAttribute("是否刷出")]
    public bool isEnable;
    [HeaderAttribute("Npc配置Id")]
    public int npcCfgId;

#if UNITY_EDITOR
    [HeaderAttribute("编辑模式_显示线框")]
    public bool gizmosShowLine = true;
    [HeaderAttribute("编辑模式_显示实体")]
    public bool gizmosShowEntity = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        Vector3 vSize = new Vector3(1f,2f,1f);
        Vector3 vPositon = gameObject.transform.position;
        vPositon.y = vPositon.y + 1f;
        if (gizmosShowLine)
        {
            Gizmos.DrawWireCube(vPositon, vSize);
        }
        if (gizmosShowEntity)
        {
            Gizmos.DrawCube(vPositon, vSize);
        }
    }
#endif
}
