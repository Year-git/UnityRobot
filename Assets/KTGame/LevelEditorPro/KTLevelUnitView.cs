#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class KTLevelUnitView : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string _tableName;
    [SerializeField, HideInInspector]
    private int _id;

    public void RefreshView(string tableName, int id)
    {
        if (tableName == _tableName && _id == id)
        {
            return;
        }

        RemoveAllChildren();
        CreateView(tableName, id);

        _tableName = tableName;
        _id = id;
    }

    public void RemoveView()
    {
        RemoveAllChildren();
        _tableName = "";
        _id = 0;
    }

    public void ResetToPrimitiveView(PrimitiveType primitveType)
    {
        var primitve = GameObject.CreatePrimitive(primitveType);
        SwitchRoot(primitve.transform);
        CloneAllViewComponents(primitve);
        DestroyImmediate(primitve);
    }

    private void CreateView(string tableName, int id)
    {
        var prefab = KTModelViewManager.instance.CreateModelPrefab(tableName, id);
        if (prefab != null)
        {
            RemoveAllViewComponets();

            var sourceGo = Instantiate(prefab) as GameObject;
            if (tableName == KTExcels.kCreatures)
                KTModelViewManager.instance.SetCreatureModelColor(id, sourceGo);

            SwitchRoot(sourceGo.transform);
            CloneAllViewComponents(sourceGo);
            DestroyImmediate(sourceGo);
        }
    }

    private void SwitchRoot(Transform source)
    {
        var sourceChildren = GetAllChildren(source);
        foreach(var child in sourceChildren)
        {
            child.SetParent(transform, false);
        }
    }

    private const string kEditorComponentPrefix = "KTLevel";
    private bool IsViewComponent(System.Type type)
    {
        return
        type != typeof(Transform) &&
        !type.Name.StartsWith(kEditorComponentPrefix);
    }

    private void RemoveAllViewComponets()
    {
        var components = GetComponents<Component>();
        var componentsToRemove = components.Where(comp => IsViewComponent(comp.GetType())).Reverse().ToList();
        componentsToRemove.ForEach(DestroyImmediate);
    }

    private void CloneAllViewComponents(GameObject source)
    {
        var sourceComponents = source.GetComponents<Component>();
        var componentsToCopy = sourceComponents.Where(comp => IsViewComponent(comp.GetType())).ToList();
        foreach (var component in componentsToCopy)
        {
            var copyComponent = gameObject.GetComponent(component.GetType());
            if (copyComponent == null)
            {
                copyComponent = gameObject.AddComponent(component.GetType());
            }
            EditorUtility.CopySerialized(component, copyComponent);
        }
    }

    private void CloneAllChildren(Transform source)
    {
        var sourceChildren = GetAllChildren(source);
        foreach(var child in sourceChildren)
        {
            var childInst = Instantiate(child);
            childInst.SetParent(transform, true);
        }
    }

    private static List<Transform> GetAllChildren(Transform trans)
    {
        var result = new List<Transform>();
        for (int childIndex = 0; childIndex < trans.childCount; ++childIndex)
        {
            result.Add(trans.GetChild(childIndex));
        }
        return result;
    }

    private void RemoveAllChildren()
    {
        var allChilds = new List<Transform>();
        var trans = this.transform;
        int childCount = trans.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            allChilds.Add(trans.GetChild(i));
        }

        allChilds.ForEach(c => GameObject.DestroyImmediate(c.gameObject));
    }
}

#endif
