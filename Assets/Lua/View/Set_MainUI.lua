local Set_MainUI = RequireClass("Set_MainUI")
local FairyGUI = FairyGUI
local BattleLogic = RequireSingleton("BattleLogic")

function Set_MainUI:_constructor(pMainView)
    self._super._constructor(self,pMainView)
    self.topBox = pMainView:GetChild("topBox")
    self.viewInfo = pMainView:GetChild("viewInfo")
    self.silder1 = pMainView:GetChild("silder1")
    self.silder2 = pMainView:GetChild("silder2")
    self.silder3 = pMainView:GetChild("silder3")

    self.text_jintiao = self.topBox:GetChild("text_jintiao")
    self.text_jinbi = self.topBox:GetChild("text_jinbi")
    self.text_zuanshi = self.topBox:GetChild("text_zuanshi")
    
    self.btn_out = self.viewInfo:GetChild("btn_out")

    self.btn_low = pMainView:GetChild("btn_low")
    self.btn_mid = pMainView:GetChild("btn_mid")
    self.btn_height = pMainView:GetChild("btn_height")
    self.btn_top = pMainView:GetChild("btn_top")
    
    self.btn_low.onClick:Add(function( )  self:SetGameFrameLv( 1 ) end)
    self.btn_mid.onClick:Add(function( )  self:SetGameFrameLv( 2 ) end)
    self.btn_height.onClick:Add(function( )  self:SetGameFrameLv( 3 ) end)   
    self.btn_top.onClick:Add(function( )  self:SetGameFrameLv( 4 ) end)

    self.btn_out.onClick:Add(self.BtnClick_quit,self)
    self.silder1.onChanged:Add(self.Btnclicl_ChangeCamera,self)
    self.silder2.onChanged:Add(self.Btnclicl_ChangeVocality,self)
    self.silder3.onChanged:Add(self.Btnclicl_ChangeBgm,self)
end

function Set_MainUI:OnShow()
    self.viewInfo.title = "设置"
    self.silder1.value = BattleLogic.cameraValue
    self.silder2.value =  BattleLogic.musicValue
    self.silder3.value =  BattleLogic.bgmValue
    self.silder1.changeOnClick = false
    self.silder2.changeOnClick = false
    self.silder3.changeOnClick = false
    self.btnNameTable = {[1]="btn_low",[2]="btn_mid",[3]="btn_height",[4]="btn_top",}
    self:SetGameFrameLv( BattleLogic.FrameLv  )
end

function Set_MainUI:OnHide()

end

function Set_MainUI:BtnClick_quit()
    self:Close()
end

function Set_MainUI:Btnclicl_ChangeCamera(  )
    local _cameraValue = self.silder1.value / 100 
    MyPlayer.cameraSensitive = _cameraValue
    BattleLogic.cameraValue=self.silder1.value
end

function Set_MainUI:Btnclicl_ChangeVocality(  )
    local _musaValue = self.silder2.value / 100 
    MyPlayer.SetVocality(_musaValue)
    BattleLogic.musicValue=self.silder2.value 
end

function Set_MainUI:Btnclicl_ChangeBgm(  )
    local _bgmaValue = self.silder3.value / 100 
    MyPlayer.SetBGM(_bgmaValue)
    BattleLogic.bgmValue=self.silder3.value
end

function Set_MainUI:SetGameFrameLv( nBtnIndex )
    for i=1,4 do
        local btnImage ="vtmmm"
        if i == nBtnIndex then 
            btnImage ="vrfertjj"
        end 
        local _btnName =self.btnNameTable[i]
        self[_btnName].icon = FairyGUI.UIPackage.GetItemURL("Set",btnImage)
    end
    BattleLogic.FrameLv  =nBtnIndex
end