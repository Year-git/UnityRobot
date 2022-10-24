#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[KTDisplayName("客户端触发器"), ExecuteInEditMode]
public class KTLevelTriggerClient : KTLevelTrigger
{
    protected override string uuidPrefix
    {
        get
        {
            return "clientTriggers";
        }
    }
}
#endif