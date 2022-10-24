local Common_TipsUI = RequireClass("Common_TipsUI")
local FairyGUI = FairyGUI
local UnityEngine = UnityEngine
local ViewMgr = RequireSingleton("ViewMgr")


function Common_TipsUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)

    self.Textprompt = pMainView:GetChild("Textprompt")
    self.tipsMove = pMainView:GetTransition("tipsMove")
end

function Common_TipsUI:OnShow( sContent )
    self.Textprompt.text = sContent..""
    self.tipsMove:Play()--
    self.tipsMove:PlayReverse(function() self:Close() end )
    self.tipsMove.timeScale = 0.5
end

function Common_TipsUI:OnHide(  )

end