local Battle_ClearUI = RequireClass("Battle_ClearUI")
local NetworkMgr = RequireSingleton("NetworkMgr")
local  ViewMgr =RequireSingleton("ViewMgr")
local CheckpointLogic = RequireSingleton("CheckpointLogic")
local Level_C=RequireConfig("Level_C")
local RefitLogic = RequireSingleton("RefitLogic")

function Battle_ClearUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    self.aRL        = pMainView:GetChild("aRL")
    self.n4txt      = pMainView:GetChild("n4")
    self.awardlist  = pMainView:GetChild("awardlist")
    self.defeate    = pMainView:GetChild("defeate")
    self.btn_back   = pMainView:GetChild("close")
    self.Controller  = pMainView:GetController("vORd")
    self.btn_back.onClick:Add(self.BackUI,self)
    self.awardlist.itemRenderer = FairyGUI.ListItemRenderer(self.SpareRender, self)
    self._holder   =pMainView:GetChild("holder")
    --LuaExtend.CreateVictoryObj("Assets/Res/Prefabs/Effect/UI/UI_shengli.prefab",self.ShowParUI,self)
end

function Battle_ClearUI:OnShow(id,issucceed)
    self.id=id
    if issucceed then
        self.Controller.selectedIndex=0
        self.aRL.url=FairyGUI.UIPackage.GetItemURL("Battle", "img-shengli-jiesuan")
        self.awardlist.numItems=#Level_C[id].drop
    else
        self.Controller.selectedIndex=1
        self.aRL.url=FairyGUI.UIPackage.GetItemURL("Battle", "img-shibai-jiesuan")
        self.defeate.text="建议选择部件。。。。。"
    end
end

function Battle_ClearUI:ShowParUI(model)
    self._model = model
    self.wrapper = FairyGUI.GoWrapper.New(self._model)
    self.wrapper:SetWrapTarget(self._model, true)
    self._holder:SetNativeObject(self.wrapper)
    local modelTransform=model:GetComponent("Transform")
    --modelTransform.localScale =Vector3(150, 150, 150)
    modelTransform.localPosition = Vector3(7.7, -4.89,0)
    modelTransform.parent:GetComponent("Transform").localScale =Vector3(120, 120, 120)
    --self._model.transform:Rotate(Vector3(5,142,-8)) --
    --self._model.transform.localEulerAngles=Vector3(5,142,-8) --localEulerAngles
end
   
function Battle_ClearUI:SpareRender(index,item)
    local item1=item.asCom
    item1:GetChild("icon").icon=FairyGUI.UIPackage.GetItemURL("Common", RefitLogic:GetPartIcon((Level_C[self.id].drop)[index+1]))  
end

function Battle_ClearUI:BackUI()
    CS_LeaveGameLevel()
    ViewMgr:ShowUI("Checkpoint", "Check", "ViewLayer", CheckpointLogic.themeId)
end