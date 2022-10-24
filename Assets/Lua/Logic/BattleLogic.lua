local BattleLogic = RequireSingleton("BattleLogic") 
local EventMgr = RequireSingleton("EventMgr")
local NetworkMgr = RequireSingleton("NetworkMgr")
local Level_C = RequireConfig("Level_C")
local FairyGUI = FairyGUI

function BattleLogic:OnInitialize()
    self.gameBeginTime = 0
    EventMgr:RegistEvent(GasToGac.CL_GamePlay, self.CL_GamePlay, self)
    self.skillTag ={
        ["btn_attack"] ={icon = "",cd=0,skillId =-1},
        ["btn_skill1"] ={icon = "",cd=0,skillId =-1},
        ["btn_skill2"] ={icon = "",cd=0,skillId =-1},
        ["btn_reset"] ={icon = "",cd=0,skillId =-1},
    }
    -- 设置界面参数
    self.cameraValue = 50
    self.bgmValue = 50
    self.musicValue = 50
    self.FrameLv = 1

    self.playSkillNum = 0
    self.SelectPartList={}
    --缓存已选中的技能 value=技能ID
    self.SelectSkillList=
    {
        [1]=-1,
        [2]=-1,
        [3]=-1,
        [4]=-1,
    }  

    self.SkillIconPos = {
        [1]= 1760,
        [2]= 1590,
        [3]= 1420,
        [4]= 1250,
    }
 
    self.BossHpColor = {
        [0]= "ui-boss-xuetiao-hong",
        [1]= "ui-boss-xuetiao-huang",
        [2]= "ui-boss-xuetiao-zi",
        [3]= "ui-boss-xuetiao-lv",
        [4]= "ui-boss-xuetiao-lan",
    }
   
    self.BattCheckId = 0  --  当前所在关卡的关卡ID  
end

function BattleLogic:SetSkillIconPos (  )
    local _width=FairyGUI.GRoot.inst.width
    for i=1,4 do
        local _pos =_width  -( 160 *i )- ((i-1) *10)
        self.SkillIconPos[i]= _pos
    end
end

function BattleLogic:SetSelectPartList(partid)
    for index,nPartID in ipairs(self.SelectPartList)do
        if nPartID==partid then
            return
        end
    end
    table.insert(self.SelectPartList,partid)
end

--带技能的配件槽位
function BattleLogic:SetSelectSkillList(skilhole,skillid)
    for hole, id in pairs(self.SelectSkillList) do 
        if id == skillid then
            self.SelectSkillList[hole]=-1
        end
    end
    self.SelectSkillList[skilhole]=skillid
end

function BattleLogic:ClearTable()
    self.SelectPartList={}
    self.SelectSkillList=
    {
        [1]=-1,
        [2]=-1,
        [3]=-1,
        [4]=-1,
    }
end

function BattleLogic:CL_GamePlay( nBeginTime )
    self.gameBeginTime=nBeginTime
end

function BattleLogic:TimeFormat( nTime ) -- 秒
    local _scends=nTime %60
    local _min=(nTime-_scends)/60
    local _hour = (_min -(_min % 60)) /60
    local _text = ""
    if _hour < 10 then 
        _text = "0".._hour..":"
    else
        _text = "".._hour..":"
    end 
    if _min<10 then
        _text = _text.."0".. _min..":"
    else
        _text =  _text.._min..":"
    end 
    if _scends<10 then 
        _text = _text.."0".._scends
    else
        _text = _text.."".._scends
    end 
    return _text
end

function BattleLogic:Send( playInstId,inputInfo )
    LocalFrameSynServer.LuaSendPlayerOperation(2, playInstId, inputInfo);
end

function BattleLogic:GetIsBossCheck( nCheckId ) -- 判断是不是boss关卡
    local _chenckInfo=Level_C[nCheckId]
    if _chenckInfo.type == 2 then 
        return true 
    end 
    return false
end