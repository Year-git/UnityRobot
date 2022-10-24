using System;
using UnityEngine;
using TencentMobileGaming;
using Framework;

public class AudioSDKManager : Singleton<AudioSDKManager>
{
    //appId
    private readonly string _appId = "1400181865";
    //权限秘钥
    private readonly string _permissionKey = "lZthy9bXASwy1hoX";
    //用户Id
    private readonly string _openId = "10001";
    //房间号
    private readonly string _roomId = "1";
    //录制标识
    private bool _streamRecordFlag;
    private bool _isRecording;
    private bool _recordFlag;

    //语言
    private readonly string[,] _languages = new string[,] {
            {"한국어 (대한민국)","ko-KR"},
            {"普通话 (中国大陆)","cmn-Hans-CN"},
            {"普通話 (香港)","cmn-Hans-HK"},
            {"廣東話 (香港)","yue-Hant-HK"},
            {"國語 (台灣)","cmn-Hant-TW"},
            {"English (United States)","en-US"},
            {"English (Great Britain)","en-GB"},
            {"Русский (Россия)","ru-RU"},
            {"Italiano (Italia)","it-IT"},
            {"Français (France)","fr-FR"},
            {"Español (España)","es-ES"},
			//{"Arabic (Saudi Arabia)","ar-SA"},
			{"Português (Portugal)","pt-PT"},
			//{"ไทย (ประเทศไทย)","th-TH"},
			{"Filipino (Pilipinas)","fil-PH"},
        };

    /// <summary>
    /// 初始化sdk
    /// </summary>
    public void Start()
    {
        GEvent.RegistEvent(GacEvent.Update, Update);

        int ret = QAVContext.GetInstance().Init(_appId, _openId);
        if (ret != QAVError.OK)
        {
            Debug.LogError("GME Audio SDK Init Fail!");
        }
        else
        {
            ITMGContext.GetInstance().GetPttCtrl().ApplyPTTAuthbuffer(GetAuthBuffer(null));
        }
    }

    /// <summary>
    /// 游戏退出时
    /// </summary>
    public void OnDestroy()
    {
        GEvent.RemoveEvent(GacEvent.Update, Update);
        EnableMic(false);
        ExitRoom();
    }

    /// <summary>
    /// 调用 Poll 可以触发事件回调
    /// </summary>
    public void Update()
    {
        QAVContext.GetInstance().Poll();
    }

    /// <summary>
    /// 获取鉴权信息
    /// </summary>
    /// <returns></returns>
    byte[] GetAuthBuffer(string roomId)
    {
        return QAVAuthBuffer.GenAuthBuffer(int.Parse(_appId), roomId, _openId, _permissionKey);
    }

