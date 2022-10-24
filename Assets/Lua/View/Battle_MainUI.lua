local Battle_MainUI = RequireClass("Battle_MainUI")
local ReadLogic = RequireSingleton("ReadLogic")
local FairyGUI = FairyGUI
local ViewMgr = RequireSingleton("ViewMgr")
local BattleLogic = RequireSingleton("BattleLogic")
local Scene_C = RequireConfig("Scene_C")
local UnityEngine = UnityEngine
local NetworkMgr = RequireSingleton("NetworkMgr")
local EventMgr = RequireSingleton("EventMgr")
local Skill_C = RequireConfig("Skill_C")
local Part_C=RequireConfig("Part_C")

function Battle_MainUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    self.cameraMove = pMainView:GetChild("cameraMove")
    self.touchArea = pMainView:GetChild("JoystickTouchArea")
    self.btn_move = pMainView:GetChild("Joystick")
    self.thumb = self.btn_move:GetChild("thumb")
    self._center = pMainView:GetChild("JoystickCenter")
    self._InitX = self._center.x
    self._InitY = self._center.y
    self.btn_move.changeStateOnClick = false
    self.leftbox=pMainView:GetChild("leftbox")
    self.leftboxBoss=pMainView:GetChild("leftboxBoss")
    self.btn_quit=self.leftbox:GetChild("btn_out")
    --  顶部信息
    self.mapIcon = pMainView:GetChild("mapIcon")
    self.btn_reset = pMainView:GetChild("btn_reset")
    self.btn_skill1 = pMainView:GetChild("btn_skill1")
    self.btn_skill2 = pMainView:GetChild("btn_skill2")
    self.btn_attack = pMainView:GetChild("btn_attack")
    
    self.skillShow = pMainView:GetChild("skillShow")
    self.skill_NUm = self.skillShow:GetChild("skill_NUm")
    self.skill_text = self.skillShow:GetChild("skill_text")
    self.killMove=pMainView:GetTransition("t0")

    
    self.text_MatchTimebg = pMainView:GetChild("MatchTimebg")
    self.text_MatchTime = pMainView:GetChild("MatchTime")

    self.resetTop=self.btn_reset:GetChild("top")
    self.skill1Top=self.btn_skill1:GetChild("top")
    self.skill2Top=self.btn_skill2:GetChild("top")
    self.attackTop=self.btn_attack:GetChild("top")
    self.btn_refit=pMainView:GetChild("refit")

    self.btn_refit.onClick:Add(function() LuaExtend.PlayerAssembleStart()end)
    self.touchArea.onTouchBegin:Add(self.TouchBegin, self)
    self.touchArea.onTouchMove:Add(self.TouchMove, self)
    self.touchArea.onTouchEnd:Add(self.TouchEnd, self)

    -- 相机控制
    self.cameraMove.onTouchBegin:Add(self.MainViewTouchBegin, self)
    self.cameraMove.onTouchMove:Add(self.MainViewTouchMove, self)
    self.cameraMove.onTouchEnd:Add(self.MainViewTouchEnd, self)
    self.btn_attack.onClick:Add(function() self:Btnclick_useSkill("btn_attack") end)
    self.btn_skill1.onClick:Add(function() self:Btnclick_useSkill("btn_skill1") end)
    self.btn_skill2.onClick:Add(function() self:Btnclick_useSkill("btn_skill2") end)
    self.btn_reset.onClick:Add( function() self:Btnclick_useSkill("btn_reset") end)
    self.btn_quit.onClick:Add(self.BtnClick_Quit,self)

    -- 测试相机
    self.btn_video = pMainView:GetChild("btn_video")
    self.btn_video.onClick:Add(self.Btnclick_ChangeCamera, self)
    EventMgr:RegistEvent(GacEvent.UICreate, function()ViewMgr:ShowUI("Battle","Refit","ViewLayer")end, self)
    EventMgr:RegistEvent(GacEvent.PlayerAssembleStart, self.OpenRefitUI,self)
    EventMgr:RegistEvent(GacEvent.GameLevelEnd,self.GameOver,self)
    EventMgr:RegistEvent(GacEvent.MapLevelGameStart,self.MapLevelGameStart,self)
    EventMgr:RegistEvent(GacEvent.AssembleEnd,self.ChangeSkillIcon,self)
    EventMgr:RegistEvent(GacEvent.Playskill,self.PlayerSkill,self)    
end

function Battle_MainUI:RefrshSkillUI(  )
    
end

