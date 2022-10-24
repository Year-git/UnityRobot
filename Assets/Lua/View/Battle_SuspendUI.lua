local Battle_SuspendUI = RequireClass("Battle_SuspendUI")
local NetworkMgr = RequireSingleton("NetworkMgr")


function Battle_SuspendUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.lefttopBox = pMainView:GetChild("btn_Close")
    self.btn_cancel = pMainView:GetChild("btn_cancel")
    self.btn_sure = pMainView:GetChild("btn_sure")
    self.panelCentext = pMainView:GetChild("panelCentext")
    self.btn_Close = pMainView:GetChild("btn_Close")

    self.btn_sure.onClick:Add(function(  ) self:BtnClick_leaveScene() end)
    
    self.btn_cancel.onClick:Add(function(  ) self:BtnClick_GamePause(false)  end)
    self.btn_Close.onClick:Add(function(  ) self:BtnClick_GamePause(false)  end)
end

function Battle_SuspendUI:OnShow( ... )
    --self.panelCentext.text = ""  
end

function Battle_SuspendUI:OnHide( ... )
    
end

function Battle_SuspendUI:BtnClick_GamePause( bState )
    self:Close()
    CS_SetGamePause(bState)
    
end

function Battle_SuspendUI:BtnClick_leaveScene(  )
    CS_LeaveGameLevel()
end