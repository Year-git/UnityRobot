local CreateLogic = RequireSingleton("CreateLogic")
local ViewMgr = RequireSingleton("ViewMgr")
local NetworkMgr = RequireSingleton("NetworkMgr")

function CreateLogic:OnInitialize()

end

function CreateLogic:CreateRoleRequest(sNickName, nGender,nMainHeroTemplateID)
	NetworkMgr:Send("K_CreateCharReqMsg", sNickName, nGender, nMainHeroTemplateID)
	ViewMgr:HideUI("CreateView")
end