function Battle_MainUI:OnShow()
    ViewMgr:ShowUI("Battle", "BloodView", "ScreenLayer")
    self.keyInfo = {["W"] = 0,["A"] = 0,["S"] = 0,["D"] = 0}
    self.skillCD = {["btn_attack"]=0,["btn_skill2"]=0,["btn_skill1"]=0,["btn_reset"]=0}
    self.mapid = CS_GetMapId()
    self.sceneTime = Scene_C[self.mapid].gameTime
    self.sceneTime = self.sceneTime / 1000
    self.touchId = -1
    self.MaintouchId = -1
    self.leftbox.title = ""
    -- 初始化相机type
    self.cameraId = 0
    -- 初始化可移动半径范围 
    self.radius = self._center.width / 2
    self.btn_quit.icon = FairyGUI.UIPackage.GetItemURL("Common", "btn-fanhui")
    self.skillTable = { [1]="btn_skill2",[2]="btn_skill1",[3]="btn_attack",[4]="btn_reset",}
    self.btn_reset.title = ""
    self.btn_skill1.title = ""
    self.btn_skill2.title = ""
    self.btn_attack.title = ""
    self._MainLastStage = { x = 0, y = 0}
    self.nSteerAngle = nil
    self.skillShow.visible =false
    self.killShowTime = 3   
    
    local _isBoss=BattleLogic:GetIsBossCheck( BattleLogic.BattCheckId )
    self.leftbox.visible = not _isBoss
    self.leftboxBoss .visible = _isBoss
end

function Battle_MainUI:OnHide(...)
    ViewMgr:HideUI("Battle_BloodViewUI")--隐藏血条
end

function Battle_MainUI:MapLevelGameStart()
    -- 开始战斗默认选择技能操作
    local nFlagIndex = 1
    local tMapping = {}
    local nMaxHoleCount = LuaExtend.GetPlayerHoleCount()
    for nIdx = 1, nMaxHoleCount - 1 do 
        --判断该槽位有没有配件
        local holepartid = LuaExtend.GetNpcPartCfgIdByHoleIdx(MyPlayer.playerInstId,nIdx)
        if holepartid>0 then
            local skillID = Part_C[holepartid].skillID
            if (skillID > 0) and (tMapping[skillID] == nil) then
                tMapping[skillID] = true
                BattleLogic:SetSelectSkillList(nFlagIndex,skillID)
                nFlagIndex = nFlagIndex + 1
            end
            --临时
            BattleLogic:SetSelectPartList(holepartid)
        end
    end
    self:ChangeSkillIcon( )
    ViewMgr:HideUI("Common_LoadViewUI")
end

function Battle_MainUI:PlayerSkill(  )    
   self.skillShow.visible =true
    self.skill_NUm.text = ""
    if BattleLogic.playSkillNum>1 then 
        self.skill_NUm.text = "" ..BattleLogic.playSkillNum
        self.skill_text.text = "连杀" 
        else            
        self.skill_text.text = "击杀" 
    end
    self.killMove:Play()
end

function Battle_MainUI:ChangeSkillIcon(  )
    BattleLogic:SetSkillIconPos()
    self.skillIndex = 0
    for k,v in pairs(BattleLogic.SelectSkillList) do
        local _btnName  =  self.skillTable[k]
        self[_btnName].visible = false
        if  v > 0 then
            self.skillIndex =  self.skillIndex + 1 
            local _uiSkillIonPos=BattleLogic.SkillIconPos[self.skillIndex]
            local skillImage = ""
            local skillCd = 0
            skillImage=Skill_C[v].strSkillIcon ..""
            skillCd =Skill_C[v].skillCd / 1000
            BattleLogic.skillTag[_btnName].icon =skillImage
            BattleLogic.skillTag[_btnName].cd = skillCd
            BattleLogic.skillTag[_btnName].skillId =v
            self[_btnName].icon = FairyGUI.UIPackage.GetItemURL("Common",skillImage)            
            self[_btnName].visible = true
            self[_btnName].x=_uiSkillIonPos
        else
            self[_btnName].icon = ""
            BattleLogic.skillTag[_btnName].skillId = -1
        end 
    end 
end

-- 改变相机视角
function Battle_MainUI:Btnclick_ChangeCamera(  )
    -- if  self.cameraId >0 then
    --     self.cameraId = 0
    -- else
    --     self.cameraId = 1
    -- end
    -- MyPlayer.cameraType = self.cameraId
    MyPlayer.ChangeCameraHight()
    --ViewMgr:ShowUI("Set","Main", "ViewLayer")
end

function Battle_MainUI:BtnClick_Quit(  )
    CS_SetGamePause(false)
    ViewMgr:ShowUI("Battle","Suspend","ViewLayer")
end


