local ViewMgr = RequireSingleton("ViewMgr")
local Refit_MainUI = RequireClass("Refit_MainUI")
local RefitLogic = RequireSingleton("RefitLogic")
local FairyGUI = FairyGUI


function Refit_MainUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.topBox = pMainView:GetChild("topBox")
    self.sellView = pMainView:GetChild("sellView") -- 出售界面
    self.RefitView = pMainView:GetChild("RefitView") -- 改装界面

    --顶部信息
    self.btn_canel = pMainView:GetChild("btn_canel")
    self.btn_refit = pMainView:GetChild("btn_refit") --改装
    self.btn_sell = pMainView:GetChild("btn_sell") --出售
    self.text_jinbi = self.topBox:GetChild("text_jinbi")
    self.text_jintiao = self.topBox:GetChild("text_jintiao")
    self.btn_setTop = self.topBox:GetChild("btn_setTop") 
    self.wifi_1 = self.topBox:GetChild("wifi_1") 
    self.wifi_2 = self.topBox:GetChild("wifi_2") 
    self.wifi_3 = self.topBox:GetChild("wifi_3") 
    self.dianliang = self.topBox:GetChild("dianliang") 
    self.text_dianliang_value = self.topBox:GetChild("dianliang_value")
    self.text_wifi_value = self.topBox:GetChild("wifi_value")
    -- 零件类型
    self.btnparts_zhuti = pMainView:GetChild("Type_zhuti")
    self.btnparts_gongji = pMainView:GetChild("Type_gongji")
    self.btnparts_chelun = pMainView:GetChild("Type_chelun")
    self.btnparts_fuzhu = pMainView:GetChild("Type_fuzhu")
    self.btnparts_fuwen = pMainView:GetChild("Type_fuwen")
    self.btnparts_zhuangshi = pMainView:GetChild("Type_zhuangshi")
    self.btnparts_tiezhi = pMainView:GetChild("Type_tiezhi")

    
    self.btn_warehouse = self.RefitView:GetChild("btn_warehouse")  --仓库按钮

    self.list_Spareparts = self.RefitView:GetChild("list_Spareparts") --滑动列表 零件    
    self.rotate_model = self.RefitView:GetChild("rotate_model")
    self.wrapper = self.RefitView:GetChild("wrapper") -- 组装模型
    self.parts = self.RefitView:GetChild("parts")  -- 显示的零件
    -- 零件属性
    self.text_partsName =self.RefitView:GetChild("partsName")
    self.img_partIcon = self.RefitView:GetChild("partsIcon")
    self.Refit_partsAttribute1 = self.RefitView:GetChild("partsAttribute1")
    self.Refit_partsAttribute2 = self.RefitView:GetChild("partsAttribute2")
    self.Refit_partsAttribute3 = self.RefitView:GetChild("partsAttribute3")
    self.Refit_btn_skillIcon = self.RefitView:GetChild("btn_skillIcon")
    self.Refit_text_skillSay = self.RefitView:GetChild("skillSay")

    -- 机甲所有属性
    self.macAtt_attack = self.RefitView:GetChild("macAtt_attack") -- 攻击
    self.macAtt_load = self.RefitView:GetChild("macAtt_load") -- 承载
    self.macAtt_hp = self.RefitView:GetChild("macAtt_hp") -- 血量
    self.macAtt_speed = self.RefitView:GetChild("macAtt_speed") -- 速度
    self.macAtt_flexible = self.RefitView:GetChild("macAtt_flexible") --转向    
    self.skillList = self.RefitView:GetChild("skillList")  --机甲技能列表

    -- 出售界面
    self.btn_Quality = self.sellView:GetChild("btn_Quality")
    self.sell_partsName = self.sellView:GetChild("partsName")
    self.sell_partsIcon = self.sellView:GetChild("partsIcon")
    self.sell_text_Price = self.sellView:GetChild("text_Price")
    self.sell_text_num = self.sellView:GetChild("text_num")
    self.sell_btn_skillIcon = self.sellView:GetChild("btn_skillIcon")
    self.sell_skillSay = self.sellView:GetChild("skillSay")
    self.sell_partsAttribute1 = self.sellView:GetChild("partsAttribute1")
    self.sell_partsAttribute2 = self.sellView:GetChild("partsAttribute2")
    self.sell_partsAttribute3 = self.sellView:GetChild("partsAttribute3")
    self.sell_parts_list = self.sellView:GetChild("parts_list")
    self.sell_btn_sure = self.sellView:GetChild("btn_sure")

    self.btn_Quality.onChanged:Add(self.ChangeParts,self)
    self.sell_btn_sure.onClick:Add(self.Btnclick_sell,self)
    self.btn_warehouse.onChanged:Add(self.ChangeWareHouseId,self)    
    self.rotate_model.onTouchMove:Add(self.ModelRotate,self )
	self.btn_canel.onClick:Add(self.BtnClick_canel,self)
	self.btn_refit.onClick:Add(function() self:SelectionMode("btn_refit") end)
    self.btn_sell.onClick:Add(function() self:SelectionMode("btn_sell") end)
    
	self.btnparts_zhuti.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_zhuti) end)
	self.btnparts_gongji.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_gongji) end)
	self.btnparts_chelun.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_chelun) end)
	self.btnparts_fuzhu.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_fuzhu) end)
	self.btnparts_fuwen.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_fuwen) end)
	self.btnparts_zhuangshi.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_zhuangshi) end)
	self.btnparts_tiezhi.onClick:Add(function() self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_tiezhi) end)    
