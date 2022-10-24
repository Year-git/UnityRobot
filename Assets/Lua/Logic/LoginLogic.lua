local LoginLogic = RequireSingleton("LoginLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local EventMgr = RequireSingleton("EventMgr")
local NetworkMgr = RequireSingleton("NetworkMgr")

-- 本地
--  local serverIp = "127.0.0.1"
--  local serverPort = 8888
--  local serverID = 960127

-- 叶竹年
-- local serverIp = "192.168.45.85"
-- local serverPort = 8888
-- local serverID = 19945

-- 李成林
-- local serverIp = "192.168.253.79"
-- local serverPort = 8888
-- local serverID = 19945

-- 外网
local serverIp = "62.234.97.210"
local serverPort = 8883
local serverID = 9999

function LoginLogic:OnInitialize()
	EventMgr:RegistEvent(GasToGac.L_LoadCharListMsg, self.LoadCharListMsg, self)
	EventMgr:RegistEvent(GasToGac.L_CreateCharResMsg, self.CreateCharResMsg, self)

	CS_CreateOnceTimer(0.01, function() ViewMgr:ShowUI("Login", "Main", "ViewLayer") end)
end

function LoginLogic:LoadCharListMsg(nCode)
	print("nCode:"..nCode)
	if nCode == 1 then
		--请求玩家数据
		NetworkMgr:Send("K_PlayerCreate")
	else
		--创建角色
        NetworkMgr:Send("K_CreateCharReqMsg", self.sRoleId, 1, 1)
	end
end

function LoginLogic:CreateCharResMsg(nCode)
	if nCode == 1 then
		CS_LogError("same name!")
	end
end

function LoginLogic:OnConnectSuccess(inputText)
	self.sRoleId= inputText
	local data = {
		selServerIP = serverIp,
		selServerPort = serverPort,
		serverid = serverID,
		mac = CS_GetMacAddress(),
		openid = inputText,
		pf = CS_GetOSType(),
		channelCode = "editor"
	}
	-- NetworkMgr:Send("K_EnterKSReqMsg", data, false)
end

function LoginLogic:OnConnectFailed()
end

function LoginLogic:OnConnect(inputText)
	-- if serverIp and serverPort then
	-- 	CS_Connect(serverIp, serverPort, function() self:OnConnectSuccess(inputText) end, function() self:OnConnectFailed() end)
	-- end
	self:OnConnectSuccess(inputText)
end