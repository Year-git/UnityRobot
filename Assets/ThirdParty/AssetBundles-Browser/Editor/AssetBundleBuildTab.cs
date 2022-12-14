using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AssetBundleBrowser.AssetBundleDataSource;
using System.Xml;
using System.Text;
using LuaInterface;

namespace AssetBundleBrowser
{
    [System.Serializable]
    internal class AssetBundleBuildTab
    {
#if UNITY_STANDALONE
        private readonly string m_osDir = "Windows";
#elif UNITY_ANDROID
        private readonly string m_osDir = "Android";
#elif UNITY_IOS
        private readonly string m_osDir = "iOS";
#else
        private readonly string m_osDir = "";
#endif

        const string k_BuildPrefPrefix = "ABBBuild:";

        private readonly string m_streamingPath = Application.streamingAssetsPath;

        [SerializeField]
        private bool m_AdvancedSettings;

        [SerializeField]
        private Vector2 m_ScrollPosition;


        class ToggleData
        {
            internal ToggleData(bool s,
                string title,
                string tooltip,
                List<string> onToggles,
                BuildAssetBundleOptions opt = BuildAssetBundleOptions.None)
            {
                if (onToggles.Contains(title))
                    state = true;
                else
                    state = s;
                content = new GUIContent(title, tooltip);
                option = opt;
            }
            //internal string prefsKey
            //{ get { return k_BuildPrefPrefix + content.text; } }
            internal bool state;
            internal GUIContent content;
            internal BuildAssetBundleOptions option;
        }

        private AssetBundleInspectTab m_InspectTab;

        [SerializeField]
        private BuildTabData m_UserData;

        List<ToggleData> m_ToggleData;
        ToggleData m_ForceRebuild;
        ToggleData m_CopyToStreaming;
        GUIContent m_TargetContent;
        GUIContent m_CompressionContent;
        internal enum CompressOptions
        {
            Uncompressed = 0,
            StandardCompression,
            ChunkBasedCompression,
        }

        readonly GUIContent[] m_CompressionOptions =
        {
            new GUIContent("No Compression"),
            new GUIContent("Standard Compression (LZMA)"),
            new GUIContent("Chunk Based Compression (LZ4)")
        };
        readonly int[] m_CompressionValues = { 0, 1, 2 };


        internal AssetBundleBuildTab()
        {
            m_AdvancedSettings = false;
            m_UserData = new BuildTabData
            {
                m_OnToggles = new List<string>(),
                m_UseDefaultPath = true
            };
        }

