local Login_MainUI = RequireClass("Login_MainUI")
local LoginLogic = RequireSingleton("LoginLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI=FairyGUI

function Login_MainUI:_constructor(pMainView)
	self._super._constructor(self, pMainView)
	self.btn_Login = pMainView:GetChild("btn_login")
	self.btn_set = pMainView:GetChild("btn_set")
	self.btn_help = pMainView:GetChild("btn_help")
	self.SelectionBg=pMainView:GetChild("bgSelected")
	self.textD=pMainView:GetChild("n31")

	-- 添加登录按钮点击事件
	self.btn_Login.onClick:Add(self.BtnClick_Login,self)
end

function Login_MainUI:OnShow(  )	
    self.btn_set.icon = FairyGUI.UIPackage.GetItemURL("Login","btn-shezhi-denglu")
    self.btn_help.icon = FairyGUI.UIPackage.GetItemURL("Login","btn-wenda-denglu")

end

function Login_MainUI:OnHide()	
	
end

function Login_MainUI:BtnClick_Selection(pBtn)
	if self.bSelected then 
		self.bSelected=false
	else
		self.bSelected=true 
	end
end 

function Login_MainUI:BtnClick_Login(pBtn)
	-- ViewMgr:ShowUI("Login","Account","ViewLayer")
	CS_EnterMap(2)
end