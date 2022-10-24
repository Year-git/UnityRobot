local BloodMgr = RequireSingleton("BloodMgr")
local FairyGUI = FairyGUI
local UnityEngine =UnityEngine 
function BloodMgr:OnInitialize(  )
    
    self.bloodlist = {}
end

function BloodMgr:CreatBlood( sPlayId )
     local _blood= FairyGUI.UIPackage.CreateObject("Battle", "HeadInfo").asProgress
     self.bloodlist[sPlayId] = _blood
end

function BloodMgr:DestroyBlood( sPlayId )
    UnityEngine.Object.Destroy(self.bloodlist[sPlayId])
    self.bloodlist[sPlayId] = nil
end

function BloodMgr:DestroyAll(  )
    for i,v in ipairs(self.bloodlist ) do
        UnityEngine.Object.Destroy(self.bloodlist[v])
    end
end