end


function Refit_MainUI:OnShow()
    -- 零件类型 
    self.btnparts_zhuti.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-zhuti")
    self.btnparts_gongji.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-gongji")
    self.btnparts_chelun.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-yidong")
    self.btnparts_fuzhu.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-fuzhu")
    self.btnparts_fuwen.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-fuwen")
    self.btnparts_zhuangshi.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-zhuangshi")
    self.btnparts_tiezhi.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-tiezhi")

    self.macAtt_attack.icon  =FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[1]) -- 攻击
    self.macAtt_load.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[2]) -- 承载    
    self.macAtt_hp.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[3]) -- 血量    
    self.macAtt_speed.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[4]) -- 速度    
    self.macAtt_flexible.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[5]) --转向
    
    self.skillId = {}
    self.skillId[1] = -1
    self.skillId[2] = -1
    self.itemPos = {}
    for i=1,7 do
        local _child=self[RefitLogic.partByIdGetName[i]]:GetChild("seleted")
        _child.url = ""
    end
    self:SelectionMode("btn_refit") --默认打开改装界面
    RefitLogic.partsTypeId = 1  -- 默认选择第一个零件类型
    self.btn_warehouse.selectedIndex = 1 -- 默认打开第一个仓库
    self.btn_warehouse.icon = FairyGUI.UIPackage.GetItemURL("Refit", "btn-xiala-1")
    self:ChangeWareHouseId()  --机甲属性
    self:Refit_Spareparts() -- 零件属性
    self:ChangePartsType(RefitLogic.partByNameGetId.btnparts_zhuti)
end

function Refit_MainUI:OnHide()
    if self.machine~= nil then 
        self.machine = nil 
    end 
    if self._model~= nil then 
        self._model = nil 
        UnityEngine.Object.Destroy(self.wrapper.wrapTarget,0.01)
    end 
    UnityEngine.Object.Destroy(self.Gwroper.wrapTarget,0.01)
end

-- 出售按钮
function Refit_MainUI:Btnclick_sell()
    ViewMgr:ShowUI("Refit","ScendSell","ViewLayer","部件出售","您的出售列表包含下列传奇部件是否继续出售")
end
-- 筛选符合材质的零件 按钮
function Refit_MainUI:ChangeParts()
    self.btn_Quality.items = {"无星","传奇","王者","荣耀"}
    RefitLogic.Sell_QualityId= self.btn_Quality.selectedIndex
    self.sell_parts_list.itemRenderer =FairyGUI.ListItemRenderer(self.QualityRender,self)
    self.sell_parts_list.numItems = 10
end

 -- 选择零件类型
function Refit_MainUI:ChangePartsType( nTypeId )
    local partName=RefitLogic.partByIdGetName[nTypeId]
    if RefitLogic.partsTypeId ~= nTypeId then 
        local _child=self[RefitLogic.partByIdGetName[RefitLogic.partsTypeId]]:GetChild("seleted")
        _child.url = ""
    end 
    RefitLogic.partsTypeId = nTypeId
    local seleted=self[partName]:GetChild("seleted")
    seleted.url = FairyGUI.UIPackage.GetItemURL("Refit", "ui-zhuzhuang-biaoqianye-xuanzhong")
    self:ReshList_Spareparts(10)
end

-- 选择模式 
function Refit_MainUI:SelectionMode( sType )
    self.btn_refit.title = "改装"
    self.btn_sell.title = "出售"      
    if sType == "btn_sell" then 
        self.btn_refit.icon = FairyGUI.UIPackage.GetItemURL("Refit","")
        self.btn_sell.icon = FairyGUI.UIPackage.GetItemURL("Refit","btn-biaoqianye")
        self.sellView.visible = true 
        self.RefitView.visible = false
        self:ChangeParts()
        self:Sell_Spareparts() -- 设置基本信息
    else
       self.btn_refit.icon = FairyGUI.UIPackage.GetItemURL("Refit","btn-biaoqianye")
       self.btn_sell.icon =  FairyGUI.UIPackage.GetItemURL("Refit","")
       self.sellView.visible = false 
       self.RefitView.visible = true
    end    
