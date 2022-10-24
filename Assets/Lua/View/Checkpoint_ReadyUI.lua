local Checkpoint_ReadyUI    = RequireClass("Checkpoint_ReadyUI")
local ViewMgr               = RequireSingleton("ViewMgr")
local CheckpointLogic       = RequireSingleton("CheckpointLogic")
local RefitLogic            = RequireSingleton("RefitLogic")
local ReadLogic             = RequireSingleton("ReadLogic")
local Level_C               = RequireConfig("Level_C")
local PartGrow_C            = RequireConfig("PartGrow_C")
local String_C              = RequireConfig("String_C")
local BattleLogic = RequireSingleton("BattleLogic")
local FairyGUI              = FairyGUI

function Checkpoint_ReadyUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    --顶部UI
    self.title          = pMainView:GetChild("title")
    self.titletxt       =self.title:GetChild("title")
    self.topBox         = pMainView:GetChild("topBox")
    self.btn_out        = self.title:GetChild("btn_out")
    self.text_jintiao   = self.topBox:GetChild("text_jintiao")
    self.text_jinbi     = self.topBox:GetChild("text_jinbi")
    --左侧UI
    self.leftlist       = pMainView:GetChild("selectlist")
    self.list_Seletion  = self.leftlist:GetChild("list_Seletion").asList
    self.btn_Clear      = self.leftlist:GetChild("clear")
    --零件信息UI
    self.partinfo       = pMainView:GetChild("partinfo")
    self.partsIcon      = self.partinfo:GetChild("partsIcon")
    self.pro1           = self.partinfo:GetChild("pro1")
    self.pro2           = self.partinfo:GetChild("pro2")
    self.pro1icon       = self.pro1:GetChild("icon")
    self.pro1name       = self.pro1:GetChild("name")
    self.pro1num        = self.pro1:GetChild("num")
    self.pro2icon       = self.pro2:GetChild("icon")
    self.pro2name       = self.pro2:GetChild("name")
    self.pro2num        = self.pro2:GetChild("num")
    self.partsName      = self.partinfo:GetChild("partsName")
    self.parts_Skilldescribe = self.partinfo:GetChild("parts_Skilldescribe")
    self.parts_Describe = self.partinfo:GetChild("parts_Describe")
    self.partskillicon  = self.partinfo:GetChild("icon")
    self.expend1        = self.partinfo:GetChild("expend1")
    self.expend1icon    = self.expend1:GetChild("icon")
    self.expend1txt     = self.expend1:GetChild("txt")
    self.expend2        = self.partinfo:GetChild("expend2")
    self.expend2icon    = self.expend2:GetChild("icon")
    self.expend2txt     = self.expend2:GetChild("txt")
    --已有零件UI
    self.list_Item      = pMainView:GetChild("list_Item")
    self.tabController  = pMainView:GetController("tab")
    self.txt1           = pMainView:GetChild("txt1"):GetChild("txt")
    self.txt2           = pMainView:GetChild("txt2"):GetChild("txt") 
    self.txt3           = pMainView:GetChild("txt3"):GetChild("txt")
    self.txt4           = pMainView:GetChild("txt4"):GetChild("txt")
    self.txt5           = pMainView:GetChild("txt5"):GetChild("txt")
    --关卡信息UI
    self.btn_award      = pMainView:GetChild("btn_award")
    self.btn_Easy       = pMainView:GetChild("btn_eazy")
    self.btn_Eazyicon   = self.btn_Easy:GetChild("icon")
    self.btn_Eazytxt    = self.btn_Easy:GetChild("txt")
    self.btn_Eazytxtan  = self.btn_Easy:GetChild("txtan")
    self.btn_Hard       = pMainView:GetChild("btn_diff")
    self.btn_Hardicon   = self.btn_Hard:GetChild("icon")
    self.btn_Hardtxt    = self.btn_Hard:GetChild("txt")
    self.btn_Hardtxtan  = self.btn_Hard:GetChild("txtan")
    self.btn_start      = pMainView:GetChild("btn_start")
    self.passcon        = pMainView:GetChild("passcon")
    self.physical       = pMainView:GetChild("physical")
    self.physicalicon   = self.physical:GetChild("icon")
    self.physicaltxt    = self.physical:GetChild("txt")
    self._holder        = pMainView:GetChild("modelshow")
    self.n70            = pMainView:GetChild("logerror")
    self.partnum        = pMainView:GetChild("partnum")
    self.rotate         = pMainView:GetChild("rotate")
    self.rotate.onTouchMove:Add(function () self:ModelRotate() end)
    --判断进入关卡的条件，同时满足可进入关卡：关卡，移动，武器
    self.conditionjude={false,false,false}

    self.tabController.onChanged    :Add(self.OnClickCon,self)     
    self.btn_out.onClick            :Add(function() self:Close() end)
    self.btn_Easy.onClick           :Add(function() self:Btnclick_Mode(1) end)
    self.btn_Hard.onClick           :Add(function() self:Btnclick_Mode(2) end)    
    self.btn_start.onClick          :Add(function() self:Btnclick_Challenges( ) end)
    self.btn_Clear.onClick          :Add(self.Btnclick_Clear,self)
    self.btn_award.onClick          :Add(self.ShowAwardUI,self)
    
    self.list_Item.onClickItem      :Add(self.OnClickItem,self)
    self.list_Seletion.onClickItem  :Add(self.OnClickSeleItem,self)
    self.list_Item.itemRenderer     = FairyGUI.ListItemRenderer(self.ItemRender,self)
    self.list_Seletion.itemRenderer = FairyGUI.ListItemRenderer(self.SeletionRender,self)

    --按压检测
    self.longpress=FairyGUI.LongPressGesture.New(self.list_Item)
    self.longpress.once=true
    self.longpress.trigger=1
    self.longpress.onAction:Add(self.OnLongPress,self)
