using UnityEngine;
using System.Xml;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 版本管理类
/// </summary>
public class GameVersion : CoroutineManager
{
    // 包内版本文件
    private static XmlDocument streamingVersion;
    // 包内版本文件路径
    private static readonly string streamingAssetsPath = Application.streamingAssetsPath + "/" + LuaConst.osDir;
    // 沙盒版本文件
    private static XmlDocument persistentVersion;
    // 沙盒版本文件路径
#if UNITY_ANDROID && !UNITY_EDITOR
    private static readonly string persistentDataPath = "file://" + Application.persistentDataPath + "/" + LuaConst.osDir;
#else
    private static readonly string persistentDataPath = Application.persistentDataPath + "/" + LuaConst.osDir;
#endif
    private static readonly string writePersistentDataPath = Application.persistentDataPath + "/" + LuaConst.osDir;
    // cdn版本文件
    private static XmlDocument cdnVersion;
    // cdn版本文件路径
    private static readonly string cdnVersionPath = "";
    // 文件数量
    private static int fileCount = int.MaxValue;

    public static void Start()
    {
#if GAME_VERSION_ENABLED
        StartCoroutine(CheckVersion());
#else
        Main.Instance.StartLuaEngine();
#endif
    }

    /// <summary>
    /// 如果沙盒路径下没有版本文件或者版本号小于包内路径下的版本号，则复制包内文件到沙盒路径下，再进行cdn版本比较
    /// </summary>
    /// <returns></returns>
    private static IEnumerator CheckVersion()
    {
        //读取streamingVersion xml文件
        UnityWebRequest request = UnityWebRequest.Get(streamingAssetsPath + "/version.xml");
        yield return request.SendWebRequest();
        if (request.error != null || request.responseCode != 200)
        {
            request.Dispose();
            Debug.LogError("包内版本文件不存在，版本检测流程失败！");
            yield break;
        }
        else
        {
            streamingVersion = LoadVersionXml(request.downloadHandler.text);
            request.Dispose();
        }

        //读取persistentVersionPath xml文件
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!File.Exists(persistentDataPath.Replace("file://", "") + "/version.xml"))
        {
            yield return StartCoroutine(CopyFolder());
        }
#else
        if (!File.Exists(persistentDataPath + "/version.xml"))
        {
            yield return StartCoroutine(CopyFolder());
        }
#endif

        UnityWebRequest www = UnityWebRequest.Get(persistentDataPath + "/version.xml");
        yield return www.SendWebRequest();
        if (www.error != null || www.responseCode != 200)
        {
            www.Dispose();
            Debug.LogError("读取persistentVersion失败，版本检测流程失败！");
            yield break;
        }

        persistentVersion = LoadVersionXml(www.downloadHandler.text);
        www.Dispose();

        //包内版本号比较
        int persistentGameVersion = GetGameVersion(persistentVersion);
        int streamingGameVersion = GetGameVersion(streamingVersion);
        if (persistentGameVersion == -1 || streamingGameVersion == -1)
        {
            yield break;
        }
        if (streamingGameVersion > persistentGameVersion)
        {
            yield return StartCoroutine(CopyFolder());
        }

        //cdn版本号比较
        if (!string.IsNullOrEmpty(cdnVersionPath))
        {
            yield return StartCoroutine(CheckCdnVersion());
        }

