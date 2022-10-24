#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;

[KTDisplayName("位置")]
public class KTLevelPoint : KTLevelEntityAdjust
{
    protected override string uuidPrefix
    {
        get
        {
            return "monsters";
        }
    }

    public void RefreshUnitView(string tableName, int id)
    {
        var view = GetComponent<KTLevelUnitView>();
        if (view == null)
        {
            view = gameObject.AddComponent<KTLevelUnitView>();
        }

        view.RefreshView(tableName, id);
    }

    public void RemoveUnitView()
    {
        var view = GetComponent<KTLevelUnitView>();
        if (view != null)
        {
            view.RemoveView();
            view.ResetToPrimitiveView(PrimitiveType.Sphere);
            DestroyImmediate(view);
        }
    }
}
#endif