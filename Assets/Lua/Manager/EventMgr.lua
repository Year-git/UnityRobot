local setmetatable = setmetatable
local xpcall = xpcall
local unpack = unpack
local error = error
local pairs = pairs
local string_sub = string.sub
local string_len = string.len
local table_insert = table.insert
local table_remove = table.remove
local debugTraceback = debug.traceback

local _eventList = {};

local EventMgr = RequireSingleton("EventMgr")


local function traceback(msg)
	msg = debugTraceback(msg, 2)
	return msg
end

local function functor(func, obj)
	local slot	= {}
	slot.func	= func
	slot.obj	= obj
	setmetatable(slot, slot)	
	
	slot.__call	= function(self, ...)
		local flag 	= true	
		local msg = nil	
		
		local args = {...}
		
		if nil == self.obj then
			local func = function() self.func(unpack(args)) end
			flag, msg = xpcall(func, traceback)						
		else		
			local func = function() self.func(self.obj, unpack(args)) end
			flag, msg = xpcall(func, traceback)		
		end	
		
		if not flag then						
			CS_LogError(msg)
		end
	
		return flag, msg
	end
	
	return slot
end

local function CreateEvent(name)
	local ev = {}
	ev.name 	= name	
	ev.list		= {}
	setmetatable(ev, ev)	
	
	ev.Add = function(self, func, obj)
		 if nil == func then
			error("Add a nil function to event ".. self.name or "")
			return
		end
		table_insert(self.list, functor(func, obj))				
	end

	ev.Remove = function(self, func, obj)
		if nil == func then
			return
		end

		for i = #self.list, 1, -1 do
			local slot = self.list[i]
			if slot.func == func and slot.obj == obj then
				table_remove(self.list, i)
				return
			end 
		end
	end
	
	ev.Count = function(self)
		return #self.list
	end	
	
	ev.__call = function(self, ...)
		for _, slot in pairs(self.list) do					
			local flag,msg = slot(...)
			if not flag then
				error(msg)
			end
		end	
	end
	
	return ev
end

function EventMgr:RegistEvent(eventType, func, obj)
	local event = _eventList[eventType]
	if not event then
		event = CreateEvent(eventType)
		if not event then
			return;
		end
		_eventList[eventType] = event
	end
	event:Add(func, obj)
	return event
end

function EventMgr:RemoveEvent(eventType, func, obj)
	local event = _eventList[eventType]
	if event then
	   event:Remove(func, obj)
	   if event:Count() <= 0 then
	   		_eventList[eventType] = nil
	   end
	end
end

function EventMgr:DisplayEvent(eventType, ...)
	local event = _eventList[eventType]
	if event then
	   event(...)
	end
end

--派发从c#接收到的事件
function DisplayCSharpEvent(eventType, ...)
	local arg = {...}
	local params = {}
	for i,v in ipairs(arg) do
		if type(v) == "string" then
			local first = string_sub(v, 0, 3)
			if first == "_t[" or first == "_t{" then
				v = string_sub(v, 3, string_len(v))
				v = CS_Json.decode(v)
			end
		end
		table_insert(params, v)
		
	end
	EventMgr:DisplayEvent(eventType, unpack(params))
end