
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BehaviacManager
{
    /// <summary>
    /// Behaviac行为树初始化
    /// </summary>
    public static void Init()
    {
        behaviac.Workspace.Instance.FilePath = ExportedFilePath;
        behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

        // socket连接是否打开，只有打开socket连接，连调功能才会支持
        behaviac.Config.IsSocketing = false;

        // 是否是阻塞模式，当是阻塞模式的时候，游戏会阻塞，等待编辑器的连接，
        // 只有成功建立连接后，游戏才继续运行
        behaviac.Config.IsSocketBlocking = false;

        // logging是否打开
        behaviac.Config.IsLogging = false;

        // logging打开的情况下，是否每次logging的时候都Flush
        behaviac.Config.IsLoggingFlush = false;
        
        // 热加载是否打开
        behaviac.Config.IsHotReload = false;

        // 设置使用整数
        behaviac.Workspace.Instance.UseIntValue = true;
    }

    /// <summary>
    /// 设置Behaviac行为树的等待时间接口的时间
    /// </summary>
    /// <param name="nTime"></param>
    public static void SynSinceStartup(int nTime)
    {
        behaviac.Workspace.Instance.IntValueSinceStartup = nTime;
    }

    /// <summary>
    /// 设置Behaviac行为树的等待帧数接口的帧数
    /// </summary>
    /// <param name="nFrame"></param>
    public static void SynFrameSinceStartup(int nFrame)
    {
        behaviac.Workspace.Instance.FrameSinceStartup = nFrame;
    }

    /// <summary>
    /// 行为树开始
    /// </summary>
    public static void Start()
    {
        // 重置Behaviac行为树的等待时间接口的时间
        SynSinceStartup(0);
        // 重置Behaviac行为树的等待帧数接口的帧数
        SynFrameSinceStartup(0);
    }

    private static string ExportedFilePath
    {
        get
        {
            string relativePath = "/Res/Behaviac";

            if (Application.platform == RuntimePlatform.WindowsEditor) {
                return Application.dataPath + relativePath;
            }
            else if (Application.platform == RuntimePlatform.Android) {
                return Application.dataPath + relativePath;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                return Application.dataPath + relativePath;
            }
            // else if (Application.platform == RuntimePlatform.WindowsPlayer) {
            //     return Application.dataPath + relativePath;
            // }
            else {
                return "Assets" + relativePath;
            }
        }
    }
}