    /// <summary>
    /// 进入房间
    /// </summary>
    public void EnterRoom()
    {
        ITMGContext.GetInstance().OnEnterRoomCompleteEvent += new QAVEnterRoomComplete(OnEnterRoomComplete);
        ITMGContext.GetInstance().EnterRoom(_roomId, ITMGRoomType.ITMG_ROOM_TYPE_FLUENCY, GetAuthBuffer(_roomId));
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public void ExitRoom()
    {
        ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= new QAVEnterRoomComplete(OnEnterRoomComplete);
        ITMGContext.GetInstance().ExitRoom();
    }

    /// <summary>
    /// 进入房间回调
    /// </summary>
    /// <param name="err"></param>
    /// <param name="errInfo"></param>
    void OnEnterRoomComplete(int err, string errInfo)
    {
        if (err != 0)
        {
            Debug.LogError(string.Format("join room failed, err:{0}, errInfo:{1}", err, errInfo));
        }
        else
        {
            Debug.Log(string.Format("当前语音房间id:{0},请在别的终端加入这个房间进行通话", _roomId));
        }
    }

    /// <summary>
    /// 打开、关闭麦克风
    /// </summary>
    /// <param name="bOpen"></param>
    public void EnableMic(bool bOpen)
    {
        ITMGContext.GetInstance().GetAudioCtrl().EnableMic(bOpen);
    }

    /// <summary>
    /// 按下事件
    /// </summary>
    public void OnDownBtn()
    {
        OnStartStreamingRecord();
    }

    /// <summary>
    /// 松开事件
    /// </summary>
    public void OnUpBtn()
    {
        OnExitStreamingRecord();
    }

    /// <summary>
    /// 离开事件
    /// </summary>
    /// <param name="pointData"></param>
    public void OnLeaveBtn()
    {
        OnLeaveStreamingRecord();
    }

    /// <summary>
    /// 开始流式录制
    /// </summary>
    void OnStartStreamingRecord()
    {
        _streamRecordFlag = true;
        Debug.Log("开始流式录制");

        _isRecording = true;
        Debug.Log("录制中,放开按钮以停止录制...");
        AudioStreamingRecordDelegateImpl audioRecordDelegate = new AudioStreamingRecordDelegateImpl(delegate (int code, string fileid, string filePath, string result)
        {
            if (this == null)
            {
                return;
            }
            _isRecording = false;
            if (code == 0)
            {
                Debug.Log("录制完成");
                GEvent.DispatchEvent(GacEvent.StreamingRecordResult, result);
            }
            else if (code == -1)
            {
                Debug.LogError("录制失败:流式正在录制中");
            }
            else
            {
                if (ITMGContext.GetInstance().IsRoomEntered())
                {
                    Debug.LogError("当前正在通话中, 无法录制语音");
                }
                else if (code == 4103)
                {
                    Debug.LogError("录制时长太短");
                }
                else if (code == 32775)
                {
                    Debug.LogError(" 上传和翻译失败但是录音成功");
                }
                else if (code == 32777)
                {
                    Debug.LogError(" 翻译失败但是录音和上传成功");
                }
                else
                {
                    Debug.LogError("录制失败:" + Convert.ToString(code));
                }
            }
        });
        audioRecordDelegate.Start(_languages[1, 1]);
    }

    /// <summary>
    /// 结束流式录制
    /// </summary>
    void OnExitStreamingRecord()
    {
        _streamRecordFlag = false;
        Debug.Log("结束录制");
        ITMGContext.GetInstance().GetPttCtrl().StopRecording();
    }

    /// <summary>
    /// 取消流式录制
    /// </summary>
    void OnLeaveStreamingRecord()
    {
        if (!_streamRecordFlag)
        {
            return;
        }
        _isRecording = false;
        Debug.Log("取消录制");
        ITMGContext.GetInstance().GetPttCtrl().CancelRecording();
        Debug.Log("已取消录制");
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    void OnStartRecord()
    {
        _recordFlag = true;
        if (_isRecording)
        {
            return;
        }
        Debug.Log("开始录制");
        _isRecording = true;
        Debug.Log("录制中,放开按钮以停止录制...");
        AudioRecordDelegateImpl audioRecordDelegate = new AudioRecordDelegateImpl(delegate (int code, string filePath)
        {
            if (this == null)
            {
                return;
            }
            _isRecording = false;
            if (code == 0)
            {
                Debug.Log("录制完成");
            }
            else
            {
                if (ITMGContext.GetInstance().IsRoomEntered())
                {
                    Debug.LogError("当前正在通话中, 无法录制语音");
                }
                else if (code == 4103)
                {
                    Debug.LogError("录制时长太短");
                }
                else
                {
                    Debug.LogError("录制失败:" + Convert.ToString(code));
                }
            }
        });
        audioRecordDelegate.Start();
    }

    /// <summary>
    /// 结束录制
    /// </summary>
    void OnExitRecord()
    {
        _recordFlag = false;
        Debug.Log("结束录制");
        ITMGContext.GetInstance().GetPttCtrl().StopRecording();
    }

    /// <summary>
    /// 取消录制
    /// </summary>
    void OnLeaveRecord()
    {
        if (!_recordFlag)
        {
            return;
        }
        Debug.Log("取消录制");
        ITMGContext.GetInstance().GetPttCtrl().CancelRecording();
        _isRecording = false;
        Debug.Log("已取消录制");
    }

    /// <summary>
    /// 语音转文本
    /// </summary>
    void ConvertText(string fileToConvert)
    {
        if (fileToConvert == null || fileToConvert.Equals(""))
        {
            Debug.LogError("请先下载音频再转文本");
            return;
        }
        ConvertTextDelegateImpl convertTextDelegate = new ConvertTextDelegateImpl(delegate (int code, string fileID, string result)
        {
            if (code == 0)
            {
                Debug.Log("转换成功" + result);
            }
            else
            {
                Debug.LogError("转换失败" + Convert.ToString(code));
            }
        });
        convertTextDelegate.Start(fileToConvert, _languages[1, 1]);
    }
}

class AudioStreamingRecordDelegateImpl
{
    static int _uid = 10000;
    readonly QAVStreamingRecognitionCallback _handler = null;
    readonly QAVStreamingRecognitionCallback _innerHandler = null;
    public AudioStreamingRecordDelegateImpl(QAVStreamingRecognitionCallback handler)
    {
        _handler = handler;
        _innerHandler = new QAVStreamingRecognitionCallback(delegate (int code, string fileid, string filepath, string result)
        {
            InnerHandlerImpl(code, fileid, filepath, result);
        });
        ITMGContext.GetInstance().GetPttCtrl().OnStreamingSpeechComplete += _innerHandler;
    }

    public void Start(string language)
    {
        string recordPath = Application.persistentDataPath + string.Format("/{0}.silk", _uid++);
        int ret = ITMGContext.GetInstance().GetPttCtrl().StartRecordingWithStreamingRecognition(recordPath, language);
        if (ret != 0)
        {
            InnerHandlerImpl(-1, "", "", "");
        }
    }

    private void InnerHandlerImpl(int code, string fileid, string filepath, string result)
    {
        _handler?.Invoke(code, fileid, filepath, result);
        ITMGContext.GetInstance().GetPttCtrl().OnStreamingSpeechComplete -= _innerHandler;
    }
}

class AudioRecordDelegateImpl
{
    private static int _uid = 0;
    private readonly QAVRecordFileCompleteCallback _handler = null;
    private readonly QAVRecordFileCompleteCallback _innerHandler = null;
    public AudioRecordDelegateImpl(QAVRecordFileCompleteCallback handler)
    {
        _handler = handler;
        _innerHandler = new QAVRecordFileCompleteCallback(delegate (int code, string filePath)
        {
            InnerHandlerImpl(code, filePath);
        });
        ITMGContext.GetInstance().GetPttCtrl().OnRecordFileComplete += _innerHandler;
    }

    public void Start()
    {
        string recordPath = Application.persistentDataPath + string.Format("/{0}.silk", _uid++);
        int ret = ITMGContext.GetInstance().GetPttCtrl().StartRecording(recordPath);
        if (ret != 0)
        {
            InnerHandlerImpl(-1, "");
        }
    }

    private void InnerHandlerImpl(int code, string filePath)
    {
        _handler?.Invoke(code, filePath);
        ITMGContext.GetInstance().GetPttCtrl().OnRecordFileComplete -= _innerHandler;
    }
}

class ConvertTextDelegateImpl
{
    readonly QAVSpeechToTextCallback _handler = null;
    readonly QAVSpeechToTextCallback _innerHandler = null;
    public ConvertTextDelegateImpl(QAVSpeechToTextCallback handler)
    {
        _handler = handler;
        _innerHandler = new QAVSpeechToTextCallback(delegate (int code, string fileid, string result)
        {
            InnerHandlerImpl(code, fileid, result);
        });
        ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextComplete += _innerHandler;
    }

    public void Start(string fileId, string language)
    {
        ITMGContext.GetInstance().GetPttCtrl().SpeechToText(fileId, language);
    }

    private void InnerHandlerImpl(int code, string fileid, string result)
    {
        _handler?.Invoke(code, fileid, result);
        ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextComplete -= _innerHandler;
    }
}