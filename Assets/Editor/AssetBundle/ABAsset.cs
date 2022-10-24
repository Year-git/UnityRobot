using UnityEngine;
using UnityEditor;
using System.IO;
using FairyGUI;
using System.Collections.Generic;

public enum ABType
{
    //单个文件打包，以文件名为AssetBundlesName
    File = 0,
    //多个文件打包，以文件名为AssetBundlesName
    Files = 1,
    //目录打包，以文件的父目录名为AssetBundlesName
    Directory = 2,
}

[CreateAssetMenu]
public class ABAsset : ScriptableObject
{
    public ABType type = ABType.Directory;
    public string path;
}

[CustomEditor(typeof(ABAsset))]
public class ABAssetEditor : Editor
{
    SerializedProperty path;
    SerializedProperty type;

    private static bool showYes = false;

    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        path = serializedObject.FindProperty("path");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (type.intValue == 0)
        {
            if (GUILayout.Button("Select File"))
            {
                path.stringValue = EditorUtility.OpenFilePanel("", Application.dataPath, "");
                int index = path.stringValue.LastIndexOf("Assets");
                path.stringValue = path.stringValue.Substring(index, path.stringValue.Length - index);
            }
        }
        else
        {
            if (GUILayout.Button("Select Folder"))
            {
                path.stringValue = EditorUtility.OpenFolderPanel("", Application.dataPath, "");
                int index = path.stringValue.LastIndexOf("Assets");
                path.stringValue = path.stringValue.Substring(index, path.stringValue.Length - index);
            }
        }
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    /// <summary>
    /// 开启游戏版本号管理
    /// </summary>
    [MenuItem("Tools/AssetBundle/Set GameVersion Enabled True", false)]
    static void SetGameVersionEnabledTrue()
    {
        BuildTargetGroup type;
#if UNITY_STANDALONE
        type = BuildTargetGroup.Standalone;
#elif UNITY_ANDROID
        type = BuildTargetGroup.Android;
#elif UNITY_IOS
        type = BuildTargetGroup.iOS;
#else
        EditorUtility.DisplayDialog("fail", "set gameVersion enabled true failed", "Yes");
        return;
#endif
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(type);
        if (!symbols.Contains("GAME_VERSION_ENABLED"))
        {
            symbols += ";GAME_VERSION_ENABLED";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(type, symbols);
        }
        EditorUtility.DisplayDialog("success", "set gameVersion enabled true success", "Yes");
    }

    /// <summary>
    /// 关闭游戏版本号管理
    /// </summary>
    [MenuItem("Tools/AssetBundle/Set GameVersion Enabled False", false)]
    static void SetGameVersionEnabledFalse()
    {
        BuildTargetGroup type;
#if UNITY_STANDALONE
        type = BuildTargetGroup.Standalone;
#elif UNITY_ANDROID
        type = BuildTargetGroup.Android;
#elif UNITY_IOS
        type = BuildTargetGroup.iOS;
#else
        EditorUtility.DisplayDialog("fail", "set gameVersion enabled false failed", "Yes");
        return;
#endif
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(type);
        if (symbols.Contains(";GAME_VERSION_ENABLED"))
        {
            symbols = symbols.Replace(";GAME_VERSION_ENABLED", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(type, symbols);
        }
        EditorUtility.DisplayDialog("success", "set gameVersion enabled false success", "Yes");
    }

    /// <summary>
    /// 清除所有的AssetBundleName
    /// </summary>
    [MenuItem("Tools/AssetBundle/Clear AssetBundles Names", false)]
    static void ClearAssetBundlesName()
    {
        EditorUtility.DisplayProgressBar("Hold on...", "Clear AssetBundles Names", 1f);
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
        EditorUtility.ClearProgressBar();
        if(!showYes){
            EditorUtility.DisplayDialog("success", "clear assetBundles names success", "Yes");
        }
    }

    /// <summary>
    /// Res
    /// 创建资源AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/Sigle AssetBundle/Set Res AssetBundle Names", false)]
    public static void BuildResBundles()
    {
        //加载ABAsset配置文件
        DirectoryInfo dir = new DirectoryInfo("Assets/Editor/ABAssets");
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Hold on...", "Set Res AssetBundle Names", 1f);
            if (!files[i].Name.EndsWith(".meta"))
            {
                ABAsset asset = AssetDatabase.LoadAssetAtPath<ABAsset>("Assets/Editor/ABAssets/" + files[i].Name);
                //解析ABAsset配置文件
                if (asset.type == ABType.File)
                {
                    FileInfo info = new FileInfo(asset.path);
                    if (!info.Name.EndsWith(".meta"))
                    {
                        SetABName(info.FullName, info.FullName, info.Name);
                    }
                    else
                    {
                        Debug.LogError("打包失败：Meta文件不能参与打包！错误路径：" + asset.path + "！");
                    }
                }
                else
                {
                    SetAssetBundlesName(asset.path, asset.type);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        if(!showYes){
            EditorUtility.DisplayDialog("success", "set res assetbundle names success", "Yes");
        }
    }

    /// <summary>
    /// FGUI
    /// 创建资源AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/Sigle AssetBundle/Set FGUI AssetBundle Names", false)]
    public static void BuildFGUIBundles()
    {
        EditorUtility.DisplayProgressBar("Hold on...", "Set FGUI AssetBundle Names", 1f);
        //加载ABAsset配置文件
        DirectoryInfo dir = new DirectoryInfo("Assets/Res/FGUI");
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileSystemInfo file = files[i];
            if (file.Name.EndsWith(".bytes"))
            {
                //这个路径必须是以Assets开始的路径
                string importerPath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                //得到Asset
                AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);
                //最终设置assetBundleName
                int index = file.Name.IndexOf(".bytes");
                string sName = "fgui_" + file.Name.Substring(0, index);
                assetImporter.assetBundleName = sName.ToLower();
            }
            else if (!file.Name.EndsWith(".meta"))
            {
                //这个路径必须是以Assets开始的路径
                string importerPath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                //得到Asset
                AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);

                string[] nameList = file.Name.Split('_');
                string sName = "fgui_" + nameList[0] + "_res";
                assetImporter.assetBundleName = sName.ToLower();
            }
        }

        EditorUtility.ClearProgressBar();
        if(!showYes){
            EditorUtility.DisplayDialog("success", "set FGUI assetbundle names success", "Yes");
        }
    }

