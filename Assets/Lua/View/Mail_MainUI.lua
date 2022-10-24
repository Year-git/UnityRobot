local Mail_MainUI = RequireClass("Mail_MainUI")
local MainLogic = RequireSingleton("MainLogic")
local ReadLogic = RequireSingleton("ReadLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI = FairyGUI

function Mail_MainUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.btn_system = pMainView:GetChild("btn_system")
    self.btn_friend = pMainView:GetChild("btn_friend")
    self.btn_canel = pMainView:GetChild("btn_canel")
    self.btn_Maildel = pMainView:GetChild("btn_Maildel")
    self.btn_set = pMainView:GetChild("btn_set")
    self.btn_Mailrecrive = pMainView:GetChild("btn_Mailrecrive")
    self.btn_goodsReceive = pMainView:GetChild("btn_Receive")
    self.text_MailContent = pMainView:GetChild("mail_Content")
    self.text_viewname = pMainView:GetChild("ViewName")
    self.List_mail = pMainView:GetChild("List_mail")
    self.list_Receive = pMainView:GetChild("list_Receive")
    
    self._Bar = pMainView:GetChild("Bar")
    self._add=self._Bar:GetChild("Add")
    self._BarValue = 0

    self.btn_set.onClick:Add(self.BtnClick_MailSet, self)
    self.btn_system.onClick:Add(self.BtnClick_MailSystem, self)
    self.btn_friend.onClick:Add(self.BtnClick_MailFriend, self)
    self.btn_canel.onClick:Add(self.BtnClick_MailCanel, self)
    self.btn_Mailrecrive.onClick:Add(self.BtnClick_MailRecrive, self)
    self.btn_goodsReceive.onClick:Add(self.BtnClick_GoodsRecrive, self)
    self.btn_Maildel.onClick:Add(self.BtnClick_MailDel, self)
    self:OnShow()
end

function Mail_MainUI:ReceiveRender(nIndex,item)
    local item_Btn=item.asButton
    item_Btn.icon = FairyGUI.UIPackage.GetItemURL("Login", "btn-wenhao")
    item_Btn.title = tostring(nIndex)   
end

function Mail_MainUI:MailRender(nIndex, item)
    local item_Btn=item.asButton
    item_Btn.icon=FairyGUI.UIPackage.GetItemURL("Login", "btn-shezhi")
    item_Btn.title=tostring(nIndex)
    local _mailTimer = item_Btn:GetChild("mailTimer")
    local _mailValidity = item_Btn:GetChild("mailValidity")
    _mailValidity.text = tostring(nIndex)
    item_Btn.onClick:Add(function (  )
        self:ShowMailContent(nIndex)
    end)
end

function Mail_MainUI:ShowMailContent( nIndex )
    self.text_MailContent.text = "nIndex"..tostring(nIndex)
end

function Mail_MainUI:BtnClick_MailDel()
    self.List_mail.itemRenderer =  FairyGUI.ListItemRenderer(self.MailRender, self)
    self.List_mail.numItems = 0
end

function Mail_MainUI:BtnClick_MailSet()
    ViewMgr:ShowUI("Main","Main","ViewLayer")
end

function Mail_MainUI:BtnClick_MailSystem()
    self._BarValue= self._BarValue - 10
    self._Bar.value = self._BarValue
    self._add.visible=false
end

function Mail_MainUI:BtnClick_MailFriend()
    self._BarValue= self._BarValue +10
    self._add.visible =true
    self._add.width = self._BarValue
    --self._Bar.value = self._BarValue
    self._Bar:TweenValue(self._BarValue,1)
end

function Mail_MainUI:BtnClick_MailCanel()
    ViewMgr:ShowUI("Main", "Main", "ViewLayer") --Mail_MainUI
    self:Close()
end

function Mail_MainUI:BtnClick_MailRecrive()
end

function Mail_MainUI:BtnClick_GoodsRecrive()
end

function Mail_MainUI:OnShow()    
    self.list_Receive.itemRenderer =  FairyGUI.ListItemRenderer(self.ReceiveRender, self)
    self.List_mail.itemRenderer =  FairyGUI.ListItemRenderer(self.MailRender, self)
    self.list_Receive.numItems=10
    self.List_mail.numItems = 10
    self.list_Receive:SetVirtual()
    self.List_mail:SetVirtual()
    self.text_viewname.text = "邮件"
end

function Mail_MainUI:OnHide()

    
end
