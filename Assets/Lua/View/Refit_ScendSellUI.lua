local Refit_ScendSellUI = RequireClass("Refit_ScendSellUI")
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI= FairyGUI

function Refit_ScendSellUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.ViewName = pMainView:GetChild("ViewName")
    self.viewcenter = pMainView:GetChild("viewcenter")
    self.btn_sure = pMainView:GetChild("btn_sure")
    self.btn_cancel = pMainView:GetChild("btn_cancel")
    self.sell_list = pMainView:GetChild("sell_list")

    self.btn_cancel.onClick:Add(self.BtnClick_cancel,self)
    self.btn_sure.onClick:Add(self.BtnClick_cancel,self)
end

function Refit_ScendSellUI:OnShow(sTipsName,sTipsCenter)
    self.btn_cancel .icon = FairyGUI.UIPackage.GetItemURL("Common","btn-zhong-haung")
    self.btn_sure .icon = FairyGUI.UIPackage.GetItemURL("Common","btn-zhong-lan")
    self.btn_cancel .title = "取消"
    self.btn_sure .title = "确定"
    self.ViewName.text = sTipsName
    self.viewcenter.text = sTipsCenter
    
end

function Refit_ScendSellUI:OnHide()
    
end


function Refit_ScendSellUI:BtnClick_cancel()
    self:Close()
end

function  Refit_ScendSellUI:SellPartsList( nLength )
    self.sell_list.itemRenderer = FairyGUI.ListItemRenderer(self.SellPartsRender, self)
    self.list_Spareparts.numItems = nLength
end

function Refit_ScendSellUI:SellPartsRender( nIndex , item )
    
end