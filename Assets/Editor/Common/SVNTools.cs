using UnityEngine;
using UnityEditor;
using Microsoft.Win32;

/// <summary>
/// SVN指令工具
/// <para>指令参考文档：https://tortoisesvn.net/docs/release/TortoiseSVN_zh_CN/tsvn-automation.html</para>
/// </summary>
public class SVNTools
{
    private static string m_SVNExePath = null;
    private static string SvnExePath
    {
        get
        {
            try
            {
                if (m_SVNExePath == null)
                {
                    RegistryKey subKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TortoiseSVN\", false);
                    if (subKey != null)
                    {
                        object obj = subKey.GetValue("ProcPath");
                        if (obj != null)
                        {
                            m_SVNExePath = obj.ToString();
                        }
                    }
                }

                if (m_SVNExePath == null)
                {
                    Debug.LogError("Can not find svn");
                }

                return m_SVNExePath;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return "Error";
            }
        }
    }

    private static string m_dataPath = null;
    private static string DataPath
    {
        get
        {
            //-6是减去项目路径末尾的Assets
            if (m_dataPath == null)
                m_dataPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            return m_dataPath;
        }
    }

    [MenuItem("Assets/Tortoise SVN/Commit")]
    public static void SVNCommit()
    {
        OperationMultipleFile("commit");
    }

    [MenuItem("Assets/Tortoise SVN/Update")]
    public static void SVNUpdate()
    {
        OperationMultipleFile("update");
    }

    [MenuItem("Assets/Tortoise SVN/Revert")]
    public static void SVNRevert()
    {
        OperationMultipleFile("revert");
    }

    [MenuItem("Assets/Tortoise SVN/Show log")]
    public static void SVNShowLog()
    {
        Object[] selAllObjs = GetAllSelObject();

        if (selAllObjs.Length == 0)
        {
            //从Assets目录提交
            ProcessCommand(SvnExePath, "/command:log /path:" + Application.dataPath);
        }
        else if (selAllObjs.Length == 1)
        {
            ProcessCommand(SvnExePath, "/command:log /path:" + DataPath + AssetDatabase.GetAssetPath(selAllObjs[0]));
        }
        else
        {
            Debug.LogError("Can not show multiple file log. Please select one file.");
        }
    }

    /// <summary>
    /// 对多个文件进行操作
    /// </summary>
    /// <param name="instruct">操作指令</param>
    private static void OperationMultipleFile(string instruct)
    {
        Object[] selAllObjs = GetAllSelObject();

        if (selAllObjs.Length == 0)
        {
            //从Assets目录提交
            ProcessCommand(SvnExePath, "/command:" + instruct + " /path:" + Application.dataPath);
        }
        else
        {
            System.Text.StringBuilder strCache = new System.Text.StringBuilder("/command:" + instruct + " /path:", 200);

            //多项选择
            foreach (var item in selAllObjs)
            {
                strCache.Append(DataPath);
                strCache.Append(AssetDatabase.GetAssetPath(item));
                strCache.Append("*");
            }

            ProcessCommand(SvnExePath, strCache.ToString());
        }
    }

    /// <summary>
    /// 获取当前选择的所有Assets目录下的对象
    /// </summary>
    private static Object[] GetAllSelObject()
    {
        return Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
    }

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="command">执行程序</param>
    /// <param name="argument">参数</param>
    private static void ProcessCommand(string command, string argument)
    {
        //连接执行程序
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command)
        {
            Arguments = argument,
            CreateNoWindow = true,
            ErrorDialog = true,
            UseShellExecute = true
        };

        if (info.UseShellExecute)
        {
            info.RedirectStandardInput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardOutput = false;
        }
        else
        {
            info.RedirectStandardInput = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.StandardErrorEncoding = System.Text.Encoding.UTF8;
            info.StandardOutputEncoding = System.Text.Encoding.UTF8;
        }

        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

        if (!info.UseShellExecute)
        {
            Debug.Log(process.StandardOutput);
            Debug.Log(process.StandardError);
        }

        //Note：等待退出会导致Unity主线程阻塞
        //process.WaitForExit();
        process.Close();
    }
}