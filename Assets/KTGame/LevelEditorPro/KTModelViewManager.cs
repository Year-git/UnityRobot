#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class KTModelViewManager : KTSimpleSingleton<KTModelViewManager>
{
    public const int kInvalidAssetId = -1;
    public const string kModelPrefabPath = "Assets/actors/prefab";

    public void SetCreatureModelColor(int id,GameObject prefab)
    {
        var color1 = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "人形小怪部位一颜色");
        var color2 = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "人形小怪部位二颜色");
        var color3 = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "人形小怪部位三颜色");
        var defaultHSV = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "小怪色相");
        var specularColor = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "小怪高光");
        
        var renderers= prefab.transform.GetComponentsInChildren<Renderer>();
        renderers.ToList().ForEach(item => 
        {
            if(item.sharedMaterial != null)
            {
                if(item.sharedMaterial.shader.name== "KTGame/KTCharacter/KTCharacter_SimpleClip")
                {
                    if(!string.IsNullOrEmpty(defaultHSV))
                    {
                        var hsv = item.sharedMaterial.GetVector("_HSV");
                        item.sharedMaterial.SetVector("_HSV", new Vector4(float.Parse(defaultHSV), hsv.y, hsv.z, hsv.w));
                        Debug.LogFormat("_HSV r:{0}g:{1}b:{2}", hsv.y, hsv.z, hsv.w);
                    }

                    if (!string.IsNullOrEmpty(specularColor))
                    {
                        float r = float.Parse(specularColor.Split(';')[0]);
                        float g = float.Parse(specularColor.Split(';')[1]);
                        float b = float.Parse(specularColor.Split(';')[2]);
                        Debug.LogFormat("_SpecularColor0 r:{0}g:{1}b:{2}", r.ToString(), g.ToString(), b.ToString());
                        var _SpecularColor0 = new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1);
                        item.sharedMaterial.SetColor("_SpecularColor0", _SpecularColor0);
                    }
                }
                else if (item.sharedMaterial.shader.name == "KTGame/KTCharacter/KTCharacter_Body")
                {
                    if (!string.IsNullOrEmpty(color1))
                    {
                        float r = float.Parse(color1.Split(';')[0]);
                        float g = float.Parse(color1.Split(';')[1]);
                        float b = float.Parse(color1.Split(';')[2]);
                        Debug.LogFormat("color1 r:{0}g:{1}b:{2}", r.ToString(), g.ToString(), b.ToString());
                        item.sharedMaterial.SetColor("_Color1", new Color(r / 255.0f, g / 255.0f, b / 255.0f, 0f));
                    }

                    if (!string.IsNullOrEmpty(color2))
                    {
                        float r = float.Parse(color2.Split(';')[0]);
                        float g = float.Parse(color2.Split(';')[1]);
                        float b = float.Parse(color2.Split(';')[2]);
                        Debug.LogFormat("color2 r:{0}g:{1}b:{2}", r.ToString(), g.ToString(), b.ToString());
                        item.sharedMaterial.SetColor("_Color2", new Color(r / 255.0f, g / 255.0f, b / 255.0f, 0f));
                    }

                    if (!string.IsNullOrEmpty(color3))
                    {
                        float r = float.Parse(color3.Split(';')[0]);
                        float g = float.Parse(color3.Split(';')[1]);
                        float b = float.Parse(color3.Split(';')[2]);
                        Debug.LogFormat("color3 r:{0}g:{1}b:{2}", r.ToString(), g.ToString(), b.ToString());
                        item.sharedMaterial.SetColor("_Color3", new Color(r / 255.0f, g / 255.0f, b / 255.0f, 0f));
                    }
                }
            }
        });
    }

    public GameObject CreateModelPrefab(string tableName, int id)
    {
        var assetId = GetAssetId(tableName, id);
        if (assetId == kInvalidAssetId)
        {
            Debug.LogWarningFormat("获取AssetId失败. id:{0}", id);
            return null;
        }

        var prefabName = KTExcelManager.instance.Get(KTExcels.kModelAssets, assetId, "预制件名");
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogWarningFormat("从{0}获取模型路径失败. id:{1}", KTExcels.kModelAssets, assetId);
            return null;
        }
        Debug.Log(prefabName);
        return CreatePrefab(prefabName);
    }

    private static int GetAssetId(string tblName, int id)
    {
        var assetIdStr = KTExcelManager.instance.Get(tblName, id, KTExcels.GetAssetIdColumeName(tblName));
        if (string.IsNullOrEmpty(assetIdStr))
        {
            return kInvalidAssetId;
        }

        int assetId;
        if (int.TryParse(assetIdStr, out assetId))
        {
            return assetId;
        }
        return kInvalidAssetId;
    }

    private static GameObject CreatePrefab(string prefabName)
    {

        var path = string.Format("{0}/{1}.prefab", kModelPrefabPath, prefabName);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    private static void HideAll(Transform trans)
    {
        const HideFlags flags = HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector; ;
        trans.gameObject.hideFlags = flags;
        int childCount = trans.childCount;
        for(int i=0; i<childCount; ++i)
        {
            HideAll(trans.GetChild(i));
        }
    }

    private static void AnimateAllParticles(GameObject go)
    {
        var particles = go.GetComponentsInChildren<ParticleSystem>();
        foreach(var particle in particles)
        {
            particle.Play(true);
        }
    }
}

#endif
