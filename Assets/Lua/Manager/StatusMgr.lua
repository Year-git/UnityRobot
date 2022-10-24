--[[
	--@brief 状态管理类、记录游戏当前状态且唯一
--]]
local StatusMgr = RequireSingleton("StatusMgr")
local EventMgr = RequireSingleton("EventMgr")

function StatusMgr:OnInitialize()
	EventMgr:RegistEvent(GacEvent.StatusOnEnter, self.OnEnter, self)
	EventMgr:RegistEvent(GacEvent.StatusOnExit, self.OnExit, self)
end

--[[
	--@brief 进入状态
--]]
function StatusMgr:OnEnter(strStatus)
	print("Lua:Status:OnEnter:" .. strStatus)
end

--[[
	--@brief 退出状态
--]]
function StatusMgr:OnExit(strStatus)
	print("Lua:Status:OnExit:" .. strStatus)
end