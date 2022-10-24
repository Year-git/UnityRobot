local Shop_MainUI = RequireClass("Shop_MainUI")
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI = FairyGUI

function Shop_MainUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)

    --顶部基本信息
    self.topBox = pMainView:GetChild("topbox")
    self.text_jintiao = self.topBox:GetChild("text_jintiao")
    self.text_jinbi = self.topBox:GetChild("text_jinbi")

    self.ViewTitle = pMainView:GetChild("ViewTitle")
    self.Btn_Out = self.ViewTitle:GetChild("Btn_Out")
    self.ViewTitle = self.ViewTitle:GetChild("ViewTitle")

    self.shopList = pMainView:GetChild("shopList")
    self.ItemPrice = pMainView:GetChild("ItemPrice")
    self.Btn_refresh = pMainView:GetChild("Btn_refresh")
    self.Btn_ItemType1 = pMainView:GetChild("Btn_ItemType1")
    self.Btn_ItemType2 = pMainView:GetChild("Btn_ItemType2")
    self.Btn_ItemType3 = pMainView:GetChild("Btn_ItemType3")

    self.Btn_Out.onClick:Add(self.BtnClick_Out, self)
    self.Btn_refresh.onClick:Add(self.BtnClick_Refresh, self)
    self.Btn_ItemType1.onClick:Add(function() self:BtnClick_ItemType(1) end)                              
    self.Btn_ItemType2.onClick:Add(function() self:BtnClick_ItemType(2) end)
    self.Btn_ItemType3.onClick:Add(function() self:BtnClick_ItemType(3) end)    
end

function Shop_MainUI:OnShow(...)
    self.ViewTitle.text = "商店"
    self:SetShopList(10)
    self.text_jintiao = ""
    self.text_jinbi = ""
end

function Shop_MainUI:OnHide(...)
end

function Shop_MainUI:BtnClick_Out()
    self:Close()
    ViewMgr:ShowUI("Main", "Main", "ViewLayer")
    --ViewMgr:OnShow()
end

function Shop_MainUI:BtnClick_Refresh()

end

-- 选择商店页签 商品类型
function Shop_MainUI:BtnClick_ItemType(nItemTypeId)

end

function Shop_MainUI:SetShopList(nListLength)
    self.shopList.itemRenderer = FairyGUI.ListItemRenderer(self.ShopRender, self)
    self.shopList.numItems = nListLength
end

function Shop_MainUI:ShopRender(nIndex, gItem)
    local itemBtn = gItem.asButton
    local nItemId = 0
    local itemIcon = itemBtn:GetChild("ItemIcon")
    local ItemName = itemBtn:GetChild("ItemName")
    local Btn_purchase = itemBtn:GetChild("Btn_purchase")
    local ItemNum = itemBtn:GetChild("ItemNum")
    local hot = itemBtn:GetChild("hot")
    local Icon_discount = itemBtn:GetChild("Icon_discount")

    itemIcon.icon = FairyGUI.UIPackage.GetItemURL("Common", "ui-jineng-1")
    ItemName.text = "道具名称"
    ItemNum.text = "每日限购:0/1"

    itemIcon.onClick:Add(function() self:BtnClick_details(nItemId) end)
    Btn_purchase.onClick:Add(function() self:BtnClick_purchase(nItemId) end)                               
end

-- 购买商品
function Shop_MainUI:BtnClick_purchase(nItemId)

end

-- 查看商品详情
function Shop_MainUI:BtnClick_details(nItemId)

end