end

function Checkpoint_ReadyUI:OnShow(levelid)
    self.table          = Level_C[levelid].ownPart
    self.text_jintiao.text  = "15"
    self.text_jinbi.text    = "18"
    self.partsName.text     = ""
    self.parts_Skilldescribe.text = "" 
    self.parts_Describe.text = "" 
    self.titletxt.text      ="部件选择"
    self.btn_Eazyicon.url   = FairyGUI.UIPackage.GetItemURL("Checkpoint", "ubtn-guanka-jiandan")
    self.btn_Eazytxt.text   = "简单"
    self.btn_Eazytxtan.text = "简单"
    self.btn_Hardicon.url   = FairyGUI.UIPackage.GetItemURL("Checkpoint", "btn-guanka-kunnan")
    self.btn_Hardtxt.text   = "困难"
    self.btn_Hardtxtan.text = "困难"
    self.txt1.text          = "全部"
    self.txt2.text          = "车身"
    self.txt3.text          = "部件"
    self.txt4.text          = "推进器"
    self.txt5.text          = "面具"
    self.n70.text           =""
    self.expend1icon.url    = FairyGUI.UIPackage.GetItemURL("Common", "ui-lingshi-2")
    self.expend2icon.url    = FairyGUI.UIPackage.GetItemURL("Common", "ui-lingshi-2")
    self.physicalicon.url   = FairyGUI.UIPackage.GetItemURL("Common", "ui-lingshi-1")

    --控制器选择全部
    self.tabController.selectedIndex=4
    --设置list
    self.list_Item.numItems = #self.table
    --模拟点击第一个
    self.list_Item:GetChildAt(0).asButton:FireClick(true)
    self.list_Item:GetChildAt(0).asButton.onClick:Call()
    --困难模式
    CheckpointLogic.ModeId =1
    --关卡信息设置  
    if levelid ~=nil then
        self.id=levelid
        self.Leveltable=RefitLogic:GetMonsterModelid(self.id)
        self.monstermodel=CS_CreatNpcGameObj(self.Leveltable[3][1], self.LoadModelDone, self)
        self.passcon.text="击败怪物"--通过self.Leveltable[1]在国际化语言表中找 
        self.physicaltxt.text=self.Leveltable[2][1]
    end
    --深度调整(待研究) 
    --self.list_Item:InvalidateBatchingState()
    --清理一遍数据(先放这里)
    BattleLogic:ClearTable()
end

function Checkpoint_ReadyUI:LoadModelDone(nNpcInstId)
    -- 异步操作检测是否依然打开着界面
    if not self:IsVisible() then
        return
    end
    self._model = CS_GetNpcGameObj(nNpcInstId);
    self.wrapper = FairyGUI.GoWrapper.New(self._model)
    self.wrapper:SetWrapTarget(self._model, true)
    self._holder:SetNativeObject(self.wrapper)
    local modelTransform=self._model:GetComponent("Transform")
    modelTransform.localScale =Vector3(150, 150, 150)
    modelTransform.localPosition = Vector3(430, -598,684)
    modelTransform.parent:GetComponent("Transform").localPosition = Vector3(1090,-118,500)
    self._model.transform.localEulerAngles=Vector3(6.5,219,6.6)
