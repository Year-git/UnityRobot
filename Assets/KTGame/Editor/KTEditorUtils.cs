using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;
using System.Reflection;

public struct KTAssetInfo<T>
{
    public T asset;
    public string path;

    public bool isValid { get { return asset != null; } }
    public string filename { get { return Path.GetFileName(path); } }
    public string filenameNoExt { get { return Path.GetFileNameWithoutExtension(path); } }
    public string dirname { get { return Path.GetDirectoryName(path); } }

    public string ParallelFile(string ext)
    {
        return string.Format("{0}/{1}{2}", dirname, filenameNoExt, ext);
    }
}

public class ProfileTime : System.IDisposable
{
    private string _label;
    private int _startTime;

    public ProfileTime(string label)
    {
        _label = label;
        _startTime = System.Environment.TickCount;
    }

    public void Dispose()
    {
        var elapse = System.Environment.TickCount - _startTime;
        Debug.LogFormat("{0} elapse is {1}", _label, elapse);
    }
}

[System.Flags]
public enum KTTextureImportFlags
{
    Readable = 1 << 0,
    Uncompressed = 1 << 1,
    FormatRGBA = 1 << 2,

    LightmapColorEditMode = Readable | Uncompressed,
    LightmapShadowMaskEditMode = Readable | Uncompressed | FormatRGBA,
    LightmapRuntimeMode = 0,
}

public static class KTEditorUtils
{

    #region Colume Editor
    static Stack<float> s_lineHeightStack = new Stack<float>();

    public static Rect BeginColume(Rect position, float lineHeight, float offset, float width = 0.0f) {
        var gap = (lineHeight - EditorGUIUtility.singleLineHeight) / 2.0f;
        var result = new Rect(position);
        result.x += offset;
        result.y += gap;
        if (width > 0.0f) {
            result.width = width;
        } else {
            result.width = position.width - offset;
        }
        result.height = EditorGUIUtility.singleLineHeight;
        s_lineHeightStack.Push(lineHeight);
        return result;
    }

    public static Rect NextLine(Rect rc) {
        var result = new Rect(rc);
        var lineHeight = s_lineHeightStack.Peek();
        result.y += lineHeight;
        return result;
    }

    public static void EndColume() {
        s_lineHeightStack.Pop();
    }
    #endregion

    public static bool PropertyComponentTypeField(Rect? rc, SerializedProperty componentTypeProp, GameObject go) {
        if (go == null || componentTypeProp == null) {
            return false;
        }

        var componentType = componentTypeProp.stringValue;
		string []  result = go.GetComponents<Component>().Select(t => t.GetType().FullName).ToArray();

		string [] componentTypeStrs = new string[result.Length + 1];

		int result_i = 0;
		for (; result_i < result.Length; result_i++) {
			componentTypeStrs [result_i] = result [result_i];
		}
		componentTypeStrs[result_i] = "gameObject";

        int componentIndex = componentTypeStrs.Length - 1;
        for (int i = 0; i < componentTypeStrs.Length; ++i) {
            var name = componentTypeStrs[i];
            if (name == componentType) {
                componentIndex = i;
                break;
            }
        }



        if (rc != null) {
            componentIndex = EditorGUI.Popup(rc.Value, componentIndex, componentTypeStrs.ToArray());
        } else {
            componentIndex = EditorGUILayout.Popup(componentIndex, componentTypeStrs.ToArray());
        }
        bool changed = componentTypeProp.stringValue != componentTypeStrs[componentIndex];
        componentTypeProp.stringValue = componentTypeStrs[componentIndex];

        return changed;
    }

    public static bool IsAssetFolder(Object obj) {
        if (obj == null) {
            return false;
        }

        var path = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(path)) {
            return Directory.Exists(path);
        }

