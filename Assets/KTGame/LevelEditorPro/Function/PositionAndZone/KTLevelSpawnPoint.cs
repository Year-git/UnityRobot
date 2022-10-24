#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

[KTDisplayName("出生点")]
public class KTLevelSpawnPoint : KTLevelEntityAdjust
{
    protected override string uuidPrefix
    {
        get
        {
            return "bornLocations";
        }
    }
}
#endif
