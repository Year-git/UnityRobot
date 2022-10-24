using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将log写入文件
/// </summary>
class CommWriteLog{
    private static Dictionary<string,CommWriteLog> _recordContainer = new Dictionary<string, CommWriteLog>();
    private static float _writeIntervalTime = 10f;
    private static float _writeNextTime = 0f;

    public static CommWriteLog Open(string sFileName = "Client", bool bAppend = false)
    {
        if (!_recordContainer.ContainsKey(sFileName))
        {
            _recordContainer[sFileName] =  new CommWriteLog(sFileName, bAppend);
        }
        return _recordContainer[sFileName];
    }

    private static void WriteLogToFile()
    {
        foreach(var kvPair in _recordContainer)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(UnityEngine.Application.dataPath + "//" + kvPair.Key + ".log");
            System.IO.StreamWriter sw;

            CommWriteLog pCommWriteLog = kvPair.Value;
            if (!fi.Exists)
            {
                sw = fi.CreateText();
            }
            else
            {
                if (pCommWriteLog._isAppend == false)
                {
                    sw = fi.CreateText();
                    pCommWriteLog._isAppend = true;
                }
                else
                {
                    sw = fi.AppendText();
                }
            }

            for(int i = 0; i < pCommWriteLog._stringContainer.Count; i++)
            {
                sw.WriteLine(pCommWriteLog._stringContainer[i]);
            }
            pCommWriteLog._stringContainer.Clear();
            
            sw.Close();
            sw.Dispose();
            sw = null;
            fi = null;
        }
    }

    public static void WriteLogUpdate()
    {
        float nCurTime = Time.realtimeSinceStartup;
        if (_writeNextTime < nCurTime){
            _writeNextTime = nCurTime + _writeIntervalTime;
            WriteLogToFile();
        }
    }

    public static void Clear()
    {
        WriteLogToFile();
    }

    private List<string> _stringContainer = new List<string>();
    private string _filename;
    private bool _isAppend;

    private CommWriteLog(string sFileName,bool bAppend)
    {
        _filename = sFileName;
        _isAppend = bAppend;
    }

    public void Input(string sInfo = "")
    {
        // 非调试模式，不让写
        if (!Debug.unityLogger.logEnabled)
        {
            return;
        }
        // 每次存入的Log都会重置下次写入的时间，防止频繁写入而导致的卡顿
        _writeNextTime = Time.realtimeSinceStartup + _writeIntervalTime;
        _stringContainer.Add(sInfo);
    }
}