        internal void OnDisable()
        {
            var dataPath = System.IO.Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserBuild.dat";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(dataPath);

            bf.Serialize(file, m_UserData);
            file.Close();

        }
        internal void OnEnable(EditorWindow parent)
        {
            m_InspectTab = (parent as AssetBundleBrowserMain).m_InspectTab;

            //LoadData...
            var dataPath = Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserBuild.dat";

            if (File.Exists(dataPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(dataPath, FileMode.Open);
                if (bf.Deserialize(file) is BuildTabData data)
                    m_UserData = data;
                file.Close();
            }

            m_ToggleData = new List<ToggleData>
            {
                new ToggleData(
                false,
                "Exclude Type Information",
                "Do not include type information within the asset bundle (don't write type tree).",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.DisableWriteTypeTree),
                new ToggleData(
                false,
                "Force Rebuild",
                "Force rebuild the asset bundles",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.ForceRebuildAssetBundle),
                new ToggleData(
                false,
                "Ignore Type Tree Changes",
                "Ignore the type tree changes when doing the incremental build check.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.IgnoreTypeTreeChanges),
                new ToggleData(
                false,
                "Append Hash",
                "Append the hash to the assetBundle name.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.AppendHashToAssetBundleName),
                new ToggleData(
                false,
                "Strict Mode",
                "Do not allow the build to succeed if any errors are reporting during it.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.StrictMode),
                new ToggleData(
                false,
                "Dry Run Build",
                "Do a dry run build.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.DryRunBuild)
            };


            m_ForceRebuild = new ToggleData(
                false,
                "Clear Folders",
                "Will wipe out all contents of build directory as well as StreamingAssets/AssetBundles if you are choosing to copy build there.",
                m_UserData.m_OnToggles);
            m_CopyToStreaming = new ToggleData(
                false,
                "Copy to StreamingAssets",
                "After build completes, will copy all build content to " + m_streamingPath + " for use in stand-alone player.",
                m_UserData.m_OnToggles);

            m_TargetContent = new GUIContent("Build Target", "Choose target platform to build for.");
            m_CompressionContent = new GUIContent("Compression", "Choose no compress, standard (LZMA), or chunk based (LZ4)");

            if (m_UserData.m_UseDefaultPath)
            {
                ResetPathToDefault();
            }
        }

        internal void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            bool newState = false;
            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.UpperCenter
            };
            GUILayout.Label(new GUIContent("Example build setup"), centeredStyle);
            //basic options
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            // build target
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildTarget))
            {
                ValidBuildTarget tgt = (ValidBuildTarget)EditorGUILayout.EnumPopup(m_TargetContent, m_UserData.m_BuildTarget);
                if (tgt != m_UserData.m_BuildTarget)
                {
                    m_UserData.m_BuildTarget = tgt;
                    if (m_UserData.m_UseDefaultPath)
                    {
                        m_UserData.m_OutputPath = "AssetBundles/";
                        m_UserData.m_OutputPath += m_UserData.m_BuildTarget.ToString();
                    }
                }
            }

            ////output path
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory))
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                var newPath = EditorGUILayout.TextField("Output Path", m_UserData.m_OutputPath);
                if (!string.IsNullOrEmpty(newPath) && newPath != m_UserData.m_OutputPath)
                {
                    m_UserData.m_UseDefaultPath = false;
                    m_UserData.m_OutputPath = newPath;
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                var strVersion = EditorGUILayout.TextField("Version Text", m_UserData.m_VersionText);
                if (!string.IsNullOrEmpty(strVersion) && strVersion != m_UserData.m_VersionText)
                {
                    m_UserData.m_VersionText = strVersion;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                    BrowseForFolder();
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                    ResetPathToDefault();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                newState = GUILayout.Toggle(
                    m_ForceRebuild.state,
                    m_ForceRebuild.content);
                if (newState != m_ForceRebuild.state)
                {
                    if (newState)
                        m_UserData.m_OnToggles.Add(m_ForceRebuild.content.text);
                    else
                        m_UserData.m_OnToggles.Remove(m_ForceRebuild.content.text);
                    m_ForceRebuild.state = newState;
                }
                newState = GUILayout.Toggle(
                    m_CopyToStreaming.state,
                    m_CopyToStreaming.content);
                if (newState != m_CopyToStreaming.state)
                {
                    if (newState)
                        m_UserData.m_OnToggles.Add(m_CopyToStreaming.content.text);
                    else
                        m_UserData.m_OnToggles.Remove(m_CopyToStreaming.content.text);
                    m_CopyToStreaming.state = newState;
                }
            }

            // advanced options
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOptions))
            {
                EditorGUILayout.Space();
                m_AdvancedSettings = EditorGUILayout.Foldout(m_AdvancedSettings, "Advanced Settings");
                if (m_AdvancedSettings)
                {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 1;
                    CompressOptions cmp = (CompressOptions)EditorGUILayout.IntPopup(
                        m_CompressionContent,
                        (int)m_UserData.m_Compression,
                        m_CompressionOptions,
                        m_CompressionValues);

                    if (cmp != m_UserData.m_Compression)
                    {
                        m_UserData.m_Compression = cmp;
                    }
                    foreach (var tog in m_ToggleData)
                    {
                        newState = EditorGUILayout.ToggleLeft(
                            tog.content,
                            tog.state);
                        if (newState != tog.state)
                        {

                            if (newState)
                                m_UserData.m_OnToggles.Add(tog.content.text);
                            else
                                m_UserData.m_OnToggles.Remove(tog.content.text);
                            tog.state = newState;
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel = indent;
                }
            }

            // build.
            EditorGUILayout.Space();
            if (GUILayout.Button("Build"))
            {
                EditorApplication.delayCall += ExecuteBuild;
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void ExecuteBuild()
        {
            if (!Directory.Exists(Application.dataPath + "/LuaTemp/Lua"))
            {
                EditorUtility.DisplayDialog("error", "no set lua assetbundle names", "Yes");
                return;
            }

            if (!CheckVersionFormat(m_UserData.m_VersionText))
            {
                EditorUtility.DisplayDialog("error", "versionText format is error, because a similar format should be used(0.0.0.0)", "Yes");
                return;
            }

            if (AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory)
            {
                if (string.IsNullOrEmpty(m_UserData.m_OutputPath))
                {
                    BrowseForFolder();
                }

                if (string.IsNullOrEmpty(m_UserData.m_OutputPath)) //in case they hit "cancel" on the open browser
                {
                    Debug.LogError("AssetBundle Build: No valid output path for build.");
                    return;
                }

                if (m_ForceRebuild.state)
                {
                    string message = "Do you want to delete all files in the directory " + m_UserData.m_OutputPath;
                    if (m_CopyToStreaming.state)
                    {
                        message += " and " + m_streamingPath + "/" + m_osDir;
                    }
                    message += "?";
                    if (EditorUtility.DisplayDialog("file delete confirmation", message, "Yes", "No"))
                    {
                        try
                        {
                            if (Directory.Exists(m_UserData.m_OutputPath))
                            {
                                Directory.Delete(m_UserData.m_OutputPath, true);
                            }
                            if (m_CopyToStreaming.state && Directory.Exists(m_streamingPath + "/" + m_osDir))
                            {
                                Directory.Delete(m_streamingPath + "/" + m_osDir, true);
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                if (!Directory.Exists(m_UserData.m_OutputPath))
                {
                    Directory.CreateDirectory(m_UserData.m_OutputPath);
                }
            }

            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
            if (AssetBundleModel.Model.DataSource.CanSpecifyBuildOptions)
            {
                if (m_UserData.m_Compression == CompressOptions.Uncompressed)
                {
                    opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
                }
                else if (m_UserData.m_Compression == CompressOptions.ChunkBasedCompression)
                {
                    opt |= BuildAssetBundleOptions.ChunkBasedCompression;
                }
                foreach (var tog in m_ToggleData)
                {
                    if (tog.state)
                    {
                        opt |= tog.option;
                    }
                }
            }

            ABBuildInfo buildInfo = new ABBuildInfo
            {
                outputDirectory = m_UserData.m_OutputPath,
                options = opt,
                buildTarget = (BuildTarget)m_UserData.m_BuildTarget
            };

            buildInfo.onBuild = (assetBundleName) =>
            {
                if (m_InspectTab == null) return;
                m_InspectTab.AddBundleFolder(buildInfo.outputDirectory);
                m_InspectTab.RefreshBundles();
            };

            AssetBundleModel.Model.DataSource.BuildAssetBundles(buildInfo);

            if (m_CopyToStreaming.state)
            {
                string copyPath = m_streamingPath + "/" + m_osDir;
                if (!Directory.Exists(copyPath))
                {
                    Directory.CreateDirectory(copyPath);
                }
                DirectoryCopy(m_UserData.m_OutputPath, copyPath);
            }

            CreateVersionXML(m_osDir);

            if (Directory.Exists(Application.dataPath + "/LuaTemp"))
            {
                Directory.Delete(Application.dataPath + "/LuaTemp", true);
            }

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("success", "build bundle success", "Yes");
        }

        private void CreateVersionXML(string osDir)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement data = xml.CreateElement("data");
            XmlElement game = xml.CreateElement("game");
            game.SetAttribute("GameVersion", m_UserData.m_VersionText);
            data.AppendChild(game);

            List<string> md5List = new List<string>();
            string output = string.Format("{0}/{1}", m_streamingPath, osDir);
            DirectoryInfo dirctory = new DirectoryInfo(output);
            FileInfo[] files = dirctory.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                XmlElement file = xml.CreateElement("file");
                string fileName = files[i].Name;
                string md5 = GetFileContentMD5(output + "/" + files[i].Name);
                if (md5List.Contains(md5))
                {
                    Debug.LogError("????????????????????md5????????AssetBundle????????: " + fileName);
                    return;
                }
                md5List.Add(md5);
                file.SetAttribute(fileName, md5);
                data.AppendChild(file);
            }
            xml.AppendChild(data);

            //??????utf8????????????????????????
            StreamWriter sw1 = new StreamWriter(output + "/version.xml", false, new UTF8Encoding(false));
            xml.Save(sw1);
            sw1.Close();

            //??????utf8????????????????????????
            StreamWriter sw2 = new StreamWriter(m_UserData.m_OutputPath + "/version.xml", false, new UTF8Encoding(false));
            xml.Save(sw2);
            sw2.Close();
        }

        /// <summary>
        /// ???????????????????????????????(0.0.0.0)
        /// </summary>
        /// <param name="strVersion"></param>
        /// <returns></returns>
        private bool CheckVersionFormat(string strVersion)
        {
            if (strVersion.Length != 7)
            {
                Debug.LogError("???????????????????????????????????????????????????????????");
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
                Debug.LogError("???????????????????????????????????????????????????????????");
                return false;
            }
            return true;
        }

        private string GetFileContentMD5(string file)
        {
            if (!File.Exists(file))
            {
                return string.Empty;
            }

            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = StringBuilderCache.Acquire();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceDirName, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(folderPath.Replace(sourceDirName, destDirName)))
                    Directory.CreateDirectory(folderPath.Replace(sourceDirName, destDirName));
            }

            foreach (string filePath in Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories))
            {
                var fileDirName = Path.GetDirectoryName(filePath).Replace("\\", "/");
                var fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(fileDirName.Replace(sourceDirName, destDirName), fileName);

                File.Copy(filePath, newFilePath, true);
            }
        }

        private void BrowseForFolder()
        {
            m_UserData.m_UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", m_UserData.m_OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                m_UserData.m_OutputPath = newPath;
            }
        }
        private void ResetPathToDefault()
        {
            m_UserData.m_UseDefaultPath = true;
            m_UserData.m_OutputPath = "AssetBundles/";
            m_UserData.m_OutputPath += m_UserData.m_BuildTarget.ToString();
        }

        //Note: this is the provided BuildTarget enum with some entries removed as they are invalid in the dropdown
        internal enum ValidBuildTarget
        {
            //NoTarget = -2,        --doesn't make sense
            //iPhone = -1,          --deprecated
            //BB10 = -1,            --deprecated
            //MetroPlayer = -1,     --deprecated
            //StandaloneOSXUniversal = 2,
            //StandaloneOSXIntel = 4,
            Windows = 5,
            //WebPlayer = 6,
            //WebPlayerStreamed = 7,
            iOS = 9,
            //PS3 = 10,
            //XBOX360 = 11,
            Android = 13,
            //StandaloneLinux = 17,
            //StandaloneWindows64 = 19,
            //WebGL = 20,
            //WSAPlayer = 21,
            //StandaloneLinux64 = 24,
            //StandaloneLinuxUniversal = 25,
            //WP8Player = 26,
            //StandaloneOSXIntel64 = 27,
            //BlackBerry = 28,
            //Tizen = 29,
            //PSP2 = 30,
            //PS4 = 31,
            //PSM = 32,
            //XboxOne = 33,
            //SamsungTV = 34,
            //N3DS = 35,
            //WiiU = 36,
            //tvOS = 37,
            //Switch = 38
        }

        [System.Serializable]
        internal class BuildTabData
        {
            internal List<string> m_OnToggles;
            internal ValidBuildTarget m_BuildTarget = ValidBuildTarget.Windows;
            internal CompressOptions m_Compression = CompressOptions.StandardCompression;
            internal string m_OutputPath = string.Empty;
            internal string m_VersionText = string.Empty;
            internal bool m_UseDefaultPath = true;
        }
    }
}