function Battle_MainUI:Btnclick_useSkill(sBtnName)
    local skillInfo=BattleLogic.skillTag[sBtnName]
    if skillInfo.skillId >=0 then 
        BattleLogic:Send(MyPlayer.playerInstId, '{\r\n  "' .. "Skill" .. '": ' .. skillInfo.skillId .. "\r\n}")
        if self.skillCD[sBtnName] <= 0 then
            self.skillCD[sBtnName] = skillInfo.cd
        end
    end
end

function Battle_MainUI:BtnClick_OpenPanel()
    self:Close()
end

function Battle_MainUI:OnUpdate(deltaTime)
    local Player = MyPlayer.player
    if not Player then
        return
    end
    if  self.skillShow.visible and self.skillShow.x>= 774 then
        self.killShowTime =self.killShowTime - deltaTime * 3
        if self.killShowTime<= 0 then 
            self.killShowTime = 3
            self.skillShow.visible =false
        end 
    end 
    if BattleLogic.gameBeginTime ~= 0 then
        local time = self.sceneTime - (CS_GTime.ServerSeconds - BattleLogic.gameBeginTime)
        time = BattleLogic:TimeFormat(time)
        --self.text_testSpeed.text = "速度:"..MyPlayer.GetPlayerSpeed()
        self.text_MatchTime.text = ""..time
        self.text_MatchTimebg.text = ""..time
    end   
    for key, val in pairs(self.skillCD) do
        local skillInfo =BattleLogic.skillTag[key]
        if val >0 then 
            self[key]:GetChild("top").visible =true
            val = val - CS_GTime.DeltaTime
            self[key].title =tostring(math.ceil(val))
            self[key].touchable= false
            self.skillCD[key] = val
            self[key]:GetChild("top").fillAmount=(skillInfo.cd-val)/skillInfo.cd
        else
            val =0                
            self[key].touchable= true
            self.skillCD[key] = val
            self[key].title = ""
            self[key]:GetChild("top").fillAmount=1
            self[key]:GetChild("top").visible =false
        end             
    end

    local movePos = Vector3(self.btn_move.x, self.btn_move.y, 0)
    local centerPos = Vector3(self._center.x, self._center.y, 0)
    local dis = movePos - centerPos
    local angle = Vector3.Angle(dis, Vector3.up)
    local speech = dis.x + dis.y + dis.z
    local LorR = Vector3.Cross(dis.normalized, Vector3.up)
    if speech ~= 0 then
        if LorR.z > 0 then
            angle = 180 - angle
        elseif LorR.z < 0 then
            angle = angle + 180
        end
    else
        -- 松开摇杆
        angle = -1
    end

    if self.nSteerAngle ~= angle then
        self.nSteerAngle = angle
        BattleLogic:Send(MyPlayer.playerInstId, '{\r\n  "' .. "SteerAngle" .. '": ' .. angle .. "\r\n}")
    end

    -- local LorR = Vector3.Cross(dis.normalized, Vector3.up)
    -- local speech = dis.x + dis.y + dis.z
    
    -- for key,val in ipairs(self.keyInfo) do
    --     BattleLogic:Send(MyPlayer.playerInstId, '{\r\n  "' .. key .. '": ' .. val .. "\r\n}")
    -- end
    -- if LorR.z == 0 then
    --     for key, val in pairs(self.keyInfo) do
    --         if self.keyInfo[key] == 1 then
    --             BattleLogic:Send(MyPlayer.playerInstId, '{\r\n  "' .. key .. '": ' .. 0 .. "\r\n}")
    --             self.keyInfo[key] = 0
    --         end
    --     end
    --     return
    -- end
    -- local nowKeyInfo = {}
    -- nowKeyInfo["A"] = 0
    -- nowKeyInfo["D"] = 0
    -- nowKeyInfo["W"] = 0
    -- nowKeyInfo["S"] = 0

    -- if angle > 90 then
    --     nowKeyInfo["W"] = 1
    -- else
    --     nowKeyInfo["S"] = 1
    -- end

    -- -- if angle > 20 and angle < 160 then
    -- --     if LorR.z > 0 then
    -- --         nowKeyInfo["D"] = 1
    -- --     else
    -- --         nowKeyInfo["A"] = 1
    -- --     end
    -- -- end

    -- for key, val in pairs(nowKeyInfo) do
    --     if self.keyInfo[key] ~= val then
    --         BattleLogic:Send(MyPlayer.playerInstId, '{\r\n  "' .. key .. '": ' .. val .. "\r\n}")
    --     end
    -- end
    -- self.keyInfo = nowKeyInfo
end

function Battle_MainUI:MainViewTouchBegin(context)
    if self.MaintouchId ~= -1 then
        return
    end
    local evt = context.data
    self.MaintouchId = evt.touchId
    local pt = FairyGUI.GRoot.inst:GlobalToLocal(Vector2(evt.x, evt.y))
    local bx = pt.x
    local by = pt.y
    self._MainLastStage = { x = pt.x, y = pt.y }
    context:CaptureTouch()
