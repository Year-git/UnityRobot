local FairyGUI = FairyGUI
local Battle_BloodViewUI = RequireClass("Battle_BloodViewUI")
local BloodMgr = RequireSingleton("BloodMgr")
local EventMgr = RequireSingleton("EventMgr")
local BattleLogic = RequireSingleton("BattleLogic") 
local Npc_C = RequireConfig("Npc_C")

function Battle_BloodViewUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    self.attackedImg = pMainView:GetChild("attackedImg")
    self.bossHp = pMainView:GetChild("bossHp")
    self.HaemalBoss=self.bossHp:GetChild("haemal")
    self.bossIcon=self.bossHp:GetChild("bossIcon")
    self.bossLv=self.bossHp:GetChild("bossLv")
    self.bossHpNum=self.bossHp:GetChild("bossHpNum")

    self.haemalredBoss=self.HaemalBoss:GetChild("red")
    self.bossHpBg=self.HaemalBoss:GetChild("bossHpBg")
    self.bossHpColor=self.HaemalBoss:GetChild("bar")
    self.HeadFreeUseList =
        FreeUseList:Generate(
        function()
            return self:GenerateHeadInfo()
        end
    )
    EventMgr:RegistEvent(GacEvent.NpcLuaUpdate, self.NpcLuaUpdate, self)
end


function Battle_BloodViewUI:OnShow()
    self.tHeadInfo = {}
    self.tTagetHp = {}
    -- 目标透明度
    --self.bossHp.visible =false
    self.nTargetAlpha = 0
    self.attackedImg.alpha = 0
    self.npcKilled = {}
    BattleLogic.playSkillNum = 0
    self.bossColorIndex = 0
    self.lastHpvalue = 0
    self.bossHp.visible=BattleLogic:GetIsBossCheck( BattleLogic.BattCheckId )
    self.bossID  = 0
end


function Battle_BloodViewUI:OnUpdate(deltaTime)
    local nNowAlpha = self.attackedImg.alpha
    if self.nTargetAlpha > nNowAlpha then
        local targer = nNowAlpha + deltaTime * 3
        if targer > 1 then
            targer = 1
        end
        self.attackedImg.alpha = targer
    elseif self.nTargetAlpha < nNowAlpha then
        local targer = nNowAlpha - deltaTime * 1.5
        if targer < 0 then
            targer = 0
        end
        self.attackedImg.alpha = targer
    else
        self.nTargetAlpha = 0
    end
end

function Battle_BloodViewUI:GenerateHeadInfo()
    local pHeadInfoView = FairyGUI.UIPackage.CreateObject("Battle", "HeadInfo").asCom
    self.pMainView:AddChild(pHeadInfoView)
    return pHeadInfoView
end

function Battle_BloodViewUI.Recover(pHeadInfoView)
    pHeadInfoView.visible = false
end

function Battle_BloodViewUI:OnHide()
    self.HeadFreeUseList:RecoverAll(self.Recover)
    self.tHeadInfo = {}
    self.npcKilled = {}
    BattleLogic.playSkillNum = 0
end

function Battle_BloodViewUI:OnDispose()
    self.HeadFreeUseList = nil
    EventMgr:RemoveEvent(GacEvent.NpcLuaUpdate, self.NpcLuaUpdate, self)
end

function Battle_BloodViewUI:NpcLuaUpdate(nNpcID, sName, nX, nY, nZ, nNowHp, nMaxHp,nNpccfgId, bVisible) 
    if not self:IsVisible(true) then
        return;
    end
    local _monsterType=Npc_C[nNpccfgId].monsterType
    if _monsterType == 2 then
        self:SetBossHpInfo(nNpcID,nNowHp,nMaxHp,nNpccfgId,bVisible)  -- boss关卡
    else
        self:NpcInfo(nNpcID, sName, nX, nY, nZ, nNowHp, nMaxHp,nNpccfgId, bVisible)
    end 
end

function Battle_BloodViewUI:NpcInfo( nNpcID, sName, nX, nY, nZ, nNowHp, nMaxHp,nNpccfgId, bVisible )
    local pHeadInfoView = self.tHeadInfo[nNpcID]
    if not pHeadInfoView then
        pHeadInfoView = self.HeadFreeUseList:Assign()
        self.tHeadInfo[nNpcID] = pHeadInfoView
    end
    pHeadInfoView.visible = bVisible and nZ > 0
    if MyPlayer.playerInstId ~= nNpcID then
        if nNowHp == 0  and self.npcKilled[nNpcID]== nil  then
            BattleLogic.playSkillNum=BattleLogic.playSkillNum+ 1
            self.npcKilled[nNpcID] =  BattleLogic.playSkillNum
            EventMgr:DisplayEvent(GacEvent.Playskill)
        end 
    end
    -- 判断血量
    if pHeadInfoView.visible then
        pHeadInfoView.visible = nNowHp > 0
    end
        
    if not pHeadInfoView.visible then
        return
    end

    local pt = self.pMainView:GlobalToLocal(Vector2(nX, nY))
    local nPosX, nPosY = pt.x, self.pMainView.height - pt.y
    
    -- 设置血条
    local Haemal = pHeadInfoView:GetChild("haemal")
    local bar=Haemal:GetChild("bar")
    local HaemalRed = Haemal:GetChild("red")
    local hpImage =""
    if MyPlayer.playerInstId ~= nNpcID then
        pHeadInfoView:SetXY(nPosX, nPosY)
        hpImage = "ui-xurtiao-diren"
        local _monsterType=Npc_C[nNpccfgId].monsterType
        if  _monsterType  then
            if _monsterType==2 then 
                hpImage = "ui-xurtiao-boss"
            end 
        end      
    else
        pHeadInfoView:SetXY(self.pMainView.width / 2, self.pMainView.height / 2.3)
        hpImage = "ui-xurtiao-ziji"
        if self.tTagetHp[nNpcID] and (self.tTagetHp[nNpcID] > nNowHp) then
            self.nTargetAlpha = 1
        end
    
    end
    bar.url=FairyGUI.UIPackage.GetItemURL("Battle",hpImage)
    if nMaxHp ~= Haemal.max then
        Haemal.max = nMaxHp
        HaemalRed.max = nMaxHp
    end
    if self.tTagetHp[nNpcID] ~= nNowHp then
        HaemalRed:TweenValue(nNowHp, 1)
        Haemal.value = nNowHp
        self.tTagetHp[nNpcID] = nNowHp
    end
    -- 设置名字
    local Name = pHeadInfoView:GetChild("name")
    Name.text = sName
