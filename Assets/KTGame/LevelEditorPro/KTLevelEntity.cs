#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

public class KTLevelEntity : MonoBehaviour
{
    protected virtual string uuidPrefix { get { return string.Empty; } }

    [LabelText("全局ID"), ReadOnly, KTLevelExport("addr_uuid")]
    public string uuid;

    public void GenerateUUID()
    {
        var isExported = PrefabUtility.GetPrefabParent(this) != null;
        if (isExported && uuid != null && uuid.StartsWith(uuidPrefix))
        {
            return;
        }

        uuid = string.Format("{0}##{1}", uuidPrefix, System.Guid.NewGuid().ToString());
    }

    public virtual void PreExport()
    {

    }

    public virtual void PostExport()
    {

    }

    public void CheckUniqueList<T>(List<T> values, string info)
    {
        if (values == null)
        {
            return;
        }

        var uniqueList = values.Distinct();
        if (uniqueList.Count() != values.Count)
        {
            throw new System.Exception(string.Format("{0}, {1}", info, gameObject.name));
        }
    }
}
#endif