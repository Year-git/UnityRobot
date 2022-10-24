local Checkpoint_MainUI = RequireClass("Checkpoint_MainUI")
local UnityEngine = UnityEngine
local ViewMgr = RequireSingleton("ViewMgr")
local FairyGUI = FairyGUI
local CheckpointLogic = RequireSingleton("CheckpointLogic") 
local Level_C = RequireConfig("Level_C")
local Chapter_C = RequireConfig("Chapter_C")

function Checkpoint_MainUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    --顶部基本信息
    self.topBox = pMainView:GetChild("topBox")
    self.text_jintiao =  self.topBox:GetChild("text_jintiao")
    self.text_jinbi =  self.topBox:GetChild("text_jinbi")
    self.text_zuanshi =  self.topBox:GetChild("text_zuanshi")

    self.lefttopBox = pMainView:GetChild("lefttopBox")
    self.btn_out = self.lefttopBox:GetChild("btn_out")
    self.btn_technology = pMainView:GetChild("btn_technology")
    self.starNum = pMainView:GetChild("starNum")
    self.starParent = pMainView:GetChild("starParent") 
    self.btn_out.onClick:Add(function( )self:Close() end)
    self.btn_technology.onClick:Add(function( ) self:OpenCheckView()  end)

    for i=1,5 do
        self["btn_theme"..i] = self.starParent:GetChild("btn_theme"..i)
        self["btn_IntoCheck"..i] = self["btn_theme"..i]:GetChild("btn_IntoCheck")
        self["btn_IntoCheck"..i].onClick:Add(function( ) self:OpenCheckView(1)  end)
        
        self["btn_theme"..i].onTouchBegin:Add(self.TouchBegin, self)
        self["btn_theme"..i].onTouchMove:Add(self.TouchMove, self)
        self["btn_theme"..i].onTouchEnd:Add(self.TouchEnd, self)
    end
end

function  Checkpoint_MainUI:OnShow( ... )
    self.themePos = {}
    self.text_jintiao.text = "1235415"
    self.text_jinbi.text = "132156"
    self.text_zuanshi.text = "156165"
    self.ThemeId = 1  -- 默认选择第一个主题星球
    CheckpointLogic:GetChapterInfo()
    CheckpointLogic:GetThemeStarNum(self.ThemeId)
    self.starNum.text = ""..CheckpointLogic.MainStarNum[self.ThemeId]
    for i=1,5 do
        self.themePos[i]=self["btn_theme"..i].x
        self["btn_IntoCheck"..i].icon = FairyGUI.UIPackage.GetItemURL("Checkpoint","ui-gunaka-xingqiu-"..i)
    end
    self.touchId=-1
end
function  Checkpoint_MainUI:OnHide( ... )
        
end

function Checkpoint_MainUI:OpenCheckView( nIndex )
    --  判断主题是否开启
    self:OpenView("Checkpoint","Check", nIndex)

end

--解锁关卡
function Checkpoint_MainUI:BtnClick_unlock( nCheckId )
    -- 判断星星数量
    if nCheckId ~= 0 then
        ViewMgr:ShowUI("Common","Tips","TopLayer","需前置主题简单难度主线关卡全部通关")
    end
end

function Checkpoint_MainUI:OpenView( sBagName,sViewName,nCheckId )
    ViewMgr:ShowUI(sBagName,sViewName,"ViewLayer",nCheckId)
end

-- 二次确认面板 确定按钮的点击事件
function Checkpoint_MainUI:BtnClick_sure(  )
    ViewMgr:HideUI("Common_SecondPromptUI")
    ViewMgr:ShowUI("Main","Main","ViewLayer")
end

function Checkpoint_MainUI:ListMove(  )
    local  midX = self.list_checkpoint.scrollPane.posX + self.list_checkpoint.viewWidth / 2
    local cnt = self.list_checkpoint.numChildren
    for i=1,cnt do
        local obj = self.list_checkpoint:GetChildAt(i-1)
        local _x=midX - obj.x - obj.width / 2
        local dist = Mathf.Abs(_x)
        if dist > obj.width then
            obj:SetScale(1, 1)
        else
            local ss = 1 + (1 - dist / obj.width) * 0.24
            obj:SetScale(ss, ss)
        end
    end
end

function Checkpoint_MainUI:OnUpdate(  )
        -- if self.themePos[i] > 944 then
        --     self.rightArr=self["btn_theme"..i]  -- 
        -- else
        --     self.leftArr = self["btn_theme"..i]  --  
        -- end
        -- if  self["btn_theme"..self.ThemeId].x < 523 then 
        --     self["btn_theme"..self.ThemeId].alpha =0.52
        -- end 
    
    -- -- if  当前的星球移动距离大于了半径 
    --     -- 触发星球移动事件 根据移动X的坐标与原坐标的差比较 计算下一个显示的星球等级
    -- -- end    