end

function Battle_BloodViewUI:SetBossHpInfo( nNpcID,nNowHp,nMaxHp,nNpccfgId,bVisible )
    self.bossHp.visible = bVisible
    if not bVisible then
        return
    end
    if MyPlayer.playerInstId ~= nNpcID then 
        if nNowHp <= 0 then 
            self.HaemalBoss:TweenValue(0,0.2)
            self.haemalredBoss:TweenValue(0,0.3)            
            self:SetBossHp(0,0)
            self.bossHpBg.url = FairyGUI.UIPackage.GetItemURL("Battle","")
        else
            local _hpnum = 0 
            local _hp= 0
            local _RemainHp= 0    
            if self.bossID == 0 then 
                self:SetBossInfo(nNpccfgId)          
                self:SetBossHp(_hpnum,nNowHp)
            end 
            _hpnum=math.floor(nNowHp / self.bossHpAverage)
            _hp=_hpnum*self.bossHpAverage
            _RemainHp= nNowHp-_hp
            
            self.bossHpNum.text ="X".._hpnum
            
                -- -------boss 血量信息
            _hp=_hpnum*self.bossHpAverage
            _RemainHp= nNowHp-_hp           
            if self.bossHpNowColor ~=_hpnum then                 
                self.lastHpNum =_hpnum 
                self.bossColorIndex = self.bossColorIndex +1                
                self:SetBossHp(_hpnum,nNowHp)
            else
                if self.lastHpvalue~= _RemainHp  then  
                    self.lastHpvalue= _RemainHp 
                end                
                self:SetBossHp(_hpnum,nNowHp)
            end            
            if self.haemalredBoss.value <_RemainHp then 
                self.haemalredBoss.value =self.bossHpAverage
            end            
            self.haemalredBoss.max = self.bossHpAverage
            self.HaemalBoss.max = self.bossHpAverage
            self.haemalredBoss:TweenValue(_RemainHp,0.55)
            self.HaemalBoss.value = _RemainHp
        end        
    end 
end

function Battle_BloodViewUI:SetBossHp( nHpNum ,nNowHp )
    self.bossHpNowColor = nHpNum
    local hpImage = ""
    local nextHpImage = ""
    local _nNextIndex  =0  
    local _nIndex  = 0
    if self.bossHpMax ~= nHpNum  then
        _nIndex = nHpNum%self.bossHpMax
        _nNextIndex = (nHpNum-1)%self.bossHpMax
        hpImage=BattleLogic.BossHpColor[_nIndex]
        nextHpImage=BattleLogic.BossHpColor[_nNextIndex]
        if  nHpNum == 0 then 
            nextHpImage =""
            hpImage="ui-boss-xuetiao-hong"
        end
        if  nHpNum == 1 then 
            nextHpImage="ui-boss-xuetiao-hong"
        end 
    else
       nextHpImage=BattleLogic.BossHpColor[4]
    end 
    local bossHp =self.bossHpAverage * self.bossHpMax 
    if bossHp <=nNowHp and nHpNum ==0 then
        nextHpImage=BattleLogic.BossHpColor[0]
    end 
    self.bossHpColor.url =FairyGUI.UIPackage.GetItemURL("Battle",hpImage)
    self.bossHpBg.url = FairyGUI.UIPackage.GetItemURL("Battle",nextHpImage)
end

function Battle_BloodViewUI:SetBossInfo( nbossId )
    self.bossID = nbossId
    local bossInfo=Npc_C[self.bossID]
    local bossHp=bossInfo.NPCAttribute[1][2]
    if bossInfo.hpNumber == 0 then 
        bossInfo.hpNumber = 1
    end 
    self.bossHpNum.text = "X"..bossInfo.hpNumber
    self.bossHpAverage = bossHp /bossInfo.hpNumber  -- boss 一管血的值
    self.bossHpMax= bossInfo.hpNumber
    self.bossHpNowColor = bossInfo.hpNumber
    self.bossLv.text = ""..bossInfo.lv  
    local bossIcon = bossInfo.strIcon
    self.bossIcon.url = FairyGUI.UIPackage.GetItemURL("Battle",bossIcon)
end