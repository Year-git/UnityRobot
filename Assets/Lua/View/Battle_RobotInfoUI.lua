local ViewMgr=RequireSingleton("ViewMgr")
local Battle_RobotInfoUI=RequireClass("Battle_RobotInfoUI")
local FairyGUI = FairyGUI
local UnityEngine = UnityEngine 
local Part_C=RequireConfig("Part_C")
local String_C=RequireConfig("String_C")
local Skill_C=RequireConfig("Skill_C")
local RefitLogic=RequireSingleton("RefitLogic")

function Battle_RobotInfoUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.table={"icon-shuxing-chengzai","icon-shuxing-gongji","icon-shuxing-sudu","icon-shuxing-xueliang","icon-shuxing-zhuangxiang"}
    self.table1={"容量","攻击","速度","血量","巡航"}
    self.partUI=pMainView:GetChild("partUI")
    self.robotUI=pMainView:GetChild("robotUI")
    self.BigClose=pMainView:GetChild("Bigclose")
    self.BigClose.onClick:Add(self.CloseUI,self)
    self.Close=pMainView:GetChild("close")
    self.Close.onClick:Add(self.CloseUI,self)
    --装备UI
    self.PartIcon_img=self.partUI:GetChild("icon")
    self.PartName_txt=self.partUI:GetChild("name")
    self.PartLvl_txt    =self.partUI:GetChild("lvl")
    self.PartList_list  =self.partUI:GetChild("prolist")
    self.PartList_list.itemRenderer     = FairyGUI.ListItemRenderer(self.PartListItemRender,self)
    self.SkillUI=self.partUI:GetChild("skillui")
    self.SkillIcon_img  =self.SkillUI:GetChild("icon")
    self.SkillName_txt  =self.SkillUI:GetChild("name")
    self.SkillDec_txt   =self.SkillUI:GetChild("dec")
    --车体UI
    self.RobotCap=self.robotUI:GetChild("n48")
    self.RobotProList=self.robotUI:GetChild("infolist")
    self.RobotProList.itemRenderer     = FairyGUI.ListItemRenderer(self.RobotProListItemRender,self)
    self.hp_txt=self.robotUI:GetChild("hp")
    self.attack_txt=self.robotUI:GetChild("attack")
    self.cap_txt=self.robotUI:GetChild("cap")
    self.speed_txt=self.robotUI:GetChild("speed")
    self.cruise_txt=self.robotUI:GetChild("cruise")
    --动效
    self.transition=pMainView:GetTransition("UIShow")
end

function Battle_RobotInfoUI:OnShow(nSelectedPartID)
    self.transition:Play()
    self.PartIcon_img.icon=FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon(nSelectedPartID))
    self.PartName_txt.text=String_C[nSelectedPartID].str
    self.PartLvl_txt:SetVar("lvl","100"):FlushVars()
    if not Skill_C[nSelectedPartID] then
        self.SkillIcon_img.icon=nil
        self.SkillName_txt.text="无"
        self.SkillDec_txt.text="无"
    else
        self.SkillIcon_img.icon=FairyGUI.UIPackage.GetItemURL("Common", Skill_C[nSelectedPartID].strSkillIcon ) 
        self.SkillName_txt.text=""
        self.SkillDec_txt.text=""
    end
    --LuaExtend.PlayerBattleMap(self.RobotCap.asGraph.shape)
    self.PartList_list.numItems=5
    self.RobotProList.numItems=9

    self.hp_txt.text="生命"
    self.attack_txt.text="攻击"
    self.cap_txt.text="航速"
    self.speed_txt.text="速度"
    self.cruise_txt.text="容量"

end

function Battle_RobotInfoUI:PartListItemRender(nIndex,gItem)
    local item=gItem.asCom
    item:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", self.table[nIndex+1]) 
    item:GetChild("name").text=self.table1[nIndex+1]
    item:GetChild("num").text="999999"
end

function Battle_RobotInfoUI:RobotProListItemRender(nIndex,gItem)
    local item=gItem.asCom
    if (nIndex+1)%3==0 then
        item:GetChild("line").visible=false
    else
        item:GetChild("line").visible=true
    end
    if nIndex>4 then
        nIndex=1
    end
    local info=item:GetChild("info")
    info:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", self.table[nIndex+1]) 
    info:GetChild("name").text=self.table1[nIndex+1]
    info:GetChild("num").text="999999"
end

function Battle_RobotInfoUI:CloseUI()
    self:Hide()
end
