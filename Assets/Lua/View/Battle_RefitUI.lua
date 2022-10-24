local ViewMgr=RequireSingleton("ViewMgr")
local Battle_RefitUI=RequireClass("Battle_RefitUI")
local FairyGUI = FairyGUI
local UnityEngine = UnityEngine 
local EventMgr = RequireSingleton("EventMgr")
local RefitLogic = RequireSingleton("RefitLogic")
local PartGrow_C=RequireConfig("PartGrow_C")
local Part_C=RequireConfig("Part_C")
local Slot_C=RequireConfig("Slot_C")
local Skill_C=RequireConfig("Skill_C")
local BattleLogic = RequireSingleton("BattleLogic")

function Battle_RefitUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.parttable      ={}--记录当前车体槽位对应的零件
    self.holetypeiconlist=
    {
        {"btn-gaizhuang-gongji-weixuan","btn-gaizhuang-gongji-xuanzhong"},
        {"btn-gaizhuang-yingdong-weixuan","btn-gaizhuang-yingdong-xuanzhong"},
        {"btn-gaizhuang-fuzhu-weixuan","btn-gaizhuang-fuzhu-xuanzhong"},
    }
    self.holetyoedsc    ={"攻击位","移动位","辅助位","装饰位"}
    self.PartList       =pMainView:GetChild("itemlist")
    self.modelrotate    =pMainView:GetChild("rotate")
    self.dele           =pMainView:GetChild("btn_close")
    self.deletitle      =self.dele:GetChild("title")
    self.btn_pro        =pMainView:GetChild("btn_pro")
    self.btn_pro.onClick:Add(function() ViewMgr:ShowUI("Battle","RobotInfo","InfoLayer",self.nSelectPartId) end)
    
    self.skillUI        =pMainView:GetChild("skillUI")
    self.skillUIList    =self.skillUI:GetChild("list")
    self.skillUIList.scrollPane:CancelDragging()
    self.skillUIList.onClickItem      :Add(self.SkillReplace,self)

    self.holelist       =pMainView:GetChild("holelist")
    self.holelist.onClickItem      :Add(self.OnClickHoleItem,self)
    self.holelist.itemRenderer     = FairyGUI.ListItemRenderer(self.ItemRender,self)
    self.listbg         =pMainView:GetChild("listbg")
    self.holetypetext   =self.listbg:GetChild("text")
    self.title          =pMainView:GetChild("title")
    self.titletext      =self.title:GetChild("title")
    self.btn_out        =self.title:GetChild("btn_out")
    self.btn_out.onClick:Add(
        function()           
            --self.transition:PlayReverse()
            self:RefitOver()
        end
    )
    --temp 金钱
    self.top            =pMainView:GetChild("top")
    self.money1         =pMainView:GetChild("text_jinbi")
    self.money2         =pMainView:GetChild("text_zuanshi")
    self.log            =pMainView:GetChild("log")

    self.modelrotate.onTouchMove:Add(function () self:ModelRotate() end)
    --动效
    self.transition=self.pMainView:GetTransition("UIOpen")
    self.machine=CS_GetNpcGameObj(MyPlayer.playerInstId)

    self.PartList.onClickItem:Add(self.OnClickItem,self)
    self.PartList.itemRenderer     = FairyGUI.ListItemRenderer(self.PartListItemRender,self)
    self.dele.onClick:Add(
        function()           
            --self.transition:PlayReverse()
            self:RefitOver()    
        end
    )
    self.btn_pro.onClick:Add(self.OpenPartInfoUI,self)
    --特效实例ID
    self.EffectInstID=-1
    --槽位类型
    self.holetype  = 0
    self.holenum=1
    --当前选择的配件 默认车体
    self.nSelectPartId=BattleLogic.SelectPartList[1] or (10000)
     --进入界面 放置车体且不能替换.槽位ID=0
    --[[if next(BattleLogic.SelectPartList)~=nil then
        LuaExtend.NpcRobotPartInstall(MyPlayer.playerInstId,0,BattleLogic.SelectPartList[1],self.LoadModelDone,self)
    else
        CS_LogError("错误 未选择零件")
    end]]--   

    EventMgr:RegistEvent(GacEvent.SelectSkillOver,self.RefrshSkillUI,self)
end

function Battle_RefitUI:OnShow()
    self.transition:Play()
    self.titletext.text="改装"
    self.deletitle.text="继续战斗"
    self.nFlagSlelectSkillID = 0
    self.log.text=""
    --特效时间间隔
    self.transtime=0
    for i=0,self.skillUIList.numChildren-1 do
        local item=self.skillUIList:GetChildAt(i)
        item:GetChild("hongdian").visible=false
    end
    self:InitialUI()
    self:RefrshSkillUI()
