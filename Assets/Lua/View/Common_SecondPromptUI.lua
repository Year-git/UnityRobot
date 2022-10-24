local Common_SecondPromptUI = RequireClass("Common_SecondPromptUI")
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI = FairyGUI

function Common_SecondPromptUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    
	self.btn_sure = pMainView:GetChild("btn_sure")
    self.btn_cancel = pMainView:GetChild("btn_cancel")        
	self.text_tipName = pMainView:GetChild("tipName")
	self.text_viewContent = pMainView:GetChild("viewContent")
	self.leftIcon = pMainView:GetChild("leftIcon")
	self.rightIcon = pMainView:GetChild("rightIcon")
	-- 添加登录按钮点击事件
	
    self.btn_cancel.onClick:Add(self.BtnClick_cancel,self)
    --self.btn_sure.onClick:Add(function() self:BtnClick() end)    
end

function Common_SecondPromptUI:BtnClick_sure()     
    self:Close()
end

function Common_SecondPromptUI:BtnClick_cancel()  
    self:Close()
end

function Common_SecondPromptUI:OnHide()    
end

function Common_SecondPromptUI:OnShow(sTipsName,sTipsContent,leftIconPath,rightIconPath,func)
    self.text_tipName.text =sTipsName
    self.text_viewContent.text =sTipsContent
    self.btn_sure.title = "确定"
    self.btn_cancel.title = "取消"
    self.btn_sure.icon =  FairyGUI.UIPackage.GetItemURL("Common", "btn-zhong-haung")
    self.btn_cancel.icon =  FairyGUI.UIPackage.GetItemURL("Common", "btn-zhong-lan")
    self.leftIcon.icon =  FairyGUI.UIPackage.GetItemURL("Common", leftIconPath)
    self.rightIcon.icon =  FairyGUI.UIPackage.GetItemURL("Common", rightIconPath)
    self.btn_sure.onClick:Add(func,self)
end