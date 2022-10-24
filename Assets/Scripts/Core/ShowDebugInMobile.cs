using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Logdata
{
    public string output = "";
    public string stack = "";
    public static Logdata Init(string o, string s)
    {
        Logdata log = new Logdata
        {
            output = o,
            stack = s
        };
        return log;
    }

    public void Show()
    {
        GUIStyle style1 = new GUIStyle
        {
            fontSize = 22
        };
        style1.normal.textColor = Color.red;
        GUILayout.Label(output, style1);

        GUIStyle style2 = new GUIStyle
        {
            fontSize = 20
        };
        style2.normal.textColor = Color.black;
        GUILayout.Label(stack, style2);
    }
}

/// <summary>
/// 手机调试脚本
/// 本脚本挂在一个空对象或转换场景时不删除的对象即可
/// 错误和异常输出日记路径 Application.persistentDataPath
/// </summary>
public class ShowDebugInPhone : MonoBehaviour
{
    //log链表
    List<Logdata> logDatas = new List<Logdata>();
    //错误和异常链表
    List<Logdata> errorDatas = new List<Logdata>();
    //警告链表
    List<Logdata> warningDatas = new List<Logdata>();
    static List<string> mWriteTxt = new List<string>();
    Vector2 uiLog;
    Vector2 uiError;
    Vector2 uiWarning;
    bool open = false;
    bool showLog = false;
    bool showError = false;
    bool showWarning = false;
    private string outpath;

    void Start()
    {
        //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
        //Debug.Log(Application.persistentDataPath);
        outpath = Application.persistentDataPath + "/outLog.txt";
        //每次启动客户端删除之前保存的Log
        if (File.Exists(outpath))
        {
            File.Delete(outpath);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HangleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HangleLog;
    }

    void HangleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                logDatas.Add(Logdata.Init(logString, stackTrace));
                break;
            case LogType.Error:
            case LogType.Exception:
                errorDatas.Add(Logdata.Init(logString, stackTrace));
                mWriteTxt.Add(logString);
                mWriteTxt.Add(stackTrace);
                break;
            case LogType.Warning:
                warningDatas.Add(Logdata.Init(logString, stackTrace));
                break;
        }
    }

    void Update()
    {
        //因为写入文件的操作必须在主线程中完成，所以在Update中才给你写入文件。
        if (errorDatas.Count > 0)
        {
            string[] temp = mWriteTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                {
                    writer.WriteLine(t);
                }
                mWriteTxt.Remove(t);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(">>Open", GUILayout.Height(50), GUILayout.Width(150)))
            open = !open;
        if (open)
        {
            if (GUILayout.Button("清理", GUILayout.Height(50), GUILayout.Width(150)))
            {
                logDatas = new List<Logdata>();
                errorDatas = new List<Logdata>();
                warningDatas = new List<Logdata>();
            }
            if (GUILayout.Button("显示log日志:" + showLog, GUILayout.Height(50), GUILayout.Width(150)))
            {
                showLog = !showLog;
                if (open == true)
                    open = !open;
            }
            if (GUILayout.Button("显示error日志:" + showError, GUILayout.Height(50), GUILayout.Width(150)))
            {
                showError = !showError;
                if (open == true)
                    open = !open;
            }
            if (GUILayout.Button("显示warning日志:" + showWarning, GUILayout.Height(50), GUILayout.Width(150)))
            {
                showWarning = !showWarning;
                if (open == true)
                    open = !open;
            }
        }
        GUILayout.EndHorizontal();
        if (showLog)
        {
            GUI.color = Color.white;
            uiLog = GUILayout.BeginScrollView(uiLog);
            foreach (var va in logDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
        if (showError)
        {
            GUI.color = Color.red;
            uiError = GUILayout.BeginScrollView(uiError);
            foreach (var va in errorDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
        if (showWarning)
        {
            GUI.color = Color.yellow;
            uiWarning = GUILayout.BeginScrollView(uiWarning);
            foreach (var va in warningDatas)
            {
                va.Show();
            }
            GUILayout.EndScrollView();
        }
    }
}