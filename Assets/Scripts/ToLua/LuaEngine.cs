using UnityEngine;
using UnityEngine.Networking;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LuaEngine : LuaClient
{
    int bundleCount = int.MaxValue;

    protected override void Init()
    {
#if GAME_VERSION_ENABLED
        //AssetBundle启动
        LuaResLoader luaResLoader = new LuaResLoader
        {
            beZip = true
        };
        StartCoroutine(LoadBundles());
#else
        //非AssetBundle启动
            LuaResLoader luaResLoader = new LuaResLoader();
            OnBundleLoad();
#endif
    }

    protected override void OpenLibs()
    {
        base.OpenLibs();
        OpenCJson();
    }

    /// <summary>
    /// 可添加或修改搜索 lua 文件的目录
    /// </summary>
    protected override void LoadLuaFiles()
    {
#if UNITY_EDITOR && !GAME_VERSION_ENABLED
        // 添加编辑器环境下获取 lua 脚本的路径（Assets/Lua）
        luaState.AddSearchPath(Application.dataPath + "/Lua");
#endif
        OnLoadFinished();

        //启动游戏逻辑
        Main.Instance.StartGame();
    }

    IEnumerator LoadBundles()
    {
        ResourcesManager.Instance.InitAssetBundleManifest();
        List<string> list = new List<string>(ResourcesManager.Instance.AssetBundleManifest.GetAllAssetBundles());
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (!list[i].Contains("lua") || !list[i].Contains("unity3d"))
            {
                list.RemoveAt(i);
            }
        }

        bundleCount = list.Count;
        for (int i = 0; i < list.Count; i++)
        {
            string str = list[i];
#if UNITY_ANDROID && !UNITY_EDITOR
            string path = "file://" + Application.persistentDataPath + "/" + LuaConst.osDir + "/" + str;
#else
            string path = Application.persistentDataPath + "/" + LuaConst.osDir + "/" + str;
#endif
            string name = Path.GetFileNameWithoutExtension(str);
            StartCoroutine(CoLoadBundle(name, path));
        }
        yield return StartCoroutine(LoadFinished());
    }

    IEnumerator CoLoadBundle(string name, string path)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path);
        if (request == null)
        {
            Debugger.LogError(name + " bundle not exists");
            yield break;
        }

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debugger.LogError(string.Format("Read {0} failed: {1}", path, request.error));
            yield break;
        }

        --bundleCount;
        AssetBundle assetBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        request.Dispose();
        LuaFileUtils.Instance.AddSearchBundle(name, assetBundle);
    }

    IEnumerator LoadFinished()
    {
        while (bundleCount > 0)
        {
            yield return null;
        }
        OnBundleLoad();
    }

    void OnBundleLoad()
    {
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        Bind();
        LoadLuaFiles();
    }
}