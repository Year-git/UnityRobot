local Common_LoadViewUI = RequireClass("Common_LoadViewUI")
local EventMgr = RequireSingleton("EventMgr")

function Common_LoadViewUI:_constructor( pMainView )
    self._super._constructor(self, pMainView)
    self._loadbar = pMainView:GetChild("loadbar")

    self.loadValue = pMainView:GetChild("loadValue")

    EventMgr:RegistEvent(GacEvent.LoadProgressOpen, self.LoadProgressOpen, self)
    EventMgr:RegistEvent(GacEvent.LoadProgressUpdate, self.LoadProgressUpdate, self)
end

function Common_LoadViewUI:OnShow()

end

function Common_LoadViewUI:OnHide()

end

function Common_LoadViewUI:LoadProgressOpen()
    self.loadValue.text = "0%"
end

function Common_LoadViewUI:LoadProgressUpdate()
    self.loadValue.text = math.floor( CS_GetMapProgressValue() / CS_GetMapProgressTotal() * 100 ).."%"
end