local ViewMgr = RequireSingleton("ViewMgr")
local EventMgr = RequireSingleton("EventMgr")
local FairyGUI = FairyGUI

-- 默认已经存在的包
local UIConfigAddPackages = {
    ["Common"] = true,
}

function ViewMgr:OnInitialize()
    self.tTopViewList = {}
    self.tModuleViewList = {}
    self.tBagList = {}
    -- update 刷新列表
    self.tUpdateModuleList = {}
    
    -- 导入公共资源包
    --self:AddBag("BlueSkin") -- 测试用
    EventMgr:RegistEvent(GacEvent.LuaUpdate, self.Update, self)
    
    -- 初始化TOP层级
    -- 场景UI,如血条，点击建筑物查看信息 一般置于场景之上，UI界面之下
    self:CreateTopUI("ScreenLayer",0,2000)
    -- 背景UI,如主界面，一般用户不能主动关闭，永远处于其他UI的最底层
    self:CreateTopUI("BackgroundLayer",1000,1000)
    -- 普通UI,一级，二级，三级窗口，一般由玩家点击打开的多级窗口
    self:CreateTopUI("ViewLayer",2000,0)
    -- 信息UI，如广播，跑马灯，一般永远置于用户打开窗户顶层
    self:CreateTopUI("InfoLayer",3000,-1000)
    -- 提示UI，如错误弹窗，网络连接弹窗等
    self:CreateTopUI("TipLayer",4000,-2000)
    -- 顶层UI，场景加载 Loading图
    self:CreateTopUI("TopLayer",5000,-3000)
end

-- C# updte
function ViewMgr:Update(deltaTime)
    for sUIName, tModleClass in pairs(self.tUpdateModuleList) do 
        tModleClass:OnUpdate(deltaTime)
    end
end

-- 添加一个资源包
--isr
function ViewMgr:AddBag(sBagName)
    if UIConfigAddPackages[sBagName] then
        return
    end
    if self.tBagList[sBagName] then
        CS_LogWarning(sBagName .. "已经存在，导入资源包失败！")
        return
    end
    self.tBagList[sBagName] = FairyGUI.UIPackage.AddPackage("Assets/Res/FGUI/" .. sBagName)
end

-- 判断是否导入资源包
function ViewMgr:IsAddBag(sBagName)
    return self.tBagList[sBagName] and true or false
end

-- 创建一个top层级
function ViewMgr:CreateTopUI(sTopName,orderInLayer,planeDistance)
    if self.tTopViewList[sTopName] then
        CS_LogWarning(sTopName .. "已经存在，创建TopUI失败！")
        return
    end
    local pTopView = FairyGUI.Window.New()
    pTopView.name = sTopName
    pTopView.rootContainer.name = sTopName
    pTopView.rootContainer.gameObject.name = sTopName
    pTopView.sortingOrder=orderInLayer
    pTopView.z=planeDistance
    FairyGUI.GRoot.inst:AddChild(pTopView)
    pTopView:MakeFullScreen()
    self.tTopViewList[sTopName] = pTopView
    return pTopView
end

-- 显示UI  包名  组件名  层级名字
function ViewMgr:ShowUI(sBagName, sModuleName, sTopName, ...)
    local sUIName = sBagName .. "_" .. sModuleName .. "UI"

    local pTopView = self.tTopViewList[sTopName]
    if not pTopView then
        CS_LogWarning(sTopName .. "没有找到topview 显示ui失败！")
        return
    end
    if not self:IsAddBag(sBagName) then
        self:AddBag(sBagName)
    end
    local tBagViewList = self.tModuleViewList[sBagName]
    if not tBagViewList then
        tBagViewList = {}
        self.tModuleViewList[sBagName] = tBagViewList
    end
    local tModleClass = tBagViewList[sModuleName]
    if not tModleClass then
        local pMainView = FairyGUI.UIPackage.CreateObject(sBagName, sModuleName).asCom
        pMainView.name = sUIName
        pMainView.rootContainer.name = sUIName
        pMainView.rootContainer.gameObject.name = sUIName
        --开启深度调整
        pMainView.fairyBatching=true
        tModleClass = NewClass(sUIName, pMainView)
        tModleClass.sBagName = sBagName
        tModleClass.sModuleName = sModuleName
        tBagViewList[sModuleName] = tModleClass
    end
    local pMainView = tModleClass:GetMainView()
    pTopView:AddChildAt(pMainView,pTopView.numChildren)
    pMainView:MakeFullScreen()
    pMainView.visible = true
    if tModleClass.OnUpdate then
        self.tUpdateModuleList[sUIName] = tModleClass
    end
    if tModleClass.OnShow then
        tModleClass:OnShow(...)
    end
    tModleClass.bVisible = true
