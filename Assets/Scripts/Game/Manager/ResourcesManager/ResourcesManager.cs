using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public AssetBundleManifest AssetBundleManifest { get; set; }
    public delegate void LoadedCallBack1(UnityEngine.Object obj);
    public delegate void LoadedCallBack2(UnityEngine.Object[] obj);
    private readonly string _path = Application.persistentDataPath + "/" + LuaConst.osDir + "/" + LuaConst.osDir;
    private Dictionary<string, AssetBundleLoader> _assetBundleLoaderCaches = new Dictionary<string, AssetBundleLoader>();

    public void InitAssetBundleManifest()
    {
#if GAME_VERSION_ENABLED
        if (System.IO.File.Exists(_path))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(_path);
            AssetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            assetBundle.Unload(false);
            assetBundle = null;
        }
        else
        {
            Debug.LogError(_path + "：不存在，ResourcesManager.Start失败！");
        }
#endif
    }

#if GAME_VERSION_ENABLED
    /// <summary>
    /// 资源加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public async void LoadAsync<T>(string assetName, Action<T> action) where T : UnityEngine.Object
    {
        assetName = assetName.Replace("Assets/", "");
        string bundleName = assetName.Substring(0, assetName.LastIndexOf('/'));
        bundleName = bundleName.Replace("/", "_");
        bundleName = bundleName.ToLower();

        if (!_assetBundleLoaderCaches.TryGetValue(bundleName, out AssetBundleLoader loader))
        {
            loader = new AssetBundleLoader();
            _assetBundleLoaderCaches.Add(bundleName, loader);
        }
        if (!loader.IsDone)
        {
            await loader.LoadAssetBundle(bundleName);
        }

        string resName = assetName.Substring(0, assetName.LastIndexOf('.'));
        string[] strSplit = resName.Split('/');
        resName = strSplit[strSplit.Length - 1];
        T obj = await loader.LoadAssetAsync<T>(resName);
        action?.Invoke(obj);
    }
#else
    /// <summary>
    /// 资源加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadAsync<T>(string assetName, Action<T> action) where T : UnityEngine.Object
    {
        //模拟异步
        System.Random ra = new System.Random();
        float time = ra.Next(1, 2) * 0.017f;
        Timer.CreateTimer(time, false, delegate
        {
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetName);
            action?.Invoke(obj);
        });
    }
#endif

    /// <summary>
    /// Resources资源加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    /// <param name="async"></param>
    public async void Load<T>(string assetName, Action<T> action, bool async = true) where T : UnityEngine.Object
    {
        T obj;
        if (async)
        {
            using (ResourcesAssetLoader request = new ResourcesAssetLoader())
            {
                obj = await request.LoadAssetAsync<T>(assetName);
            }
        }
        else
        {
            obj = Resources.Load<T>(assetName);
        }
        action?.Invoke(obj);
    }

    /// <summary>
    /// 加载指定的AssetBundle
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="action"></param>
    public async void LoadAssetBundle(string bundleName, LoadedCallBack2 action)
    {
        if (!_assetBundleLoaderCaches.TryGetValue(bundleName, out AssetBundleLoader loader))
        {
            loader = new AssetBundleLoader();
            _assetBundleLoaderCaches.Add(bundleName, loader);
        }
        if (!loader.IsDone)
        {
            await loader.LoadAssetBundle(bundleName);
        }
        UnityEngine.Object[] objs = await loader.LoadAllAssetsAsync();
        action?.Invoke(objs);
    }

    /// <summary>
    /// 移除指定的AssetBundle
    /// </summary>
    /// <param name="bundleName"></param>
    public void RemoveAssetBundleLoader(string bundleName)
    {
        if (_assetBundleLoaderCaches.TryGetValue(bundleName, out AssetBundleLoader loader))
        {
            loader.Dispose();
            _assetBundleLoaderCaches.Remove(bundleName);
        }
    }

    /// <summary>
    /// 释放所有AssetBundle
    /// </summary>
    public void DisposeAssetBundle()
    {
        foreach (var loader in _assetBundleLoaderCaches)
        {
            loader.Value.Dispose();
        }
        _assetBundleLoaderCaches.Clear();
        Resources.UnloadUnusedAssets();
    }
}