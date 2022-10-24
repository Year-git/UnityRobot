using UnityEngine;
using System.IO;
using UnityEngine.Assertions;

//[SLua.CustomLuaClass]
public class KTConfigs
{
    public const string kSceneExt = ".unity";
    public const string kLuaExt = ".lua";
    public const string kLuaBytesExt = ".bytes";
    public const string kLuaBundleName = "luacode.kt";
    public const string kAssetBundleExt = ".kt";
    public const string kManifestAssetName = "AssetBundleManifest";
    public const string kAssetLookTableAssetPath = "Assets/Package/assets_look_table.asset";
    public const string kLookTableBundleName = "look_table.kt";

    public const string kBundleAssetDir = "Assets/StreamingAssets";
    public const string kUiAssetDir = "Assets/ui";
    public const string kUiEditAssetDir = "Assets/ui/edit";
    public const string kLuaAssetDir = "Assets/luacode";
	public const string kWeatherDir = "Assets/weathers";
    public static readonly string kClientDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../"));
    public static readonly string kLuaCodeDir = Path.Combine(kClientDir, "luacode").Replace('\\', '/');
    public static readonly string kBuildRootDir = Path.Combine(kClientDir, "build").Replace('\\', '/');
    public const bool kUseBundleInEditor = false;

    public static string GetBundlePublishDir(string platformName)
    {
        return Path.Combine(Application.streamingAssetsPath, platformName);
    }

    public static string GetBuildDir(string platformName, string mode)
    {
        return Path.Combine(kBuildRootDir, Path.Combine(platformName, mode));
    }
}