end

-- 关闭所有界面
function ViewMgr:HideAllView()
    for sBagName, tBagViewList in pairs(self.tModuleViewList) do 
        for sModuleName, tModleClass in pairs(tBagViewList) do 
            self:HideUI(tModleClass:GetName())
        end
    end
end

-- 隐藏UI
function ViewMgr:HideUI(sUIName,...)
    local pos=string.find(sUIName,"_")
    local pos1=string.find(sUIName,"U")
    if (not pos) or (not pos1) then
        CS_LogWarning("请检查传入参数的格式(参考:Login_MainUI)")
        return
    end
    local sBagName=string.sub(sUIName,1,pos-1)       
    local sModuleName=string.sub(sUIName,pos+1,pos1-1)

    local tBagViewList = self.tModuleViewList[sBagName]
    if not tBagViewList then
        return
    end
    local tModleClass = tBagViewList[sModuleName]
    if not tModleClass then
        return
    end
    local pMainView = tModleClass:GetMainView()
    pMainView.visible = false
    if self.tUpdateModuleList[sUIName] then
        self.tUpdateModuleList[sUIName] = nil
    end
    tModleClass:OnHide(...)
    tModleClass.bVisible = false
end

function ViewMgr:IsVisible(sUIName)
    local pos=string.find(sUIName,"_")
    local pos1=string.find(sUIName,"U")
    if (not pos) or (not pos1) then
        CS_LogWarning("请检查传入参数的格式(参考:Login_MainUI)")
        return
    end
    local sBagName=string.sub(sUIName,1,pos-1)       
    local sModuleName=string.sub(sUIName,pos+1,pos1-1)

    local tBagViewList = self.tModuleViewList[sBagName]
    if not tBagViewList then
        return false
    end
    local tModleClass = tBagViewList[sModuleName]
    if not tModleClass then
        return false
    end
    return tModleClass:IsVisible()
end

--删除UI
function ViewMgr:DisposeUI(sUIName)
    local pos=string.find(sUIName,"_")
    local pos1=string.find(sUIName,"U")
    if (not pos) or (not pos1) then
        CS_LogWarning("请检查传入参数的格式(参考:Login_MainUI)")
        return
    end
    local sBagName=string.sub(sUIName,1,pos-1)       
    local sModuleName=string.sub(sUIName,pos+1,pos1-1)

    local tBagViewList = self.tModuleViewList[sBagName]
    if not tBagViewList then
        return
    end
    local tModleClass = tBagViewList[sModuleName]
    if not tModleClass then
        return
    end
    if tModleClass.OnDispose then
        tModleClass:OnDispose()
    end
    tModleClass.pMainView:Dispose()
    tBagViewList[sModuleName] = nil
end

--卸载包
--[[
--1.包卸载后，所有包里包含的贴图等资源均会被卸载，由包里创建出来的组件也无法显示正常（虽然不会报错），所以这些组件应该（或已经）被销毁。
--2.UIPackage.RemovePackage("package");可以使用包的id，包的名称，包的路径，均可以
--3.切换场景时 除了常驻包和有依赖关系的包  其余包卸载掉
--]]
function ViewMgr:UnLoadPackage()
    for sBagName, tBagViewList in pairs(self.tModuleViewList) do 
        for sModuleName, tModleClass in pairs(tBagViewList) do 
            if tModleClass~=nil then
                FairyGUI.UIPackage.RemovePackage(sBagName)
            end
        end
    end 
end

--根据层级隐藏窗口
function ViewMgr:HideWindowByLayer(sTopName)
    local pTopView = self.tTopViewList[sTopName]
    for i=0,pTopView.numChildren-1 do
        self:HideUI(pTopView:GetChildAt(i).gameObjectName)
    end
end

--隐藏其他层级窗口
function ViewMgr:HideWindowExceptLayer(sTopName)
     for index,value in pairs(self.tTopViewList)do
        if index~=sTopName then
            self:HideWindowByLayer(index)
        end
     end
end

--销毁其它层级窗口  
function ViewMgr:DestroyWindowExceptLayer(sTopName)
    --先关闭再销毁
    self:HideWindowExceptLayer(sTopName)
    local tab={}
    for index,value in pairs(self.tTopViewList)do
        if index~=sTopName and value.numChildren>0 then
            for i=0,value.numChildren-1 do
                table.insert(tab,value:GetChildAt(i).gameObjectName)
            end       
        end
    end
    for index,value in pairs(tab)do
        self:DisposeUI(value)
    end
end
