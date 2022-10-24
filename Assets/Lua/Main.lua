--  断点调试
local breakSocketHandle, debugXpCall = require("LuaDebugjit")("localhost", 7003)
local timer =
    Timer.New(
    function()
        breakSocketHandle()
    end,
    1,
    -1,
    false
)
timer:Start()

--主入口函数。从这里开始lua逻辑
require("include")

function Main()
	--初始化管理类
	StartSingleton();
end

--场景切换通知
function OnLevelWasLoaded(level)
	--执行一个完整的垃圾收集循环
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
	--销毁管理类
	DestroySingleton();
end