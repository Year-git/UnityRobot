local Main_MainUI = RequireClass("Main_MainUI")
local MainLogic = RequireSingleton("MainLogic")
local ReadLogic = RequireSingleton("ReadLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local LoginLogic = RequireSingleton("LoginLogic")
local FairyGUI = FairyGUI
local UnityEngine = UnityEngine 

function Main_MainUI:_constructor(pMainView)
    self._super._constructor(self, pMainView)
    self._holder = pMainView:GetChild("holder")
    self.holderRotate = pMainView:GetChild("holderRotate")
    -- 顶部按钮 文本
    
    self.topBox = pMainView:GetChild("topBox")
    --self.btn_add = self.topBox:GetChild("btn_add")
    self.text_jintiao = self.topBox:GetChild("text_jintiao")
    self.text_jinbi = self.topBox:GetChild("text_jinbi")
    --self.btn_setTop = self.topBox:GetChild("btn_setTop")
    -- 玩家信息
    
    self.playInfo = pMainView:GetChild("playInfo")
    self.text_playLv = self.playInfo:GetChild("playLv")
    self.text_playName = self.playInfo:GetChild("playName")
    self.text_RankName = self.playInfo:GetChild("RankName")
    self.icon_playHead = self.playInfo:GetChild("playHeadIcon")
    self.icon_playRank = self.playInfo:GetChild("rankIcon")

    -- 右上角信息
    self.text_dianliang = self.topBox:GetChild("dianliang_value")
    self.text_wifi = self.topBox:GetChild("wifi_value")

    -- 锦标赛按钮
    self.Btn_Match = pMainView:GetChild("Btn_Match")
	self.Btn_MatchRank = pMainView:GetChild("Btn_MatchRank")
	
    --左边按钮
    self.btn_action1 = pMainView:GetChild("btn_action1")
    self.btn_action2 = pMainView:GetChild("btn_action2")
    self.btn_action3 = pMainView:GetChild("btn_action3")
    self.btn_action1:GetChild("tishi").visible = true
    self.btn_action2:GetChild("tishi").visible = false
    self.btn_action3:GetChild("tishi").visible = false
    
    --底部按钮
    self.btn_setDown = pMainView:GetChild("btn_setDown")
    self.btn_rank = pMainView:GetChild("btn_rank")  --排行榜
    self.btn_showbooth = pMainView:GetChild("btn_showbooth")
    self.btn_more = pMainView:GetChild("btn_more")
    self.box1 = pMainView:GetChild("box1")
    self.box2 = pMainView:GetChild("box2")
    self.box3 = pMainView:GetChild("box3")
    self.box_null = pMainView:GetChild("btn_nullbox") 
	--聊天按钮 向上
	self.btn_chatUp = pMainView:GetChild("btn_chatUp")
    self.text_chatLine = pMainView:GetChild("text_chatLine")
    
	self.btn_setDown.onClick:Add(self.BtnClick_setDown,self)
	self.btn_rank.onClick:Add(self.BtnClick_rank,self)
	self.btn_showbooth.onClick:Add(self.BtnClick_showbooth,self)
	self.btn_more.onClick:Add(self.BtnClick_more,self)
	self.btn_action1.onClick:Add(self.BtnClick_action1,self)
	self.btn_action2.onClick:Add(self.BtnClick_action2,self)
	self.btn_action3.onClick:Add(self.BtnClick_action3,self)
	self.Btn_MatchRank.onClick:Add(self.BtnClick_MatchRank,self)
	self.Btn_Match.onClick:Add(self.BtnClick_Match,self)
    self.holderRotate.onTouchMove:Add(function () self:ModelRotate() end)
end

function Main_MainUI:ModelRotate(  )
    local modelPosx =UnityEngine.Input.GetAxis("Mouse X") * 12 * 0.5
    local modelPosy =UnityEngine.Input.GetAxis("Mouse Y") * 12 * 0.5
    --self.modelPosy =self.modelPosy- UnityEngine.Input.GetAxis("Mouse Y") * self.ySpeed * 0.02
    if self._model~= nil then 
        self._model.transform:Rotate(Vector3(0,-modelPosx,0))
    end 
end

function Main_MainUI:LoadModel(nNpcCfgId)
    self:RemoveModel()
    CS_Log("123456789")
    self.NpcInstId = CS_CreatNpcGameObj(nNpcCfgId, self.LoadModelDone, self)
end

function Main_MainUI:LoadModelDone(nNpcInstId)
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
    modelTransform.parent:GetComponent("Transform").localPosition = Vector3(213,-118,500)
    --self._model.transform:Rotate(Vector3(5,142,-8)) --
    self._model.transform.localEulerAngles=Vector3(5,142,-8) --localEulerAngles
    ViewMgr:HideUI("Common_LoadViewUI")
end

function Main_MainUI:RemoveModel()
    if self.NpcInstId ~= nil or self._model ~= nil then
        -- 先释放缓存的Npc的GameObj
        self._model = nil
        -- 删除Npc
        CS_RemoveNpcGameObj(self.NpcInstId)
        if self.wrapper then
            UnityEngine.Object.Destroy(self.wrapper.wrapTarget,0.01)
        end
    end
end

function Main_MainUI:OnShow()
    self.text_jintiao.text = "1535"  --金条数量
    self.text_jinbi.text = "65262" -- 金币数量
    self.text_playLv.text = "10" --玩家等级
    self.text_playName.text = LoginLogic.sRoleId  -- 玩家名字
	self.text_RankName.text = "菜鸡" --玩家段位名字
    self.btn_action3.title = "活动" --左侧三个按钮的文字
    self.btn_action2.title = "活动"
    self.btn_action1.title = "看视频"
    self.btn_action1:GetChild("num").text = "N"
    self.btn_setDown.title = "设置"  --右下方按钮文字
    self.btn_rank.title = "排行"
	self.btn_showbooth.title = "展台"
    self.text_chatLine.text = "[color=#FF7566][世界]:[/color]组队刷boss"  --聊天框内容
    self.icon_playHead.url =FairyGUI.UIPackage.GetItemURL("Main", "icon-touxiang-1")
    self.box1.icon =FairyGUI.UIPackage.GetItemURL("Main", "btn-baoxaing-1")
    self.box2.icon =FairyGUI.UIPackage.GetItemURL("Main", "btn-baoxaing-1")
    self.box3.icon =FairyGUI.UIPackage.GetItemURL("Main", "btn-baoxaing-1")
    self.box_null.icon = FairyGUI.UIPackage.GetItemURL("Main", "btn-baoxiang-wu")
    --self.btn_add.icon = FairyGUI.UIPackage.GetItemURL("Common", "btn-jia")
    self.icon_playRank.icon =FairyGUI.UIPackage.GetItemURL("Main", "")
    self.holderRotate.icon = FairyGUI.UIPackage.GetItemURL("Main","")
	self.btn_setDown.icon = FairyGUI.UIPackage.GetItemURL("Main", "icon-shezhi")
	self.btn_rank.icon = FairyGUI.UIPackage.GetItemURL("Main", "icon-gongyun")
    self.btn_showbooth.icon = FairyGUI.UIPackage.GetItemURL("Main", "icon-zhantai")
    self.btn_action3.icon = FairyGUI.UIPackage.GetItemURL("Main", "btn-huodong")
    self.btn_action2.icon = FairyGUI.UIPackage.GetItemURL("Main", "btn-huodong-1")
    self.btn_action1.icon = FairyGUI.UIPackage.GetItemURL("Main", "btn-kanshipin")
    self.holderRotate.title = ""
    self.holderRotate:GetChild("tishi").visible =false
    self.holderRotate:GetChild("num").visible =false
    self.box1.title = "1小时"
    self.box2.title = "2小时"
    self.box3.title = "3小时"
    self.box_null.title = "空宝箱位"
    
    self:LoadModel(1)
end

function Main_MainUI:OnHide()
    self:RemoveModel()
end

function Main_MainUI:BtnClick_setDown()
    ViewMgr:ShowUI("Set","Main", "ViewLayer")
    
end

function Main_MainUI:BtnClick_rank()
end

function Main_MainUI:BtnClick_showbooth()
end

function Main_MainUI:BtnClick_more()
end

function Main_MainUI:BtnClick_action1()
end

function Main_MainUI:BtnClick_action2()
   
end

function Main_MainUI:BtnClick_action3()
   
end


function Main_MainUI:BtnClick_Match()    
    ViewMgr:ShowUI("Checkpoint","Check", "ViewLayer",1)
end

function Main_MainUI:BtnClick_MatchRank()
end