end

function Checkpoint_ReadyUI:OnHide( ... )
    
end

function Checkpoint_ReadyUI:Btnclick_OpenView( sBagName,sViewName )
    ViewMgr:ShowUI(sBagName,sViewName,"ViewLayer")
end

-- 选择关卡的难易模式
function Checkpoint_ReadyUI:Btnclick_Mode(nModeId)
    CheckpointLogic.ModeId =nModeId
    self.physicaltxt.text=self.Leveltable[2][nModeId]
end

-- 已拥有的道具列表
function Checkpoint_ReadyUI:ItemRender( nIndex, gItem )
    local item=gItem.asCom
    item:GetChild("n10").text   = self.table[nIndex+1]
    item:GetChild("icon").icon  = FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(self.table[nIndex+1])) 
    item:GetChild("bg").visible=false
    local nums=self.list_Seletion.numChildren-1
    for i=0,nums do
        local item1=self.list_Seletion:GetChildAt(i)
        if self.table[nIndex+1]==tonumber(item1:GetChild("n10").text) then
            item:GetChild("bg").visible=true
        end
    end 
end

--判断通关条件
function Checkpoint_ReadyUI:SetConditionJude(id,bol)
    if RefitLogic:GetItemType(id)==0 then
        self.conditionjude[1]=bol
    elseif RefitLogic:GetItemType(id)==2 then
        self.conditionjude[2]=bol
    elseif RefitLogic:GetItemType(id)==1 then
        self.conditionjude[3]=bol
    end
end

function Checkpoint_ReadyUI:SetChildInfo(id)
    local item=self.list_Seletion:AddItemFromPool().asCom
    item:GetChild("n10").text   =id
    item:GetChild("icon").icon  =FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(id))
    self.partsIcon.url          = FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(id))
    --self.expend1icon.icon       = self.expend1:GetChild("icon")
    self.expend1txt.text        = PartGrow_C[id].consumRes[1][2]
    --self.expend2icon.icon       = self.expend2:GetChild("icon")
    self.expend2txt.text        = PartGrow_C[id].consumRes[2][2]
    self:SetConditionJude(id,true)
    return item
end



-- 选择道具
function Checkpoint_ReadyUI:OnClickItem(content)
    local id=tonumber(content.data.asCom:GetChild("n10").text)
    if content.data.asCom:GetChild("bg").visible then
        content.data.asCom:GetChild("bg").visible=false
        --当点击已选择的零件时撤回
        local nums=self.list_Seletion.numChildren-1
        for i=0,nums do
            local item=self.list_Seletion:GetChildAt(i)
            if tonumber(item:GetChild("n10").text)==tonumber(content.data.asCom:GetChild("n10").text) then
                self.list_Seletion:RemoveChildToPool(item)
                self:SetConditionJude(id,false)
                break
            end
        end 
    else
        content.data.asCom:GetChild("bg").visible=true
        if RefitLogic:GetItemType(id)==0 then
            --判断已选择的列表中是否有车体 如果有则替换
            if self.list_Seletion.numChildren>0 then
                local haveitem=self.list_Seletion:GetChildAt(0)
                local haveitemid=tonumber(haveitem:GetChild("n10").text)
                if RefitLogic:GetItemType(haveitemid)==0 then
                    for i=0,self.list_Item.numChildren-1 do
                        local item=self.list_Item:GetChildAt(i)
                        if tonumber(item:GetChild("n10").text)==haveitemid then
                            if item:GetChild("bg").visible then
                                item:GetChild("bg").visible=false
                            end
                        end
                    end
                    haveitem:GetChild("n10").text=id
                    haveitem:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(id))
                    self.partsIcon.url= haveitem:GetChild("icon").icon
                else
                    local item=self:SetChildInfo(id)
                    self.list_Seletion:SetChildIndex(item,0)
                end 
            else
                self:SetChildInfo(id)              
            end
        else
            self:SetChildInfo(id)
        end         
        self.list_Seletion.scrollPane:ScrollBottom()
    end
    self.partnum.text=self.list_Seletion.numChildren.."/99"
    --滑到底部
    self.list_Seletion.scrollPane:ScrollBottom()
    --信息同步(暂无信息)
    self.partsName.text=String_C[tonumber(content.data:GetChild("n10").text)].str  
