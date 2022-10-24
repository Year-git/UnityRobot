--在这里注册c#的函数、变量全局化，函数必须增加CS_前缀，变量必须增加CSV_前缀

--Json库
CS_Json = require("cjson")

--公共方法
CS_GetOSType = Common.GetOSType
CS_GetMacAddress = Common.GetMacAddress
CS_GetEquipmentID = Common.GetEquipmentID

-- 获取地图加载进度
CS_GetMapProgressValue = LuaExtend.GetMapProgressValue
CS_GetMapProgressTotal = LuaExtend.GetMapProgressTotal

CS_GetMapId = LuaExtend.GetMapId
CS_CreatNpcGameObj = LuaExtend.CreatNpcGameObj
CS_GetNpcGameObj = LuaExtend.GetNpcGameObj
CS_RemoveNpcGameObj = LuaExtend.RemoveNpcGameObj
CS_GetPlayerPassGameLevel = LuaExtend.GetPlayerPassGameLevel
CS_EnterGameLevel = LuaExtend.EnterGameLevel
CS_EnterMap = LuaExtend.EnterMap
CS_LeaveGameLevel=LuaExtend.LeaveGameLevel
CS_SetGamePause=LuaExtend.SetGamePause
CS_GetPlayerHoleCount=LuaExtend.GetPlayerHoleCount
CS_GetNpcPartCfgIdByHoleIdx=LuaExtend.GetNpcPartCfgIdByHoleIdx



--时间
CS_GTime = Framework.GTime

--计时器
CS_CreateOnceTimer = LuaTimer.CreateOnceTimer
CS_CreateLoopTimer = LuaTimer.CreateLoopTimer
CS_StopTimer = LuaTimer.StopTimer

--打印
CS_Log = LuaInterface.Debugger.Log
CS_LogError = LuaInterface.Debugger.LogError
CS_LogWarning = LuaInterface.Debugger.LogWarning
CS_LogException = LuaInterface.Debugger.LogException