--[[
	--@brief 场景管理类
--]]
local MapMgr = RequireSingleton("MapMgr")
local EventMgr = RequireSingleton("EventMgr")
local MainLogic = RequireSingleton("MainLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local ReadLogic = RequireSingleton("ReadLogic") 

function MapMgr:OnInitialize()
	EventMgr:RegistEvent(GacEvent.MapOnLoad, self.OnLoad, self)
	EventMgr:RegistEvent(GacEvent.MapOnEnter, self.OnEnter, self)
	EventMgr:RegistEvent(GacEvent.MapOnExit, self.OnExit, self)
end

--[[
	--@brief 加载地图
--]]
function MapMgr:OnLoad(mapID)
    ViewMgr:HideAllView()    
    ViewMgr:ShowUI("Common","LoadView","TopLayer")
    ViewMgr:DestroyWindowExceptLayer("TopLayer")
    --ViewMgr:ShowUI("LoadingView")  _UI
end

--[[
	--@brief 进入地图
--]]
function MapMgr:OnEnter(mapID)
    CS_Log("MapMgr:OnEnter :" .. mapID)
    -- 当前如果是主界面 显示主界面UI 暂时这么写
    if mapID == 2 then
        ViewMgr:ShowUI("Main", "Main", "BackgroundLayer")
    elseif mapID >= 10000 then
        self:ShowBattleView()
    end
end

-- 显示战斗界面
function MapMgr:ShowBattleView()
    ViewMgr:ShowUI("Battle", "Main", "ViewLayer")
end

--[[
	--@brief 退出地图
--]]
function MapMgr:OnExit(mapID)
	
end