end

-- 设置 刷新机甲技能列表
function Refit_MainUI:ReshList_skill( nLength )
    self.skillList:SetVirtual()
    self.skillList.itemRenderer = FairyGUI.ListItemRenderer(self.SkillRender, self)
    self.skillList.numItems = nLength
end

--设置 刷新零件列表长度
function Refit_MainUI:ReshList_Spareparts( nLength )
    self.list_Spareparts.itemRenderer = FairyGUI.ListItemRenderer(self.SpareRender, self)
    self.list_Spareparts.numItems = nLength
end

-- 改装零件属性信息
function Refit_MainUI:Refit_Spareparts(  )
    self.text_partsName.text = "低级剪刀" --  名字 
    self.img_partIcon.url = FairyGUI.UIPackage.GetItemURL("Refit","")
    self.Refit_partsAttribute1.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[1])
    self.Refit_partsAttribute2.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[2])
    self.Refit_partsAttribute3.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[3])
    self.Refit_partsAttribute1.title  = "1"
    self.Refit_partsAttribute2.title  = "2"
    self.Refit_partsAttribute3.title  = "3"
    self.Refit_btn_skillIcon.icon  = FairyGUI.UIPackage.GetItemURL("Refit","") -- 技能图标
    self.Refit_text_skillSay.text  = "剪刀（jiǎn dāo）是切割布、纸、钢板、绳、圆钢等片状或线状物体的双刃工具，两刃交错，可以开合。"
end

-- 出售零件属性信息
function Refit_MainUI:Sell_Spareparts(  )
    self.sell_partsName.text = "低级剪刀" --  名字 
    self.sell_partsIcon.url = FairyGUI.UIPackage.GetItemURL("Refit","")
    self.sell_partsAttribute1.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[1])
    self.sell_partsAttribute2.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[2])
    self.sell_partsAttribute3.icon = FairyGUI.UIPackage.GetItemURL("Refit",RefitLogic.AttTypeIcon[3])
    self.sell_partsAttribute1.title  = "1"
    self.sell_partsAttribute2.title  = "2"
    self.sell_partsAttribute3.title  = "3"
    self.sell_btn_skillIcon.icon  = FairyGUI.UIPackage.GetItemURL("Refit","") -- 技能图标
    self.sell_skillSay.text  = "剪刀是切割布、纸、钢板、绳、圆钢等片状或线状物体的双刃工具，两刃交错，可以开合。"
end

-- 机甲属性
function Refit_MainUI:MacInfo(  )       
    self.macAtt_attack.title = "0"
    self.macAtt_load.title = "1"
    self.macAtt_hp.title = "2"
    self.macAtt_speed.title = "3"
    self.macAtt_flexible.title = "4"        
    self:ReshList_skill( 10 )
end

-- 改变仓库 
function Refit_MainUI:ChangeWareHouseId()
    if self.btn_warehouse.selectedIndex== 0 then
        ViewMgr:ShowUI("Common","SecondPrompt","ViewLayer","仓库扩充","消耗钻石扩充仓库位置1个！","","")-- left right
    else
        if self.modleId ~= self.btn_warehouse.selectedIndex then 
            self.modleId = self.btn_warehouse.selectedIndex
            self:LoadModel(1)
            self:MacInfo()
        end
    end
    self.btn_warehouse.items = {"增加车库","车库:1","车库:2","车库:3"}        
end

function Refit_MainUI:RemoveModel()
    if self.NpcInstId ~= nil or self.machine ~= nil then
        -- 先释放缓存的Npc的GameObj
        self.machine = nil
        -- 删除Npc
        CS_RemoveNpcGameObj(self.NpcInstId)
        if self.wrapper then
            UnityEngine.Object.Destroy(self.Gwroper.wrapTarget,0.01)
        end
    end
end

function Refit_MainUI:LoadModel(nNpcCfgId)
    self:RemoveModel()
    self.NpcInstId = CS_CreatNpcGameObj(nNpcCfgId, self.LoadModelDone, self)
end

function Refit_MainUI:LoadModelDone(nNpcInstId)
    self.machine=CS_GetNpcGameObj(nNpcInstId)
    if not self.Gwroper then
        self.Gwroper = FairyGUI.GoWrapper.New(self.machine)
    end
    self.Gwroper:SetWrapTarget(self.machine,true)
    self.wrapper:SetNativeObject(self.Gwroper)
    self.machine.transform.localScale =Vector3(160,160,160)
    self.machine.transform.localPosition =Vector3(306,-527,500)
    self.machine.transform.localEulerAngles =Vector3(10,142.9,-11)
    
    --self.Gwroper.wrapTarget = self.machine
    --local modeltransform =self.machine:GetComponent("Transform")
    --modeltransform.transform:Rotate(Vector3(12,-215,-4))
