--客户端内部使用的消息需要在此注册
GacEvent = 
{
	-----------------------------------------------LuaToLua-----------------------------------------------

	AssembleEnd="AssembleEnd",
	SelectSkillOver="SelectSkillOver",	
	Playskill="Playskill",
	-----------------------------------------------LuaToCSharp-----------------------------------------------
	-----------------------------------------------CSharpToLua-----------------------------------------------
	MapOnLoad = "MapOnLoad",
	MapOnEnter = "MapOnEnter",
	MapOnExit = "MapOnExit",

	StatusOnEnter = "StatusOnEnter",
	StatusOnExit = "StatusOnExit",
	LuaUpdate = "LuaUpdate",

	NpcLuaUpdate = "NpcLuaUpdate",

	PlayerAssembleStart = "PlayerAssembleStart", -- 玩家开始组装事件0
	GameStartWait = "GameStartWait", -- 游戏开始等待事件
	UICreate="UICreate",
	GameLevelEnd = "GameLevelEnd", -- 关卡结束事件

	MapLevelGameStart = "MapLevelGameStart", -- 地图关卡游戏开始


    LoadProgressOpen = "LoadProgressOpen", -- 加载进度开启
    LoadProgressUpdate = "LoadProgressUpdate", -- 加载进度刷新
}