    /// <summary>
    /// Behaviac
    /// 创建资源AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/Sigle AssetBundle/Set Behaviac AssetBundle Names", false)]
    public static void BuildBehaviacBundles()
    {
        EditorUtility.DisplayProgressBar("Hold on...", "Set Behaviac Bundle AssetBundle Names", 1f);
        
        RecursionDirectory("Assets/Res/Behaviac", delegate (FileSystemInfo pFile)
            {
                if (pFile.Name.EndsWith(".xml"))
                {
                    //这个路径必须是以Assets开始的路径
                    string importerPath = "Assets" + pFile.FullName.Substring(Application.dataPath.Length);
                    //得到Asset
                    AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);
                    //最终设置assetBundleName
                    string[] sList = pFile.Name.Split(new char[1] {'.'});
                    string sName = "behaviac_" + sList[0];
                    assetImporter.assetBundleName = sName.ToUpper();
                }
            }
        );

        EditorUtility.ClearProgressBar();
        if(!showYes){
            EditorUtility.DisplayDialog("success", "set Behaviac assetbundle names success", "Yes");
        }
    }

    /// <summary>
    /// 递归目录
    /// </summary>
    public static void RecursionDirectory(string sFullName, System.Action<FileSystemInfo> pDispose)
    {
        DirectoryInfo dir = new DirectoryInfo(sFullName);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileSystemInfo file = files[i];
            if (file.Attributes == FileAttributes.Directory)
            {
                string sDirFullName = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                sDirFullName = sDirFullName.Replace('\\', '/');
                RecursionDirectory(sDirFullName, pDispose);
            }
            else
            {
                pDispose?.Invoke(file);
            }
        }
    }

    /// <summary>
    /// Scene
    /// 创建资源AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/Sigle AssetBundle/Set Scene AssetBundle Names", false)]
    public static void BuildSceneBundles()
    {
        EditorUtility.DisplayProgressBar("Hold on...", "Set Scenes Bundle AssetBundle Names", 1f);
        //加载ABAsset配置文件
        DirectoryInfo dir = new DirectoryInfo("Assets/Scenes");
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileSystemInfo file = files[i];
            if (file.Name.EndsWith(".unity"))
            {
                //这个路径必须是以Assets开始的路径
                string importerPath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                //得到Asset
                AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);
                //最终设置assetBundleName
                int index = file.Name.IndexOf(".unity");
                string sName = "scene_" + file.Name.Substring(0, index);
                assetImporter.assetBundleName = sName.ToUpper();
            }
        }
        EditorUtility.ClearProgressBar();
        
        if(!showYes){
            EditorUtility.DisplayDialog("success", "set Scene assetbundle names success", "Yes");
        }
    }


    /// <summary>
    /// Lua
    /// 创建Lua AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/Sigle AssetBundle/Set Lua AssetBundle Names", false)]
    public static void BuildNotJitBundles()
    {
        ToLuaMenu.BuildNotJitBundles();
        EditorUtility.DisplayDialog("success", "set lua assetbundle names success", "Yes");
    }

    
    /// <summary>
    /// Lua
    /// 创建Lua AssetBundle临时文件
    /// </summary>
    [MenuItem("Tools/AssetBundle/OneKey", false)]
    public static void OneKey()
    {
        showYes = true;
        ClearAssetBundlesName();
        BuildResBundles();
        BuildFGUIBundles();
        BuildBehaviacBundles();
        BuildSceneBundles();
        BuildNotJitBundles();
        showYes = false;
    }

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// /// </summary>
    /// <param name="_assetsPath"></param>
    static void SetAssetBundlesName(string _assetsPath, ABType type)
    {
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                SetAssetBundlesName(files[i].FullName, type);
            }
            else if (!files[i].Name.EndsWith(".meta"))
            {
                SetABName(files[i].FullName, files[i].FullName, files[i].Name);
            }
        }
    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="fullName"></param>
    /// <param name="fileName"></param>
    static void SetABName(string assetPath, string fullName, string fileName)
    {
        //这个路径必须是以Assets开始的路径
        string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
        //得到Asset
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);

        string directoryName = fullName.Replace("\\", "/");
        directoryName = directoryName.Replace(fileName, "");
        directoryName = directoryName.Replace(Application.dataPath, "");
        directoryName = directoryName.Substring(1, directoryName.Length - 2);
        directoryName = directoryName.Replace("/", "_");
        //最终设置assetBundleName
        assetImporter.assetBundleName = directoryName;
    }


}