end

function Refit_MainUI:ModelRotate()
    local x = UnityEngine.Input.GetAxis("Mouse X")*5    
    self.machine.transform:Rotate(Vector3(0,-x,0))
end

function Refit_MainUI:BtnClick_canel(btn)
    --ViewMgr:ShowUI("Main","Main","ViewLayer")
    self:Close()
end

function Refit_MainUI:TypeRender(nIndex,gitem)
    local item_Btn=gitem.asButton
    item_Btn.icon = FairyGUI.UIPackage.GetItemURL("Login", "btn-shezhi")
    item_Btn.title = tostring(nIndex)
end

-- 零件列表
function Refit_MainUI:SpareRender(nIndex,gitem)
    local item_Btn = gitem.asButton
    local _bg = item_Btn:GetChild("bg")
    local _starParent = item_Btn:GetChild("starParent")
    self.selePartId = 0
    
    item_Btn.draggable =true 
    item_Btn.onDragStart:Add(self.DragStart,self)
    item_Btn.onDragEnd:Add(function() self:onDragEnd(nIndex,item_Btn) end)
    gitem.onClick:Add(function() self:ItemSeleted(item_Btn) end)
    RefitLogic:LoadItemStar( 3,"Refit","Refit_MainUI","ViewLayer","ui-zhuzhuang-xing",_starParent)   
    self:Refit_Spareparts()
end

function Refit_MainUI:SkillRender(nIndex,gitem)
    local item_Btn=gitem.asButton
    item_Btn:GetChild("bg").visible = false
    item_Btn.icon = FairyGUI.UIPackage.GetItemURL("Refit", "ui-jineng-1")    
    if nIndex == self.skillId[1] or nIndex == self.skillId[2] then
        item_Btn:GetChild("bg").visible = true
    end
    item_Btn.onClick:Add(function() self:SelectionSkill(nIndex) end)
end

function Refit_MainUI:SelectionSkill( nIndex )
    print("nIndex:"..nIndex)
    if nIndex == self.skillId[1] or nIndex == self.skillId[2] then
    else
        self.skillId[1]=self.skillId[2]
        self.skillId[2] = nIndex
        --self.skillList:RefreshVirtualList()
        self.skillList.numItems = 10
    end
end

--  出售界面 零件列表
function Refit_MainUI:QualityRender( nIndex,gitem )
    local y=math.floor( nIndex / 5 ) *gitem.height
    local x = (nIndex % 5 ) * gitem.width
    local partsPos = Vector2(x,y)
    self.itemPos[nIndex]=partsPos
    -- gitem.draggable =true 
    -- gitem.onDragStart:Add(self.DragStart,self)
    -- gitem.onDragEnd:Add(function() self:onDragEnd(nIndex,gitem) end)
    gitem.onClick:Add(function() self:ItemSeleted(gitem) end)
end

function Refit_MainUI:ItemSeleted( gItem )
   local bg=gItem:GetChild("bg")
   bg.url =FairyGUI.UIPackage.GetItemURL("Refit","ui-zhuzhuang-bujian-xuanzhong")       
end


function Refit_MainUI:DragStart( context )
    if self._model~= nil then 
        self._model = nil 
        UnityEngine.Object.Destroy(self.parts.wrapTarget,0.01)
    end 
    if not self._model then
        local _modelTrans=UnityEngine.Resources.Load("Models/Chariot02_chelun_Front_da")
        self._model=UnityEngine.GameObject.Instantiate(_modelTrans)
        if not self.partsWrapper then
            self.partsWrapper = FairyGUI.GoWrapper.New(self._model)
        end
        self._model:GetComponent("Transform").localScale = Vector3(15,15,15)
        self.partsWrapper:SetWrapTarget(self._model,true)
        self.parts:SetNativeObject(self.partsWrapper)
    end 
end

function Refit_MainUI:onDragEnd( nID,gItem )
    -- print("onDragEnd")
    -- local itemPos=self.itemPos[nID]
    -- gItem:SetXY(itemPos.x,-itemPos.y)
end

function Refit_MainUI:OnUpdate(  )
    if self._model~=nil then 
        local mousePos = UnityEngine.Input.mousePosition
        local viewPos=FairyGUI.GRoot.inst:GlobalToLocal(Vector2(mousePos.x,mousePos.y))
        local modelTransform=self._model:GetComponent("Transform")
        local y = viewPos.y-FairyGUI.GRoot.inst.height+self.RefitView.y
        local x = viewPos.x -self.RefitView.x
        modelTransform.localPosition= Vector3(x,y,0)
        -- if UnityEngine.Input:GetMouseButtonUp(0) then
        --     UnityEngine.Object.Destroy(self.parts.wrapTarget,0.01)
        -- end 
    end    
end