end
function Battle_RefitUI:LoadModelDone()
    self:InitialUI()
end

function Battle_RefitUI:InitialUI()
    --除去车体
    if LuaExtend.GetPlayerHoleCount()>0 then
        self.holelist.numItems=LuaExtend.GetPlayerHoleCount()-1
        self.holelist.scrollPane:CancelDragging()
    end
end
--槽位渲染
function Battle_RefitUI:ItemRender( nIndex, gItem )
    local item=gItem.asCom
    if self.holetype==LuaExtend.GetPlayerHoleType(nIndex+1)then
        self.holenum=self.holenum+1  
    else
        self.holenum=1
        self.holetype=LuaExtend.GetPlayerHoleType(nIndex+1)
    end
    item.visible=false
    self.transtime=self.transtime+0.1
    item:GetTransition("holeTrans"):Play(1,self.transtime,nil)
    item:GetChild("holetext").text  = self.holetyoedsc[self.holetype]..tostring(self.holenum)
    item:GetChild("up").icon        = FairyGUI.UIPackage.GetItemURL("Battle", self.holetypeiconlist[self.holetype][1]) 
    item:GetChild("down").icon      = FairyGUI.UIPackage.GetItemURL("Battle", self.holetypeiconlist[self.holetype][2]) 
    if nIndex==self.holelist.numItems-1 then
        self.holelist:GetChildAt(0).asButton:FireClick(true)
        self.holelist:GetChildAt(0).asButton.onClick:Call()
    end
end

--点击槽位
function Battle_RefitUI:OnClickHoleItem(content)
    self.PartList.numItems = 0
    --[[local nums=self.PartList.numChildren
    for i=0,nums-1 do
        local dItem=self.PartList:GetChildAt(i)
        self.PartList:RemoveChild(dItem)
        dItem:Dispose()
    end]]--
   
    self.holeid= self.holelist:GetChildIndex(content.data)+1
    self:CreateHoleEffect()
    --特效时间间隔
    local transform=0
    --判断该槽位有没有配件
    local holepartid=LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,self.holeid)
    local holetype  = LuaExtend.GetPlayerHoleType(self.holeid)
    for index,value in pairs(BattleLogic.SelectPartList)do
        if RefitLogic:GetItemType(value)==holetype then
            --local item= FairyGUI.UIPackage.CreateObject("Battle", "RefitPartItem").asCom
            --self.PartList:AddChild(item)
            local item=self.PartList:AddItemFromPool().asCom
            --item.visible=false
            --transform=transform+0.1
            --item:GetTransition("itemTrans"):Play(1,transform,nil)
            item:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(value)) 
            item:GetChild("pro1text").text=PartGrow_C[value].consumRes[1][2]
            item:GetChild("pro2text").text=PartGrow_C[value].consumRes[2][2]
            item:GetChild("id").text=value
            if holepartid==value then
                item.asButton.selected=true
            end
        end
    end
    self.holetypetext.text=content.data:GetChild("holetext").text
end

function Battle_RefitUI:PartListItemRender(nIndex,gItem)
    
end
--点击配件
function Battle_RefitUI:OnClickItem(content)
    self.nSelectPartId=tonumber(content.data:GetChild("id").text)
    --判断当前槽位ID是否相同
    local holepartid=LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,self.holeid)

    if holepartid==self.nSelectPartId then
        --卸载
        LuaExtend.NpcRobotPartUnInstall(MyPlayer.playerInstId,self.holeid)
        self.nFlagSlelectSkillID=-1
        self:LoadModelOver()
        content.data.asButton.selected=false
    else
        --安装
        self.nFlagSlelectSkillID = Part_C[self.nSelectPartId].skillID
        LuaExtend.NpcRobotPartInstall(MyPlayer.playerInstId,self.holeid,self.nSelectPartId,self.LoadModelOver,self);  
    end
    
end

