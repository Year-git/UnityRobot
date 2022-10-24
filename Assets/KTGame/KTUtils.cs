using System.IO;
using System.Text;
using System;
using UnityEngine;

public static partial class KTUtils
{
    public static string EnsureDir(string dir) {
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    public static void ResetLocalTransform(Transform tr) {
        tr.localPosition = Vector3.zero;
        tr.localScale = Vector3.one;
        tr.localRotation = Quaternion.identity;
    }

    public static bool IsSceneFile(string path) {
        return path.EndsWith(KTConfigs.kSceneExt);
    }

    public static string GetAssetBundleNameFromPath(string path) {
        return path + KTConfigs.kAssetBundleExt;
    }

    public enum DebugFileExistResult
    {
        OK,
        FileNotExist,
        FilePathCaseSensitive,
    }

#if UNITY_EDITOR_WIN
    [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetLongPathName(string path, System.Text.StringBuilder longPath, int longPathLength);

    

    public static DebugFileExistResult DebugFileExists(string path)
    {
        if (!File.Exists(path))
        {
            return DebugFileExistResult.FileNotExist;
        }

        var longPath = KTStringBuilderCache.Acquire(512);
        GetLongPathName(path, longPath, longPath.Capacity);

        var realDir = Path.GetDirectoryName(longPath.ToString());
        var baseName = Path.GetFileName(path);
        var listFiles = Directory.GetFiles(realDir, baseName, SearchOption.TopDirectoryOnly);
        bool result = System.Array.Exists(listFiles, s => s.Replace('\\', '/') == path);
        return result ? DebugFileExistResult.OK : DebugFileExistResult.FilePathCaseSensitive;
    }
#else
    public static DebugFileExistResult DebugFileExists(string path)
    {
        return File.Exists(path) ? DebugFileExistResult.OK : DebugFileExistResult.FileNotExist;
    }
#endif


    // 递归查询返回第一个子对象
    public static Transform RecursivelyFindFirstChild(Transform parent, string name, bool isFullName = true)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform childTrsf = parent.GetChild(i);
            if (childTrsf.name == name)
            {
                return childTrsf;
            }
            else
            {
                Transform recursiveChildTrsf = RecursivelyFindFirstChild(childTrsf, name);
                if (recursiveChildTrsf) { return recursiveChildTrsf; }
            }
        }
        return null;

    }


    // 递归遍历所有子对象
    public static void RecursiveHandleChildren(Transform parent, Action<GameObject> handlerFunc)
    {
        handlerFunc(parent.gameObject);

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform childTrsf = parent.GetChild(i);
            RecursiveHandleChildren(childTrsf, handlerFunc);
        }
    }

    // pacman 2018-12-27 复制自Voxeland5
    public static Transform FindChildRecursive(this Transform tfm, string name)
    {
        int numChildren = tfm.childCount;

        for (int i = 0; i < numChildren; i++)
            if (tfm.GetChild(i).name == name) return tfm.GetChild(i);

        for (int i = 0; i < numChildren; i++)
        {
            Transform result = tfm.GetChild(i).FindChildRecursive(name);
            if (result != null) return result;
        }

        return null;
    }
}

public static class KTStringBuilderCache
{
    [ThreadStatic]
    static StringBuilder m_cache = new StringBuilder();
    const int kMaxBuilderSize = 512;

    public static StringBuilder Acquire(int capacity = 256) {
        StringBuilder cache = m_cache;
        if (cache == null || cache.Capacity < capacity) {
            return new StringBuilder(capacity);
        }
        m_cache = null;
        cache.Length = 0;
        return cache;
    }

    public static void Release(StringBuilder sb) {
        if (sb.Capacity > kMaxBuilderSize) {
            return;
        }
        m_cache = sb;
    }

    public static string GetStringAndRelease(StringBuilder sb) {
        string str = sb.ToString();
        KTStringBuilderCache.Release(sb);
        return str;
    }
}

public class KTAsyncOpResult
{
    public bool ok;
}
