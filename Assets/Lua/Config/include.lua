--配置表需在在此注册
local tConfigName = {
	"Attribute_C",
	"GameParam_C",
	"Model_C",
	"Npc_C",
	"Part_C",
	"RandomNameConfig_C",
	"Scene_C",
	"Slot_C",
	"String_C",
	"SysPrompt_C",
	"Effect_C",
	"BuffCfg_C",
	"Level_C",
	"Chapter_C",
	"PartGrow_C",
	"Skill_C",
	"BuffTypeCfg_C"
}


----------------------------------------以下方法未经同意不允许改动----------------------------------------
local ipairs= ipairs
local string_format = string.format
local debugTraceback = debug.traceback
local table_insert = table.insert

--异常抛出
local function traceback(msg)
	msg = debugTraceback(msg, 2)
	return msg
end

--缓存配置表数据
local tConfig = {}
for _, sConfigName in ipairs(tConfigName) do
	local sPath = string_format("Config/%s", sConfigName)
	local func = function() return require(sPath) end
	local ret, configTbl = xpcall(func, traceback)
	if ret then
		if type(configTbl) == "table" then
			tConfig[sConfigName] = {
				value = configTbl
			}
			--将配置同步到c#
			ConfigManager.SetConfig(sConfigName, configTbl)
		else
			CS_LogError("ERROR!!! config not a table!!!")
		end
	end
end

--获取配置表数据
RequireConfig = function(sConfigName)
	local ret = tConfig[sConfigName]
	if ret then
		return ret.value
	else
		CS_LogError("ERROR!!! config not found!!! : " .. sConfigName)
	end
end