using System;
using UnityEngine;
using System.Collections.Generic;
public class Model
{
    public MapObject mapObject{ get; private set; }
    public GameObject gameObject{ get; private set; }
    public Transform transform;
    public MeshRenderer meshRenderer;
    public Rigidbody rigidbody;
    public Collider collider;
    /// <summary>
    /// 是否已经调用删除
    /// </summary>
    private bool isDestroy = false;
    /// <summary>
    /// 是否加载完成
    /// </summary>
    public bool isLoadEnd { get; private set; } = false;

    public Model(MapObject pMapObject, string sModelName, Vector3 pPosition, Vector3 pEulerAngles, float nScale, Action<Model> fInitCall = null)
    {
        this.mapObject = pMapObject;
        Load(sModelName, pPosition, pEulerAngles, nScale, fInitCall);
    }

    private void Load(string sModelName, Vector3 pPosition, Vector3 pEulerAngles, float nScale, Action<Model> fInitCall)
    {
        ResourcesManager.Instance.LoadAsync(sModelName, delegate (GameObject pGameObj)
            {
                if (pGameObj == null)
                {
                    Debug.LogError("Model.cs=>Load->Model File Does Not Exist! -> sModelName = " + sModelName);
                }

                this.gameObject = UnityEngine.Object.Instantiate<GameObject>(pGameObj, pPosition, Quaternion.Euler(pEulerAngles));
                this.transform = this.gameObject.transform;
                if (nScale != 0)
                {
                    this.transform.localScale = new Vector3(nScale,nScale,nScale);
                }

                this.meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
                this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
                this.collider = this.gameObject.GetComponent<Collider>();

                fInitCall?.Invoke(this);
                this.isLoadEnd = true;
                
                // 服务器碰撞显示工具
                // ObjectModel.AddComponent<DrawTool>();

                //这里处理有的时候模型没有加载完就调用删除
                if(this.isDestroy)
                {
                    this.Destroy();
                }
            }
        );
    }

    public void Update(){}

    public void Destroy()
    {
        this.isDestroy = true;
        if(!this.isLoadEnd)
        {
            return;
        }
        UnityEngine.Object.Destroy(this.gameObject);
        // UnityEngine.Object.DestroyImmediate(GameObject);
    }

    //----------------------------------------------------------------------------------------------------

    /// <summary>
    /// 获取模型名称
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static string GetModelName(int nCfgId)
    {
        return ConfigManager.GetValue<string>("Model_C", nCfgId, "strSkinName");
    }

    /// <summary>
    /// 获取模型缩放
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <returns></returns>
    public static float GetModelScale(int nCfgId)
    {
        return (float)ConfigManager.GetValue<int>("Model_C", nCfgId, "modelScale") / 1000f;
    }

    /// <summary>
    /// 获取动作名
    /// </summary>
    /// <param name="nCfgId"></param>
    /// <param name="nIdx"></param>
    /// <returns></returns>
    public static string GetModelAnimation(int nCfgId, int nIdx)
    {
        return ConfigManager.GetValue<string>("Model_C", nCfgId, "strAction" + nIdx);
    }
}