end

function Battle_MainUI:MainViewTouchMove(context)
    if self.MaintouchId == -1 then
        return
    end
    local evt = context.data

    if self.MaintouchId ~= evt.touchId then
        return
    end
    local pt = FairyGUI.GRoot.inst:GlobalToLocal(Vector2(evt.x, evt.y))
    local bx = pt.x
    local by = pt.y
    local deltaX = self._MainLastStage.x - pt.x
    local deltaY = self._MainLastStage.y - pt.y
    self._MainLastStage = { x = pt.x, y = pt.y } 
    LocalFrameSynServer.LuaSendCameraTouch(deltaX, deltaY)
end

function Battle_MainUI:MainViewTouchEnd(context)
    local evt = context.data
    if self.MaintouchId == -1 then
        return
    end
    if self.MaintouchId ~= evt.touchId then
        return
    end
    self.MaintouchId = -1
end

function Battle_MainUI:TouchBegin(context)
    if self.touchId ~= -1 then
        return
    end
    local evt = context.data
    self.touchId = evt.touchId
    local pt = FairyGUI.GRoot.inst:GlobalToLocal(Vector2(evt.x, evt.y))
    local bx = pt.x
    local by = pt.y
    self.btn_move.selected = true
    if bx < 0 then
        bx = 0
    elseif bx > self.touchArea.width then
        bx = self.touchArea.width
    end
    if by > FairyGUI.GRoot.inst.height then
        by = FairyGUI.GRoot.inst.height
    elseif by < self.touchArea.y then
        by = self.touchArea.y
    end
    self._lastStageX = bx
    self._lastStageY = by
    self.startStageX = bx
    self.startStageY = by
    self._center.visible = true
    self._center:SetXY(bx, by)
    self.btn_move:SetXY(bx, by)
    local deltaX = bx - self._InitX
    local deltaY = by - self._InitY
    local degrees = math.atan2(deltaY, deltaX) * 180 / math.pi
    self.thumb.rotation = degrees + 90
    context:CaptureTouch()
end

function Battle_MainUI:TouchMove(context)
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
    local buttonX = self.btn_move.x + moveX
    local buttonY = self.btn_move.y + moveY

    local offsetX = buttonX - self.startStageX
    local offsetY = buttonY - self.startStageY
    local rad = math.atan2(offsetY, offsetX)
    local degree = rad * 180 / math.pi
    self.thumb.rotation = degree + 90
    local maxX = self.radius * math.cos(rad)
    local maxY = self.radius * math.sin(rad)
    if math.abs(offsetX) > math.abs(maxX) then
        offsetX = maxX
    end
    if math.abs(offsetY) > math.abs(maxY) then
        offsetY = maxY
    end
    buttonX = self.startStageX + offsetX
    buttonY = self.startStageY + offsetY
    if buttonX < 0 then
        buttonX = 0
    end
    if buttonY > FairyGUI.GRoot.inst.height then
        buttonY = FairyGUI.GRoot.inst.height
    end
    self.btn_move:SetXY(buttonX, buttonY)
end

function Battle_MainUI:TouchEnd(context)
    local evt = context.data
    if evt.touchId == -1 then
        return
    end
    if evt.touchId ~= self.touchId then
        return
    end
    self.touchId = -1
    self.thumb.rotation = self.thumb.rotation + 180
    self._center.visible = true
    self.btn_move.selected = false
    self.thumb.rotation = 0
    self._center.visible = true
    self._center:SetXY(self._InitX, self._InitY)
    self.btn_move:SetXY(self._InitX, self._InitY)
end

function Battle_MainUI:OpenRefitUI()
    self:Close()
    MyPlayer.RefitCameraShow(true)
end

function Battle_MainUI:GameOver(id,issucceed)
    ViewMgr:ShowUI("Battle","Clear","ViewLayer",id,issucceed)
end

function Battle_MainUI:OnDispose()
    --取消事件监听
    EventMgr:RemoveEvent(GacEvent.UICreate, function()ViewMgr:ShowUI("Battle","Refit","ViewLayer")end, self)
    EventMgr:RemoveEvent(GacEvent.PlayerAssembleStart, self.OpenRefitUI, self)
    EventMgr:RemoveEvent(GacEvent.GameLevelEnd,self.GameOver,self)
    EventMgr:RemoveEvent(GacEvent.MapLevelGameStart,self.MapLevelGameStart,self)
    EventMgr:RemoveEvent(GacEvent.AssembleEnd,self.ChangeSkillIcon,self)
    EventMgr:RemoveEvent(GacEvent.Playskill,self.PlayerSkill,self)
end