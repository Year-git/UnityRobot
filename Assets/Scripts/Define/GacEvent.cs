/// <summary>
/// 客户端内部使用的消息需要在此注册
/// </summary>
public static class GacEvent
{
    //CSharpToCSharp

    public const string Update = "Update";
    public const string LateUpdate = "LateUpdate";
    public const string FixedUpdate = "FixedUpdate";
    public const string StreamingRecordResult = "StreamingRecordResult";
    public const string NlpEvent = "NlpEvent";
    public const string UpdateFrameLogic = "UpdateFrameLogic";
    public const string UpdateFrameRender = "UpdateFrameRender";
    public const string UpdateMapFrameSyn = "UpdateMapFrameSyn";


    //CSharpToLua

    public const string LuaUpdate = "LuaUpdate";
    public const string LuaLateUpdate = "LuaLateUpdate";
    public const string LuaFixedUpdate = "LuaFixedUpdate";
    public const string MapOnLoad = "MapOnLoad";
    public const string MapOnEnter = "MapOnEnter";
    public const string MapOnExit = "MapOnExit";
    public const string StatusOnEnter = "StatusOnEnter";
    public const string StatusOnExit = "StatusOnExit";
    public const string NpcLuaUpdate = "NpcLuaUpdate";

    /// <summary>
    /// 关卡结束通知
    /// </summary>
    public const string GameLevelEnd = "GameLevelEnd";

    /// <summary>
    /// 玩家组装开始事件
    /// </summary>
    public const string PlayerAssembleStart = "PlayerAssembleStart";
    
    /// <summary>
    /// 地图关卡游戏开始
    /// </summary>
    public const string MapLevelGameStart = "MapLevelGameStart";

    /// <summary>
    /// 游戏开始等待事件
    /// </summary>
    public const string GameStartWait = "GameStartWait";
    public const string UICreate="UICreate";

    /// <summary>
    /// 加载进度开启
    /// </summary>
    public const string LoadProgressOpen = "LoadProgressOpen";
    /// <summary>
    /// 加载进度刷新
    /// </summary>
    public const string LoadProgressUpdate = "LoadProgressUpdate";

    
    //LuaToCSharp
}
