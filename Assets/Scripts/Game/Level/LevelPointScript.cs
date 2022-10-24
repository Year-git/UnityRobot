using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPointScript : MonoBehaviour
{
#if UNITY_EDITOR
    [HeaderAttribute("编辑模式_显示线框")]
    public bool gizmosShowLine = true;
    [HeaderAttribute("编辑模式_显示实体")]
    public bool gizmosShowEntity = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float fRadius = 0.5f;
        
        if (gizmosShowLine)
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, fRadius);
        }
        if (gizmosShowEntity)
        {
            Gizmos.DrawSphere(gameObject.transform.position, fRadius);
        }
    }
#endif
}