        //启动toLua引擎
        Main.Instance.StartLuaEngine();
    }

    /// <summary>
    /// 与cdn版本文件对比差异
    /// </summary>
    private static IEnumerator CheckCdnVersion()
    {
        string path = cdnVersionPath + "version.xml?" + DateTime.Now.ToString();
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.error != null || request.responseCode != 200)
        {
            request.Dispose();
            Debug.LogError("读取cdnVersion文件失败，版本检测流程失败！");
            yield break;
        }
        else
        {
            //读取cdnVersion xml文件
            cdnVersion = LoadVersionXml(request.downloadHandler.text);
            //对比xml文件差异，并更新差异文件
            int cdnGameVersion = GetGameVersion(cdnVersion);
            int persistentGameVersion = GetGameVersion(persistentVersion);
            if (cdnGameVersion == -1 || persistentGameVersion == -1)
            {
                yield break;
            }
            if (cdnGameVersion > persistentGameVersion)
            {
                yield return StartCoroutine(CheckCdnVersionDifference());
                File.WriteAllBytes(writePersistentDataPath + "/version.xml", request.downloadHandler.data);
            }
            request.Dispose();
        }
    }

    /// <summary>
    /// 对比版本差异，并更新文件
    /// </summary>
    private static IEnumerator CheckCdnVersionDifference()
    {
        Dictionary<string, string> cdnFilesMD5 = GetFilesMD5(cdnVersion);
        Dictionary<string, string> persistentFilesMD5 = GetFilesMD5(persistentVersion);
        foreach (string fileName in cdnFilesMD5.Keys)
        {
            cdnFilesMD5.TryGetValue(fileName, out string cdnFileMD5);
            if (persistentFilesMD5.ContainsKey(fileName))
            {
                persistentFilesMD5.TryGetValue(fileName, out string persistentFileMD5);
                if (persistentFileMD5 != cdnFileMD5)
                {
                    yield return StartCoroutine(UpdateFile(fileName));
                    UpdateFileMD5(persistentVersion, fileName, cdnFileMD5);
                }
            }
            else
            {
                yield return StartCoroutine(UpdateFile(fileName));
                UpdateFileMD5(persistentVersion, fileName, cdnFileMD5);
            }
        }
    }

    /// <summary>
    /// 加载xml文件
    /// </summary>
    /// <param name="str"></param>
    private static XmlDocument LoadVersionXml(string str)
    {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(str);
        return xml;
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    private static int GetGameVersion(XmlDocument version)
    {
        XmlNodeList xmlNodeList = version.SelectNodes("data/game");
        if (xmlNodeList.Count < 0)
        {
            Debug.LogError("GetGameVersion错误，版本检测流程失败！");
            return -1;
        }
        XmlNode item = xmlNodeList[0];
        string gameVersion = item.Attributes["GameVersion"].InnerText;

        if (CheckVersionFormat(gameVersion))
        {
            for (int i = 0; i < gameVersion.Length; i++)
            {
                gameVersion = gameVersion.Replace(".", "");
            }
            return int.Parse(gameVersion);
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 检测版本号是否合法(0.0.0.0)
    /// </summary>
    /// <param name="strVersion"></param>
    /// <returns></returns>
    private static bool CheckVersionFormat(string strVersion)
    {
        if (strVersion.Length != 7)
        {
            Debug.LogError("非法版本号，版本检测流程失败！");
            return false;
        }
        string[] strVersionCheck = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." };
        string strCheck = strVersion;
        for (int i = 0; i < strVersionCheck.Length; i++)
        {
            strCheck = strCheck.Replace(strVersionCheck[i], "");
        }
        if (!string.IsNullOrEmpty(strCheck))
        {
            Debug.LogError("非法版本号，版本检测流程失败！");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取AssetBundle文件MD5集合
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GetFilesMD5(XmlDocument version)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        XmlNodeList xmlNodeList = version.SelectNodes("data/file");
        if (xmlNodeList.Count < 0)
        {
            return ret;
        }
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XmlNode item = xmlNodeList[i];
            XmlAttribute attribute = item.Attributes[0];
            ret.Add(attribute.Name, attribute.Value);
        }
        return ret;
    }

    /// <summary>
    /// 更新文件md5值
    /// </summary>
    /// <param name="version"></param>
    /// <param name="fileName"></param>
    /// <param name="md5"></param>
    private static void UpdateFileMD5(XmlDocument version, string fileName, string md5)
    {
        XmlNodeList xmlNodeList = version.SelectNodes("data/file");
        if (xmlNodeList.Count < 0)
        {
            return;
        }
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XmlNode item = xmlNodeList[i];
            XmlAttribute attribute = item.Attributes[0];
            if (attribute.Name == fileName)
            {
                attribute.Value = "md5";

                //以utf8格式写入文件
                StreamWriter sw = new StreamWriter(persistentDataPath + "/version.xml", false, new UTF8Encoding(false));
                version.Save(sw);
                sw.Close();
                break;
            }
        }
    }

    private static IEnumerator CopyFolder()
    {
        string filePath = streamingAssetsPath + "/" + LuaConst.osDir;
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(filePath);
        yield return request.SendWebRequest();

        if (request.error != null || request.responseCode != 200)
        {
            request.Dispose();
            Debug.LogError("CopyBundleFile失败，版本检测流程失败！");
            yield break;
        }
        else
        {
            //FindFiles
            AssetBundle assetBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            request.Dispose();
            AssetBundleManifest manifest = (AssetBundleManifest)assetBundle.LoadAsset("AssetBundleManifest");
            assetBundle.Unload(false);
            assetBundle = null;
            List<string> fileList = new List<string>(manifest.GetAllAssetBundles());
            List<string> manifestList = new List<string>();
            for (int i = 0; i < fileList.Count; i++)
            {
                manifestList.Add(fileList[i] + ".manifest");
            }
            fileList.AddRange(manifestList);
            fileList.Add(LuaConst.osDir);
            fileList.Add(LuaConst.osDir + ".manifest");
            fileList.Add("version.xml");

            //CopyFiles
            fileCount = fileList.Count;
            for (int i = 0; i < fileList.Count; i++)
            {
                StartCoroutine(CopyFile(fileList[i]));
            }
            yield return StartCoroutine(CopyFinished());
        }
    }

    private static IEnumerator CopyFile(string fileName)
    {
        UnityWebRequest request = UnityWebRequest.Get(streamingAssetsPath + "/" + fileName);
        yield return request.SendWebRequest();

        if (request.error != null || request.responseCode != 200)
        {
            throw new Exception("CoLoadBundle失败，版本检测流程失败！");
        }
        else
        {
            if (!Directory.Exists(writePersistentDataPath))
            {
                Directory.CreateDirectory(writePersistentDataPath);
            }
            File.WriteAllBytes(writePersistentDataPath + "/" + fileName, request.downloadHandler.data);
            request.Dispose();
            --fileCount;
        }
    }

    private static IEnumerator CopyFinished()
    {
        while (fileCount > 0)
        {
            yield return null;
        }
    }

    private static IEnumerator UpdateFile(string fileName)
    {
        string srcPath = cdnVersionPath + fileName;
        UnityWebRequest request = UnityWebRequest.Get(srcPath);
        yield return request.SendWebRequest();

        if (request.error != null || request.responseCode != 200)
        {
            request.Dispose();
            Debug.LogError("UpdateFile失败，版本检测流程失败！");
            yield break;
        }
        else
        {
            if (!Directory.Exists(writePersistentDataPath))
            {
                Directory.CreateDirectory(writePersistentDataPath);
            }
            File.WriteAllBytes(writePersistentDataPath + "/" + fileName, request.downloadHandler.data);
            request.Dispose();
        }
    }
}