end

-- 撤销选择的道具
function Checkpoint_ReadyUI:OnClickSeleItem(content)
    local id=tonumber(content.data.asCom:GetChild("n10").text)
    for i=0,self.list_Item.numChildren-1 do
        local item=self.list_Item:GetChildAt(i)
        if tonumber(item:GetChild("n10").text)==tonumber(content.data.asCom:GetChild("n10").text) then
            if item:GetChild("bg").visible then
                item:GetChild("bg").visible=false
            end
        end
    end
    self.list_Seletion:RemoveChildToPool(content.data)
    self:SetConditionJude(id,false)
    self.partnum.text=self.list_Seletion.numChildren.."/99"
end

-- 清空已选择的零件
function Checkpoint_ReadyUI:Btnclick_Clear()
    self.list_Seletion.numItems=0
    self.partnum.text=self.list_Seletion.numChildren.."/99"
    for i=0,self.list_Item.numChildren-1 do
        local item=self.list_Item:GetChildAt(i)
        if item:GetChild("bg").visible then
           item:GetChild("bg").visible=false
        end
    end
end

-- 长按显示信息
function Checkpoint_ReadyUI:OnLongPress(content)
    --长按时取消点击事件
    FairyGUI.Stage.inst:CancelClick(content.inputEvent.touchId);
    --取消list的滑动
    self.list_Item.scrollPane:CancelDragging()
    local obj = FairyGUI.GRoot.inst.touchTarget;
    self.partsName.text=String_C[tonumber(obj.parent:GetChild("n10").text)].str
    self.partsIcon.url= obj.parent:GetChild("icon").icon
end

-- 零件分类
function Checkpoint_ReadyUI:OnClickCon()
    --tab控制器={4,0,1,2,3}
    if self.tabController.selectedIndex==4 then
        self.list_Item.numItems = #self.table
    else
        self.list_Item.numItems = 0--清空
        for index,value in pairs(self.table)do
            if RefitLogic:GetItemType(value)==self.tabController.selectedIndex then    
                local item=self.list_Item:AddItemFromPool().asCom
                item:GetChild("n10").text=value
                item:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(value))
            end
        end
    end
end

-- 发送挑战请求
function Checkpoint_ReadyUI:Btnclick_Challenges()
    -- 判断体力 
    --[[if self.id~=nil then
        for index,value in pairs(self.conditionjude) do
            if not value then
                self.n70.text="缺少 车体,移动或武器组件"
                return               
            end
        end
        self.n70.text=""
        for i=0,self.list_Seletion.numChildren-1 do
            local haveitem=self.list_Seletion:GetChildAt(i)
            BattleLogic:SetSelectPartList(tonumber(haveitem:GetChild("n10").text))
        end
        CS_EnterGameLevel(self.id)
    end]]--
    for i=0,self.list_Seletion.numChildren-1 do
        local haveitem=self.list_Seletion:GetChildAt(i)
        BattleLogic:SetSelectPartList(tonumber(haveitem:GetChild("n10").text))
    end
    CS_EnterGameLevel(self.id)
    BattleLogic.BattCheckId = self.id
end

-- 奖励预览
function Checkpoint_ReadyUI:ShowAwardUI()
end

function Checkpoint_ReadyUI:OnClose()
    self.list_Seletion.numItems=0
    self.tabController.selectedIndex=4
    self.list_Item:GetChildAt(0).asButton:FireClick(true)
    self.list_Item:GetChildAt(0).asButton.onClick:Call()
    CheckpointLogic.ModeId =1

    if self.monstermodel ~= nil or self._model ~= nil then
        -- 先释放缓存的Npc的GameObj
        self._model = nil
        -- 删除Npc
        CS_RemoveNpcGameObj(self.monstermodel)
        if self.wrapper then
            UnityEngine.Object.Destroy(self.wrapper.wrapTarget,0.01)
        end
        self.wrapper:Dispose()
    end

    self:Hide()
end
function Checkpoint_ReadyUI:ModelRotate()
    
    local modelPosx =UnityEngine.Input.GetAxis("Mouse X") * 12 * 0.5
    local modelPosy =UnityEngine.Input.GetAxis("Mouse Y") * 12 * 0.5
    if self._model~= nil then 
        self._model.transform:Rotate(Vector3(0,-modelPosx,0))
    end 
end

