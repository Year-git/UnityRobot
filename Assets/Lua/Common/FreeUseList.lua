--==========================================================
--@ FreeUseList
--==========================================================
FreeUseList = {}
local FreeUseList = FreeUseList
FreeUseList.__index = FreeUseList
function FreeUseList:Generate(pCreate)
	local tbNew = setmetatable({}, self)
	tbNew:PreInit()
	tbNew.Create = pCreate
	return tbNew
end

function SCreateFreeUseList(pCreator)
	local new = setmetatable({}, FreeUseList)
	new:PreInit()
	new.Create = pCreator
	return new
end

function FreeUseList:PreInit()
	self.UseId = 1
	self.FreeList = {}
	self.UseList = {}
	self.Create = nil
end

function FreeUseList:Assign(...)
	if not self.Create then
		return
	end
	local element = table.remove(self.FreeList, 1)
	element = element or self.Create(...)
	self.UseList[self.UseId] = element
	self.UseId = self.UseId + 1
	return element, self.UseId - 1
end

function FreeUseList:Recover(nUseId, pRecover)
	local element = self.UseList[nUseId]
	if not element then
		return
	end
	self.UseList[nUseId] = nil
	if pRecover then
		pRecover(element)
	end
	table.insert(self.FreeList, element)
	return element
end

function FreeUseList:RecoverAll(pRecover)
	for _, element in pairs(self.UseList) do
		if pRecover then
			pRecover(element)
		end
		table.insert(self.FreeList, element)
	end
	self.UseList = {}
end

function FreeUseList:AccessUseList()
	return self.UseList
end

