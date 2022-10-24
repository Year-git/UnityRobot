local Login_AccountUI = RequireClass("Login_AccountUI")
local FairyGUI = FairyGUI
local ViewMgr = RequireSingleton("ViewMgr")
local LoginLogic = RequireSingleton("LoginLogic")

function Login_AccountUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)	  
	self.btn_sure = pMainView:GetChild("btn_sure")
	self.btn_cancel = pMainView:GetChild("btn_cancel")
	self.btn_Close = pMainView:GetChild("btn_Close")
	self.text_Account = pMainView:GetChild("text_Account")  --账号
	self.text_PassWord = pMainView:GetChild("text_PassWord")  --密码  
	-- 添加登录按钮点击事件
	self.btn_sure.onClick:Add(self.BtnClick_sure,self)
    self.btn_cancel.onClick:Add(self.BtnClick_cancel,self)
    self.btn_Close.onClick:Add(function() self:Close() end)
    
end

function Login_AccountUI:BtnClick_cancel(pBtn)   
	self:Close()
end

function Login_AccountUI:BtnClick_sure(pBtn)
    local inputText = self.text_Account.text
	if inputText == "" then
		CS_LogError("请输入账号！")
	else
		--self.pLoginbtn.enabled = false
		LoginLogic:OnConnect(inputText)
	end
	self:Close()
end

function Login_AccountUI:OnHide()    
end

function Login_AccountUI:OnShow()
end
