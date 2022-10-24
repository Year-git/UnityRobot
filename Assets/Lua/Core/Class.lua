-- global function
local type	= type
local pairs	= pairs
local ipairs= ipairs
local setmetatable		= setmetatable
local table_insert		= table.insert

local fEmpty = function() end
local tClass = {}

local registclass = function(sClassName)
	if type(sClassName) == "string" then
		if tClass[sClassName] then
			CS_LogError("err!!! class already exist!!!", sClassName)
			CS_LogError(debug.traceback())
			return
		end
		local objClass = {}
		objClass._constructor	= fEmpty
		objClass.__index		= objClass
		tClass[sClassName]		= objClass
		return objClass
	else
		CS_LogError("err!!! regist class name type err!!!", sClassName)
		CS_LogError(debug.traceback())
	end
end

local requireclass = function(sClassName)
	local oClass = tClass[sClassName]
	if not oClass then
		CS_LogError("err!!! require class not exist!!!", sClassName)
		CS_LogError(debug.traceback())
	end
	return oClass
end

local newclass = function(sClassName, ...)
	local oClass = requireclass(sClassName)
	if oClass then
		local object = {}
		setmetatable(object, oClass)
		object:_constructor(...)
		return object
	end
end

local inheritclass = function(sClassNameDerive, sClassNameBase)
	local object = registclass(sClassNameDerive)
	if object then
		local oClass = requireclass(sClassNameBase)
		if oClass then
			object._super = oClass
			setmetatable(object, oClass)
		end
		return object
	end
end


RegistClass		= registclass
RequireClass	= requireclass
NewClass		= newclass
InheritClass	= inheritclass