local ViewMgr=RequireSingleton("ViewMgr")
local EventMgr=RequireSingleton("EventMgr")
local Battle_AllSkillUI=RequireClass("Battle_AllSkillUI")
local FairyGUI = FairyGUI
local UnityEngine = UnityEngine 
local Part_C=RequireConfig("Part_C")
local Skill_C=RequireConfig("Skill_C")
local BattleLogic = RequireSingleton("BattleLogic")

function Battle_AllSkillUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.closebig=pMainView:GetChild("closebig")
    self.list=pMainView:GetChild("list").asList
    self.list.onClickItem:Add(self.SkillReplace,self)
    self.btn_out=pMainView:GetChild("btn_out")
    self.closebig.onClick:Add(function() self:Close()  end)
    self.btn_out.onClick:Add(function() self:Close()  end)
end

function Battle_AllSkillUI:OnShow(holeid)
    self.holeid=holeid
    self.tSkillList = {}
    local tMapping = {}
    local nMaxHoleCount = LuaExtend.GetPlayerHoleCount()
    for nIdx = 1, nMaxHoleCount - 1 do 
        --判断该槽位有没有配件
        local holepartid = LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,nIdx)
        if holepartid>0 then
            local skillID = Part_C[holepartid].skillID
            if (skillID > 0) and (tMapping[skillID] == nil) then
                table.insert(self.tSkillList, skillID)
                tMapping[skillID] = true
            end
        end
    end
    for index,skillID in pairs(self.tSkillList)do
        local item=self.list:AddItemFromPool().asCom
        item:GetChild("n4").icon   =FairyGUI.UIPackage.GetItemURL("Common", Skill_C[skillID].strSkillIcon) 
    end
end

function Battle_AllSkillUI:SkillReplace(content)
    local index=self.list:GetChildIndex(content.data)+1
    local skilhole=self.holeid+1
    local nSelectSkillId=self.tSkillList[index]
    if not BattleLogic.SelectSkillList[skilhole] then
        self:Close()
        return
    end
    if BattleLogic.SelectSkillList[skilhole] == nSelectSkillId then
        self:Close()
        return
    end
    BattleLogic:SetSelectSkillList(skilhole, nSelectSkillId)
    EventMgr:DisplayEvent(GacEvent.SelectSkillOver)
    self:Close()
end

function Battle_AllSkillUI:OnClose()
    self.list.numItems=0
    self:Hide()
end