end

function Checkpoint_MainUI:SetStarPos(  )
    if self["btn_theme" .. self.ThemeId].x ~= 944 then
        if self["btn_theme" .. self.ThemeId].x < 944 then --  向左移动 星球索引减1
            self.ThemeId = self.ThemeId - 1
            if self.ThemeId == 0 then
                self.ThemeId = 5
            end
            self["btn_theme" .. self.ThemeId]:SetXY(944, 518)
        else
            --  向右移动  星球索引加1
            self.ThemeId = (self.ThemeId + 1) % 6
        end
        print("self.ThemeId:" .. self.ThemeId)
        for i = 1, 5 do
            local _nIndex = i % 6                    
            print("_nIndex:" .. _nIndex)
            if i == self.ThemeId then
                local _starInfo = CheckpointLogic.starnIndex[1]  --- 
                self.starParent:SetChildIndex(self["btn_theme" .. self.ThemeId], 4)
                self["btn_theme" .. self.ThemeId ]:SetXY(_starInfo.x, _starInfo.y)
                self["btn_theme" .. self.ThemeId ].alpha = _starInfo.alpha
                self["btn_theme" .. self.ThemeId].scale = Vector2(_starInfo.scale, _starInfo.scale)
            else
                local childIndex = 0
                if _nIndex > 0 then 
                    if self.ThemeId > _nIndex then 
                        _nIndex =_nIndex +1
                        if _nIndex == 5 then 
                            childIndex = 0
                        else
                            childIndex=_nIndex
                        end 
                    end 
                    local _starInfo = CheckpointLogic.starnIndex[_nIndex]
                    self.starParent:SetChildIndex(self["btn_theme" .. _nIndex], childIndex)
                    self["btn_theme" .. i]:SetXY(_starInfo.x, _starInfo.y)
                    self["btn_theme" .. i].alpha = _starInfo.alpha
                    self["btn_theme" .. i].scale = Vector2(_starInfo.scale, _starInfo.scale)
                end 
            end
        end
    end
end

function Checkpoint_MainUI:BtnDrop_begin(  )
    -- 获取鼠标坐标  跟着鼠标移动 切换子物体索引 
end

function Checkpoint_MainUI:ThemeMove(  )
    
end

function Checkpoint_MainUI:TouchBegin(context)
    local nstarIndex = 0 
    if self.touchId ~= -1 then
        return
    end
    if nstarIndex ~= self.ThemeId then 
        nstarIndex = self.ThemeId
    end 
    local evt = context.data
    self.touchId = evt.touchId
    local pt = FairyGUI.GRoot.inst:GlobalToLocal(Vector2(evt.x, evt.y))
    local bx = pt.x
    local by = pt.y
    self["btn_theme" .. nstarIndex].selected = true
    if bx < 0 then
        bx = 0
    elseif bx > self["btn_theme" .. nstarIndex].width then
        bx = self["btn_theme" .. nstarIndex].width
    end
    if by > FairyGUI.GRoot.inst.height then
        by = FairyGUI.GRoot.inst.height
    elseif by < self["btn_theme" .. nstarIndex].y then
        by = self["btn_theme" .. nstarIndex].y
    end
    self._lastStageX = bx
    self._lastStageY = by
    self.startStageX = bx 
    self.startStageY = by
    self["btn_theme" .. nstarIndex]:SetXY(bx, by)
    context:CaptureTouch()
end

function Checkpoint_MainUI:TouchMove(context)
    local evt = context.data
    if evt.touchId == -1 then
        return
    end
    if self.touchId ~= evt.touchId then
        return
    end
    local pt = FairyGUI.GRoot.inst:GlobalToLocal(Vector2(evt.x, evt.y))
    local bx = pt.x
    local by = pt.y
    local moveX = bx - self._lastStageX
    local moveY = by - self._lastStageY
    self._lastStageX = bx
    self._lastStageY = by
    local buttonX = self["btn_theme" .. self.ThemeId].x + moveX
    local buttonY = self["btn_theme" .. self.ThemeId].y + moveY

    local offsetX = buttonX - self.startStageX
    local offsetY = buttonY - self.startStageY
    buttonX = self.startStageX + offsetX
    buttonY = self.startStageY + offsetY
    if buttonX < 0 then
        buttonX = 0
    end
    if buttonY > FairyGUI.GRoot.inst.height then
        buttonY = FairyGUI.GRoot.inst.height
    end
    self["btn_theme" .. self.ThemeId]:SetXY(buttonX, buttonY)
end

function Checkpoint_MainUI:TouchEnd(  context )
    local evt = context.data
    if evt.touchId == -1 then
        return
    end
    if evt.touchId ~= self.touchId then
        return
    end
    self.touchId = -1    
    self:SetStarPos(  )
end