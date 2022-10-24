local Checkpoint_CheckUI = RequireClass("Checkpoint_CheckUI")
local UnityEngine = UnityEngine
local FairyGUI = FairyGUI
local CheckpointLogic = RequireSingleton("CheckpointLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local Level_C = RequireConfig("Level_C")
local Part_C = RequireConfig("Part_C")

function Checkpoint_CheckUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self.topBox = pMainView:GetChild("topBox")
    self.lefttopBox = pMainView:GetChild("lefttopBox")
    self.copyView = pMainView:GetChild("copyView")
    self.btn_starBox = pMainView:GetChild("btn_starBox")
    self.starIcon = pMainView:GetChild("starIcon")
    self.starNum = pMainView:GetChild("starNum")
    self.btn_out = self.lefttopBox:GetChild("btn_out")

    self.text_jinbi = self.topBox:GetChild("text_jinbi")
    self.text_jintiao = self.topBox:GetChild("text_jintiao")

    self.CheckTable = {}
    self.StarAntion = {}
    self.CheckLineTbel = {}    

    self.btn_out.onClick:Add( function() self:Close() end )
    self.btn_starBox.onClick:Add(function()  end )
end

function Checkpoint_CheckUI:OnShow( nChenckId )
    CheckpointLogic:GetCheckNum(nChenckId)
    CheckpointLogic:GetThemeStarNum(nChenckId)
    CheckpointLogic.themeId = nChenckId
    self.checkedId = CS_GetPlayerPassGameLevel()  
    for i = 1, 10 do 
        self.CheckTable[i] = self.copyView:GetChild("check" .. i)
        self.CheckLineTbel[i] = {}
        self.checkIndex = i 
        if i>1 then 
            for k=1,6 do         
                self.CheckLineTbel[i][k] = {}                   
                local checkLine = self.copyView:GetChild("check"..i.."_"..k)
                if checkLine then 
                    self.CheckLineTbel[i][k] = checkLine
                end 
            end
        end         
        self:SetCheckInfo(i)
    end

    self.lefttopBox.title = "主题名称"
    self.starNum.text = "" .. CheckpointLogic.MainStarNum[nChenckId]
    self.btn_starBox.icon = FairyGUI.UIPackage.GetItemURL("Checkpoint", "btn-jiangli")
    self.starIcon.icon = FairyGUI.UIPackage.GetItemURL("Checkpoint", "icon-guanka-xingxing")
    for i=1,#self.StarAntion do
        local _transition=self.StarAntion[i]
        _transition:Play()
    end
end

function Checkpoint_CheckUI:SetCheckInfo(nCheckIndex)
    local _checkBtn = self.CheckTable[nCheckIndex]
    local _checkId = CheckpointLogic.CheckInfo[nCheckIndex]
    local btnIndex = _checkId % 1000
    local _checkInfo = Level_C[_checkId]    

    local _checkLineImg = ""
    local t_move = _checkBtn:GetTransition("t_move")
    local _btnCotro = _checkBtn:GetController("ShowContro")
    _btnCotro.selectedIndex = 2
    self.StarAntion[nCheckIndex]=t_move

    local _rewardAdopt = _checkBtn:GetChild("rewardAdopt")
    local _rewardSelection = _checkBtn:GetChild("rewardSelection")
    local _rewardUnlocked = _checkBtn:GetChild("rewardUnlocked")
    local _rewardIcon = _checkBtn:GetChild("rewardIcon")
    local _checkName = _checkBtn:GetChild("checkName")
    local _precheckId = _checkInfo.preLevelId
    local _type = _checkInfo.type
    if _type == 2 then
        _rewardAdopt.visible = true
        _rewardSelection.visible = true
        _rewardUnlocked.visible = true
        _rewardIcon.visible = true
        local rewardImage = Part_C[_checkInfo.RewardIcon].strIcon
        _rewardIcon.url = FairyGUI.UIPackage.GetItemURL("Common", rewardImage)
    else
        _rewardAdopt.visible = false
        _rewardSelection.visible = false
        _rewardUnlocked.visible = false
        _rewardIcon.visible = false
        if _checkName then
            _checkName:SetXY(_rewardIcon.x, _checkName.y)
        end
    end
    local _checkMAX = CheckpointLogic:GetCheckMax(CheckpointLogic.themeId) --主题最大关卡
    local _checkMaxInfo = Level_C[_checkMAX]
    if self.checkedId <= 0 then 
        --  从未打过
        if nCheckIndex == 1 then
            _btnCotro.selectedIndex = 0
            _checkBtn.alpha = 1
        else
            _btnCotro.selectedIndex = 2
            _checkBtn.alpha = 0.7
        end
    else
        if self.checkedId >_precheckId then 
            _btnCotro.selectedIndex = 1
            self:LoadCheckStar(btnIndex, 3)
            if self.checkedId >= _checkMaxInfo.preLevelId then    -- 全部通关
                if _checkMAX == nCheckIndex then
                    _btnCotro.selectedIndex = 0
                    _checkBtn.alpha = 1
                    self:LoadCheckStar(btnIndex, 3)
                end                
            end
        else
            if self.checkedId == _precheckId then
                _btnCotro.selectedIndex = 0
            else
                _checkBtn.alpha=0.7 
                _btnCotro.selectedIndex = 2
            end
        end 
    end
    _checkLineImg =CheckpointLogic.checkLineImg[_btnCotro.selectedIndex]
    if nCheckIndex > 1 then 
        self:ChangeCheckLineImg(nCheckIndex,_checkLineImg)
    end
    _checkBtn.onClick:Add( function()self:OpenReadView(_checkId, _precheckId) end)            
end

function Checkpoint_CheckUI:OnHide(...)
end

function Checkpoint_CheckUI:Btnclick_OpenView(sBagName, sViewName)
    ViewMgr:ShowUI(sBagName, sViewName, "ViewLayer")
end

function Checkpoint_CheckUI:OpenReadView(nIndex, nPrecheckId)
    -- if nPrecheckId == 0 then
        ViewMgr:ShowUI("Checkpoint", "Ready", "ViewLayer", nIndex)
    -- else
    --     if nPrecheckId > self.checkedId then
    --         -- 文字提示未通过
    --     else
    --         ViewMgr:ShowUI("Checkpoint", "Ready", "ViewLayer", nIndex)
    --     end
    -- end
end

--加载关卡星星数
function Checkpoint_CheckUI:LoadCheckStar(nIndex, nStarNum)
    local _btn = self.CheckTable[nIndex]
    for i = 1, 3 do
        local _star = _btn:GetChild("star" .. i)
        if nStarNum >= i then
            _star.url = FairyGUI.UIPackage.GetItemURL("Checkpoint", CheckpointLogic.sratImage[1])
        end
    end
end

function Checkpoint_CheckUI:ChangeCheckLineImg ( nCheckIndex,sImgPath )
    local _nLength=#self.CheckLineTbel[nCheckIndex]
    for i=1,_nLength do
        local _gCheckLine=self.CheckLineTbel[nCheckIndex][i]
        _gCheckLine.url = FairyGUI.UIPackage.GetItemURL("Checkpoint",sImgPath)
    end
end