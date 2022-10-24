local CreateView = RequireClass("CreateView")
local CreateLogic = RequireSingleton("CreateLogic")

function CreateView:OnAwake()
end

function CreateView:OnStart()
end

function CreateView:OnClick()
    local inputText = self.inputField:GetComponent("TMP_InputField").text
    if inputText == "" then
        CS_LogError("请输入角色名！")
    else
        self.createBtn:GetComponent("Button").enabled = false
        CreateLogic:CreateRoleRequest(inputText, 1, 1)
    end
end

function CreateView:OnEnable()
    CS_OnClick(
        self.createBtn,
        function()
            self:OnClick()
        end
    )
end

function CreateView:OnDisable()
end

function CreateView:OnDestroy()
end