        return false;
    }

    public static string GetSelectAssetDir() {
        var obj = Selection.activeObject;
        if (obj == null) {
            return "Assets";
        }

        var path = AssetDatabase.GetAssetPath(obj);
        if (IsAssetFolder(obj)) {
            return path;

        } else {
            return Path.GetDirectoryName(path);
        }
    }

    public static bool CheckUniqueResource(System.Type type) {
        var settings = Resources.FindObjectsOfTypeAll(type);
        var exist = false;
        if (settings.Length > 0) {
            for (int i = 0; i < settings.Length; ++i) {
                var obj = settings[i];
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path)) {
                    exist = true;
                    Debug.LogWarningFormat("资源已经存在, path={0}", path);
                }
            }
        }
        return exist;
    }

    public static void CreateUniqueScriptObject(ScriptableObject obj, string name) {
        if (CheckUniqueResource(obj.GetType())) {
            return;
        }

        var dir = GetSelectAssetDir();
        var path = Path.Combine(dir, name + ".asset");
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
    }

    public static KTAssetInfo<T> CheckSelectAsset<T>() where T : Object
    {
        var ret = new KTAssetInfo<T>();

        var selAsset = Selection.activeObject as T;
        if (selAsset != null)
        {
            var path = AssetDatabase.GetAssetPath(selAsset);
            if (path != null)
            {
                ret.asset = selAsset;
                ret.path = path;
            }
        }

        return ret;
    }

    public static void FrameSelect(GameObject go)
    {
        var sceneView = SceneView.lastActiveSceneView;
        Selection.activeGameObject = go;
        sceneView.FrameSelected(true);
    }

    public static void RemoveComponentsInChildren<T>(GameObject go) where T : Component
    {
        var comps = go.GetComponentsInChildren<T>();
        foreach (var comp in comps)
        {
            GameObject.DestroyImmediate(comp);
        }
    }

    public static void ReplaceShaderInChildren(GameObject go, Shader shader)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mtl in renderer.sharedMaterials)
            {
                mtl.shader = shader;
            }
        }
    }

    public static void EnsureTextureReadable(Texture2D tex, bool readable = true)
    {
        if (tex == null)
        {
            return;
        }

        var path = AssetDatabase.GetAssetPath(tex);
        if (path == null)
        {
            return;
        }

        var importer = AssetImporter.GetAtPath(path);
        var texImporter = importer as TextureImporter;
        if (texImporter != null)
        {
            if (texImporter.isReadable != readable)
            {
                texImporter.isReadable = readable;
                texImporter.SaveAndReimport();
            }
        }
    }

    public static string ParallelFile(string file, string ext)
    {
        return string.Format("{0}/{1}{2}", 
            Path.GetDirectoryName(file),
            Path.GetFileNameWithoutExtension(file),
            ext);
    }

    public static string[] GetFiles(string path, string pattern = "*.*", System.IO.SearchOption mode = SearchOption.AllDirectories)
    {
        if(Directory.Exists(path))
        {
            return Directory.GetFiles(path, pattern, mode);
        }
        return new string[0];
    }

    public static IEnumerable<T> Each<T>(IEnumerable<T> values, System.Action<T> func)
    {
        foreach (var v in values)
        {
            func(v);
        }
        return values;
    }

    public static IEnumerable<string> DumpInfos(IEnumerable<string> infos, string title = null, string logFile = null)
    {
        var sb = KTStringBuilderCache.Acquire();
        if (!string.IsNullOrEmpty(title))
        {
            sb.AppendLine(title);
        }

        foreach (var path in infos)
        {
            sb.AppendLine(path);
        }

        var log = KTStringBuilderCache.GetStringAndRelease(sb);
        Debug.Log(log);

        if (!string.IsNullOrEmpty(logFile))
        {
            File.WriteAllText(logFile, log);
        }
        return infos;
    }

    public static IEnumerable<string> GetAllAssetsPathOfType<T>() where T : Object
    {
        var filter = string.Format("t:{0}", typeof(T).Name);
        return AssetDatabase
           .FindAssets(filter)
           .Select(guid => AssetDatabase.GUIDToAssetPath(guid));
    }

    public static IEnumerable<T> LoadAllAssetsOfType<T>() where T : Object
    {
        return GetAllAssetsPathOfType<T>()
           .Select(p => AssetDatabase.LoadAssetAtPath<T>(p))
           .Where(s => s != null);
    }

    public static T SaveScriptObjectAsset<T>(T asset, string dir = null) where T : ScriptableObject
    {
        if (dir == null)
        {
            dir = KTEditorUtils.GetSelectAssetDir();
        }

        var path = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", dir, typeof(T).Name));
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    public static T CreateScriptObjectAsset<T>(string dir = null) where T : ScriptableObject
    {
        return SaveScriptObjectAsset(ScriptableObject.CreateInstance<T>(), dir);
    }

    public static void ClearAssetUnderDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        var di = new DirectoryInfo(dir);
        foreach (var fileInfo in di.GetFiles())
        {
            var filename = fileInfo.Name;
            if (!filename.EndsWith(".meta"))
            {
                AssetDatabase.DeleteAsset(string.Format("{0}/{1}", dir, filename));
            }
        }
        foreach (var dirInfo in di.GetDirectories())
        {
            AssetDatabase.DeleteAsset(string.Format("{0}/{1}", dir, dirInfo.Name));
        }
    }

    public static T GetFirstAssetOfType<T>() where T : Object
    {
        var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
        if (guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return null;
    }

    public static T FocusAsset<T>(T asset) where T : Object
    {
        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
        return asset;
    }

    public static T FocusFirstAssetOfType<T>() where T : Object
    {
        return FocusAsset(GetFirstAssetOfType<T>());
    }

    public static void ChangeTextureImport(Texture2D tex, KTTextureImportFlags flags)
    {
        var path = AssetDatabase.GetAssetPath(tex);
        ChangeTextureImport(path, flags);
    }

    public static void ChangeTextureImport(string path, KTTextureImportFlags flags)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            bool dirty = false;

            bool nextReadable = (flags & KTTextureImportFlags.Readable) != 0;
            if (importer.isReadable != nextReadable)
            {
                importer.isReadable = nextReadable;
                dirty = true;
            }

            bool nextUncompressed = (flags & KTTextureImportFlags.Uncompressed) != 0;
            bool curUncompressed = importer.textureCompression == TextureImporterCompression.Uncompressed;
            if (nextUncompressed != curUncompressed)
            {
                importer.textureCompression = nextUncompressed ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;
                dirty = true;
            }

            var buildTargetName = EditorUserBuildSettings.activeBuildTarget.ToString();
            var platformName = buildTargetName.StartsWith("Standalone") ? "Standalone" : buildTargetName;
            var settings = importer.GetPlatformTextureSettings(platformName);
            if (settings != null)
            {
                if ((flags & KTTextureImportFlags.FormatRGBA) != 0)
                {
                    if (!settings.overridden || settings.format != TextureImporterFormat.RGBA32)
                    {
                        settings.overridden = true;
                        settings.format = TextureImporterFormat.RGBA32;
                        importer.SetPlatformTextureSettings(settings);
                        dirty = true;
                    }
                }
                else if (settings.overridden)
                {
                    settings.overridden = false;
                    dirty = true;
                }
            }

            if (dirty)
            {
                importer.SaveAndReimport();
            }
        }
    }

    public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : System.Attribute
    {
        return provider.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
    }

    public static Component GetLightmapPaintRenderer(Transform trans)
    {
        var meshRenderer = trans.GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.enabled)
        {
            return meshRenderer;
        }
        var terrain = trans.GetComponent<Terrain>();
        if (terrain != null && terrain.enabled)
        {
            return terrain;
        }
        return null;
    }

    public static int GetLightmapIndex(Component renderer)
    {
        var meshRenderer = renderer as MeshRenderer;
        if (meshRenderer != null)
        {
            return meshRenderer.lightmapIndex;
        }
        var terrain = renderer as Terrain;
        if (terrain != null)
        {
            return terrain.lightmapIndex;
        }

        Debug.LogErrorFormat("GetLightmapScaleOffset unknown renderer type {0}", renderer.GetType());
        return -1;
    }

    public static Vector4 GetLightmapScaleOffset(Component renderer)
    {
        var meshRenderer = renderer as MeshRenderer;
        if (meshRenderer != null)
        {
            return meshRenderer.lightmapScaleOffset;
        }
        var terrain = renderer as Terrain;
        if (terrain != null)
        {
            return terrain.lightmapScaleOffset;
        }

        Debug.LogErrorFormat("GetLightmapScaleOffset unknown renderer type {0}", renderer.GetType());
        return Vector4.zero;
    }

    private static string SimpleFileMapping(string dstDir, string relativePath)
    {
        return string.Format("{0}/{1}", dstDir, relativePath);
    }

    public static void SyncFilesToProject(string srcRootDir, string dstRootDir, string pattern = "*.*", System.Func<string, string, string> mapPathFunc = null)
    {
        const string title = "SyncFilesToProject";
        EditorUtility.DisplayProgressBar(title, string.Format("clean assets under {0}", dstRootDir), 0.0f);
        if (Directory.Exists(dstRootDir))
        {
            Directory.Delete(dstRootDir, true);
        }

        var info = string.Format("[{0}]{1}=>{2}", pattern, srcRootDir, dstRootDir);
        var paths = Directory.GetFiles(srcRootDir, pattern, SearchOption.AllDirectories);

        int relIdx = srcRootDir.Length;
        float progress = 0.0f;
        float progressStep = 1.0f / paths.Count();

        if (mapPathFunc == null)
        {
            mapPathFunc = SimpleFileMapping;
        }

        foreach (var path in paths)
        {
            var relativePath = path.Substring(relIdx + 1);
            var dstPath = mapPathFunc(dstRootDir, relativePath);
            var dstDir = Path.GetDirectoryName(dstPath);
            KTUtils.EnsureDir(dstDir);

            File.Copy(path, dstPath, true);

            progress += progressStep;
            EditorUtility.DisplayProgressBar(title, info, progress);
        }
        EditorUtility.ClearProgressBar();
    }

    public static T GetOrCreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        if (File.Exists(path))
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        var result = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(result, path);
        return result;
    }
}
