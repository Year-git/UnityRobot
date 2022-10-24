using UnityEngine;

public class Main : MonoBehaviour
{
    public bool enableDebug;
    public RefreshRate refreshRate = RefreshRate.None;
    public LuaEngine _luaEngine;

    public static Main Instance
    {
        get;
        protected set;
    }

    private void Awake()
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        //添加打印信息
        gameObject.AddComponent<ShowDebugInPhone>();
#endif
        Instance = this;
    }

    private void Start()
    {
        //设置游戏帧率
        Application.targetFrameRate = refreshRate != RefreshRate.None ? (int)refreshRate : -1;

        //防止游戏根节点被销毁
        DontDestroyOnLoad(gameObject);

        //标记是否为debug模式
        Debug.unityLogger.logEnabled = enableDebug;

        //启动版本检测流程
        GameVersion.Start();
    }

    /// <summary>
    /// 启动toLua引擎
    /// </summary>
    public void StartLuaEngine()
    {
        gameObject.AddComponent<LuaEngine>();
    }

    /// <summary>
    /// 启动游戏逻辑
    /// </summary>
    public void StartGame()
    {
        GameApplication.Start();
        //切换状态
        StatusManager.Instance.ChangeStatus(typeof(LoginStatus));
    }

    private void Update()
    {
        GameApplication.Update();
    }

    private void LateUpdate()
    {
        GameApplication.LateUpdate();
    }

    private void FixedUpdate()
    {
        GameApplication.FixedUpdate();
    }

    private void OnDestroy()
    {
        GameApplication.OnDestroy();
    }

    private void OnApplicationQuit()
    {
        GameApplication.OnApplicationQuit();
    }
}