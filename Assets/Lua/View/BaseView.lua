local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI = FairyGUI

local BaseView = RequireClass("BaseView")
-- 初始化
function BaseView:_constructor(pMainView, sTopName)
    self.pMainView = pMainView
    self.sTopName = sTopName
    self.sBagName = nil
    self.sModuleName = nil
    self.bVisible = false
end

function BaseView:OnShow()
end

function BaseView:OnHide()
end

-----------------------------对外接口-----------------------------
-- 获取名字
function BaseView:GetName()
    return self.pMainView.name
end

function BaseView:Show(...)
end

-- 友好的关闭界面
function BaseView:Close(...)
    if self.OnClose then
        self:OnClose(...)
    else
        self:Hide(...)
    end
end

-- 暴力关闭界面(只有OnClose里可以调用)
function BaseView:Hide(...)
    ViewMgr:HideUI(self:GetName(), ...)
end

-- 获取主窗口
function BaseView:GetMainView()
    return self.pMainView
end

-- 是否显示
function BaseView:IsVisible( bEndView )
    if bEndView then
        return self.pMainView.visible and self.bVisible
    end
    return self.pMainView.visible
end