--加载完配件模型之后的回调 除车体
--在技能面板添加技能
function Battle_RefitUI:LoadModelOver()
    local nMaxHoleCount = LuaExtend.GetPlayerHoleCount()
    local tSkillList = {}
    local tMapping = {}
    local nSeleskilllist={}
    for nIdx = 1, nMaxHoleCount - 1 do 
        --判断该槽位有没有配件
        local holepartid = LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,nIdx)
        if holepartid>0 then
            local skillID = Part_C[holepartid].skillID
            if (skillID > 0) and (tMapping[skillID] == nil) then
                table.insert(tSkillList, skillID)
                tMapping[skillID] = true
            end
        end
    end
    for skilhole, nSelectSkillId in ipairs(BattleLogic.SelectSkillList) do 
        if nSelectSkillId>0 then
            nSeleskilllist[nSelectSkillId]=true
        end      
    end
    -- 检测已选技能栏是否有移除掉的部件技
    for skilhole, nSelectSkillId in ipairs(BattleLogic.SelectSkillList) do 
        if nSelectSkillId > 0 then
            if not tMapping[nSelectSkillId] then
                self:SelectSkill(skilhole, -1)
                break
            end
        end
    end
    --添加新技能(替换也在这里)
    if not nSeleskilllist[self.nFlagSlelectSkillID] then
        for skilhole, nSelectSkillId in ipairs(BattleLogic.SelectSkillList) do 
            if nSelectSkillId<0 and self.nFlagSlelectSkillID>0 then
                self:SelectSkill(skilhole, self.nFlagSlelectSkillID)
                break
            end
        end      
    end
end

function Battle_RefitUI:OnHide()
end

function Battle_RefitUI:ModelRotate()
    local modelPosx =UnityEngine.Input.GetAxis("Mouse X") * 12 * 0.5
    local modelPosy =UnityEngine.Input.GetAxis("Mouse Y") * 12 * 0.5
    if self.machine~= nil then 
        self.machine.transform:Rotate(Vector3(0,-modelPosx,0))
    end 
end

function Battle_RefitUI:OnClose()
    EventMgr:DisplayEvent(GacEvent.AssembleEnd)     
    LuaExtend.DestroyHoleEffect(self.EffectInstID)
    LuaExtend.PlayerAssembleEnd() 
    MyPlayer.RefitCameraShow(false)  
    ViewMgr:ShowUI("Battle","Main","ViewLayer")
    ViewMgr:HideUI(self.pMainView.name)
    self.PartList.numItems = 0
    self.holelist.numItems=0
end

--点击技能槽位 打开全部技能
function Battle_RefitUI:SkillReplace(content)
    --获取当前技能槽位id
    local curskillhole = self.skillUIList:GetChildIndex(content.data)
    ViewMgr:ShowUI("Battle","AllSkill","InfoLayer",curskillhole)
end

-- 设置选中技能
function Battle_RefitUI:SelectSkill(skilhole, nSelectSkillId)
    if not BattleLogic.SelectSkillList[skilhole] then
        return
    end
    if BattleLogic.SelectSkillList[skilhole] == nSelectSkillId then
        return
    end
    BattleLogic:SetSelectSkillList(skilhole, nSelectSkillId)
    self:RefrshSkillUI()
end

function Battle_RefitUI:CreateHoleEffect()
    self.EffectInstID=LuaExtend.CreatHoleEffect(self.holeid,self.EffectInstID)
end

function Battle_RefitUI:RefitOver()
    local conditionjude={false,false,}
    local nMaxHoleCount = LuaExtend.GetPlayerHoleCount()
    for nIdx = 1, nMaxHoleCount - 1 do 
        local holepartid = LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,nIdx)
        if holepartid>0 then
            if RefitLogic:GetItemType(holepartid)==1 then
                conditionjude[1]=true
            elseif RefitLogic:GetItemType(holepartid)==2 then
                conditionjude[2]=true
            end
        end
    end
    if not conditionjude[1] then
        self.log.text="缺少武器配件"
        return 
    elseif not conditionjude[2]then
        self.log.text="缺少移动配件"
        return 
    end
    self.log.text=""
    self:Close()            
    EventMgr:DisplayEvent(GacEvent.RefitOver)
end

function Battle_RefitUI:RefrshSkillUI()
    for i, nSelectId in ipairs(BattleLogic.SelectSkillList) do 
        local item=self.skillUIList:GetChildAt(i-1)
        if nSelectId > 0 then 
            item:GetChild("click").visible=false
            item:GetChild("skillicon").icon=FairyGUI.UIPackage.GetItemURL("Common", Skill_C[BattleLogic.SelectSkillList[i]].strSkillIcon) 
        else
            item:GetChild("skillicon").icon=nil
            item:GetChild("click").visible=true
        end
    end
end

function Battle_RefitUI:OnDispose()
    EventMgr:RemoveEvent(GacEvent.SelectSkillOver,self.RefrshSkillUI,self)
end