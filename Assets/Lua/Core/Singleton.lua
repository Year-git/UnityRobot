--global function
local type				= type
local pairs				= pairs
local ipairs			= ipairs
local pcall				= pcall
local table_insert		= table.insert

--local
local tName2Singleton	= {}
local tSingletonSet		= {}

RegistSingleton = function(i_sSingletonName)
	if type(i_sSingletonName) == "string" then
		if tName2Singleton[i_sSingletonName] then
			CS_LogWarning("ERROR!!! regist singleton repeat.", i_sSingletonName)
		else
			local singleton = {_name = i_sSingletonName}
			tName2Singleton[i_sSingletonName] = singleton
			table_insert(tSingletonSet, singleton)
			return singleton
		end
	else
		CS_LogWarning("err!!! regist singleton type err!!!", i_sSingletonName)
		CS_LogWarning(debug.traceback())
	end
end

RequireSingleton = function(i_sSingletonName)
	local oSingleton = tName2Singleton[i_sSingletonName]
	if not oSingleton then
		CS_LogWarning("err!!! require singleton err!!!", i_sSingletonName)
		CS_LogWarning(debug.traceback())
	end
	return oSingleton
end

StartSingleton = function()
	for k, v in ipairs(tSingletonSet) do
		local fun = v.OnInitialize
		if fun then
			pcall(fun, v)
		end
	end
	return true
end

UpdateSingleton = function()
	for k, v in ipairs(tSingletonSet) do
		local fun = v.OnDayRefresh
		if fun then
			pcall(fun, v)
		end
	end
end

DestroySingleton = function()
	for k, v in ipairs(tSingletonSet) do
		local fun = v.OnLogout
		if fun then
			pcall(fun, v)
		end
	end
end
