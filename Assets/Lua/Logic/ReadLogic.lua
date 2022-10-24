--ReadLogic   -- 选择 游戏 类型 模式都在这个类中缓存数据
local ReadLogic = RequireSingleton("ReadLogic")
local EventMgr = RequireSingleton("EventMgr")  
local ViewMgr = RequireSingleton("ViewMgr")
local NetworkMgr = RequireSingleton("NetworkMgr")

function ReadLogic:OnInitialize()
    self.matchplay = {}
    EventMgr:RegistEvent(GasToGac.L_EnterInstMap, self.L_EnterInstMap, self)
    EventMgr:RegistEvent(GasToGac.L_CreatPrepareWin, self.L_CreatPrepareWin, self) --房间进入准备时间
    EventMgr:RegistEvent(GasToGac.CL_ReqLoadSucceed, self.CL_ReqLoadSucceed, self) -- 玩家全部加载地图成功
end

-- 初始化
function ReadLogic:Init()

end

--请求 加入匹配
function ReadLogic:K_Matching(nPlayRuleID)
    -- NetworkMgr:Send(GacToGas.K_Matching, nPlayRuleID)
end


--请求 退出匹配
function ReadLogic:K_LeaveMatching()
    -- NetworkMgr:Send(GacToGas.K_LeaveMatching)
end

--进入场景
function ReadLogic:L_EnterInstMap(playloadInfo)
    ViewMgr:HideAllView()
end

--发送玩家地图加载值
function ReadLogic:K_ReqLoadScene(nValue)
    --NetworkMgr:Send(GacToGas.K_ReqLoadScene, nValue)
end

--玩家全部加载成功回调
function ReadLogic:CL_ReqLoadSucceed()
    
end


